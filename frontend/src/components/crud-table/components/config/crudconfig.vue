<script lang="ts" setup>
import { reactive, ref } from 'vue';
import type { Rule } from 'ant-design-vue/es/form';
import handleSql from './handleSql.vue';
import columnconfig from './columnconfig.vue';

const props = defineProps(['config'])

const form = reactive({
    name: '',
    url: '',
});

const rules: Record<string, Rule[]> = {
    url: [{ required: true, message: 'please enter url' }],
};

const open = ref<boolean>(false);

const showDrawer = () => {
    open.value = true;
};

const onClose = () => {
    open.value = false;
};
</script>
<template>
    <a-tooltip placement="bottom">
        <template #title>配置表格</template>
        <a-button @click="showDrawer">
            <template #icon>
                <SettingOutlined />
            </template>
        </a-button>
    </a-tooltip>
    <a-drawer title="修改表格配置" :width="1000" :open="open" :body-style="{ paddingBottom: '20px' }"
        :footer-style="{ textAlign: 'right' }" @close="onClose">
        <a-form :model="form" :rules="rules" layout="vertical">
            <a-row :gutter="16">
                <a-col :span="24">
                    <a-form-item label="" name="name">
                        <columnconfig :config="props.config"></columnconfig>
                    </a-form-item>
                </a-col>
            </a-row>
            <a-row :gutter="16">
                <a-col :span="24">
                    <a-form-item label="" name="name">
                        <handleSql :config="props.config"></handleSql>
                    </a-form-item>
                </a-col>
            </a-row>
        </a-form>
        <template #extra>
            <a-space>
                <a-button @click="onClose">取消</a-button>
                <a-button type="primary" @click="onClose">提交</a-button>
            </a-space>
        </template>
    </a-drawer>
</template>
