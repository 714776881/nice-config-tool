<template>
  <a-directory-tree>
    <a-tree-node :title="'根节点'" :key="'0'">
      <a-tree-node :title="'子节点1'" :key="'0-0'" />
      <a-tree-node :title="'子节点2'" :key="'0-1'" />
    </a-tree-node>
  </a-directory-tree>
</template>

<script>
export default {
  methods: {
    // 其他逻辑
  },
};
</script>