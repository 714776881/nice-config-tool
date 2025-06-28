<template>
  <a-upload
    v-model:file-list="fileList"
    name="file"
    action="https://www.mocky.io/v2/5cc8019d300000980a055e76"
    :headers="headers"
    :max-count="1"
    @change="handleChange"
  >
    <a-button>
      <upload-outlined></upload-outlined>
      {{ fileList.length > 0 ? '覆盖' : '上传' }}
    </a-button>
     <template #itemRender="{ file, actions }">
      <a-space>
        <span :style="file.status === 'error' ? 'color: red' : ''">{{ file.name }}</span>
        <!-- <a href="javascript:;" @click="actions.download">download</a> -->
        <a href="javascript:;" @click="actions.remove">删除</a>
      </a-space>
    </template>
  </a-upload>
</template>
<script lang="ts" setup>
import { ref,watch } from 'vue';
import { message } from 'ant-design-vue';
import { UploadOutlined } from '@ant-design/icons-vue';
import type { UploadChangeParam } from 'ant-design-vue';

const props = defineProps<{
  url?: string;
}>();

const handleChange = (info: UploadChangeParam) => {
  if (info.file.status !== 'uploading') {
    console.log(info.file, info.fileList);
  }
  if (info.file.status === 'done') {
    const file = info.file.originFileObj as File;
    if (file) {
      const reader = new FileReader();
      reader.onload = (e) => {
        const content = e.target?.result?.toString() || '';
        modelValue.value = content;
      };
      reader.readAsText(file); // 以纯文本方式读取文件
    }

    message.success(`${info.file.name} 上传成功~`);
  } else if (info.file.status === 'error') {
    message.error(`${info.file.name} 上传失败.`);
  }
};

const fileList = ref([]);
const headers = {
  authorization: 'authorization-text',
};


const modelValue = defineModel<string>('value');

watch(modelValue, (newValue) => {
    if (!newValue || newValue.length === 0) {
        fileList.value = [];
        return;
    }

    fileList.value = [{
        uid: '-1',
        name: '模板文件.html',
        status: 'done',
        response: {
            data: newValue
        },
    }];
}, { immediate: true })

watch(fileList, (newValue) => {
    if (!newValue) {
        modelValue.value = '';
        return;
    }

    // modelValue.value = newValue[0]?.response?.data || '';
})


</script>

