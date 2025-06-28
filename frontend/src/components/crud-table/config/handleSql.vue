
<script lang="ts" setup>
import { ref } from 'vue';
import { useConfig } from "@/stores/config"


const props = defineProps(['config'])

const sqlTemplates = props.config.sqlTemplates

const panes = ref<{ title: string; content: string; key: string; closable?: boolean }[]>([
    { title: '查询', content: sqlTemplates.select, key: '1', closable: false },
    { title: '新增', content: sqlTemplates.insert, key: '2', closable: false },
    { title: '更新', content: sqlTemplates.update, key: '3', closable: false },
    { title: '删除', content: sqlTemplates.delete, key: '4', closable: false },
    { title: '导出', content: sqlTemplates.exportData, key: '5', closable: false },
]);

const formModel = ref();

const activeKey = ref(panes.value[0].key);

const newTabIndex = ref(0);

const switchChecked = ref(false)

const add = () => {
    activeKey.value = `newTab${++newTabIndex.value}`;
    panes.value.push({ title: 'New Tab', content: 'Content of new Tab', key: activeKey.value });
};

const remove = (targetKey: string) => {
    let lastIndex = 0;
    panes.value.forEach((pane, i) => {
        if (pane.key === targetKey) {
            lastIndex = i - 1;
        }
    });
    panes.value = panes.value.filter(pane => pane.key !== targetKey);
    if (panes.value.length && activeKey.value === targetKey) {
        if (lastIndex >= 0) {
            activeKey.value = panes.value[lastIndex].key;
        } else {
            activeKey.value = panes.value[0].key;
        }
    }
};

const onEdit = (targetKey: string | MouseEvent, action: string) => {
    if (action === 'add') {
        add();
    } else {
        remove(targetKey as string);
    }
};
</script>
<template>
    <a-tabs size="small" v-model:activeKey="activeKey" type="editable-card" @edit="onEdit">
        <a-tab-pane v-for="pane in panes" :key="pane.key" :tab="pane.title" :closable="pane.closable">
            <a-textarea v-model:value="pane.content" placeholder="Basic usage" :rows="4" />
            <!--
            <a-space direction="vertical" style="width: 100%;">
                <a-form ref="formRef" :model="formModel" layout="inline">
                    <a-form-item label="">
                        <a-switch v-model:checked="switchChecked" checked-children="生成" un-checked-children="关" />
                    </a-form-item>
                </a-form>
            </a-space>
            -->
        </a-tab-pane>
    </a-tabs>
</template>