<script setup lang="ts">
import { useRoute, useRouter } from 'vue-router'
import { ref } from 'vue'
import { fetchLogin } from '@/service/api/auth'
import { useAuthStore } from '@/stores/auth'
import { notification } from 'ant-design-vue';
import { onMounted } from 'vue';
const openNotification = (message) => {
    notification.open({
        message: message,
        description: '',
        placement: 'top'
    });
};

const authStore = useAuthStore()

const { login, logout } = authStore

const route = useRoute() // Http请求信息
const router = useRouter()
// 验证并登录
const getUserInfo = async () => {
    const loginToken = Object.keys(route.query)[0]

    await fetchLogin(loginToken).then(res => {
        if (res.data) {
            login(res.data)
            router.push('/');
            console.log('登录成功')
        }
        else {
            logout()
            router.push('/401');
            openNotification('登录失败！请尝试返回后重新进入！')
        }
    }).catch(err => {
        logout()
        router.push('/401');
        openNotification('登录失败！请尝试返回后重新进入！')
    })
}
onMounted(getUserInfo)

// 错误信息
const errorMessage = ref('')

</script>

<template>
    <div>
        {{ errorMessage }}
    </div>
</template>