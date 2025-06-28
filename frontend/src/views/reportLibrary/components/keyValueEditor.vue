<template>
    <div style="margin-top:20px;margin-bottom: 10px;">
        <!-- 关键字列表编辑器 可拖动 -->
        <draggable v-model="keywords" itemKey="index" handle=".drag-handle">
            <template #item="{ element, index }">
                <div style="margin-bottom: 6px;">
                    <!-- 文本数组元素 拖拽移动 -->
                    <a-space>
                        <a-tooltip title="" color="rgb(102, 200, 138)">
                            <a-button type="text" class="drag-handle" icon style="margin-right: -14px;">
                                <HolderOutlined />
                            </a-button>
                        </a-tooltip>
                        <a-textarea style="width: 500px;font-size:medium;" :autosize="true"
                            v-model:value="keywords[index]" />
                        <a-tooltip title="删除">
                            <MinusCircleOutlined style="color: rgb(102, 200, 138);margin:0px 8px 0px 4px;"
                                :disabled="1 != 1" @click="remove(index)" />
                        </a-tooltip>
                    </a-space>
                </div>
            </template>
        </draggable>
        <!-- 新增关键字输入框 -->
        <a-button style="width: 500px;margin-left: 39px;" type="dashed" @click="addKeyword">
            <PlusOutlined />
            添加
        </a-button>
    </div>
</template>

<script setup lang="ts">
import { ref, watch, onMounted } from 'vue';
import draggable from 'vuedraggable'

const text = defineModel<string>();
const keywords = ref<string[]>([]);

onMounted(() => {
    keywords.value = text.value.split(';').filter(Boolean);
});

const addKeyword = () => {
    keywords.value.push('');
};

const remove = (index: number) => {
    keywords.value.splice(index, 1);
};

watch(keywords, (newValue) => {
    text.value = newValue.join(';'); // 将 keywards 重新拼接为 text 字符串
}, { deep: true })


</script>
