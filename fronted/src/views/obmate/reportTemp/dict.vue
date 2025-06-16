<script setup lang="ts">
import { ref, onBeforeMount, provide, watch } from 'vue'
import { fetchGetConfigFile, fetchPostConfigFile } from '@/service/api/file'
import { fetchCrudData, fetchCrudExeSql, fetchCrudExeBatchSql } from '@/service/api/crud'
import { useConfig } from '@/stores/config'
import KeyValue from './keyValue.vue'
import { isRuntimeOnly } from 'vue';
import { replaceSql, addWhereSql, uuid, deepClone } from '@/utils/tool'

const props = defineProps(['configFileName', 'state'])
const config = ref<App.Dict.Config>()
const crudStore = useConfig()

// 向子组件传递配置数据,以便取用
provide('config', config)
const loadConfig = async () => {
    const file = await crudStore.getConfig(props.configFileName)
    config.value = deepClone(file.fileContent)

    if (config.value.inputConfig) {
        for (const item of config.value.inputConfig) {
            if (item.dicSql) {
                await fetchCrudData(item.dicSql).then(res => {
                    const options = res.data.data.map(element => ({
                        label: element.LABEL,
                        value: element.VALUE
                    }));
                    item.component.props.options = options;
                });
            }
        }
    }
    updateAnchorItems()
};
/**
 * 更新锚点项
 */
// 锚点项数组
const anchorItems = ref<{ href: string, title: string }[]>([])
const updateAnchorItems = () => {
    if (config.value && config.value.keyValueConfig) {
        anchorItems.value = config.value.keyValueConfig.map((item, index) => ({
            href: `#${index}`,
            title: item.name,
        }))
    }
}


onBeforeMount(loadConfig)
</script>
<template>
    <a-row v-if="config">
        <a-col v-if="anchorItems.length > 1" :span="4">
            <a-anchor :items="anchorItems"
                style="border-radius: 10px;padding-right: 10px;margin-right: 10px;background-color: rgb(244, 247, 245);"></a-anchor>
        </a-col>
        <a-col :span="anchorItems.length > 1 ? 20 : 24">
            <a-space direction="vertical" style="width: 100%;">
                <KeyValue v-for="(item, index) in config.keyValueConfig" :id="index" :key="index" :filePath="item.filePath"
                    :nodePath="item.nodePath" :name="item.name"></KeyValue>
            </a-space>
        </a-col>
    </a-row>
    <div v-else style="height: 100px;">
        <a-spin size="large" />
    </div>
</template>