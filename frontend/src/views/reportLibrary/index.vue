<template>
    <!-- 知识库页面 -->
    <div v-if="config">
        <a-layout style="margin-top: -20px;margin-left: 16px;">
            <!-- 侧边栏 -->
            <a-layout-sider width="350px" style="width: 350px;background-color: rgb(246, 254, 250);">
                <Scope v-if="isSuperMan" :hideLabel="true" style="margin-left: 24px; margin-top: 14px;" :isShow="true"></Scope>
                <!-- 检查类型 -->
                <a-space style="margin-left: 26px;margin-top: 10px;margin-bottom:5px">
                    <span style="font-size:medium;">检查类型：</span>
                    <a-select v-model:value="modality" :options="modalityDic" style="width: 200px;">
                    </a-select>
                </a-space>
                <!-- 目录树 -->
                <CatelogTree v-if="catelogTreeData" :key="modality" v-model="catelogTreeData"
                    @changeTemplateId="changeTemplateId" :state="state" :modality="modality" :isSuperMan="isSuperMan"
                    :isLoadExamItem="config.isLoadExamItem">
                </CatelogTree>
                <div v-else style="height: 200px; display: flex; justify-content: center; align-items: center;">
                    <a-spin size="large" />
                </div>
            </a-layout-sider>
            <!-- 报告模板 根据 templateId 显示不同模板，获取和同步数据 -->
            <a-layout :key="modality" style="background-color: white;padding-left: 48px;">
                <div style="margin-top: 14px;">
                    <ReportTemplate v-model="templateId" @changeTemplateNode="changeTemplateNode"></ReportTemplate>
                </div>
            </a-layout>
        </a-layout>
    </div>
</template>
<script setup lang="ts">
import { ref, reactive, provide, watch, onMounted } from 'vue';
import CatelogTree from './tree.vue';
import ReportTemplate from './components/reportTemplate.vue';
import { fetchGetCatelog, fetchGetModality } from '@/service/api/reptCatelog'
import { fetchGetTemplate } from '@/service/api/reptTemp'
import { computed } from 'vue';
import { useAuthStore } from '@/stores/auth'
import { useTheme } from '@/stores/theme'
import { useConfig } from '@/stores/config'
import { deepClone } from '@/utils/tool'
import { getCatelogNode } from './treeNodes'
import Scope from '@/components/common/scope.vue'

const { userInfo,scopeState} = useAuthStore()

const config = ref<App.ReptCatelog.Config>({
    isLoadExamItem: false,
    adminRoles: [],
    catelogMaxLayerNumber: 0,
    modality: ""
});
const modalityDic = ref([]) // 模态字典
const modality = ref('') // 当前模态
const templateId = ref('') // 模板id
const catelogTreeData = ref<Api.ReptCatelog.Node[]>() // 目录树数据

// 当前知识库状态
const state = ref({
    UserId: userInfo.userId,
    UserCode: userInfo.userCode,
    RegionId: userInfo.regionId,
    HospitalId: userInfo.hospitalId,
    DepartmentId: userInfo.departmentId,
    CategoryType: 1002,
    IsLoadExamItem: false,
    IsGM : false
})
// 是否超级管理员
const isSuperMan = computed(() => {
    var roles = config.value.adminRoles
    for (var role in roles) {
        if (userInfo.userRole.includes(roles[role])) {
            return true
        }
    }
    return false
})

watch(() => scopeState, async (val) => {
    templateId.value = ''
    catelogTreeData.value = null
    state.value.RegionId = val.REGIONID
    state.value.HospitalId = val.HOSPITALID
    state.value.DepartmentId = val.DEPARTMENTID

    const res = await fetchGetCatelog(modality.value, state.value)
    if (res.data.nodes) {
        catelogTreeData.value = res.data.nodes
    }
},{deep: true, immediate: true})


// 获取当前模态下的目录树数据
watch(() => modality.value, async (val) => {
    templateId.value = ''
    catelogTreeData.value = null

    const res = await fetchGetCatelog(val, state.value)
    if (res.data.nodes) {
        catelogTreeData.value = res.data.nodes
    }
})

// 更新当前模板ID
const changeTemplateId = (val) => {
    templateId.value = val
}
// 更新当前模板节点
const changeTemplateNode = (val) => {
    const node = getCatelogNode(catelogTreeData.value, val.reptTempId)
    node.title = val.reptTemp
}
// 加载配置文件
const onload = async () => {
    const file = await useConfig().getConfig(useTheme().currentMenuItem.fileName)
    config.value = deepClone(file.fileContent)
    state.value.IsLoadExamItem = config.value.isLoadExamItem

    const res = await fetchGetModality()
    if (res.data) {
        modalityDic.value = res.data
        if(config.value.modality)
        {
            modalityDic.value = res.data.filter((item) => item.value == config.value.modality)
        }
        else
        {
            modalityDic.value = res.data
        }

        if (modalityDic.value.length > 0) {
            modality.value = modalityDic.value[0].value
        }
    }
}
onMounted(onload)
</script>
