<template>
  <div v-if="(scopeConfig && scopeConfig.isShow) || props.isShow">
    <span v-if="props.hideLabel?false:true">部门：</span>
    <a-cascader :style="cascaderWidth" @change="change" :allowClear="false" size="" v-model:value="modelValue"
      :options="options" placeholder="请从中选择一个" />
  </div>
</template>
<script lang="ts" setup>
import { ref, inject, computed, onMounted } from 'vue';
import type { Ref } from 'vue';
import type { CascaderProps } from 'ant-design-vue';
import { useAuthStore } from '@/stores/auth';
import { fetchScopeOptions } from '@/service/api/scope'

// Model
const emits = defineEmits(['change'])
const config = inject<Ref<App.Crud.Config>>('config')
const scopeState = useAuthStore().scopeState
const options = ref<CascaderProps['options']>([])
const modelValue = ref<string[]>([]);

const props = defineProps(['isShow','hideLabel'])

const level = computed(() => {
  if (config == undefined) {
    return 2
  }
  if (config.value.scopeConfig == undefined) {
    return 2
  }
  return config.value.scopeConfig.level
})

// Control
const scopeConfig = computed(() => {

  if (config == undefined) {
    return
  }
  if (config.value.scopeConfig == undefined) {
    return
  }
  return config.value.scopeConfig
})

onMounted(async () => {
  const { data: scopeOptions } = await fetchScopeOptions()
  if (scopeOptions) {
    options.value = buildTree(scopeOptions)
    modelValue.value.push(scopeState.REGIONID)
    if (level.value == 0) {
      return
    }
    modelValue.value.push(scopeState.HOSPITALID)
    if (level.value == 1) {
      return
    }
    modelValue.value.push(scopeState.DEPARTMENTID)
  }
})

type TreeNode = {
  value: string;
  label: string;
  children?: TreeNode[];
};

const cascaderWidth = computed(() => {
    if (level.value == 0) {
      return {
        width: '180px'
      }
    }
    if (level.value == 1) {
      return {
        width: '240px'
      }
    }
    return {
      width: '290px'
    }
})

const buildTree = (data: Api.Scope.Option[]) => {
  const regionMap = new Map<string, TreeNode>();

  data.forEach(item => {
    // 处理地区层级
    if (!regionMap.has(item.RegionCode)) {
      regionMap.set(item.RegionCode, {
        value: item.RegionCode,
        label: item.RegionName,
        children: []
      });
    }
    const regionNode = regionMap.get(item.RegionCode)!;

    if (level.value == 0) {
      return
    }

    // 处理医院层级
    let hospitalNode = regionNode.children?.find(h => h.value === item.HospitalCode);
    if (!hospitalNode) {
      hospitalNode = {
        value: item.HospitalCode,
        label: item.HospitalName,
        children: []
      };
      regionNode.children!.push(hospitalNode);
    }

    if (level.value == 1) {
      return
    }

    // 处理科室层级
    const departmentNode = {
      value: item.DepartmentCode,
      label: item.DepartmentName
    };
    hospitalNode.children!.push(departmentNode);
  });

  return Array.from(regionMap.values());
}


const change = () => {
  if (modelValue.value.length == 0) {
    scopeState.REGIONID = ''
    scopeState.HOSPITALID = ''
    scopeState.DEPARTMENTID = ''
  } else {
    scopeState.REGIONID = modelValue.value[0]
    scopeState.HOSPITALID = modelValue.value[1]
    scopeState.DEPARTMENTID = modelValue.value[2]
  }

  // 触发父组件的change事件
  emits('change')
}

</script>
