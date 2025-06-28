<!--
    登录页面
    输入用户名、密码、验证码，点击登录按钮，登录成功后跳转至首页
-->
<template>
    <div class="login-container">
        <div class="login-box">
            <div class="login-title">登录产科配置工具</div>
            <div class="login-form">
                <el-input v-model="loginForm.username" placeholder="用户名" clearable></el-input>
                <el-input v-model="loginForm.password" placeholder="密码" type="password" clearable></el-input>
                <el-button type="primary" @click="login">登录</el-button>
                <div class="login-footer">Copyright © 2023</div>
            </div>
        </div>
    </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'

const loginForm = ref({
    username: '',
    password: ''
})

const login = () => {
    // 登录

}

</script>

<style>
.login-container {
    background-color: #f0f2f5;
    height: 100vh;
    display: flex;
    justify-content: center;
    align-items: center;
}

.login-box {
    width: 400px;
    height: 300px;
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

.login-footer {
    margin-top: 20px;
    font-size: 14px;
    color: #999;
}
</style>