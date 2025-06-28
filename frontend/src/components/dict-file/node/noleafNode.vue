<script setup lang="ts">
import { ref, computed, inject } from 'vue'
import Node from './node.vue'
import type { Ref } from 'vue';

/**
 * 中间节点，显示为边框
 *
<item name="LayoutConfiguration">
    <!-- 报告模式下历史和同名列表显示方式 0 IEIS底部, 1 miniWorklist底部, 2 都不显示 -->
    <item name="ReportHistoryDisplay" value="0" />
    <!-- 是否显示视图tab栏 0 隐藏，1 显示-->
    <item name="WorklistViewTabDisplay" value="1" />      
</item>
 */
const props = defineProps(['level'])
const level = computed(() => props.level + 1)

const config = inject<Ref<App.Dict.Config>>('config')

const node = defineModel<App.Dict.Node>()

const commentStyle = ref({ backgroundColor: 'rgb(255, 255, 255)' })

const extendNode = ref(true)

// 改变边框颜色 弃用
const changeCommentStyle = () => {
    if (commentStyle.value.backgroundColor === 'rgb(238, 244, 238)' && extendNode.value == false) {
        commentStyle.value = { backgroundColor: 'rgb(255, 255, 255)' }
        extendNode.value = true
        return
    }

    extendNode.value = false
    commentStyle.value = { backgroundColor: 'rgb(238, 244, 238)' }
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
    <!-- 第一层为根节点，不再展示出来 -->
    <div v-if="level == 1">
        <a-space :direction="'vertical'">
            <div v-for="(child, index) in node.item" :key="index">
                <Node :level="level" v-model="node.item[index]"></Node>
            </div>
        </a-space>
    </div>

    <a-comment v-else class="node" :style="commentStyle">
        <!-- 显示节点的Key,有Label显示Label -->
        <template #author>
            <a-typography-paragraph style="color: black;" v-if="node[':@']">
                {{ getLabel(node) }}
            </a-typography-paragraph>
        </template>
        <template #avatar>
            <!--
            <a-spin :spinning="true" size="small" @click="changeCommentStyle" />
            <StarOutlined style="color: rgb(102, 200, 138);" @click="changeCommentStyle" />-->
        </template>
        <template #content>
            <!-- 显示节点的Value -->
            <a-space :direction="'vertical'">
                <div v-for="(child, index) in node.item" :key="index">
                    <Node :level="level" v-model="node.item[index]"></Node>
                </div>
            </a-space>
        </template>
    </a-comment>
</template>
<style>
.ant-comment-inner {
    padding: 3px 0px !important
}

.node {
    border: 1px solid rgb(223, 232, 228);
    border-radius: 10px;
    /* 圆角半径 */
}
</style>