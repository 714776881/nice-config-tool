  
<script lang="ts" setup>
import { ref, computed, onMounted, onActivated, nextTick } from 'vue';
import { fetchCrudData, fetchCrudExeSql, fetchCrudExeBatchSql } from '@/service/api/crud'
import { watch } from 'vue';
import { replaceSql, deepClone } from '@/utils/tool'
import { useAuthStore } from '@/stores/auth'
import { useValid, replaceSqlInGlobalState } from '../crud.js'

const props = defineProps(['config', 'formType', 'state'])
const fromState = defineModel()

const labelCol = { style: { width: '100px' } };
const wrapperCol = {};

const formItems = ref<App.Crud.ColumnConfig[]>(props.config.columnConfig)
const formRef = ref(null);
// 加载表单验证规则的扩展
const onloadFormItems = () => {
    formItems.value.forEach(item => {
        if (item.formItem && item.formItem.rules) {
            item.formItem.rules.forEach(rule => {
                if (rule.validator) {
                    rule.validator = useValid(rule.validator)
                }
            })
        }
    });
}

// 计算表单项目的宽度占位比例
const getSpan = (node) => {
    if (node.formItem != undefined && node.formItem.span != undefined) {
        return node.formItem.span
    }
    return formItems.value.length < 8 ? 24 : 12
}

// 替换sql中的状态变量
const replaceSqlByState = (sql: string, state: any) => {
    // 替换父组件状态
    if (props.state) {
        sql = replaceSql(sql, props.state, "SUPER")
    }
    sql = replaceSqlInGlobalState(sql, state)
    return sql
}

// 监听表单状态的变化
watch(fromState, async (newValue, oldValue) => {
    if (props.formType == 'add') {
        formItems.value.forEach(item => {
            if (item.formItem.defaultValueBySql) {
                fetchCrudData(item.formItem.defaultValueBySql).then(res => {
                    res.data.data.forEach(element => {
                        fromState.value[item.key] = element.VALUE
                    });
                })
            }
        });
    }
    else {
    }
    // 加载字典数据
    formItems.value.forEach(async item => {
        if (item.dicSql) {
            const exeSql = replaceSqlByState(item.dicSql, newValue)
            const res = await fetchCrudData(exeSql);
            if (res.data) {
                const options = []
                res.data.data.forEach(element => {
                    options.push({
                        label: element.LABEL, value: element.VALUE, category: element.CATEGORY
                    });
                });
                item.component.props.options = options
                item.formItem.allOptions = options
            }
        }
    })
}, { immediate: true })

// 表单值变化时，更新候选项
const change = (item: App.Crud.ColumnConfig) => {
    const affectedItmes = formItems.value.filter(i => {
        if (i.formItem.groupBy) {
            return i.formItem.groupBy.includes(item.key)
        }
        return false
    })
    if (affectedItmes && affectedItmes.length > 0) {
        affectedItmes.forEach(i => {
            i.component.props.options = i.formItem.allOptions.filter(option => {
                return option.group == fromState.value[i.key]
            })
        })
    }
}
// 判断新增时，元素是否显示
function IsAddShow(item: App.Crud.ColumnConfig) {
    if (props.formType == 'add') {
        return item.formItem.addShow != undefined ? item.formItem.addShow : true
    }
    else {
        return true;
    }
}

// 表单提交时进行规则验证
const submitForm = async () => {
    try {
        await formRef.value.validate();
        return true;
    } catch (error) {
        return false;
    }
};

defineExpose({
    submitForm
})

onMounted(onloadFormItems)
</script>

<template>
    <a-space direction="vertical" style="width: 100%;">
        <!-- 查询表单 -->
        <a-form ref="formRef" :label-col="labelCol" :wrapper-col="wrapperCol" :model="fromState" style="margin:16px;"
            @submit="submitForm" :layout="formItems.length >= 8 ? 'horizontal' : 'vertical'"
            :bodyStyle="{ padding: '80px' }">
            <a-row :gutter="24">
                <a-col v-for="(item, index) in formItems" :key="index" :span="getSpan(item)">
                    <!-- 动态生成表单 -->
                    <a-form-item v-show="item.isEdit && IsAddShow(item)" :label="item.title" :rules="item.formItem.rules"
                        :name="item.key">
                        <component :key="index" :is="item.component.name" v-bind="item.component.props" :state="fromState"
                            v-model:value="fromState[item.key]" @change="change(item)" :style="item.component.props" />
                    </a-form-item>
                </a-col>
            </a-row>
        </a-form>
    </a-space>
</template>
  
<style scoped></style>