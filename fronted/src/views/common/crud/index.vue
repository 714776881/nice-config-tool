<script lang="ts" setup>
import { useTheme } from '@/stores/theme'
import { computed, ref } from 'vue'

const theme = useTheme();

const configFileName = computed(() => {
    return theme.currentMenuItem.fileName
})

</script>
<template>
    <lxCrud :key="configFileName" :configFileName="configFileName"></lxCrud>
</template>
<style></style>