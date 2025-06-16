
<!---->
<script setup lang="ts">

import { ref } from 'vue';
import { useTheme } from '../../stores/theme';
import { ValidateErrorEntity } from 'ant-design-vue/es/form/interface';
import { deepClone } from '@/utils/tool';

const theme = useTheme();

const columns = [
    { title: '名称', dataIndex: 'label', key: 'label' },
    { title: '编码', dataIndex: 'key', key: 'key' },
    { title: '图标', dataIndex: 'icon', key: 'icon' },
    { title: '路由路径', dataIndex: 'path', key: 'path' },
    { title: '操作', key: 'action', width: '20%' },
];

const formRef = ref<any>(null);
const menuData = ref(deepClone(theme.menuConfig))// 确保 menuData 是响应式的
const isAddModalVisible = ref(false);
const isAddingChildren = ref(false); // 标识是否为子项添加
const formModel = ref({
    label: '',
    key: '',
    icon: '',
    path: '',
});
let editingKey: string | null = null;
let parentKey: string | null = null; // 记录当前选择的父节点

function showAddMenuModal() {
    formModel.value = { label: '', key: '', icon: '', path: '' };
    isAddingChildren.value = false;
    isAddModalVisible.value = true;
}

function addChildrenMenuModal(record) {
    formModel.value = { label: '', key: '', icon: '', path: '' };
    parentKey = record.key; // 记录选择的父节点
    isAddingChildren.value = true;
    isAddModalVisible.value = true;
}

function showEditMenuModal(record) {
    formModel.value = { ...record };
    editingKey = record.key;
    isAddingChildren.value = false;
    isAddModalVisible.value = true;
}

function handleAddOk() {
    if (!formRef.value) return;

    formRef.value.validate()
        .then(() => {
            const newItem = { ...formModel.value };
            if (isAddingChildren.value && parentKey) {
                // 添加子项
                addMenuItemToParent(menuData.value, newItem, parentKey);
            } else if (editingKey) {
                // 编辑模式
                updateMenuData(menuData.value, editingKey, newItem);
            } else {
                // 添加主菜单项
                menuData.value.push(newItem);
            }
            isAddModalVisible.value = false;
            editingKey = null;
            parentKey = null; // 重置父节点
        })
        .catch((error: ValidateErrorEntity) => {
            console.log('error', error);
        });
}

function handleAddCancel() {
    isAddModalVisible.value = false;
    parentKey = null; // 重置父节点
}

function deleteMenuItem(key) {
    removeMenuItem(menuData.value, key);
}

function moveUp(key) {
    moveMenuItem(menuData.value, key, -1);
}

function moveDown(key) {
    moveMenuItem(menuData.value, key, 1);
}

function isMoveUpDisabled(key) {
    return isMoveDisabled(menuData.value, key, -1);
}

function isMoveDownDisabled(key) {
    return isMoveDisabled(menuData.value, key, 1);
}

function isAddChildrenDisabled(record) {
    if (record.path == undefined || record.path == "") {
        return false;
    }
    return true;
}

function exportConfig() {
    const blob = new Blob([JSON.stringify(menuData.value, null, 2)], { type: 'application/json' });
    const url = URL.createObjectURL(blob);
    const a = document.createElement('a');
    a.href = url;
    a.download = 'menu-config.json';
    a.click();
    URL.revokeObjectURL(url);
}

function handleImport(file) {
    const reader = new FileReader();
    reader.onload = (e) => {
        try {
            const importedData = JSON.parse(e.target.result);
            menuData.value = [...importedData]; // 确保导入的数据也保持响应式
        } catch (error) {
            alert('导入失败，请检查文件格式。');
        }
    };
    reader.readAsText(file.file);
    return false;
}

function addMenuItemToParent(data, newItem, parentKey) {
    for (const item of data) {
        if (item.key === parentKey) {
            item.children = item.children || [];
            item.children.push(newItem);
            return;
        }
        if (item.children) {
            addMenuItemToParent(item.children, newItem, parentKey);
        }
    }
}

function updateMenuData(data, key, newItem) {
    for (const item of data) {
        if (item.key === key) {
            Object.assign(item, newItem);
            return;
        }
        if (item.children) {
            updateMenuData(item.children, key, newItem);
        }
    }
}

function removeMenuItem(data, key) {
    for (let i = 0; i < data.length; i++) {
        if (data[i].key === key) {
            data.splice(i, 1);
            return;
        }
        if (data[i].children) {
            removeMenuItem(data[i].children, key);
        }
    }
}

function moveMenuItem(data, key, direction) {
    for (let i = 0; i < data.length; i++) {
        if (data[i].key === key) {
            const index = i;
            const newIndex = index + direction;
            if (newIndex >= 0 && newIndex < data.length) {
                [data[index], data[newIndex]] = [data[newIndex], data[index]];
            }
            return;
        }
        if (data[i].children) {
            moveMenuItem(data[i].children, key, direction);
        }
    }
}

function isMoveDisabled(data, key, direction) {
    for (let i = 0; i < data.length; i++) {
        if (data[i].key === key) {
            const index = i;
            const newIndex = index + direction;
            return newIndex < 0 || newIndex >= data.length;
        }
        if (data[i].children) {
            if (isMoveDisabled(data[i].children, key, direction)) {
                return true;
            }
        }
    }
    return false;
}

</script>
<template>
    <a-space direction="vertical" style="width: 100%;">
        <div style="display: flex; justify-content: space-between; margin-bottom: 1px;">
            <!-- 左边的按钮 -->
            <a-space>
                <a-button type="primary" @click="showAddMenuModal">添加菜单项</a-button>
                <a-button type="primary" @click="">提交</a-button>
            </a-space>

            <!-- 右边的按钮 -->
            <a-space>
                <a-button @click="exportConfig">
                    <template #icon>
                        <DownloadOutlined />
                    </template></a-button>
                <a-upload :beforeUpload="handleImport" show-upload-list="false">
                    <a-button>
                        <template #icon>
                            <CloudUploadOutlined />
                        </template>
                    </a-button>
                </a-upload>
            </a-space>
        </div>

        <a-table size="small" :columns="columns" :dataSource="menuData" rowKey="key" style="margin-bottom: 16px;" bordered>
            <template #bodyCell="{ column, record }">
                <template v-if="column.key === 'action'">
                    <a-space>
                        <a-button ghost type="primary" size="small" @click="addChildrenMenuModal(record)"
                            :disabled="isAddChildrenDisabled(record)">添加子项</a-button>
                        <a-button ghost type="primary" size="small" @click="showEditMenuModal(record)">编辑</a-button>
                        <a-button size="small" @click="moveUp(record.key)"
                            :disabled="isMoveUpDisabled(record.key)">上移</a-button>
                        <a-button size="small" @click="moveDown(record.key)"
                            :disabled="isMoveDownDisabled(record.key)">下移</a-button>
                        <a-button danger size="small" @click="deleteMenuItem(record.key)">删除</a-button>
                    </a-space>
                </template>
            </template>
        </a-table>

        <a-modal v-model:visible="isAddModalVisible" :title="isAddingChildren ? '添加子菜单项' : '添加菜单项'" @ok="handleAddOk"
            @cancel="handleAddCancel">
            <a-form ref="formRef" :model="formModel" layout="vertical">
                <a-form-item required label="名称" name="label">
                    <a-input v-model:value="formModel.label" />
                </a-form-item>
                <a-form-item required label="编码" name="key">
                    <a-input v-model:value="formModel.key" />
                </a-form-item>
                <a-form-item required label="图标" name="icon">
                    <a-input v-model:value="formModel.icon" />
                </a-form-item>
                <a-form-item label="路由路径" name="path">
                    <a-input v-model:value="formModel.path" />
                </a-form-item>
            </a-form>
        </a-modal>
    </a-space>
</template>
  
