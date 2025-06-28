<template>
    <a-date-picker
        v-model:value="model"
        :placeholder="'选择日期'"
        :format="'YYYY-MM-DD'"
        :show-time="true"
        :allow-clear="true"
        :locale="locale"
    ></a-date-picker>
</template>
<script lang="ts" setup>
import { ref, watch } from 'vue';
import dayjs, { Dayjs } from 'dayjs';
import locale from 'ant-design-vue/es/date-picker/locale/zh_CN';

const modelValue = defineModel<string>('value');
const model = ref<Dayjs | null>(null);

watch(modelValue, (newValue) => {
    if (newValue) {
        model.value = dayjs(newValue);
    } else {
        model.value = null;
    }
});

watch(model, (newValue) => {
    if (newValue) {
        modelValue.value = newValue.format('YYYY-MM-DD');
    } else {
        modelValue.value = '';
    }
});


</script>