<template>
    <!-- 显示外部网站链接 -->
    <iframe :src="url" security="restricted" sandbox="allow-same-origin allow-scripts allow-modals" frameborder="0" width="100%" :style="{ height: calHeight }" scrolling="auto"></iframe>
</template>
  
<script setup lang="ts">
import { useTheme } from '@/stores/theme'
import { onMounted, watch } from 'vue';
import { computed, ref } from 'vue'
import { useAuthStore } from '@/stores/auth'

const {userInfo, scopeState} = useAuthStore()

// 从配置中读取外部访问的根路径地址，再加上当前菜单的文件名组成完整的URL地址
const webConfigBaseUrl = window.config.webConfigBaseUrl

const theme = useTheme();
// 计算URL地址
const url = computed(() => {
  // 如果当前菜单项没有文件名，则返回空字符串
  if (!theme.currentMenuItem.fileName) {
    return '';
  }
  // 返回完整的URL地址
  return webConfigBaseUrl + '/' + theme.currentMenuItem.fileName + '?' + 'userId=' + userInfo.userId;
});


// 计算iframe高度
const calHeight = computed(() => {
  return (window.innerHeight ) + 'px'; 
});

</script>
  