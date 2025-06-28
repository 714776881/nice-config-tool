<script lang="ts" setup>
import { ref, computed, onMounted, watch } from 'vue';
import { deepClone } from '@/utils/tool';
import { fetchCrudData } from '@/service/api/crud'
import Scope from '../../../common/scope.vue'
import { userState } from '../../composables/useState';
import { logger } from '@/utils/logger';
import { useAuthStore } from '@/stores/auth'

const {scopeState} = useAuthStore()

const buildSql = userState().buildSql;

// Model
const props = defineProps(['config'])
const emit = defineEmits(['search'])
const searchState = defineModel()

const formItems = ref(deepClone(props.config.columnConfig.filter(item => item.isQuery)))

watch(() => scopeState, async (val) => {
    onload();
},{deep: true})

// Control
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
            var sql = buildSql(item.dicSql,searchState.value);
            fetchCrudData(sql).then(async res => {
                logger.info('[字典]', sql)
                const options = []
                res.data.data.forEach(element => {
                    options.push({
                        label: element.LABEL, value: element.VALUE, category: element.CATEGORY
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
                if (option.category == undefined) {
                    return false
                }
                if (searchState.value[i.formItem.groupBy] == undefined) {
                    return true
                }
                return option.category.trim() == searchState.value[i.formItem.groupBy].trim()
            })
        })
    }

    // 选择器和单选框，查询条件变化后，立即查询
    var compName = item.component.name;
    if (compName == "a-select" || compName == "a-radio-group") {
        emit('search', searchState);
    }
}



const rangeChange = () => {
    emit('search', searchState);
}

const handleEnter = () => {
    emit('search', searchState);
}

</script>

<template>
    <a-space direction="vertical" style="width: 100%;">
        <!-- 查询表单 -->
        <a-form v-if="formItems.length > 0" :model="searchState" ref="formRef" style="justify-content: space-between;"
            @submit="" layout="inline">
            <Scope @change="rangeChange"></Scope>
            <!-- 查询条件 -->
            <div style="margin-bottom:12px;" v-for="(item, index) in formItems">
                <a-form-item :key="index" :label="item.title" :rules="item.formItem.rules">
                    <component :is="item.component.name" v-bind="item.component.props"
                        v-model:value="searchState[item.key]" @change="change(item)" @pressEnter="handleEnter" />
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