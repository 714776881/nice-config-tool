<template>
    <div class="rich-text-editor">
        <!-- 工具栏 -->
        <a-space v-show="showToolbar" class="toolbar" align="center" size="small">
            <!-- 富文本编辑器工具栏
            <a-button type="default" @click="applyStyle('bold')" size="small">
                <template #icon>
                    <BoldOutlined />
                </template>
            </a-button>
            <a-button type="default" @click="applyStyle('italic')" size="small">
                <template #icon>
                    <ItalicOutlined />
                </template>
            </a-button>
            <a-button type="default" @click="applyStyle('underline')" size="small">
                <template #icon>
                    <UnderlineOutlined />
                </template>
            </a-button>
            <a-button type="default" @click="applyStyle('strikeThrough')" size="small">
                <template #icon>
                    <StrikethroughOutlined />
                </template>
            </a-button>
            <a-button type="default" @click="applyStyle('insertOrderedList')" size="small">
                <template #icon>
                    <OrderedListOutlined />
                </template>
            </a-button>
            <a-button type="default" @click="applyStyle('insertUnorderedList')" size="small">
                <template #icon>
                    <UnorderedListOutlined />
                </template>
            </a-button>
            -->
            <!-- 关键字 -->
            <a-button type="dashed" @click="addKeywords" size="small">
                <template #icon>
                    <CommentOutlined />
                </template>关键字
            </a-button>
            <!-- 特殊字符-->
            <a-space size="" style="margin-left: 10px;">
                <a-button type="text" @click="applyStyleWithValue('insertText', 'Ω')" size="small">
                    Ω
                </a-button>
                <a-button type="text" @click="applyStyleWithValue('insertText', '$')" size="small">
                    $
                </a-button>
                <a-button type="text" @click="applyStyleWithValue('insertText', '￥')" size="small">
                    ￥
                </a-button>
            </a-space>
        </a-space>

        <!-- 编辑区域 -->
        <div ref="editorRef" style="height: 90%;font-size: large;" class="editor" contenteditable="true"
            v-html="editorContent" @input="updateContent" @click="handleLinkClick">
        </div>

        <!-- 编辑关键字弹窗 -->
        <a-modal style="width: 624px;" :mask="false" ref="keyValueFromRef" v-model:open="isEditing" title="编辑关键字"
            ok-text="保存" cancel-text="删除" @ok="saveKeyword" @cancel="cancelEdit" centered>
            <KeyValueEditor :key="editingLink" v-model="currentKeywords"></KeyValueEditor>
            <template #footer>
                <a-button danger type="text" @click="deleteKeyword">删除</a-button>
                <a-button type="primary" @click="saveKeyword">保存</a-button>
            </template>
        </a-modal>
    </div>
</template>
<script setup lang="ts">
import { ref } from "vue";
import { uuid } from "@/utils/tool";
import { onMounted } from "vue";
import { computed } from "vue";
import KeyValueEditor from "./keyValueEditor.vue";

const model = defineModel<string>()

const editorContent = ref();
const editorRef = ref<HTMLElement | null>(null);
const isEditing = ref(false); // 控制弹窗显示
const editingLink = ref<HTMLElement | null>(null);
const currentKeywords = ref("");
const showToolbar = ref(true); // 控制工具栏的显示
const keyValueFromRef = ref<HTMLElement | null>();
const contenteditable = ref('true') // 是否可编辑

onMounted(() => {
    editorContent.value = model.value;
})
// 更新内容
const updateContent = () => {
    const editor = editorRef.value;
    if (editor) {
        model.value = editor.innerHTML;
    }
};

// 应用格式命令
const applyStyle = (command: string) => {
    document.execCommand(command, false, null);
};
const applyStyleWithValue = (command: string, value?: string) => {
    document.execCommand(command, false, value);
};

// 获取选区范围
const getSelectionRange = () => {
    const selection = window.getSelection();
    if (!selection || selection.rangeCount === 0) return null;
    return selection.getRangeAt(0);
};

// 插入关键字功能
const addKeywords = () => {
    const range = getSelectionRange();
    if (!range) return;

    const selectedText = range.toString();
    if (selectedText.trim()) {
        const link = document.createElement("a");
        link.setAttribute("id", "keyword_" + uuid());
        link.setAttribute("class", "KnowledgeItem");
        link.setAttribute("option", selectedText);
        link.setAttribute("onclick", 'OnClickKnowledgeItem(event)');
        link.textContent = selectedText;

        range.deleteContents();
        range.insertNode(link);

        isEditing.value = true;
        currentKeywords.value = selectedText;

        editingLink.value = link;
    }
};

// 取消编辑
const cancelEdit = () => {
    editingLink.value = null;
    currentKeywords.value = "";
    isEditing.value = false;
};

// 点击关键字链接时，显示弹窗
const handleLinkClick = (event: MouseEvent) => {
    const target = event.target as HTMLElement;
    if (target.tagName === "A" && target.hasAttribute("option")) {
        editingLink.value = target;
        currentKeywords.value = target.getAttribute("option") || "";
        isEditing.value = true;
        event.preventDefault();
    }
};

// 保存关键字
const saveKeyword = () => {
    if (editingLink.value) {
        editingLink.value.setAttribute("option", currentKeywords.value);
        //editingLink.value.textContent = currentKeywords.value.replace(/;/g, ", ");
        editingLink.value = null;
        currentKeywords.value = "";
        isEditing.value = false;
        updateContent();
    }
};

// 取消编辑
const deleteKeyword = () => {
    if (editingLink.value) {
        const link = editingLink.value;
        link.parentNode?.replaceChild(document.createTextNode(link.textContent), link);// 从DOM中删除关键字链接

        editingLink.value = null; // 清空当前编辑的链接
        currentKeywords.value = ""; // 清空输入框内容
        isEditing.value = false; // 关闭弹窗
        updateContent(); // 更新内容
    }

};


const OnClickKnowledgeItem = (event) => {
    console.log()
}


// 获取纯文本
const getPlainText = () => {
    const editor = editorRef.value;
    if (editor) {
        const plainText = editor.innerText;
        return plainText;
    }
    return "";
};
// 使用 defineExpose 暴露方法
defineExpose({
    getPlainText
});

</script>
<style scoped>
.rich-text-editor {
    border: 0px solid #d9d9d9;
    padding: 0px;
    border-radius: 8px;
}

.toolbar {
    margin-bottom: 6px;
}

.editor {
    border: 1px solid #d9d9d9;
    border-radius: 4px;
    padding: 10px;
    min-height: 220px;
    outline: none;
    overflow-y: auto;
    background-color: #ffffff;
}
</style>
