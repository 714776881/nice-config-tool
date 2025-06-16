<script lang="ts" setup>
import { ref, onMounted } from 'vue';
import { xmlToObject, objectToXml } from '@/utils/xmltool';
import { fetchGetXmlFileNode, fetchPostXmlFileNode } from '@/service/api/file'
import { computed, inject } from 'vue';
import { App, message } from 'ant-design-vue';
import { deepClone } from '@/utils/tool'
import { clearNodeValue } from '@/components/dict/node/nodeTool'

const props = defineProps(['name', 'filePath', 'nodePath'])

const nodeObject = ref({})
// 加载数据
const loadData = async () => {
    await fetchGetXmlFileNode(props.filePath, props.nodePath).then(async (res) => {
        //res.data = '<root>' + res.data + '</root>'
        nodeObject.value = await xmlToObject(res.data)
    })
}
// 提交数据
const submitData = async () => {
    debugger;
    var xmlStr = objectToXml(nodeObject.value);
    await fetchPostXmlFileNode(props.filePath, props.nodePath, xmlStr).then(async (res) => {
        if (res.data == true) {
            message.success('保存成功！')
        }
        else {
            message.success('保存失败！')
        }
    })
}
onMounted(() => {
    loadData()
})

// 显示XML文本
const nodeXml = computed(() => {
    return objectToXml(nodeObject.value).trim()
})
const nodeXmlShow = ref(false)

const visible = ref(false)
const nodeTemplate = ref('')
// 添加新内容
const addNewNode = () => {
    visible.value = true
}
const addFirstNode = () => {
    visible.value = false
    console.log(nodeTemplate.value)
    nodeObject.value[0].item = [...nodeObject.value[0].item,
    ...xmlToObject(nodeTemplate.value)
    ]
}

// 逻辑树的层数,用来处理首个节点的情况，以及通过层数和name来判断节点位置
const level = ref(0)


// 获取检查模式数据列表

const config = inject('config')
const currentModality = ref('')

const nodeConfig = computed(() => {
    if (config == undefined) {
        return
    }
    if (config.value.inputConfig == undefined) {
        return
    }

    const key = 'ExamMode'

    const result = config.value.inputConfig.find(item => item.key.split(';').includes(key))

    if (result == undefined) {
        return
    }

    currentModality.value = result.formItem.defaultValue

    return result
})

const changeModality = (value) => {
    // 改变当前模态，重新加载数据
    console.log(value)
    const items = nodeObject.value[0].item

    const index = items.findIndex(item => {
        const examModeItem = item.item.find(ele => ele[':@'].name == 'ExamMode')
        if (examModeItem) {
            return examModeItem[':@'].value == value
        }
        return false
    })
    if (index >= 0) {
        currentNode.value = index
    }
    else {
        message.success('当前检查模式未配置节点模板，需要你重新创建节点！')
        addTemplateNode(items, value)
        currentNode.value = items.length - 1
    }
}


const addTemplateNode = (items, value) => {
    if (items.length == 0) {
        message.error('请创建一个模板！')
        visible.value = true
    }
    // 若第一个节点为注释，需要特殊处理

    const item1 = deepClone(items[0])

    clearNodeValue(item1)

    const examModeItem = item1.item.find(ele => ele[':@'].name == 'ExamMode')
    if (examModeItem) {
        examModeItem[':@'].value = value
    }

    items.splice(items.length, 0, item1)
}

const currentNode = ref(0)

</script>

<template>
    <div>
        <a-card>
            <template #title>
                {{ props.name + "：" }}
                <component v-if="nodeConfig" :is="nodeConfig.component.name" v-bind="nodeConfig.component.props"
                    v-model:value="currentModality" @change="changeModality" />

                <!-- 正式内容，就不用医生用户通过xml修改模板了
                <a-button type="link" size="small" @click="nodeXmlShow = !nodeXmlShow">xml</a-button>
                -->
            </template>
            <template #extra>
                <a-button @click="submitData" size="small" type="primary" ghost>保存</a-button>
            </template>
            <div v-if="nodeXmlShow">
                <a-textarea v-model:value="nodeXml" :autosize="true" readonly />
                <a-textarea v-if="false" :value="JSON.stringify(nodeObject, null, 4)" :autosize="true"
                    readonly></a-textarea>
                <a-button size="small" type="dashed" style="width: 120px" @click="addNewNode">
                    <PlusOutlined />
                    添加新内容
                </a-button>
            </div>
            <div v-else style="margin-top: -30px;">
                <lxNode :level="level" v-if="nodeObject && nodeObject[0]" v-model="nodeObject[0].item[currentNode]">
                </lxNode>
                <div v-else style="display: flex; justify-content: center; align-items: center; height: 10vh;">
                    <a-spin size="large" />
                </div>
            </div>
        </a-card>
        <a-modal v-model:visible="visible" title="添加节点模板" @ok="addFirstNode">
            <a-textarea v-model:value="nodeTemplate" :autosize="true" />
        </a-modal>
    </div>
</template>@/components/dict-file/node/nodeTool