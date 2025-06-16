<template>
    <div v-if="templateData">
        <!-- 报告模板 -->
        <a-space direction="vertical" style="width: 100%;margin-top: 26px;">
            <!-- 模板名称 -->
            <div style="margin-bottom: 14px;">
                <a-space direction="vertical">
                    <a-input v-model:value="templateData.reptTemp" :bordered="false"
                        style="font-size: 24px;font-weight:bold;width:600px" placeholder="请输入模板名称" />
                    <div style="font-size: small;margin-left: 11px;">{{ templateData.isPublic ? '公有模板' : '私有模板' }} | {{
                        saveTemplateInfo }}</div>
                </a-space>
            </div>
            <!-- 检查所见 -->
            <a-card size="small" style="background-color: rgb(246, 254, 250);">
                <template #title>
                    <a-space>
                        <div style="width: 10px;background-color: rgb(115, 230, 157);">
                            <span style="visibility: hidden;">.</span>
                        </div>
                        <span style="font-size:medium;font-weight: bold;">检查所见</span>
                    </a-space>
                </template>
                <template #extra><a href="#"></a></template>
                <ReportEditor ref="studyResultEditorRef" :key="templateData.reptTempId" v-model="templateData.studyResult"
                    style="height: 350px;">
                </ReportEditor>
            </a-card>
            <!-- 检查诊断 -->
            <a-card size="small" style="background-color: rgb(246, 254, 250);">
                <template #title>
                    <a-space>
                        <div style="width: 10px;background-color: rgb(115, 230, 157);">
                            <span style="visibility: hidden;">.</span>
                        </div>
                        <span style="font-size:medium;font-weight: bold;">检查诊断</span>
                    </a-space>
                </template>
                <template #extra><a href="#"></a></template>
                <ReportEditor ref="diagResultEditorRef" :key="templateData.reptTempId" v-model="templateData.diagResult"
                    style="height: 20%;">
                </ReportEditor>
            </a-card>
        </a-space>
    </div>
</template>
<script setup lang="ts">
import { ref, reactive, toRefs, watch, defineModel, nextTick } from 'vue';
import ReportEditor from './reportEditor.vue'
import { fetchGetTemplate, fetchPostTemplate } from '@/service/api/reptTemp'
import { onMounted } from 'vue';
import { watchDebounced } from '@vueuse/core'

const emit = defineEmits(['changeTemplateNode']);

const templateId = defineModel<string>();
const templateData = ref<Api.ReptCatelog.ReptTempVO>();

const saveTemplateInfo = ref('');

const studyResultEditorRef = ref(null);
const diagResultEditorRef = ref(null);

watch(() => templateId.value, () => {
    loadTemplate();
})

// 加载模板数据
const loadTemplate = async () => {
    if (templateId.value == null || templateId.value == '') {
        return;
    }
    const res = await fetchGetTemplate(templateId.value);
    templateData.value = res.data;
    if (templateData.value.modifyDT) {
        saveTemplateInfo.value = templateData.value.modifyDT + ' 修改';
    }
    else {
        saveTemplateInfo.value = '已经保存到云端';
    }

}

// 防止频繁保存，进行防抖和节流。
watchDebounced(() => templateData.value, async (newValue, oldValue) => {
    if (oldValue == undefined || oldValue.reptTempId == undefined) {
        return
    }
    if (newValue.reptTempId != oldValue.reptTempId) {
        return;
    }

    const form: Api.ReptCatelog.ReptTempForm = {
        ReptTempId: templateData.value.reptTempId,
        ReptTemp: templateData.value.reptTemp,
        StudyResult: studyResultEditorRef.value.getPlainText(),// 提取纯文本
        DiagResult: diagResultEditorRef.value.getPlainText(),// 提取纯文本
        IsPublic: templateData.value.isPublic,
        CStudyResult: templateData.value.studyResult,
        CDiagResult: templateData.value.diagResult,
    }

    const res = await fetchPostTemplate(form);
    saveTemplateInfo.value = '刚刚修改，已经保存到云端';
    emit('changeTemplateNode', templateData.value);
}, { deep: true, debounce: 600, maxWait: 1000 })

onMounted(loadTemplate);

</script>