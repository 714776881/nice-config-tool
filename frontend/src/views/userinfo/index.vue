<script setup lang="ts">
import { useRoute } from 'vue-router'
import { onBeforeMount } from 'vue'
import { fetchLogin } from '@/service/api/auth'
import { useAuthStore } from '@/stores/auth'

const authStore = useAuthStore()

const { userInfo } = authStore

</script>

<template>
  <div v-if="userInfo">
    欢迎回来！{{ userInfo.userName }}
  </div>
</template>
<style scoped>
.main {
  background-color: #f4f6f1;
  height: 100%;
}
</style>