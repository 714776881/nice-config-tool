
<script setup lang="ts">
import { theme } from 'ant-design-vue';
import { RouterView } from 'vue-router'
import Menu from '@/components/menu/index.vue'

import zhCN from 'ant-design-vue/es/locale/zh_CN'
import { useAuthStore } from '@/stores/auth'
import { computed } from 'vue';
import i18n from './locale/locale';

// 获取用户信息
const authStore = useAuthStore()

// 侧边栏宽度
const siderWidth = computed(() => {
  if (authStore.isAuthenticated) {
    return '240px'
  }
  return '0'
})


i18n.global.locale = 'zh'

</script>

<template>
  <header>
  </header>
  <a-layout>
    <!-- 侧边栏 -->
    <a-layout-sider v-if="authStore.isAuthenticated" :width="siderWidth"
      style="background-color:rgb(233, 246, 235);height: 100vh;position: fixed">
      <el-scrollbar height="100vh">
        <Menu></Menu>
      </el-scrollbar>
    </a-layout-sider>
    <!-- 主体区域 -->
    <a-layout :style="{ marginLeft: siderWidth }" style=" background-color: white;">
      <a-layout-header v-if="false"></a-layout-header>
      <!-- 内容区域 -->
      <a-layout-content style="{overflow: 'initial'}">
        <div class="contnet">
          <a-config-provider :locale="zhCN" :theme="{
            token: {
              colorPrimary: '#2eb67d'
            }
          }">
            <RouterView />
          </a-config-provider>
        </div>
      </a-layout-content>
      <a-layout-footer v-if="false"></a-layout-footer>
    </a-layout>
  </a-layout>
</template>

<style scoped>
.contnet {
  margin-left: 22px;
  margin-top: 30px;
  margin-right: 30px;
}
</style>