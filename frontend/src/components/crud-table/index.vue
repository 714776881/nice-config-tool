<!-- DB CRUD 通过拼接SQL实现对数据库的增删改查 -->
<script lang="ts" setup>
import { ref, onBeforeMount, provide } from 'vue'
import crudtable from './components/table/index.vue';
import { uuid, deepClone } from '@/utils/tool'
import { fetchCrudData, fetchCrudExeSql, fetchCrudExeBatchSql } from '@/service/api/crud'
import { useConfig } from '@/stores/config'
import { App, message } from 'ant-design-vue';
import { userState } from './composables/useState';

const buildSql = (userState().buildSql)

const props = defineProps(['configFileName', 'state'])
const config = ref<App.Crud.Config>()
const configStore = useConfig()

// 向子组件传递配置数据,以便取用
provide('config', config)

const loadConfig = async () => {
    const file = await configStore.getConfig(props.configFileName)
    config.value = deepClone(file.fileContent)

    //loadDic();
};

// 加载字典
const loadDic = async () => {
    if (config.value.columnConfig) {
        config.value.columnConfig.forEach(item => {
            if (item.dicSql) {
                const exeSql = buildSql(item.dicSql, item, props.state)

                if (sqlDic.value.find(sql => sql.key === exeSql)) {
                    // 如果字典已经加载过，则不再加载
                    item.component.props.options = sqlDic.value.find(sql => sql.key === exeSql).options
                }

                fetchCrudData(exeSql).then(res => {
                    var options = []
                    res.data.data.forEach(element => {
                        options.push({
                            label: element.LABEL, value: element.VALUE
                        });
                    });
                    item.component.props.options = options

                    sqlDic.value.push({
                        key: exeSql,
                        options: options
                    })
                })
            }
        })
    }
}

const sqlDic = ref([])


onBeforeMount(loadConfig)

const data = ref<any[]>()

const fetchData = async (searchState: any) => {
    // 处理查询条件
    var sql = buildSql(config.value.sqlTemplates.select, searchState, props.state, searchState)
    console.log('查询SQL:' + sql)

    await loadDic()

    return new Promise((resolve, reject) => {
        fetchCrudData(sql, searchState).then((res) => {
            data.value = res.data.data
            resolve(res)
        }).catch((err) => {
            // 弹窗提醒
            message.error('查询失败！' + err)
            reject(err)
        })
    })
}

const fetchCreate = (fromState: any) => {
    const sql = buildSql(config.value.sqlTemplates.insert, fromState, props.state)
    console.log('插入SQL:' + sql)

    //var uploadFile = null
    //if (fromState.UPLOADFILE) {
    //    uploadFile = fromState.UPLOADFILE
    //    // 删除上传文件字段，避免插入到数据库中
    //    delete fromState.UPLOADFILE
    //}
    return new Promise((resolve, reject) => {
        fetchCrudExeSql(sql, fromState).then((res: any) => {
            resolve(res)
        }).catch((err: any) => {
            reject(err)
        })
    })
}

const fetchEdit = (fromState: any) => {
    const sql = buildSql(config.value.sqlTemplates.update, fromState, props.state)
    console.log('更新SQL:' + sql)

    var state = deepClone({ ...fromState })

    var uploadFile = null
    if (state.UPLOADFILE) {
        uploadFile = state.UPLOADFILE
        // 删除上传文件字段，避免插入到数据库中
        delete state.UPLOADFILE
    }

    return new Promise((resolve, reject) => {
        fetchCrudExeSql(sql, state, uploadFile).then((res: any) => {
            resolve(res)
        }).catch((err: any) => {
            reject(err)
        })
    })
}


const fetchSetState = (fromState: any) => {
    const sql = buildSql(config.value.sqlTemplates.setState, fromState, props.state)
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
        return buildSql(config.value.sqlTemplates.delete, { ID: key }, props.state)
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

    sqls.push(buildSql(config.value.sqlTemplates.setSequence, a, props.state))
    sqls.push(buildSql(config.value.sqlTemplates.setSequence, b, props.state))
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
    // 处理导出SQL
    var sql = buildSql(config.value.sqlTemplates.exportData, searchState, props.state, searchState)
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
        if (item.ID && item.ID.toString().trim() !== '') {
            // 已有 ID，生成更新语句
            sqls.push(buildSql(config.value.sqlTemplates.update, item, props.state));
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
            sqls.push(buildSql(config.value.sqlTemplates.insert, item));
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
    debugger

    if (addedItem.value && addedItem.value.exeSql && addedItem.value.exeSql.trim() != '') {
        const sqls = addedItem.value.exeSql.split(";").map(sql => { return buildSql(sql, addedState.value, props.state) })
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

const changeState = async (state: any) => {
    const sql = buildSql(addedItem.value.exeSql, state)
    return new Promise((resolve, reject) => {
        fetchCrudExeSql(sql, state).then((res: any) => {
            // 附加操作执行后，需要再次从后端获取数据来刷新状态
            crudtableRef.value.searchData()
            isAddedModalVisible.value = false
            message.success('提交成功！')
        }).catch((err: any) => {
            message.error('提交失败！' + err)
        })
    })
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
    <a-modal v-if="addedItem" :width="addedItem.width" v-model:visible="isAddedModalVisible" :title="addedItem.name"
        :footer="null">
        <slot :key="addedState.ID" :name="addedItem.name" :item="addedItem" :state="addedState" :change="changeState" :ok="submitAddedForm"
            @cancel="cancelAddedForm">
            {{ addedItem.defaultShowText }}
        </slot>
    </a-modal>
</template>