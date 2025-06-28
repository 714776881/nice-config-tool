<template>
    <!-- 密码输入框，显示密码复杂度 -->
    <div>
      <a-input type="password" v-model="password" @input="checkPasswordStrength" />
      <div>{{ passwordStrength }}</div>
    </div>
</template>
  
<script setup lang="ts">
  import { ref,watch } from 'vue';
  
  const password = defineModel<string>('value');

  const passwordStrength = ref('');
  
  const checkPasswordStrength = () => {
    const value = password.value;
    if (value.length < 6) {
      passwordStrength.value = '密码太短';
    } else if (/[a-z]/.test(value) && /[A-Z]/.test(value) && /\d/.test(value)) {
      passwordStrength.value = '强';
    } else if (/[a-zA-Z]/.test(value) && /\d/.test(value)) {
      passwordStrength.value = '中';
    } else {
      passwordStrength.value = '弱';
    }
  };
</script>