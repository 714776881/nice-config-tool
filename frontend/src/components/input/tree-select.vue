<template>
    <!-- 支持两级树状输入方式 -->
  <a-tree
    v-model:checkedKeys="componentModel"
    default-expand-all
    checkable
    :height="400"
    :tree-data="treeData"
  >
    <template #title="{ title, key }">
      <span v-if="key === '0-0-1-0'" style="color: #1890ff">{{ title }}</span>
      <template v-else>{{ title }}</template>
    </template>
  </a-tree>
</template>
  
<script setup lang="ts">
import { ref, watch, computed } from 'vue';

const props = defineProps(['options', 'mode', 'optionFilterProp', 'showSearch'])
const modelValue = defineModel<string>('value');

const componentProps = computed(() => {
    return { "options": props.options, "mode": props.mode, "option-filter-prop": props.optionFilterProp, "show-search": props.showSearch }
});

const treeData = computed(() => {
    const options : App.Common.DicElement[] = props.options;

    const categorys = []
    for (let i = 0; i < options.length; i++) {
        const category = options[i].category;
        if (categorys.indexOf(category) == -1) {
            categorys.push(category)
        }
    }

    return categorys.map(category => {
        const categoryOptions = options.filter(option => option.category == category);
        return {
            key: category,
            title: category,
            children: categoryOptions.map(option => {
                return {
                    key: option.value,
                    title: option.label
                }
            })
        }
    })
});


const componentModel = ref();

watch(modelValue, (newValue) => {
    if (!newValue) {
        componentModel.value = [];
        return;
    }
    componentModel.value = newValue.split(';');
}, { immediate: true })

watch(componentModel, (newValue) => {
    if (!newValue) {
        modelValue.value = '';
        return;
    }
    modelValue.value = newValue.join(';');
})

</script> 
<style scoped></style>
  