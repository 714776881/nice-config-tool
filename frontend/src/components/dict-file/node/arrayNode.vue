<script setup lang="ts">
import { ref, computed, inject } from 'vue'
import Node from './node.vue'
import { deepClone } from '@/utils/tool'
import { message } from 'ant-design-vue';
import { xmlToObject } from '@/utils/xmltool';
import draggable from 'vuedraggable'
import { clearNodeValue } from './nodeTool'
import type { Ref } from 'vue';
/**
 * 		<item name="RAD" value="#Array">
 *			<item value="放射科"/>
 *          <item value="超声科"/>
 *		</item>
 */
const props = defineProps(['level'])
const level = computed(() => props.level + 1)

const config = inject<Ref<App.Dict.Config>>('config')

const node = defineModel<App.Dict.Node>()

const isLeafNode = computed(() => {
    if (node.value.item.length == 0) {
        return false
    }

    const item1 = node.value.item[0]

    if (item1[':@'] == undefined) return false
    return item1[':@'].value != undefined && item1[':@'].value != "#Array"
})


const addNode = () => {
    if (node.value.item.length == 0) {
        message.info('请创建一个模板！')
        visible.value = true
        return
    }
    // 若第一个节点为注释，需要特殊处理

    const item1 = deepClone(node.value.item[0])

    clearNodeValue(item1)

    if (isLeafNode.value) {
        node.value.item.splice(node.value.item.length, 0, item1)
    }
    else {
        node.value.item.splice(0, 0, item1)
    }
}
const removeNode = (index) => {
    if (!isLeafNode.value && node.value.item.length == 1) {
        message.info("请保留一条信息作为模板！")
        return
    }

    node.value.item.splice(index, 1)
}

const upNode = (index) => {
    if (index == 0) return
    const temp = node.value.item[index - 1]
    node.value.item[index - 1] = node.value.item[index]
    node.value.item[index] = temp
}

const downNode = (index) => {
    if (index == node.value.item.length - 1) return
    const temp = node.value.item[index + 1]
    node.value.item[index + 1] = node.value.item[index]
    node.value.item[index] = temp
}

const visible = ref(false)
const nodeTemplate = ref('<item value=""/>')
const addFirstNode = () => {
    visible.value = false
    node.value.item = xmlToObject(nodeTemplate.value)
}


const dragItemLeftStyle = ref({
    float: "left",
    margin: "10px 10px 0px 0px"
})

const dragItemStyle = {
    float: "",
    margin: "10px 10px 0px 0px"
}

function findItem(node, name) {
    if (node == undefined || node[':@'] == undefined) {
        return
    }
    if (node[':@'].name == name && node[':@'].value != undefined) {
        return node[':@'].value;
    }
}

const getLabel = (item: any) => {
    if (item[':@'] == undefined || item[':@'].name == undefined) {
        return '';
    }

    // 从元配置中查找
    if (config.value.keyLabelMap) {
        const keyLable = config.value.keyLabelMap.find(ele => ele.key == item[':@'].name)
        if (keyLable) {
            return keyLable.label
        }
    }
    // 从配置文件中查找
    return item[':@'].label ? item[':@'].label : item[':@'].name
}

</script>

<template>
    <div>
        <div>
            <!-- 数组：标题 -->
            <div style="margin: 8px 0px;">
                <a-space>
                    <div style="height: 100%;width: 10px;background-color: rgb(115, 230, 157);">
                        <span style="visibility: hidden;">.</span>
                    </div>
                    <a-typography-title v-if="level != 1" :level="5" style="margin:0px 0px">
                        {{ getLabel(node) }}
                    </a-typography-title>
                    <!--
                    <div v-if="isLeafNode">
                        {{ (node[':@'].label ?
                            node[':@'].label :
                            node[':@'].name) }}
                    </div>
                    -->

                    <a-button size="small" type="dashed" @click="addNode">
                        <PlusOutlined />
                        {{ node.item.length == 0 ? '添加 节点模板' : '添加' }}
                    </a-button>
                </a-space>
            </div>

            <!-- 数组：列表  可拖拽 -->
            <draggable v-model="node.item" itemKey=":@" handle=".drag-handle">
                <template #item="{ element, index }">
                    <div class="drag-item" :style="isLeafNode ? dragItemLeftStyle : dragItemStyle">
                        <a-space :size="[12, 12]" align="start">
                            <!-- 复杂数组元素 上移 删除 下移 -->
                            <a-space :size="[8, 0]" v-if="!isLeafNode" direction="vertical">
                                <a-tooltip title="上移">
                                    <CaretUpOutlined style="color: rgb(102, 200, 138);" @click="upNode(index)" />
                                </a-tooltip>
                                <a-tooltip title="删除">
                                    <MinusCircleOutlined style="color: rgb(102, 200, 138);" :disabled="1 != 1"
                                        @click="removeNode(index)" />
                                </a-tooltip>
                                <a-tooltip title="下移">
                                    <CaretDownOutlined style="color: rgb(102, 200, 138);" @click="downNode(index)" />
                                </a-tooltip>
                            </a-space>
                            <!-- 文本数组元素 拖拽移动 -->
                            <a-tooltip v-if="isLeafNode" title="拖拽移动" color="rgb(102, 200, 138)">
                                <a-button type="text" class="drag-handle" icon style="margin-right: -20px;">
                                    <HolderOutlined />
                                </a-button>
                            </a-tooltip>
                            <!-- 数组元素 -->
                            <a-space :size="[0, 16]">
                                <Node :parentItem="node" :level="level" v-model="node.item[index]"></Node>
                                <a-tooltip title="删除">
                                    <MinusCircleOutlined v-if="isLeafNode"
                                        style="color: rgb(102, 200, 138);margin:0px 8px 0px -8px;" :disabled="1 != 1"
                                        @click="removeNode(index)" />
                                </a-tooltip>
                            </a-space>
                        </a-space>
                    </div>
                </template>
            </draggable>
        </div>
        <!-- 添加节点模板弹窗 当数组为空时，需要提供默认的节点作为模板 默认为<item value="示例，请按照模板要求填写！"/> -->
        <a-modal v-model:visible="visible" title="添加 节点模板" @ok="addFirstNode">
            <a-textarea v-model:value="nodeTemplate" :autosize="true" />
        </a-modal>
    </div>
</template>
<style>
.drag-item {
    display: flex;
}

.drag-handle {
    cursor: move;
}
</style>