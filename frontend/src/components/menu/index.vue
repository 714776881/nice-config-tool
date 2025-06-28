
<script lang="ts" setup>
import { h, ref, reactive, resolveComponent, watch } from 'vue';
import { useRouter } from 'vue-router';
import { useTheme } from '../../stores/theme';
import { useAuthStore } from '../../stores/auth';
import { computed } from 'vue';
const router = useRouter();
const useAuth = useAuthStore();

const theme = useTheme();
theme.LoadMenuConfig();

function getItem<ItemType>(item) {
    if (item.role) {
        if (!useAuth.userInfo.userRole.includes(item.role)) {
            return null
        }
    }
    if (item.actor) {
        if (!useAuth.userInfo.userActor.includes(item.actor)) {
            return null
        }
    }

    return {
        key: item.key,
        icon: item.icon ? () => h(resolveComponent(item.icon)) : undefined,
        children: item.children ? item.children.map(child => getItem(child)) : null,
        label: item.label,
        type: item.type,
        path: item.path
    }
}

const items = computed(() => {
    return theme.menuConfig.map(item => getItem(item))
})

const state = reactive({
    rootSubmenuKeys: [],
    openKeys: [],
    selectedKeys: [],
});

// 处理菜单项选择事件
const onSelect = ({ key }) => {
    const menuItem = findMenuItemByKey(items.value, key);
    if (menuItem && menuItem.path) {

        // 设置当前的路径参数
        theme.currentMenuItem = findMenuItemByKey(theme.menuConfig, key)

        if (menuItem.path.startsWith('/'))
            router.push(menuItem.path)
        else
            router.push('/' + menuItem.path)
    }
    else {
        router.push('/')
    }
};

// 根据菜单项的 key 查找对应的菜单项对象
function findMenuItemByKey(menuItems, key) {
    for (const item of menuItems) {
        if (!item) {
            continue;
        }
        if (item.key === key) {
            return item;
        }
        if (item.children) {
            const foundItem = findMenuItemByKey(item.children, key);
            if (foundItem) {
                return foundItem;
            }
        }
    }
    return null;
}

const onOpenChange = (openKeys: string[]) => {
    const latestOpenKey = openKeys.find(key => state.openKeys.indexOf(key) === -1);
    if (latestOpenKey == undefined)
        return
    if (state.rootSubmenuKeys.indexOf(latestOpenKey) === -1) {
        state.openKeys = openKeys;
    }
    else {
        state.openKeys = latestOpenKey ? [latestOpenKey] : [];
    }
};

</script>
<template>
    <!-- 菜单栏  -->
    <a-menu class="menu" v-model:selectedKeys="state.selectedKeys" style="float: left;" mode="inline"
        :open-keys="state.openKeys" :items="items" @select="onSelect" @openChange="onOpenChange">
    </a-menu>
</template>
<style>
.menu {
    background-color: #f3f7ed;
}
</style>