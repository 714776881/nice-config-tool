<template>
    <a-modal v-model:open="open" :title="props.isPublic ? '添加公有项目' : '添加私有项目'" ok-text="确认" @ok="handleOk">
        <div>
            <!-- 检查项目列表 -->
            <a-tree :tree-data="treeData" :checkable="true" v-model:checkedKeys="checkedKeys">
            </a-tree>
        </div>
    </a-modal>
</template>
<script lang="ts" setup>
import { ref, watch, onMounted, onUnmounted, computed } from 'vue'
import { fetchExamItems, fetchGetExamItemCategoryByBodypartId, fetchAddExamItemCategory } from '@/service/api/reptCatelog'
import { ReptNodeTypeEnum } from '../treeNodes'
import { deepClone, uuid } from '@/utils/tool';

const props = defineProps(['isPublic', 'bodyPartId', 'state'])
const emit = defineEmits(['handleOk'])

const open = defineModel<boolean>('open')
const treeData = ref<Api.ReptCatelog.Node[]>([])
const checkedKeys = ref([]);

// 确认添加的检查项目
const handleOk = async () => {
    var res = await fetchAddExamItemCategory(props.bodyPartId, checkedKeys.value, props.state, props.isPublic)
    if (res.data) {
        emit('handleOk')
        open.value = false
    }
}

// 获取部位下的检查项目列表
onMounted(async () => {
    var res = await fetchExamItems(props.bodyPartId)
    if (res.data) {
        treeData.value = res.data.map(item => {
            return { title: item.label, key: item.value, isPublic: props.isPublic, type: ReptNodeTypeEnum.ExamItem, children: [] }
        })
    }

    var res2 = await fetchGetExamItemCategoryByBodypartId(props.bodyPartId, props.state, props.isPublic)
    checkedKeys.value = res2.data
})
</script>