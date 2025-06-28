<script lang="ts" setup>
import { useTheme } from '@/stores/theme'
import { computed, ref } from 'vue'

const theme = useTheme();

const configFileName = computed(() => {
    console.log("获取文件" + theme.currentMenuItem.fileName)
    return theme.currentMenuItem.fileName
})

const dataFileName = 'measurementItem.json'

</script>
<template>
    <lxCrud :key="configFileName" :configFileName="configFileName">
        <template #测量项列表="{ state, item }">
            <!-- header 插槽的内容放这里 -->
            <lxCrud :key="dataFileName" :configFileName="dataFileName" :state="state">
            </lxCrud>
        </template>
    </lxCrud>
</template>
<style></style>