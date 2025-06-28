<template>
    <div class="login-container">
        <div class="login-box">
            <div class="login-title">主题配置工具</div>
            <div class="login-form">
                <a-space direction="vertical" size="large">
                    <a-input v-model:value="loginForm.userCode" placeholder="用户名" clearable></a-input>
                    <a-input v-model:value="loginForm.password" placeholder="密码" type="password" clearable></a-input>
                    
                    <!-- 滑动验证 -->
                    <div class="slider-container">
                        <div class="slider-track">
                            <div 
                                class="slider-thumb" 
                                :style="{ left: sliderLeft + 'px' }"
                                @mousedown="startDrag"
                            >
                                ➜
                            </div>
                        </div>
                        <span v-if="false" class="verified-text">✔ 验证成功</span>
                    </div>

                    <a-button type="primary" @click="getUserInfo" :disabled="!isVerified">登录</a-button>
                </a-space>
                <div class="login-footer">Copyright © 2025</div>
            </div>
        </div>
    </div>
</template>

<script setup lang="ts">
import { ref, onMounted, onUnmounted } from 'vue'
import { useAuthStore } from '@/stores/auth'
import { fetchLoginWithUP } from '@/service/api/auth'
import { useRouter } from 'vue-router'
import { notification } from 'ant-design-vue'

const loginForm = ref({
    userCode: '',
    password: ''
})

const router = useRouter()
const authStore = useAuthStore()
const isVerified = ref(false)
const sliderLeft = ref(0) // 滑块的位置
const isDragging = ref(false)
const sliderMax = ref(0) // 滑动最大值

const { login, logout } = authStore

// 监听鼠标移动
const onMouseMove = (event: MouseEvent) => {
    if (!isDragging.value) return
    const newLeft = Math.min(sliderMax.value, Math.max(0, event.clientX - sliderMax.value / 2))
    sliderLeft.value = newLeft
}

// 监听鼠标松开
const onMouseUp = () => {
    if (!isDragging.value) return
    isDragging.value = false

    // 如果滑动到最右边，则验证成功
    if (sliderLeft.value >= sliderMax.value - 10) {
        isVerified.value = true
    } else {
        // 未滑动到最右边，复位
        sliderLeft.value = 0
    }
}

// 开始拖动
const startDrag = () => {
    isDragging.value = true
}

// 计算滑动最大范围
onMounted(() => {
    const track = document.querySelector('.slider-track') as HTMLElement
    if (track) {
        sliderMax.value = track.offsetWidth - 40 // 40是滑块宽度
    }
    document.addEventListener('mousemove', onMouseMove)
    document.addEventListener('mouseup', onMouseUp)
})

// 清理监听
onUnmounted(() => {
    document.removeEventListener('mousemove', onMouseMove)
    document.removeEventListener('mouseup', onMouseUp)
})

const getUserInfo = async () => {
    if (!isVerified.value) {
        openNotification('请先完成滑动验证！')
        return
    }

    if(authStore.isAuthenticated)
    {
        logout()
    }

    await fetchLoginWithUP(loginForm.value.userCode, loginForm.value.password)
        .then(res => {
            if (res.data) {
                login(res.data)
                router.push('/')
                console.log('登录成功')
            } else {
                openNotification('登录失败！请尝试返回后重新进入！')
                resetSlider()
            }
        })
        .catch(() => {
            openNotification('登录失败！请尝试返回后重新进入！')
            resetSlider()
        })
}

// 重置滑块
const resetSlider = () => {
    sliderLeft.value = 0
    isVerified.value = false
}

const openNotification = (message: string) => {
    notification.open({
        message: message,
        description: '',
        placement: 'top'
    })
}
</script>

<style>
.login-container {
    background-color: #f0f2f5;
    height: 90vh;
    display: flex;
    justify-content: center;
    align-items: center;
}

.login-box {
    width: 400px;
    height: 350px;
    background-color: #fff;
    border-radius: 10px;
    box-shadow: 0 2px 12px 0 rgba(0, 0, 0, .1);
    display: flex;
    flex-direction: column;
    justify-content: center;
    align-items: center;
}

.login-title {
    font-size: 24px;
    font-weight: bold;
    margin-bottom: 20px;
}

.login-form {
    width: 80%;
    display: flex;
    flex-direction: column;
    justify-content: center;
}

/* 滑动验证 */
.slider-container {
    display: flex;
    align-items: center;
    flex-direction: column;
    margin: 10px 0;
}

.slider-track {
    width: 100%;
    height: 40px;
    background-color: #ddd;
    border-radius: 20px;
    position: relative;
}

.slider-thumb {
    width: 40px;
    height: 40px;
    background-color: #78f79a;
    color: white;
    border-radius: 50%;
    text-align: center;
    line-height: 40px;
    font-weight: bold;
    position: absolute;
    cursor: grab;
    transition: left 0.1s linear;
}

.verified-text {
    margin-top: 10px;
    color: #82f6a1;
    font-size: 14px;
    font-weight: bold;
}

.login-footer {
    margin-top: 20px;
    font-size: 14px;
    color: #999;
}
</style>
