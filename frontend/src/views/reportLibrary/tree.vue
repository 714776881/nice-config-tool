<template>
    <!-- 目录 -->
    <div style="height: 90vh">
        <a-space direction="vertical">
            <div>
                <a-card size="small" style="width: 350px;background-color: rgb(236, 255, 246);">
                    <!-- 切换个人/公共知识库 -->
                    <template #title>
                        <div>
                            <div v-if="!multipleSelectMode" style="float: left;">目录：</div>
                            <div v-else style="float: left;">批量操作：</div>
                            <a-space style="margin-left: 18px;">
                                <a-checkbox v-model:checked="privateLibChecked">
                                    <UserOutlined /> 个人知识库
                                </a-checkbox>
                                <a-checkbox v-model:checked="publicLibChecked">
                                    <TeamOutlined /> 公共知识库
                                </a-checkbox>
                            </a-space>
                        </div>
                    </template>
                    <!-- 搜索框 -->
                    <a-input-search style="margin-bottom: 7px;" v-model:value="searchValue" placeholder="关键词过滤"
                        size="small" />

                    <!-- 目录树 -->
                    <a-directory-tree :tree-data="treeData" @mouseleave="handleMouseLeave" @mouseenter="handleMouseEnter"
                        @select="onSelect" :auto-expand-parent="autoExpandParent" :expanded-keys="expandedKeys"
                        :filter-tree-node="filterTreeNode" @expand="onExpand" :height="treeHeight" class="draggable-tree"
                        :checkable="multipleSelectMode" style="background-color: rgb(255, 255, 255);" :draggable="draggable"
                        :allowDrop="allowDrop" @drop="onDrop" block-node>
                        <!-- 前缀图标 -->
                        <template #icon="{ type, expanded }">
                            <div v-if="type == ReptNodeTypeEnum.BodyPart || type == ReptNodeTypeEnum.ExamItem">
                                <BlockOutlined />
                            </div>
                            <div v-if="type == ReptNodeTypeEnum.Category">
                                <!-- 展开/关闭图标 -->
                                <span v-if="expanded">
                                    <FolderOpenOutlined /> <!-- 展开状态 -->
                                </span>
                                <span v-else>
                                    <FolderOutlined /> <!-- 收起状态 -->
                                </span>
                            </div>
                            <div v-if="type == ReptNodeTypeEnum.ReportTemplate">
                                <FileOutlined />
                            </div>
                        </template>

                        <!-- 标题  -->
                        <template #title="{ key, title, isPublic, type, editable, isShare }">
                            <a-input v-if="editable" ref="inputRef" v-model:value="editingValue"
                                style="width:200px;font-size: 15px;" size="small" @blur="editNode(key)"
                                @pressEnter="editNode(key)"></a-input>
                            <span v-else style="margin-left: 0px;">
                                <!-- 节点文字 -->
                                <span>
                                    <span v-if="title.indexOf(searchValue) > -1" style="font-size: 15px;">
                                        {{ title.substring(0, title.indexOf(searchValue)) }}
                                        <span style="color: #f50;font-size:15px;">{{ searchValue }}</span>
                                        {{ title.substring(title.indexOf(searchValue) + searchValue.length) }}
                                    </span>
                                    <span v-else style="font-size: 15px;">{{ title }}</span>
                                </span>

                                <!-- 操作按钮 -->
                                <a-space style="float: right;" size="small">
                                    <!-- 新增按钮组 -->
                                    <a-dropdown v-if="mouseFocuKey == key && ShowAddButtons(type, isPublic).show"
                                        :trigger="['click']">
                                        <a-button type="text" size="small" @click.stop>
                                            <template #icon>
                                                <PlusOutlined />
                                            </template>
                                        </a-button>
                                        <template #overlay>
                                            <a-menu>
                                                <div v-if="ShowAddButtons(type, isPublic).showPrivate">
                                                    <a-menu-item
                                                        v-if="ShowAddButtons(type, isPublic).showAddPrivateCategory">
                                                        <FolderAddOutlined />
                                                        <a
                                                            v-on:click="addCatelogNode(key, ReptNodeTypeEnum.Category, false)">新建个人分组</a>
                                                    </a-menu-item>
                                                    <a-menu-item
                                                        v-if="ShowAddButtons(type, isPublic).showAddPrivateExamItem">
                                                        <ProfileOutlined />
                                                        <a v-on:click="addExamItemNode(key, false)">添加个人项目</a>
                                                    </a-menu-item>
                                                    <a-menu-item
                                                        v-if="ShowAddButtons(type, isPublic).showAddPrivateTemplate">
                                                        <FileOutlined />
                                                        <a
                                                            v-on:click="addCatelogNode(key, ReptNodeTypeEnum.ReportTemplate, false)">添加个人模板</a>
                                                    </a-menu-item>
                                                </div>
                                                <a-menu-divider />
                                                <div v-if="ShowAddButtons(type, isPublic).showPublic">
                                                    <a-menu-item
                                                        v-if="ShowAddButtons(type, isPublic).showAddPublicCategory">
                                                        <FolderAddOutlined />
                                                        <a
                                                            v-on:click="addCatelogNode(key, ReptNodeTypeEnum.Category, true)">新建公共分组</a>
                                                    </a-menu-item>
                                                    <a-menu-item
                                                        v-if="ShowAddButtons(type, isPublic).showAddPublicExamItem">
                                                        <ProfileOutlined />
                                                        <a v-on:click="addExamItemNode(key, true)">添加公共项目</a>
                                                    </a-menu-item>
                                                    <a-menu-item :key="'asd'"
                                                        v-if="ShowAddButtons(type, isPublic).showAddPublicTemplate">
                                                        <FileOutlined />
                                                        <a
                                                            v-on:click="addCatelogNode(key, ReptNodeTypeEnum.ReportTemplate, true)">添加公共模板</a>
                                                    </a-menu-item>
                                                </div>
                                            </a-menu>
                                        </template>
                                    </a-dropdown>
                                    <!-- 其他操作按钮组 -->
                                    <a-dropdown v-if="mouseFocuKey == key && ShowEditButtons(type, isPublic).show"
                                        :trigger="['click']">
                                        <a-button type="text" size="small" @click.stop>
                                            <template #icon>
                                                <EllipsisOutlined />
                                            </template>
                                        </a-button>
                                        <template #overlay>
                                            <a-menu>
                                                <a-menu-item v-if="ShowEditButtons(type, isPublic).showEditName">
                                                    <FormOutlined />
                                                    <a v-on:click="editName(key)">重命名</a>
                                                </a-menu-item>
                                                <a-menu-item v-if="ShowEditButtons(type, isPublic).showRemoveNode">
                                                    <DeleteOutlined />
                                                    <a v-on:click="removeNode(key)" style="color: red;">删除</a>
                                                </a-menu-item>
                                                <a-menu-divider />
                                                <a-menu-item v-if="ShowEditButtons(type, isPublic).showCopyNode">
                                                    <SnippetsOutlined />
                                                    <a v-on:click="copyNode(key)">创建副本</a>
                                                </a-menu-item>
                                                <a-menu-item v-if="ShowEditButtons(type, isPublic).showMoveNode">
                                                    <ScissorOutlined />
                                                    <a v-on:click="moveNode(key)">移动到</a>
                                                </a-menu-item>
                                                <a-menu-item v-if="ShowEditButtons(type, isPublic).shwoMoveToPrivate">
                                                    <BookOutlined />
                                                    <a v-on:click="moveToPrivate(key)">转到个人</a>
                                                </a-menu-item>
                                            </a-menu>
                                        </template>
                                    </a-dropdown>
                                    <!-- 节点标识 -->
                                    <div v-if="isPublic == false && (privateLibChecked && publicLibChecked) && type != ReptNodeTypeEnum.ExamItem"
                                        style="margin-left: 6px;">
                                        <a-tooltip placement="bottom">
                                            <template #title>{{ userInfo.userName }}私有</template>
                                            <UserOutlined />
                                        </a-tooltip>
                                    </div>
                                    <div
                                        v-if="isPublic == true && type == ReptNodeTypeEnum.Category && (privateLibChecked && publicLibChecked == false)">
                                        <TeamOutlined />
                                    </div>
                                </a-space>
                            </span>
                        </template>
                    </a-directory-tree>
                </a-card>
            </div>
            <!-- 批量操作 
            <div v-if="multipleSelectMode">
                <a-card size="small">
                    <template #title>
                        <span style="font-weight: bold;">批量操作：</span>
                        你已经选择了 3 项
                    </template>
                    <template #extra>
                        <a-tooltip placement="bottom">
                            <template #title>推出批量操作</template>
                            <a-button type="text" size="small" @click="multipleSelectMode = !multipleSelectMode">
                                <template #icon>
                                    <CloseOutlined />
                                </template>
                            </a-button>
                        </a-tooltip>
                    </template>
                    <div style="display: flex;justify-content: space-between">
                        <a-button>创建副本</a-button>
                        <a-button>移动到</a-button>
                        <a-button>分享知识库</a-button>
                        <a-button>删除</a-button>
                    </div>
                </a-card>
            </div>
            -->
        </a-space>
    </div>
    <!-- 项目列表 -->
    <ExamItemList :key="uuid()" v-model:open="examItemListOpen" :isPublic="examItemIsPublic"
        :bodyPartId="examItemBodyPartId" :state="props.state" @handleOk="examItemHandlOk">
    </ExamItemList>
    <!-- 节点 移动/复制 -->
    <MoveCopyView :key="uuid()" v-model:open="moveViewState.isOpen" :node="moveViewState.node" v-model="catelogTreeData"
        :isLoadExamItem="props.isLoadExamItem" :modality="props.modality" :state="props.state"
        :is-copy="moveViewState.isCopy" :is-super-man="props.isSuperMan">
    </MoveCopyView>
</template>

<script setup lang="ts">
import { onMounted, onUnmounted } from 'vue';
import { ref, computed, watch } from 'vue'
import { ReptNodeTypeEnum, flattenTree, getCatelogNode, getParentNode, isHavePrivateChildren } from './treeNodes'
import { useAuthStore } from '@/stores/auth'
import { deepClone, uuid } from '@/utils/tool';
import { nextTick } from 'vue';
import { App, message } from 'ant-design-vue';
import { nodeListProps } from 'ant-design-vue/es/vc-tree/props';
import ExamItemList from './components/examitemList.vue';
import MoveCopyView from './components/moveCopyView.vue';
import { key } from 'localforage';
import { fetchAddCatelogNode, fetchUpdateCatelogNode, fetchDeleteCatelogNode, fetchGetExamItemCategory, fetchCopyCatelogNode } from '@/service/api/reptCatelog'
import { ApiFilled } from '@ant-design/icons-vue';


// Model
const props = defineProps(['catelogTreeData', 'state', 'modality', 'isLoadExamItem', 'isSuperMan'])
const emit = defineEmits(['changeTemplateId'])

const privateLibChecked = ref(true) // 是否显示私有库
const publicLibChecked = ref(true) // 是否显示公共库

const treeHeight = ref(770)
const multipleSelectMode = ref(false) // 是否批量操作
const expandedKeys = ref<string[]>([]); // 展开的节点key
const autoExpandParent = ref<boolean>(true); // 是否自动展开父节点
const searchValue = ref<string>('')  // 搜索内容

const selectedKeys = ref<string[]>(); // 选中的节点key
const mouseFocuKey = ref('') // 鼠标焦点的节点key

const authStore = useAuthStore()
const userInfo = authStore.userInfo
const isSuperMan = ref(props.isSuperMan) // 是否超级管理员

const editingValue = ref('') // 重命名输入框的临时数据
const inputRef = ref(null) // 输入框阴影

// 用于处理检查项目分类
const isLoadExamItem = ref(props.isLoadExamItem) // 是否加载检查项目
const examItemBodyPartId = ref<string>()
const examItemListOpen = ref(false)
const examItemIsPublic = ref(false)
const examItemPublic = ref<string[]>([])
const examItemPrivate = ref<string[]>([])
const moveViewState = ref({
    isOpen: false,
    node: {} as Api.ReptCatelog.Node,
    isCopy: false
})


const draggable = computed(() => {
    if (privateLibChecked.value && publicLibChecked.value) {
        return false
    }
    if (isSuperMan.value && publicLibChecked.value) {
        return true
    }
    if (privateLibChecked.value) {
        return true
    }
    return false
})

const catelogTreeData = defineModel<Api.ReptCatelog.Node[]>(); // 数据源
const treeData = computed(() => {
    if(catelogTreeData.value.length == 1)
    {
        expandedKeys.value = [catelogTreeData.value[0].key]
    }

    return filterTreeData(catelogTreeData.value); // 目录数据
})

// 根据搜索条件、个人知识库、公有知识库、已选检查项目后的符合数据
const filterTreeData = (nodes: Api.ReptCatelog.Node[], parentNode?: Api.ReptCatelog.Node) => {
    return nodes.map((node) => {
        // 递归过滤子节点
        const children = node.children ? filterTreeData(node.children, node) : null;

        let tempNode = {
            ...node,
            children,
            disabled: node.type == ReptNodeTypeEnum.BodyPart || node.type == ReptNodeTypeEnum.ExamItem
        };

        if (node.type == ReptNodeTypeEnum.ExamItem) {
            if (publicLibChecked.value) {
                if (examItemPublic.value.includes(node.key)) {
                    return tempNode
                }
            }
            if (privateLibChecked.value) {
                if (examItemPrivate.value.includes(node.key)) {
                    return tempNode
                }
                if (children && isHavePrivateChildren(children)) {
                    return tempNode
                }
            }
            return null
        }


        // 过滤私有和公有知识库
        if (publicLibChecked.value && privateLibChecked.value) {
            return tempNode
        }
        if (publicLibChecked.value && node.isPublic) {
            return tempNode
        }
        if (privateLibChecked.value) {
            if (node.type != ReptNodeTypeEnum.Category && node.type != ReptNodeTypeEnum.ReportTemplate) {
                return tempNode
            }
            // 子节点具有私节点
            /***/
            if (children && isHavePrivateChildren(children)) {
                return tempNode
            }

            if (node.isPublic == false) {
                return tempNode
            }
        }
        return null;
    }).filter((node) => node).sort((a, b) => { return a.isPublic ? 1 : -1 }).sort((a, b) => { return a.type - b.type }); // 去除空值
};

const loadExamItemCategory = async () => {
    if (isLoadExamItem.value) {
        const res = await fetchGetExamItemCategory(props.modality, props.state, true)
        examItemPublic.value = res.data
        const res2 = await fetchGetExamItemCategory(props.modality, props.state, false)
        examItemPrivate.value = res2.data
    }
}
onMounted(loadExamItemCategory)
const examItemHandlOk = () => {
    loadExamItemCategory();
}

const addExamItemNode = (parentKey: string, isPublic: boolean) => {
    const parentNode = getCatelogNode(catelogTreeData.value, parentKey)
    if (parentNode && parentNode.type == ReptNodeTypeEnum.BodyPart) {
        examItemBodyPartId.value = parentNode.key
        examItemListOpen.value = true
        examItemIsPublic.value = isPublic;
    }
}

const addCatelogNode = async (parentKey: string, nodeType: ReptNodeTypeEnum, isPublic?: boolean) => {
    const parentNode = getCatelogNode(catelogTreeData.value, parentKey)

    let node = {
        key: uuid(),
        title: '未命名',
        editable: true, // 是否进入编辑输入框
        type: nodeType,
        isPublic: isPublic,
        children: []
    }
    if (nodeType == ReptNodeTypeEnum.ReportTemplate) {
        node.children = null;
    }

    var res = await fetchAddCatelogNode(parentNode, node, props.state)
    node.key = res.data

    parentNode.children.push(node)

    expandedKeys.value.push(parentKey) // 延迟设置焦点
    nextTick(() => {
        setTimeout(() => {
            if(inputRef.value){
                inputRef.value.focus();
                inputRef.value.select();
            }
        }, 500);
    });
}

const ShowAddButtons = (type: ReptNodeTypeEnum, isPublic: boolean) => {
    const state = {
        showAddPrivateCategory: false,
        showAddPrivateExamItem: false,
        showAddPrivateTemplate: false,
        showAddPublicCategory: false,
        showAddPublicExamItem: false,
        showAddPublicTemplate: false,
    }

    if (type == ReptNodeTypeEnum.BodyPart) {
        if (privateLibChecked.value) {
            state.showAddPrivateCategory = true;
            state.showAddPrivateExamItem = true;
        }
        if (publicLibChecked.value && isSuperMan.value) {
            state.showAddPublicCategory = true;
            state.showAddPublicExamItem = true;
        }
        
        if(!isLoadExamItem.value)
        {
            state.showAddPrivateExamItem = false;
            state.showAddPublicExamItem = false;
        }

    }
    if (type == ReptNodeTypeEnum.ExamItem) {
        if (privateLibChecked.value) {
            state.showAddPrivateCategory = true;
            state.showAddPrivateTemplate = true;
        }
        if (publicLibChecked.value && isSuperMan.value) {
            state.showAddPublicCategory = true;
            state.showAddPublicTemplate = true;
        }
    }
    if (type == ReptNodeTypeEnum.Category) {
        if (privateLibChecked.value) {
            state.showAddPrivateCategory = true;
            state.showAddPrivateTemplate = true;
        }
        if (publicLibChecked.value && isSuperMan.value && isPublic) {
            state.showAddPublicCategory = true;
            state.showAddPublicTemplate = true;
        }
    }

    const showPrivate = state.showAddPrivateCategory || state.showAddPrivateExamItem || state.showAddPrivateTemplate;
    const showPublic = state.showAddPublicCategory || state.showAddPublicExamItem || state.showAddPublicTemplate;

    return {
        ...state,
        showPrivate: showPrivate,
        showPublic: showPublic,
        show: showPrivate || showPublic
    };
};

const ShowEditButtons = (type: ReptNodeTypeEnum, isPublic: boolean) => {

    const state = {
        showEditName: false,
        showRemoveNode: false,
        showCopyNode: false,
        showMoveNode: false,
        shwoMoveToPrivate: false
    };
    const canEidt = (isPublic && isSuperMan.value) || (isPublic == false);

    if (type == ReptNodeTypeEnum.Category) {
        if (canEidt) {
            state.showEditName = true;
            state.showRemoveNode = true;
            state.showMoveNode = true;
        }
        state.showCopyNode = true;
    }
    if (type == ReptNodeTypeEnum.ReportTemplate) {
        if (canEidt) {
            state.showEditName = true;
            state.showRemoveNode = true;
            state.showMoveNode = true;
        }
        state.showCopyNode = true;
    }
    const show = state.showEditName || state.showRemoveNode || state.showCopyNode || state.showMoveNode || state.shwoMoveToPrivate;
    return {
        ...state,
        show
    };
};

// 重命名
const editName = (key: string) => {
    const node = getCatelogNode(catelogTreeData.value, key)
    node.editable = true
    editingValue.value = node.title
    nextTick(() => {
        setTimeout(() => {
            inputRef.value.focus();
            inputRef.value.select();
        }, 300);
    });
}
// 创建副本
const copyNode = async (key: string) => {
    const node = getCatelogNode(catelogTreeData.value, key)
    const newNode = deepClone(node)

    if (node.type == ReptNodeTypeEnum.ReportTemplate) {
        if (node.isPublic && !isSuperMan.value) {
            newNode.isPublic = false
        }
        const res = await fetchCopyCatelogNode(node, newNode, props.state)
        if (res.data) {
            newNode.key = res.data
            newNode.title = newNode.title + ' 副本'
            const parentNode = getParentNode(catelogTreeData.value, key)
            //插入到源节点的后面
            const index = parentNode.children.findIndex((child: any) => child.key === key);
            if (index !== -1) {
                parentNode.children.splice(index + 1, 0, newNode);
            }
        }
    }
    else {
        moveViewState.value.node = node
        moveViewState.value.isOpen = true
        moveViewState.value.isCopy = true;
    }
}
// 移动到
const moveNode = (key: string) => {
    const node = getCatelogNode(catelogTreeData.value, key)
    moveViewState.value.node = node
    moveViewState.value.isOpen = true
    moveViewState.value.isCopy = false;
}
// 改为私有
const moveToPrivate = (key: string) => {
    const node = getCatelogNode(catelogTreeData.value, key)
    node.isPublic = false
}

// 删除当前节点
const removeNode = async (key: string) => {
    const node = getCatelogNode(catelogTreeData.value, key)
    const parentNode = getParentNode(catelogTreeData.value, key)
    if (node.type == ReptNodeTypeEnum.Category) {
        if (node.children.length > 0) {
            message.info("当前目录下还有模板，无法删除！");
            return
        }
        const res = await fetchDeleteCatelogNode(node)
        if (res.data) {
            parentNode.children = parentNode.children.filter((item) => item.key != key)
        }
    }
    else if (node.type == ReptNodeTypeEnum.ReportTemplate) {
        const res = await fetchDeleteCatelogNode(node)
        if (res.data) {
            parentNode.children = parentNode.children.filter((item) => item.key != key)
        }
    }
    else {
        message.error("不可删除")
        return
    }
}
// 编辑
const editNode = async (key: string) => {
    const node = getCatelogNode(catelogTreeData.value, key)
    if (node.editable) {
        node.title = editingValue.value
        const res = await fetchUpdateCatelogNode(node);
        if (res.data) {
            node.editable = false
            editingValue.value = ''
        }
        else {
            message.error("修改失败!")
        }
    }
}

// 允许拖拽
const allowDrop = (dropNode: any, dropPosition: number) => {
    if (dropNode.type == ReptNodeTypeEnum.ReportTemplate) {
        return false
    }
    return true
}

// 拖拽
const onDrop = async (info: any) => {
    const { node, dragNode, dropPosition } = info
    if (node == dragNode) return
    // 更新数据
    // 1、移动
    // 2、更新排序

    // 更新节点
    const parentNode = getParentNode(catelogTreeData.value, dragNode.key)

}


// 关键字搜索
const getParentKey = (key: string, tree: Api.ReptCatelog.Node[]) => {
    let parentKey;
    for (let i = 0; i < tree.length; i++) {
        const node = tree[i];
        if (node.children) {
            if (node.children.some((item) => item.key === key)) {
                parentKey = node.key;
            } else if (getParentKey(key, node.children)) {
                parentKey = getParentKey(key, node.children);
            }
        }
    }
    return parentKey;
}

// 展开节点
const onExpand = (keys: string[]) => {
    expandedKeys.value = keys;
    autoExpandParent.value = false;
};

// 扁平化数据，方便搜索
const dataList = computed(() => {
    return flattenTree(catelogTreeData.value)
})

// 搜索框监听
watch(searchValue, (value) => {
    let expanded = [];
    if (value != '') {
        expanded = dataList.value.map(
            (item) => {
                if (item.title.indexOf(value) > -1) {
                    return getParentKey(item.key, treeData.value);
                }
                return null;
            }
        ).filter((item, i, self) => item && self.indexOf(item) === i);
    }
    expandedKeys.value = expanded;
    autoExpandParent.value = true;
})

// 【无效】过滤节点，只高亮
const filterTreeNode = (node) => {
    if (!searchValue.value) return false;
    return node.title.includes(searchValue.value);
};

// 切换选择的报告模板ID
const onSelect = (keys, e) => {
    if (!keys || keys.length == 0) {
        return
    }

    const node = dataList.value.find(n => n.key == keys[0])

    if (node) {
        if (node.type == ReptNodeTypeEnum.ReportTemplate) {
            emit('changeTemplateId', node.key)
        }
        else {
            emit('changeTemplateId', "")
        }
    }
    selectedKeys.value = keys
};

const handleMouseEnter = (e) => {
    if (e.node && e.node.key) {
        mouseFocuKey.value = e.node.key
    }
};
const handleMouseLeave = () => {
    //mouseFocuKey.value = ""
};

onMounted(() => {
    const updateTreeHeight = () => {
        const windowHeight = window.innerHeight;
        const fixedHeight = 160; // 固定高度
        treeHeight.value = windowHeight - fixedHeight;
        if (treeHeight.value < 670) {
            treeHeight.value = 670
        }
    };

    updateTreeHeight(); // 初始计算

    // 监听页面窗口变化
    window.addEventListener('resize', updateTreeHeight);

    // 清理事件监听器
    onUnmounted(() => {
        window.removeEventListener('resize', updateTreeHeight);
    });
});

</script>

<style>
.draggable-tree .ant-tree-treenode {
    margin-bottom: 0px;
    /* 设置节点之间的垂直间距 
    height: 30px;*/
    align-items: center;
    padding-top: 5px;
    padding-bottom: 8px;
}

.draggable-tree .ant-tree-treenode .ant-tree-node-content-wrapper {
    padding-left: 0px;
    /* 调整左侧的缩进，缩短层级间距 */
}

.draggable-tree .ant-tree-treenode .ant-tree-node-content-wrapper .ant-tree-checkbox .ant-tree-checkbox-inner {
    margin-left: 0px;
    /* 如果有复选框，调整其位置 */

}

/* 修改选中节点的背景色和字体颜色 */
.draggable-tree .ant-tree-treenode-selected .ant-tree-node-content-wrapper {
    background-color: #e8f1e4;
    /* 选中节点的背景色 */
    color: white;
    /* 选中节点的字体颜色 */

}

/* 还可以修改选中时的边框 */
.draggable-tree .ant-tree-treenode-selected .ant-tree-node-content-wrapper:focus {
    outline: none;
    border: 1px solid #73d13d;
    /* 设置选中时的边框颜色 */
}

/* 自定义禁用节点的样式 */
.draggable-tree .ant-tree-treenode-disabled {
    background-color: #ffffff;
    /* 设置禁用节点的背景色 */
    color: #000000;
    /* 设置禁用节点的文字颜色 */

}

.draggable-tree .ant-tree-treenode-disabled .ant-tree-node-content-wrapper {
    color: black;
    cursor: pointer;
}
</style>./treeNodes