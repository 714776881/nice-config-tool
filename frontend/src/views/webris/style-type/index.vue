<script lang="ts" setup>
import { useTheme } from '@/stores/theme'
import { computed, ref } from 'vue'
import PrintTemp from '../styleTemplate/printTemp.vue'

const theme = useTheme();

const configFileName = computed(() => {
    console.log("获取文件" + theme.currentMenuItem.fileName)
    return theme.currentMenuItem.fileName
})

const formulaDataFileName = 'styleTempTypeData.json'

</script>
<template>
    <lxCrud :key="configFileName" :configFileName="configFileName">
        <template #打印模板列表="{ state, item }">
            <!-- header 插槽的内容放这里 -->
            <lxCrud :key="formulaDataFileName" :configFileName="formulaDataFileName" :state="state">
                <template #预览模板="{ state, item, ok, cancel, change }">
                <!-- header 插槽的内容放这里 -->
                    <PrintTemp :state="state" :ok="ok" :cancel="cancel" :change="change"></PrintTemp>
                </template>
            </lxCrud>
        </template>
    </lxCrud>
</template>
<style></style>