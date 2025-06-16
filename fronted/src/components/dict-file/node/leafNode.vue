<script setup lang="ts">
import { computed } from 'vue';
import { ref, inject } from 'vue'
import type { Ref } from 'vue';

/**
 * 叶子节点	
 * <item name="ReportHistoryDisplay" value="0" />
 */

const node = defineModel<App.Dict.Node>()

const config = inject<Ref<App.Dict.Config>>('config')

const props = defineProps(['parentItem'])

const nodeConfig = computed(() => {
    if (config == undefined) {
        return
    }
    if (config.value.inputConfig == undefined) {
        return
    }

    if (props.parentItem != undefined && props.parentItem[':@'].value === "#Array") {
        const parentKey = props.parentItem[':@'].name
        return config.value.inputConfig.find(item => item.key.split(';').includes(parentKey))
    }

    const key = node.value[':@'].name
    return config.value.inputConfig.find(item => item.key.split(';').includes(key))
})


const getLabel = (item: any) => {
    if (item[':@'] == undefined || item[':@'].name == undefined) {
        return '';
    }

    // 从元配置中查找
    if (config.value.keyLabelMap) {
        const keyLable = config.value.keyLabelMap.find(ele => ele.key == item[':@'].name)
        if (keyLable) {
            return keyLable.label
        }
    }
    // 从配置文件中查找
    return item[':@'].label ? item[':@'].label : item[':@'].name
}

const IsShow = computed(() => {
    if (nodeConfig.value == undefined) {
        return true;
    }
    if (nodeConfig.value.isShow == undefined) {
        return true;
    }

    return nodeConfig.value.isShow
})

</script>

<template>
    <div v-show="IsShow">
        <!-- 根据配置动态显示输入组件 -->
        <a-form v-if="nodeConfig" :model="node" layout="inline">
            <a-form-item :label="getLabel(node)">
                <component :is="nodeConfig.component.name" v-bind="nodeConfig.component.props"
                    v-model:value="node[':@'].value" />
            </a-form-item>
        </a-form>
        <!-- 默认输入方式：文本框 -->
        <a-form v-else :model="node" layout="inline">
            <a-form-item :label="getLabel(node)">
                <a-input v-model:value="node[':@'].value" placeholder="" />
            </a-form-item>
        </a-form>
    </div>
</template>