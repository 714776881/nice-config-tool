<template>
    <div v-if="node">
        <!-- 根据节点类型 显示不同的节点组件 -->
        <LeafNode :parentItem="props.parentItem" v-if="isLeafNode" v-model="node" />
        <NoleafNode :level="level" v-if="isNoleafNode" v-model="node" />
        <ArrayNode :level="level" v-if="isArrayNode" v-model="node" />
        <CommentNode v-if="isCommentNode" v-model="node" />
    </div>
</template>
  
<script lang="ts" setup>
import { computed, defineProps, inject, watch } from 'vue';
import LeafNode from './leafNode.vue'
import NoleafNode from './noleafNode.vue'
import ArrayNode from './arrayNode.vue'
import CommentNode from './commentNode.vue'

const props = defineProps(['level', 'parentItem'])
const level = computed(() => props.level)

const node = defineModel<App.Dict.Node>()

const isLeafNode = computed(() => {
    if (node.value[':@'] == undefined) return false
    return node.value[':@'].value != undefined && node.value[':@'].value != "#Array"  //:@ 表示属性 _ 表示注释
})

const isArrayNode = computed(() => {
    if (node.value[':@'] == undefined) return false
    return node.value[':@'].value != undefined && node.value[':@'].value == "#Array"
})

const isNoleafNode = computed(() => {
    return (node.value[':@'] == undefined || node.value[':@'].value == undefined) && node.value['_'] == undefined
})

const isCommentNode = computed(() => {
    return node.value['_'] != undefined
})

// 当label以#开头的时候，将内容添加到锚点数组中
/**
const anchorItems = inject('anchorItems')
const isAuthorItem = computed(() => {
    return node.value[':@'] != undefined && node.value[':@'].label != undefined && node.value[':@'].label.startsWith("#")
})
 */

</script>

<style scoped>
/* 可以根据需要添加样式 */
</style>