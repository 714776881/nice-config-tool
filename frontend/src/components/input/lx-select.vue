<template>
    <div>
        <component is="a-select" v-model:value="componentModel" v-bind="componentProps" />
    </div>
</template>
  
<script setup lang="ts">
import { ref, watch, computed } from 'vue';

const props = defineProps(['options', 'mode', 'optionFilterProp', 'showSearch'])
const modelValue = defineModel<string>('value');

const componentProps = computed(() => {
    return { "options": props.options, "mode": props.mode, "option-filter-prop": props.optionFilterProp, "show-search": props.showSearch }
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
  