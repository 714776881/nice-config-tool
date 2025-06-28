<template>
  <div>
    <a-space direction="vertical" style="width: 100%; margin-top: 26px;">
      <!-- 输入标识 -->
      <a-space>
        <div style="width: 10px; background-color: rgb(115, 230, 157);">
          <span style="visibility: hidden;">.</span>
        </div>
        <span style="font-size: medium; font-weight: bold;">输入</span>
      </a-space>

      <a-space style="width: 100%;">
        <!-- 导入文件 -->
        <a-upload  :show-upload-list="false" :before-upload="handleFileUpload" accept=".html,.htm,.txt">
          <a-button ghost type="primary">导入文件</a-button>
        </a-upload>
        <!-- 下载按钮 -->
        <a-button ghost type="primary" @click="downloadFile">下载文件</a-button>
        <!-- 上传模板 -->
        <a-button style="float: right;" type="primary" @click="upload">上传模板</a-button>
      </a-space>

      <!-- 输入框 -->
      <a-textarea
        v-model:value="contentHtml"
        :auto-size="{ minRows: 5, maxRows: 10 }"
        placeholder="输入 HTML 内容..." />

      <!-- 预览标识 -->
      <a-space>
        <div style="width: 10px; background-color: rgb(115, 230, 157);">
          <span style="visibility: hidden;">.</span>
        </div>
        <span style="font-size: medium; font-weight: bold;">预览</span>
      </a-space>

      <!-- iframe 预览 -->
      <iframe ref="previewFrame" style="width: 100%; height: 1100px; border: 1px solid #ccc;"></iframe>
    </a-space>
  </div>
</template>

<script setup lang="ts">
import { ref, watch, defineProps, defineEmits, onMounted } from 'vue'
import { message } from 'ant-design-vue'
import { deepClone } from '@/utils/tool'

// HTML 内容
const contentHtml = ref<string>('')
const props = defineProps(["state","change","ok"])
// iframe DOM 引用
const previewFrame = ref<HTMLIFrameElement | null>(null)

const upload = () => {
  const state = deepClone(props.state)
  state.CONTENTHTML = contentHtml.value;
  props.change(state)
}


watch(() => props.state, () => {
  if (props.state && props.state.CONTENTHTML) {
    contentHtml.value = props.state.CONTENTHTML
  }
}, { deep: true, immediate: true })

onMounted(() => {
  // 监听内容变化并刷新 iframe
  watch(
    contentHtml,
    (newHtml) => {
      if (previewFrame.value && previewFrame.value.contentDocument) {
        const doc = previewFrame.value.contentDocument
        doc.open()
        doc.write(newHtml)
        doc.close()
      }
    },
    { immediate: true }
  )
})

// 文件上传逻辑
const handleFileUpload = (file: File) => {
  const reader = new FileReader()
  reader.onload = (e) => {
    contentHtml.value = e.target?.result as string
  }
  reader.onerror = () => {
    message.error('读取文件失败喵~')
  }
  reader.readAsText(file)
  return false // 阻止默认上传行为
}

// 下载文件逻辑
const downloadFile = () => {
  const blob = new Blob([contentHtml.value], { type: 'text/html;charset=utf-8' })
  const link = document.createElement('a')
  link.href = URL.createObjectURL(blob)
  link.download = 'download.html'
  link.click()
  URL.revokeObjectURL(link.href)
}
</script>
