<template>
    <div style="margin: 0px 20px">
        <a-form :model="model" >
            <a-form-item label="病人类型" name="PatType" :rules="[{ required: true, message: '请选择病人类型' }]">
                <a-select v-model:value="model.PatType" placeholder="病人类型">
                    <a-select-option value="">不限</a-select-option>
                    <a-select-option value="O">门诊病人</a-select-option>
                    <a-select-option value="I">住院病人</a-select-option>
                    <a-select-option value="E">急诊病人</a-select-option>
                    <a-select-option value="P">体检病人</a-select-option>
                </a-select>
            </a-form-item>
            <a-form-item label="加密算法" name="Encrypt" :rules="[{ required: true, message: '请选择加密方式' }]">
                <a-select v-model:value="model.Encrypt" placeholder="加密方式">
                    <a-select-option value="0">不加密</a-select-option>
                    <a-select-option value="1">Base64加密</a-select-option>
                    <a-select-option value="2">Token加密</a-select-option>
                </a-select>
            </a-form-item>
            <a-form-item label="地址类型" name="UrlType" :rules="[{ required: true, message: '请选择URL类型' }]">
                <a-select v-model:value="model.UrlType" placeholder="URL类型">
                    <a-select-option value="WEB">调用地址</a-select-option>
                    <a-select-option value="EXE">调用客户端</a-select-option>
                    <a-select-option value="EXEURL">调用客户端URL</a-select-option>
                </a-select>
            </a-form-item>
            <a-form-item label="调用地址" name="Url" :rules="[{ required: true, message: '请输入URL地址' }]">
                <a-input v-model:value="model.Url" placeholder="URL" />
            </a-form-item>
            <a-form-item label="{启动参数}" name="Argument">
                <a-textarea v-model:value="model.Argument" placeholder="Argument" />
            </a-form-item>
        </a-form>
    </div>
</template>

<script lang="ts" setup>
import { ref, watch } from 'vue';
import { message } from 'ant-design-vue';

// model
const modelValue = defineModel<string>('value');

const props = defineProps<{
    state?: {};
}>();


const model = ref<string>({
    Modality: "",
    PatType: "",
    Encrypt: "",
    UrlType: "",
    Url: "",
    Argument: "",
});

watch(modelValue, (newValue) => {
    if (!newValue || newValue.length === 0) {
        model.value = {
            Modality: "",
            PatType: "",
            Encrypt: "0",
            UrlType: "WEB",
            Url: "https//www.baidu.com",
            Argument: "",
        };
        return;
    }

    try {
        model.value = JSON.parse(newValue);
    } catch (error) {
        message.error("Invalid JSON format");
    }
}, { immediate: true });

watch(model, (newValue) => {
    if (!newValue) {
        modelValue.value = '';
        return;
    }
    modelValue.value = JSON.stringify(newValue);
}, { deep: true });


watch(() => props.state, (newValue) => {
    model.value.Modality = props.state.MODALITY || "";
}, { immediate: true , deep: true });

</script>