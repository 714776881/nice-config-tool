  
<script lang="ts" setup>
import { ref, computed, onMounted } from 'vue';
import { deepClone } from '@/utils/tool';
import { App } from 'ant-design-vue';
import { replaceSqlInGlobalState } from '../crud.js'
import { fetchCrudData, fetchCrudExeSql, fetchCrudExeBatchSql } from '@/service/api/crud'


const props = defineProps(['config'])
const emit = defineEmits(['search'])
const searchState = defineModel()

const formItems = ref(deepClone(props.config.columnConfig.filter(item => item.isQuery)))
const onload = () => {
    formItems.value.forEach(item => {
        if (item.formItem && item.formItem.rules) {
            item.formItem.rules.forEach(rule => {
                rule.required = false;
            })
        }
        else {
            item.formItem = {};
            item.formItem.rules = [];
        }

        // 默认可清空
        if (!item.component.props.allowClear) {
            item.component.props.allowClear = true;
        }
        item.component.props.disabled = false;

        searchState.value[item.key] = undefined;

        // 状态默认正常
        if (item.key == "DELETED") {
            searchState.value[item.key] = "0";
        }
    });

    // 加载字典数据
    formItems.value.forEach(item => {
        if (item.dicSql) {
            fetchCrudData(item.dicSql).then(async res => {
                const options = []
                res.data.data.forEach(element => {
                    options.push({
                        label: element.LABEL, value: element.VALUE
                    });
                });
                item.component.props.options = options
                item.formItem.allOptions = options
            })
        }
    })
}
onMounted(onload)

const cancelForm = () => {
    searchState.value = {}
}

const submitForm = () => {
    emit('search', searchState);
}

const change = (item) => {
    // 根据item的变化，更新表单中受影响的元素的候选项
    const affectedItmes = formItems.value.filter(i => {
        if (i.formItem.groupBy) {
            return i.formItem.groupBy.includes(item.key)
        }
        return false
    })
    if (affectedItmes && affectedItmes.length > 0) {
        affectedItmes.forEach(i => {
            i.component.props.options = i.formItem.allOptions.filter(option => {
                return option.group == searchState.value[i.key]
            })
        })
    }

    // 选择器和单选框，查询条件变化后，立即查询
    var compName = item.component.name;
    if (compName == "a-select" || compName == "a-radio-group") {
        emit('search', searchState);
    }
}

</script>
  
<template>
    <a-space direction="vertical" style="width: 100%;">
        <!-- 查询表单 -->
        <a-form v-if="formItems.length > 0" :model="searchState" ref="formRef" style="justify-content: space-between;"
            @submit="" layout="inline">

            <!-- 查询条件 -->
            <div style="margin-bottom:12px;" v-for="(item, index) in formItems">
                <a-form-item :key="index" :label="item.title" :rules="item.formItem.rules">
                    <component :is="item.component.name" v-bind="item.component.props" v-model:value="searchState[item.key]"
                        @change="change(item)" />
                </a-form-item>
            </div>

            <!-- 查询按钮 -->
            <a-form-item>
                <a-space>
                    <a-button type="primary" @click="submitForm">
                        <template #icon>
                            <SearchOutlined />
                        </template>
                        查询
                    </a-button>
                    <a-button @click="cancelForm">
                        <template #icon>
                            <ReloadOutlined />
                        </template>
                        清空
                    </a-button>
                </a-space>
            </a-form-item>
        </a-form>
    </a-space>
</template>

<style scoped></style>