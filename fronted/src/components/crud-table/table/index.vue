<script lang="ts" setup>
import { computed, reactive, ref, onMounted } from 'vue';
import { DeleteOutlined, DeleteFilled, EditFilled, PlusOutlined, ArrowUpOutlined, ArrowDownOutlined } from '@ant-design/icons-vue'
import { App, message } from 'ant-design-vue';
import curdConfig from '../config/crudconfig.vue'
import search from '../search/index.vue'
import { toLabel, deepClone, uuid } from '@/utils/tool'
import editfrom from '../editfrom/index.vue'
import * as XLSX from 'xlsx';
import { watch } from 'vue';
import type { key } from 'localforage';

const data = defineModel<any[]>()

const props = defineProps(['fetchData', 'fetchCreate', 'fetchEdit', 'fetchBatchRemove', 'fetchSetState', "fetchSwitchSort", "fetchExportData", "fetchImportData", 'config', 'configFileName', 'superState', 'showAddButtonModal'])

defineExpose({
    searchData
})

const columnConfig = reactive(props.config.columnConfig)

// 表格列表配置项目列表
const columns = computed<any[]>(() => {
    if (columnConfig == undefined) {
        return []
    }

    var columns = columnConfig.filter(item => item.isShow).map(item => {
        const sort = item.isSort ? {
            defaultSortOrder: item.defaultSortOrder,
            sortDirections: item.sortDirections,
            sorter: genericSorter(item.key),
        } : {}

        return {
            title: item.title,
            dataIndex: item.key,
            width: item.width || "100px",
            align: item.align,
            options: item.component.props.options,
            ...sort
        }
    })
    if (!columns.find(item => item.dataIndex.toLowerCase() == 'action')) {
        const action = {
            title: '操作',
            fixed: 'right',
            dataIndex: 'action',
            align: 'center',
            width: "180px",
        }
        if (props.config.addedButtons && props.config.addedButtons.length > 0) {
            action.width = ((props.config.addedButtons.length + 2) * 100) + "px"
        }
        else {
            action.width = "180px";
        }
        //
        if (props.config.crudButtonLocation) {
            columns = [action].concat(columns)
        }
        columns = columns.concat([action])
    }
    return columns;
});


const defaultPageSize = computed(() => {
    if (props.config.tableConfig === undefined) {
        return 10;
    }

    if (props.config.tableConfig.defaultPageSize === undefined) {
        return 10
    }

    return props.config.tableConfig.defaultPageSize
})

// 通用排序函数
const genericSorter = (key) => (a, b) => {
    const valueA = a[key];
    const valueB = b[key];

    if (typeof valueA === "number" && typeof valueB === "number") {
        return valueA - valueB; // 数值比较
    }
    return String(valueA).localeCompare(String(valueB)); // 字符串比较
};

// 已选择的数据
const selectionState = reactive({
    selectedRowKeys: [],
    loading: false,
});

const hasSelected = computed(() => selectionState.selectedRowKeys.length > 0);
const selectedTag = computed(() => `已选择 ${selectionState.selectedRowKeys.length} 项`);
const searchState = ref({})

const cancelSelectedKeys = () => {
    selectionState.loading = true;
    // ajax request after empty completing
    setTimeout(() => {
        selectionState.loading = false;
        selectionState.selectedRowKeys = [];
    }, 400);
};

const allSelectedKeys = () => {
    selectionState.selectedRowKeys = []
    data.value.forEach(item => {
        selectionState.selectedRowKeys.push(item.ID)
    });
}

const exportSelectedKeys = () => {

    const selectData = data.value.filter(item => selectionState.selectedRowKeys.includes(item.ID))
    // 将 JSON 转为 worksheet
    const worksheet = XLSX.utils.json_to_sheet(selectData);

    // 创建一个新的工作簿
    const workbook = XLSX.utils.book_new();

    // 将 worksheet 添加到工作簿中
    XLSX.utils.book_append_sheet(workbook, worksheet, "Sheet1");

    // 生成 Excel 文件并下载
    XLSX.writeFile(workbook, "导出选择-" + props.configFileName.replace('.json', '') + ".xlsx");

}


const onSelectChange = (selectedRowKeys) => {
    console.log('已选择: ', selectedRowKeys);
    selectionState.selectedRowKeys = selectedRowKeys;
};

onMounted(() => {
    searchData()
})

// 小型列表中用于对话框内
const tableSize = ref('')
if (props.superState != undefined) {
    tableSize.value = 'small'
}

watch(() => props.superState, (newValue, oldValue) => {
    searchData()
})

// 查询
function searchData() {
    props.fetchData(searchState.value).then(res => {
        //message.success('查询成功');
        if (selectionState.selectedRowKeys == undefined || selectionState.selectedRowKeys.length == 0) {
            return
        }
        selectionState.selectedRowKeys = selectionState.selectedRowKeys.filter(key => data.value.find(item => item.ID == key))
    })
}

// 弹窗状态
const isModalVisible = ref(false)
const isAddedModalVisible = ref(false)

const fromTitle = ref('新增')
const fromState = ref<App.Crud.FormState>()
const formType = ref<"edit" | "add" | "view">()
// 新增
const showAddModal = () => {
    fromTitle.value = '新增'
    formType.value = "add"
    isModalVisible.value = true;
    fromState.value = {}
    deepClone(props.config.columnConfig).forEach(item => {
        if (item.formItem.defaultValue) {
            fromState.value[item.key] = item.formItem.defaultValue;
        }
        else {
            fromState.value[item.key] = ''
        }
    })
}

// 编辑
const showEditModal = (record) => {
    fromTitle.value = '编辑'
    formType.value = "edit"
    isModalVisible.value = true;
    fromState.value = deepClone(record)
}

// 附加操作
const showAddButtonModal = (item, record) => {
    props.showAddButtonModal(item, record)
}

// 关闭
const cancelForm = () => {
    isModalVisible.value = false;
}

const editFrom = ref(null)
// 提交
const submitForm = async () => {
    if (editFrom.value) {
        const isValid = await editFrom.value.submitForm()
        if (!isValid) {
            return
        }
    }
    if (!fromState.value.ID) {
        fromState.value.ID = uuid()
        props.fetchCreate(fromState.value).then(res => {
            data.value.push(fromState.value)
            message.success('新增成功！')
            searchData()
        }).catch(err => {
            message.error('新增失败！\n' + err)
        })
    }
    else {
        props.fetchEdit(fromState.value).then(res => {
            const index = data.value.findIndex(item => item.ID == fromState.value.ID)
            data.value[index] = fromState.value
            message.success('提交成功！')
        }).catch(err => {
            message.error('提交失败！' + err)
        })
    }
    isModalVisible.value = false;
}

// 批量删除
const batchDeleteItem = () => {
    props.fetchBatchRemove(selectionState.selectedRowKeys).then(res => {
        message.success('删除成功！')
        data.value.forEach(item => {
            if (selectionState.selectedRowKeys.includes(item.ID)) {
                item.DELETED = 1
            }
        });
        searchData()
    }).catch(err => {
        message.error('出现异常' + err)
    })
}
// 删除
const deleteItem = (record) => {
    var key = record.ID
    props.fetchBatchRemove([key]).then(res => {
        message.success('删除成功！')
        data.value.forEach(item => {
            if (key == item.ID) {
                item.DELETED = 1
            }
        });
        searchData()
    }).catch(err => {
        message.error('出现异常' + err)
    })
}

// 停用
const stopItem = (record) => {
    record.DELETED = '1'
    props.fetchSetState(record).then(res => {
        message.success('停用成功')
    }).catch(err => {
        record.DELETED = '0'
        message.error('出现异常' + err)
    })
}
// 恢复
const restoreItem = (record) => {
    record.DELETED = '0'
    props.fetchSetState(record).then(res => {
        message.success('恢复成功')
    }).catch(err => {
        record.DELETED = '1'
        message.error('出现异常' + err)
    })
}

// 导出数据
const exportData = () => {
    props.fetchExportData(searchState.value).then(res => {
        // 首行增加一行列名

        // 将 JSON 转为 worksheet
        const worksheet = XLSX.utils.json_to_sheet(res);

        // 创建一个新的工作簿
        const workbook = XLSX.utils.book_new();

        // 将 worksheet 添加到工作簿中
        XLSX.utils.book_append_sheet(workbook, worksheet, "Sheet1");

        // 生成 Excel 文件并下载
        XLSX.writeFile(workbook, props.configFileName.replace('.json', '') + ".xlsx");
    })
}

// 导入数据
const uploadFiles = ref([])
const importData = ({ file, onSuccess, onError }) => {
    const reader = new FileReader();

    reader.onload = (e) => {
        try {
            const ab = e.target.result;
            const workbook = XLSX.read(ab, { type: 'array' });
            const sheetName = workbook.SheetNames[0];
            const worksheet = workbook.Sheets[sheetName];
            const jsonData = XLSX.utils.sheet_to_json(worksheet);
            props.fetchImportData(jsonData).then(res => {
                message.success('导入成功')
                searchData()
            }).catch(err => {
                message.error('导入失败' + err)
            })
            uploadFiles.value = []
            onSuccess();
        } catch (error) {
            onError(error);
        }
    };
    reader.readAsArrayBuffer(file);
}

// 上移，先将不相邻的数据上移到一起，然后一起上移
const moveUpItem = () => {

    if (selectionState.selectedRowKeys.length > 1) {
        message.error('请只选择一条数据')
        return
    }

    selectionState.selectedRowKeys.sort((a, b) => {
        const a_index = data.value.findIndex(item => item.ID == a);
        const b_index = data.value.findIndex(item => item.ID == b);
        if (a_index > b_index) {
            return 1
        } else {
            return -1
        }
    })
    selectionState.selectedRowKeys.forEach(async ID => {
        const index = data.value.findIndex(item => item.ID == ID)
        if (index > 0) {
            const a = data.value[index]
            const b = data.value[index - 1]

            await props.fetchSwitchSort(a, b).then(res => {
                const temp = data.value[index]
                data.value[index] = data.value[index - 1]
                data.value[index - 1] = temp
            }).catch(err => {
                message.error('出现异常' + err)
            })

        }
    });
}

// 下移
const moveDownItem = () => {
    if (selectionState.selectedRowKeys.length > 1) {
        message.error('请只选择一条数据')
        return
    }

    selectionState.selectedRowKeys.sort((a, b) => {
        const a_index = data.value.findIndex(item => item.ID == a);
        const b_index = data.value.findIndex(item => item.ID == b);
        if (a_index > b_index) {
            return -1
        } else {
            return 1
        }
    })

    selectionState.selectedRowKeys.forEach(async ID => {
        const index = data.value.findIndex(item => item.ID == ID)
        if (index < data.value.length - 1) {
            const a = data.value[index]
            const b = data.value[index + 1]

            await props.fetchSwitchSort(a, b).then(res => {
                const temp = data.value[index]
                data.value[index] = data.value[index + 1]
                data.value[index + 1] = temp
            }).catch(err => {
                message.error('出现异常' + err)
            })
        }
    })
}

</script>
<template>
    <a-space direction="vertical" style="width: 100%">
        <!-- 筛选条件 -->
        <search :config="props.config" v-model="searchState" @search="searchData"></search>
        <!-- 批量操作 -->
        <div style="display: flex; justify-content: space-between; margin-bottom: 1px;">
            <!-- 左边的按钮 -->
            <a-space>
                <a-button v-show="props.config.buttonsShow.add != false" type="primary" @click="showAddModal"> <template
                        #icon>
                        <PlusOutlined />
                    </template> 新增</a-button>
                <a-popconfirm title="你确认彻底删除这些数据吗?" @confirm="batchDeleteItem">
                    <a-button v-show="props.config.buttonsShow.delete != false" v-if="hasSelected" type="primary" danger
                        ghost>
                        <template #icon>
                            <DeleteFilled />
                        </template> 彻底删除
                    </a-button>
                </a-popconfirm>

                <a-button v-show="props.config.buttonsShow.setSequence != false" v-if="hasSelected" @click="moveUpItem"
                    ghost type="primary">
                    <template #icon>
                        <ArrowUpOutlined />
                    </template> 上移</a-button>
                <a-button v-show="props.config.buttonsShow.setSequence != false" v-if="hasSelected" @click="moveDownItem"
                    ghost type="primary">
                    <template #icon>
                        <ArrowDownOutlined />
                    </template> 下移</a-button>
            </a-space>
            <!-- 导出表格 -->
            <a-space>
                <a-tooltip v-show="props.config.buttonsShow.exportData != false" placement="bottom">
                    <template #title>导出查询数据</template>
                    <a-button @click="exportData">
                        <template #icon>
                            <DownloadOutlined />
                        </template>
                    </a-button>
                </a-tooltip>

                <a-tooltip v-show="props.config.buttonsShow.importData != false" placement="bottom">
                    <template #title>导入数据</template>
                    <a-upload :fileList="uploadFiles" :custom-request="importData" accept=".xlsx, .xls"
                        show-upload-list="false">
                        <a-button>
                            <template #icon>
                                <UploadOutlined />
                            </template>
                        </a-button>
                    </a-upload>
                </a-tooltip>
                <curdConfig v-if="false"></curdConfig>
            </a-space>
        </div>
        <!-- 已选择内容 -->
        <a-alert v-if="hasSelected" :message="selectedTag" type="success">
            <template #action>
                <a-button type="link" style="color: rgb(47, 214, 139);" ghost :loading="selectionState.loading"
                    @click="cancelSelectedKeys">取消选择</a-button>
                <a-button type="link" style="color: rgb(47, 214, 139);" ghost :loading="selectionState.loading"
                    @click="allSelectedKeys">选择全部</a-button>
                <a-button type="link" style="color: rgb(47, 214, 139);" ghost :loading="selectionState.loading"
                    @click="exportSelectedKeys">导出选择</a-button>
            </template>
        </a-alert>
        <!-- 表格 -->
        <a-table :scroll="{ x: 800, }" class="custom-table"
            :row-selection="{ selectedRowKeys: selectionState.selectedRowKeys, onChange: onSelectChange }" :size="tableSize"
            :pagination="{
                position: 'bottom', showTotal: (total) => `共 ${total} 条数据`,
                pageSizeOptions: ['10', '20', '50', '100'],
                defaultPageSize: defaultPageSize,
                showSizeChanger: true,
            }" :rowClassName="(record, index) => (index % 2 === 1 ? 'table-striped' : null)" :columns="columns"
            :data-source="data" bordered rowKey="ID">
            <template #bodyCell="{ column, record }">
                <!-- 操作列 -->
                <template v-if="column.dataIndex.toLowerCase() === 'action'">
                    <a-space>
                        <a-button v-show="props.config.buttonsShow.edit != false" ghost type="primary" size="small"
                            @click="showEditModal(record)">
                            <template #icon>
                                <EditFilled />
                            </template>编辑</a-button>
                        <a-popconfirm title="你确认删除这些数据?" @confirm="deleteItem(record)">
                            <a-button v-show="props.config.buttonsShow.setStateToDelete != false" danger size="small">
                                <template #icon>
                                    <DeleteFilled />
                                </template>删除
                            </a-button>
                        </a-popconfirm>
                        <a-button v-show="props.config.buttonsShow.setState != false" v-if="record.DELETED == 0" danger
                            size="small" @click="stopItem(record)">
                            <template #icon>
                                <DeleteOutlined />
                            </template>停用</a-button>
                        <a-button v-show="props.config.buttonsShow.setState != false" v-else size="small" ghost
                            style="color: rgb(66, 144, 66);borderColor:  rgb(66, 144, 66)" type="primary"
                            @click="restoreItem(record)">
                            <template #icon>
                                <ReloadOutlined />
                            </template>启用</a-button>
                        <!-- 附加按钮 -->
                        <a-button v-for="(item, index) in props.config.addedButtons" :key="index" ghost type="primary"
                            size="small" @click="showAddButtonModal(item, record)">
                            {{ item.name }}</a-button>
                    </a-space>
                </template>
                <!-- 数据列 -->
                <template v-else>
                    <!-- 多选兼容 -->
                    <div
                        v-if="column.options && column.options.length > 2 && record[column.dataIndex] && record[column.dataIndex].includes(';')">
                        <a-tag v-for="item in record[column.dataIndex].split(';')">
                            {{ toLabel(item, column.options) }}
                        </a-tag>
                    </div>
                    <!-- 状态 -->
                    <div v-else-if="column.dataIndex === 'DELETED'">
                        <a-badge v-if="record[column.dataIndex] == '0' || record[column.dataIndex] == 0" status="success"
                            text="正常" />
                        <a-badge v-else status="error" text="停用" />
                    </div>
                    <!-- 候选项为三个 显示灰色标签 -->
                    <div v-else-if="column.options && column.options.length > 2">
                        <a-tag>{{ toLabel(record[column.dataIndex], column.options) }}</a-tag>
                    </div>
                    <!-- 候选项为两个 显示两种颜色标签 -->
                    <div v-else-if="column.options && column.options.length == 2">
                        <div v-if="record[column.dataIndex] == column.options[0].value">
                            <a-tag color="blue">{{ toLabel(record[column.dataIndex], column.options) }}</a-tag>
                        </div>
                        <div v-else>
                            <a-tag color="success">{{ toLabel(record[column.dataIndex], column.options) }}</a-tag>
                        </div>
                    </div>

                    <!-- 显示文本 -->
                    <div v-else>
                        {{ record[column.dataIndex] }}
                    </div>
                </template>
            </template>
        </a-table>
        <!-- 表单弹窗 -->
        <a-modal :width="props.config.columnConfig.length >= 8 ? 850 : 500" v-model:visible="isModalVisible"
            :title="fromTitle" @ok="submitForm" @cancel="cancelForm">
            <editfrom ref="editFrom" :config="props.config" v-model="fromState" :form-type="formType"></editfrom>
        </a-modal>
    </a-space>
</template>
<style>
.ant-table-wrapper .ant-table-tbody>tr.ant-table-row:hover>td {
    background-color: #c4f3e0 !important;
}

.ant-table-thead>tr>th {
    background-color: rgb(228, 240, 244) !important;
}

.ant-table-striped :deep(.table-striped) td {
    background-color: #fafafa !important;
}

/* 
.custom-table .ant-table {
    border: 1px solid #d8e7d8;
}


.custom-table .ant-table-thead>tr>th {
    border: 0.1px solid #e1e7e1;
}

.custom-table .ant-table-tbody>tr>td {
    border: 0.1px solid #e8e9e8;
}*/
</style>