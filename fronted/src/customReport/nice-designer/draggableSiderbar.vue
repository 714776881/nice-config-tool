<template>
    <div>
        <!-- 侧边栏内容区域 -->
        <div :style="{ width: sidebarPosition + 'px' }">
            <a-space>
                <slot></slot> <!-- 使用插槽，允许传入自定义内容 -->
                <div ref="sidebar" class="draggable-sidebar" @mousedown="startDrag">
                    asdasd
                </div>
            </a-space>

        </div>
    </div>
</template>

<script setup>
import { ref } from 'vue';

const sidebarPosition = ref(0); // 侧边栏的位置
let isDragging = ref(false); // 是否正在拖拽
let startX = ref(0); // 拖拽起始位置

// 鼠标按下时初始化拖拽
const startDrag = (e) => {
    isDragging.value = true;
    startX.value = e.clientX; // 记录鼠标按下的位置
    document.addEventListener('mousemove', onDrag); // 添加拖拽过程中鼠标移动的事件
    document.addEventListener('mouseup', stopDrag); // 添加鼠标释放时的事件
};

// 鼠标拖动时更新侧边栏位置
const onDrag = (e) => {
    if (isDragging.value) {
        const diff = e.clientX - startX.value; // 计算鼠标移动的距离
        sidebarPosition.value = sidebarPosition.value + diff; // 更新侧边栏的位置
        startX.value = e.clientX; // 更新起始位置
    }
};

// 鼠标松开时停止拖拽
const stopDrag = () => {
    isDragging.value = false;
    document.removeEventListener('mousemove', onDrag);
    document.removeEventListener('mouseup', stopDrag);
};
</script>
  
<style scoped>
.draggable-sidebar {
    /*position: fixed;*/
    top: 0;
    /* 默认宽度 */
    height: 100%;
    background-color: #333;
    color: white;
    cursor: ew-resize;
    /* 拖拽时的光标 */
}

.sidebar-content {
    padding: 0px;
}
</style>
  