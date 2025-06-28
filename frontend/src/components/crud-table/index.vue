<!-- DB CRUD 通过拼接SQL实现对数据库的增删改查 -->
<script lang="ts" setup>
import { ref, onBeforeMount, computed } from 'vue'
import crudtable from './table/index.vue';
import { replaceSql, addWhereSql, uuid, deepClone } from '@/utils/tool'
import { fetchCrudData, fetchCrudExeSql, fetchCrudExeBatchSql } from '@/service/api/crud'
import { useConfig } from '@/stores/config'
import { fetchGetConfigFile, fetchPostConfigFile } from '@/service/api/file'
import { useGlobalStore } from '@/stores/global'
import { App, message } from 'ant-design-vue';
import { useAuthStore } from '@/stores/auth'
import { replaceSqlInGlobalState } from './crud'

const props = defineProps(['configFileName', 'state'])
const config = ref<App.Crud.Config>()
const configStore = useConfig()

const loadConfig = async () => {
    const file = await configStore.getConfig(props.configFileName)
    config.value = deepClone(file.fileContent)

    if (config.value.columnConfig) {
        config.value.columnConfig.forEach(item => {
            if (item.dicSql) {
                const exeSql = replaceSqlByState(item.dicSql, item)
                fetchCrudData(exeSql).then(res => {
                    var options = []
                    res.data.data.forEach(element => {
                        options.push({
                            label: element.LABEL, value: element.VALUE
                        });
                    });
                    item.component.props.options = options
                })
            }
        })
    }
};

onBeforeMount(loadConfig)

const data = ref<any[]>()

const fetchData = (searchState: any) => {

    // 将searchState中有值的内容拼接成SQL
    var sql = addWhereSql(config.value.sqlTemplates.select, searchState)
    sql = replaceSqlByState(sql, searchState)

    console.log('查询SQL:' + sql)

    return new Promise((resolve, reject) => {
        fetchCrudData(sql).then((res) => {
            data.value = res.data.data
            resolve(res)
        }).catch((err) => {
            reject(err)
        })
    })
}

const fetchCreate = (fromState: any) => {

    const sql = replaceSqlByState(config.value.sqlTemplates.insert, fromState)

    console.log('插入SQL:' + sql)

    return new Promise((resolve, reject) => {
        fetchCrudExeSql(sql).then((res: any) => {
            resolve(res)
        }).catch((err: any) => {
            reject(err)
        })
    })
}

const fetchEdit = (fromState: any) => {
    const sql = replaceSqlByState(config.value.sqlTemplates.update, fromState)

    console.log('更新SQL:' + sql)

    return new Promise((resolve, reject) => {
        fetchCrudExeSql(sql).then((res: any) => {
            resolve(res)
        }).catch((err: any) => {
            reject(err)
        })
    })
}


const fetchSetState = (fromState: any) => {
    debugger;
    const sql = replaceSqlByState(config.value.sqlTemplates.setState, fromState);

    console.log('更新状态SQL:' + sql);

    return new Promise((resolve, reject) => {
        fetchCrudExeSql(sql).then((res: any) => {
            resolve(res)
        }).catch((err: any) => {
            reject(err)
        })
    })
}

const fetchBatchRemove = (selectedRowKeys: any) => {
    const sqls = selectedRowKeys.map(key => {
        return replaceSqlByState(config.value.sqlTemplates.delete, { ID: key })
    })

    console.log('删除SQL:' + sqls)

    return new Promise((resolve, reject) => {
        fetchCrudExeBatchSql(sqls).then((res: any) => {
            resolve(res)
        }).catch((err: any) => {
            reject(err)
        })
    })
}
// 交换排序
const fetchSwitchSort = (a, b) => {
    const sqls = []

    const sequence = b.SEQUENCE
    b.SEQUENCE = a.SEQUENCE
    a.SEQUENCE = sequence

    sqls.push(replaceSqlByState(config.value.sqlTemplates.setSequence, a))
    sqls.push(replaceSqlByState(config.value.sqlTemplates.setSequence, b))

    console.log("交换排序" + sqls)

    return new Promise((resolve, reject) => {
        fetchCrudExeBatchSql(sqls).then((res: any) => {
            resolve(res)
        }).catch((err: any) => {
            reject(err)
        })
    })
}

const fetchExportData = (searchState: any) => {
    var sql = addWhereSql(config.value.sqlTemplates.exportData, searchState)
    sql = replaceSqlByState(sql, searchState)

    console.log('导出SQL:' + sql)

    return new Promise((resolve, reject) => {
        fetchCrudData(sql).then((res) => {
            console.log('导出数据:' + res.data.data)
            resolve(res.data.data)
        }).catch((err) => {
            reject(err)
        })
    })
}

const fetchImportData = async (data) => {
    const sqls = [];

    for (const item of data) {
        debugger;
        if (item.ID && item.ID.toString().trim() !== '') {
            // 已有 ID，生成更新语句
            sqls.push(replaceSqlByState(config.value.sqlTemplates.update, item));
        } else {

            // 没有 ID，生成新 ID 并插入
            item.ID = uuid();

            for (const column of config.value.columnConfig) {
                if (column.formItem.defaultValueBySql === undefined) {
                    continue;
                }
                if (item[column.key] && item[column.key].toString().trim() !== '') {
                    continue;
                }
                // 获取默认值
                try {
                    const res = await fetchCrudData(column.formItem.defaultValueBySql);
                    res.data.data.forEach((element) => {
                        item[column.key] = element.VALUE;
                    });
                } catch (err) {
                    console.error(`获取默认值失败: ${err}`);
                }
            }

            // 生成插入语句
            sqls.push(replaceSqlByState(config.value.sqlTemplates.insert, item));
        }
    }

    console.log('导入SQL:', sqls);

    // 执行批量 SQL
    return new Promise((resolve, reject) => {
        fetchCrudExeBatchSql(sqls)
            .then((res) => {
                resolve(res);
            })
            .catch((err) => {
                reject(err);
            });
    });
};


// 替换sql中的状态变量
const replaceSqlByState = (sql: string, state: any) => {
    // 替换父组件状态
    if (props.state) {
        sql = replaceSql(sql, props.state, "SUPER")
    }
    sql = replaceSqlInGlobalState(sql, state)
    return sql
}

// 附加操作的表单和行为
const crudtableRef = ref(null);
const isAddedModalVisible = ref(false)
const addedState = ref({})
const addedItem = ref<App.Crud.AddedButton>()
const showAddButtonModal = (item, record) => {
    addedItem.value = item
    addedState.value = record
    isAddedModalVisible.value = true
}
const submitAddedForm = async () => {
    if (addedItem.value && addedItem.value.exeSql && addedItem.value.exeSql.trim() != '') {
        const sqls = addedItem.value.exeSql.split(";").map(sql => { return replaceSqlByState(sql, addedState.value) })
        console.log('附加操作 ' + addedItem.value.name + ' SQL:' + sqls)

        return new Promise((resolve, reject) => {
            fetchCrudExeBatchSql(sqls).then((res: any) => {
                // 附加操作执行后，需要再次从后端获取数据来刷新状态
                crudtableRef.value.searchData()
                isAddedModalVisible.value = false
                message.success('提交成功！')
            }).catch((err: any) => {
                message.error('提交失败！' + err)
            })
        })
    }
    isAddedModalVisible.value = false
}

const cancelAddedForm = () => {
    isAddedModalVisible.value = false
}

</script>

<template>
    <!-- 表格 -->
    <crudtable ref="crudtableRef" v-if="config && config.columnConfig" :config="config"
        :config-file-name="props.configFileName" v-model="data" :fetch-data="fetchData" :fetch-create="fetchCreate"
        :fetch-edit="fetchEdit" :fetch-batch-remove="fetchBatchRemove" :fetch-set-state="fetchSetState"
        :fetch-switch-sort="fetchSwitchSort" :fetch-export-data="fetchExportData" :fetch-import-data="fetchImportData"
        :superState="props.state" :show-add-button-modal="showAddButtonModal">
    </crudtable>

    <!-- 扩展操作 -->
    <a-modal v-if="addedItem" :width="addedItem.width" v-model:visible="isAddedModalVisible" :title="addedItem.name" @ok="submitAddedForm"
        @cancel="cancelAddedForm">
        <slot :name="addedItem.name" :item="addedItem" :state="addedState">
            {{ addedItem.defaultShowText }}
        </slot>
    </a-modal>
</template>