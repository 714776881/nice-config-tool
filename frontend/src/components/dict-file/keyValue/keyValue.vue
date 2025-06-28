<script lang="ts" setup>
import { ref, onMounted } from 'vue';
import { xmlToObject, objectToXml } from '@/utils/xmltool';
import { fetchGetXmlFileNode, fetchPostXmlFileNode } from '@/service/api/file'
import { computed } from 'vue';
import { App, message } from 'ant-design-vue';
import Node from '../node/node.vue';

const props = defineProps(['name', 'filePath', 'nodePath'])

const nodeObject = ref({})
const loadData = async () => {
    await fetchGetXmlFileNode(props.filePath, props.nodePath).then(async (res) => {
        //res.data = '<root>' + res.data + '</root>'
        nodeObject.value = await xmlToObject(res.data)
    })
}
// 提交数据
const submitData = async () => {
    var xmlStr = objectToXml(nodeObject.value)
    await fetchPostXmlFileNode(props.filePath, props.nodePath, xmlStr).then(async (res) => {
        if (res.data == true) {
            message.success('保存成功！')
        }
        else {
            message.success('保存失败！' + res.message)
        }
    })
}
onMounted(() => {
    loadData()
})

const nodeXml = computed(() => {
    return objectToXml(nodeObject.value).trim()
})
const nodeXmlShow = ref(false)


// 添加新内容
const visible = ref(false)
const nodeTemplate = ref('')
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

</script>

<template>
    <div>
        <a-card>
            <!-- 标题 -->
            <template #title>
                {{ props.name }}
                <a-button type="link" size="small" @click="nodeXmlShow = !nodeXmlShow">xml</a-button>
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
            <!-- XML节点编辑 -->
            <div v-else>
                <Node :level="level" v-if="nodeObject && nodeObject[0]" v-model="nodeObject[0]"></Node>
                <div v-else style="height: 100px;">
                    <a-spin size="large" />
                </div>
            </div>
        </a-card>
        <a-modal v-model:visible="visible" title="添加节点模板" @ok="addFirstNode">
            <a-textarea v-model:value="nodeTemplate" :autosize="true" />
        </a-modal>
    </div>
</template>