<template>
    <!-- 院区、医院、科室的级联选择器 -->
    <a-cascader v-model:value="value" :options="options" placeholder="Please select" />

</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import type { CascaderProps } from 'ant-design-vue';
import {fetchScopeOptions} from '@/service/api/scope'

const options = ref<CascaderProps['options']>([]);

const value = ref<string[]>([]);

const loadOptions = async () =>
{   
    const userId = "0123"
    
    const { data } = await fetchScopeOptions(userId)
    if(data)
    {
        
    }
}

onMounted(loadOptions);
</script>
