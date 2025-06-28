<template>
    <!-- 内显示外部网站链接 -->
    <iframe :src="url" frameborder="0" width="100%" :style="{ height: calHeight }" scrolling="auto"></iframe>
</template>
  
<script setup lang="ts">
import { useTheme } from '@/stores/theme'
import { watch } from 'vue';
import { computed, ref } from 'vue'

const theme = useTheme();

const url = computed(() => {
    return theme.currentMenuItem.fileName
})

const calHeight = computed(() => {
  return (window.innerHeight ) + 'px'; // 根据需要调整高度
});

</script>
  