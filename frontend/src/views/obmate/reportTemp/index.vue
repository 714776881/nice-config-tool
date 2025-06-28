<script lang="ts" setup>
import { useTheme } from '@/stores/theme'
import { computed, ref } from 'vue'
import Dict from './dict.vue'

const theme = useTheme();

const configFileName = computed(() => {
    console.log("获取文件" + theme.currentMenuItem.fileName)
    return theme.currentMenuItem.fileName
})

</script>
<template>
    <Dict :key="configFileName" :configFileName="configFileName"></Dict>
</template>
<style></style>