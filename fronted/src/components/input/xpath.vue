<template>
    <div>
        <a-space style="width: 100%;" direction="vertical">
            <!-- XPath 输入框 -->
            <a-textarea autosize="true" v-model:value="config.XPath" placeholder="请输入默认的 SR 值路径" />
            <div>
                <a-button type="primary" @click="addConfig" size="small">添加其他厂家的 <Sect></Sect>R 值路径</a-button>
            </div>
            <el-scrollbar max-height="300px">
                <!-- ExtendSRConfigs 表格维护 -->
                <a-table v-if="ExtendSRConfigsCount > 0" :data-source="config.ExtendSRConfigs" :columns="columns"
                    row-key="index" size="small" bordered :pagination="false">
                    <template #bodyCell="{ column, record, index }">
                        <template v-if="column.key === 'Manufactor'">
                            <a-input v-model:value="record.Manufactor" placeholder="请输入厂家代码" />
                        </template>
                        <template v-else-if="column.key === 'XPath'">
                            <a-textarea v-model:value="record.XPath" placeholder="请输入XPath路径" />
                        </template>
                        <template v-else-if="column.key === 'action'">
                            <a-button type="link" danger @click="removeConfig(index)">
                                删除
                            </a-button>
                        </template>
                    </template>
                </a-table>
            </el-scrollbar>
        </a-space>
    </div>
</template>
  
<script setup lang="ts">
import { computed } from "vue";
import { ref, watch } from "vue";

// 配置对象
const config2 = ref({
    XPath:
        '/root/srnode[@codemeaning="OB-GYN Ultrasound Procedure Report"]/srnode[@codevalue="121070"]/srnode[@codevalue="T-F1810" and srnode[@codevalue="11951-1"]={FID}]/srnode[@codevalue="12144-2" and srnode[@codemeaning="Derivation"]]',
    ExtendSRConfigs: [
        {
            Manufactor: ".*\\bGE\\b.*",
            XPath:
                '/root/srnode[@codemeaning="OB-GYN Ultrasound Procedure Report"]/srnode[@codemeaning="Fetal Doppler" and srnode[@codevalue="11951-1"]={FID}]/srnode[@codemeaning="Doppler Group"]/srnode[@codemeaning="Systolic to Diastolic Velocity Ratio" and srnode[@codemeaning="Derivation"] and srnode[@codemeaning="Finding Site" and srnode[@codemeaning="Umbilical artery"]]]',
        },
    ],
});

const modelValue = defineModel<string>('value');

const config = ref({ XPath: '', ExtendSRConfigs: [] });

watch(modelValue, (newValue) => {
    if (!newValue) {
        return;
    }
    config.value = JSON.parse(modelValue.value);
    if (!config.value.ExtendSRConfigs) {
        config.value.ExtendSRConfigs = [];
    }
}, { immediate: true })

watch(config, (newValue) => {
    modelValue.value = JSON.stringify(newValue);
})


const ExtendSRConfigsCount = computed(() => {
    if (config.value && config.value.ExtendSRConfigs) {
        return config.value.ExtendSRConfigs.length;
    }
    return 0;
});

// 表格列定义
const columns = [
    {
        title: "厂家代码",
        dataIndex: "Manufactor",
        width: "20%",
        key: "Manufactor",
    },
    {
        title: "SR值路径",
        dataIndex: "XPath",
        key: "XPath",
    },
    {
        title: "操作",
        key: "action",
        align: "center",
    },
];

// 添加新配置
const addConfig = () => {
    config.value.ExtendSRConfigs.push({
        Manufactor: "",
        XPath: "",
    });
};

// 删除配置
const removeConfig = (index) => {
    config.value.ExtendSRConfigs.splice(index, 1);
};
</script>
  
<style scoped>
/* 自定义样式 */
</style>
  