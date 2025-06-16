<template>
    <a-modal v-model:open="open" :title="title" style="top: 60px">
        <div v-if="isCopy" style="margin-top: 20px;margin-bottom: 12px;">
            模板名称：<a-input v-model:value="sourceNode.title" style="width: 400px;"></a-input>
        </div>
        <a-card size="small" style="width: 470px;background-color: rgb(246, 254, 250);">
            <!-- 切换个人/公共知识库 -->
            <template #title>
                <div>
                    <div style="float: left;">目录：</div>
                    <a-space style="margin-left: 50px;">
                        <a-checkbox v-model:checked="privateLibChecked">
                            <UserOutlined /> 个人知识库
                        </a-checkbox>
                        <a-checkbox v-model:checked="publicLibChecked" style="margin-left: 30px;">
                            <TeamOutlined /> 公共知识库
                        </a-checkbox>
                    </a-space>
                </div>
            </template>
            <a-directory-tree :treeData="treeData" v-model:selectedKeys="selectedKeys" :height="600">
                <!-- 图标 -->
                <template #icon="{ type, expanded }">
                    <div v-if="type == ReptNodeTypeEnum.BodyPart || type == ReptNodeTypeEnum.ExamItem">
                        <BlockOutlined />
                    </div>
                    <div v-if="type == ReptNodeTypeEnum.Category">
                        <span v-if="expanded">
                            <FolderOpenOutlined />
                        </span>
                        <span v-else>
                            <FolderOutlined />
                        </span>
                    </div>
                    <div v-if="type == ReptNodeTypeEnum.ReportTemplate">
                        <FileOutlined />
                    </div>
                </template>
                <!-- 文本  -->
                <template #title="{ key, title, isPublic, type, editable, isShare }">
                    <span style="margin-left: 0px;">
                        <span>
                            {{ title }}
                        </span>
                        <a-space style="float: right;" size="small">
                            <div v-if="isPublic == false && (privateLibChecked && publicLibChecked) && type != ReptNodeTypeEnum.ExamItem"
                                style="margin-left: 6px;">
                                <a-tooltip placement="bottom">
                                    <template #title>私有</template>
                                    <UserOutlined />
                                </a-tooltip>
                            </div>
                            <div
                                v-if="isPublic == true && type == ReptNodeTypeEnum.Category && (privateLibChecked && publicLibChecked == false)">
                                <TeamOutlined />
                            </div>
                        </a-space>
                    </span>
                </template>
            </a-directory-tree>
        </a-card>
        <template #footer>
            <a-checkbox v-if="isCopy" :disabled="props.isSuperMan == false" v-model:checked="toPrivateChecked"
                style="float: left;" @change="changeChecked">{{
                    copyCheckedTitle
                }}</a-checkbox>
            <a-space>
                <a-button @click="handleCancel">取消</a-button>
                <a-button type="primary" @click="handleOk">确定</a-button>
            </a-space>
        </template>
    </a-modal>
</template>
<script setup lang="ts">
import { computed, defineModel, ref, watch, onMounted } from "vue";
import { ReptNodeTypeEnum, getCatelogNode, getParentNode, isHavePrivateChildren, nodeToPublic } from '../treeNodes'
import { fetchGetExamItemCategory, fetchMoveCatelogNode, fetchCopyCatelogNode } from '@/service/api/reptCatelog'
import { message } from 'ant-design-vue'
import { deepClone } from "@/utils/tool";

const props = defineProps(['node', 'isLoadExamItem', 'modality', 'state', 'isCopy', 'isSuperMan'])
const emit = defineEmits(['handleOk']);
const open = defineModel<boolean>('open')

const isCopy = ref<boolean>(props.isCopy)
const toPrivateChecked = ref(false)

const examItemPublic = ref<string[]>([])
const examItemPrivate = ref<string[]>([])
const sourceNode = ref<Api.ReptCatelog.Node>(deepClone(props.node))

const privateLibChecked = ref(true)
const publicLibChecked = ref(false)

const catelogTreeData = defineModel<Api.ReptCatelog.Node[]>(); // 数据源
const treeData = computed(() => {
    return filterTreeData(catelogTreeData.value); // 目录数据
})
const selectedKeys = ref<string[]>([])

const title = computed(() => {
    if (isCopy.value) {
        return '创建副本';
    }
    return `移动 ${sourceNode.value.title}` + ' 到'
})

const copyCheckedTitle = computed(() => {
    if (isCopy.value == false) return ''

    if (props.isSuperMan == false) {
        toPrivateChecked.value = true
        sourceNode.value.isPublic = false
        return '创建私有副本'
    }

    if (props.node.isPublic == false && props.isSuperMan) {
        return '创建公有副本'
    }
    else {
        return '创建私有副本'
    }
})

const changeChecked = () => {
    sourceNode.value.isPublic = !sourceNode.value.isPublic
    selectedKeys.value = []
}

// 根据搜索条件、个人知识库、公有知识库、已选检查项目后的符合数据
const filterTreeData = (nodes: Api.ReptCatelog.Node[], parentNode?: Api.ReptCatelog.Node) => {
    return nodes.map((node) => {
        // 递归过滤子节点
        const children = node.children ? filterTreeData(node.children, node) : null;

        let tempNode = {
            ...node,
            children,
            disabled: sourceNode.value.isPublic && node.isPublic == false
        };

        if (node.key == sourceNode.value.key) {
            return null;
        }
        if (node.type == ReptNodeTypeEnum.ExamItem) {
            if (publicLibChecked.value) {
                if (examItemPublic.value.includes(node.key)) {
                    return tempNode
                }
            }
            if (privateLibChecked.value) {
                if (examItemPrivate.value.includes(node.key)) {
                    return tempNode
                }
                if (children && isHavePrivateChildren(children)) {
                    return tempNode
                }
            }
            return null
        }
        if (node.type == ReptNodeTypeEnum.ReportTemplate) {
            return null;
        }

        // 过滤私有和公有知识库
        if (publicLibChecked.value && privateLibChecked.value) {
            return tempNode
        }
        if (publicLibChecked.value && node.isPublic) {
            return tempNode
        }
        if (privateLibChecked.value) {
            if (node.type != ReptNodeTypeEnum.Category && node.type != ReptNodeTypeEnum.ReportTemplate) {
                return tempNode
            }
            // 子节点具有私节点
            /***/
            if (children && isHavePrivateChildren(children)) {
                return tempNode
            }

            if (node.isPublic == false) {
                return tempNode
            }
        }
        return null;
    }).filter((node) => node).sort((a, b) => { return a.isPublic ? 1 : -1 }).sort((a, b) => { return a.type - b.type }); // 去除空值
};

const handleCancel = () => {
    open.value = false;
};
const handleOk = async () => {
    if (selectedKeys.value.length == 0) {
        message.info('请选择一个目标目录!')
        open.value = false;
    }

    const targetNode = getCatelogNode(catelogTreeData.value, selectedKeys.value[0])

    // 移动节点
    if (isCopy.value == false) {
        const res = await fetchMoveCatelogNode(sourceNode.value, targetNode, props.state)
        if (res) {
            const parentNode = getParentNode(catelogTreeData.value, sourceNode.value.key)
            parentNode.children = parentNode.children.filter((item) => item.key != sourceNode.value.key)
            targetNode.children.push(sourceNode.value)
            open.value = false;
        }
    }
    // 复制节点
    else {
        nodeToPublic([sourceNode.value], sourceNode.value.isPublic)
        const res = await fetchCopyCatelogNode(sourceNode.value, targetNode, props.state)
        if (res.data) {
            targetNode.children.push(res.data)
            open.value = false;
        }
    }
}

const loadExamItemCategory = async () => {
    if (props.isLoadExamItem) {
        const res = await fetchGetExamItemCategory(props.modality, props.state, true)
        examItemPublic.value = res.data
        const res2 = await fetchGetExamItemCategory(props.modality, props.state, false)
        examItemPrivate.value = res2.data
    }
}
onMounted(loadExamItemCategory)
</script>../treeNodes