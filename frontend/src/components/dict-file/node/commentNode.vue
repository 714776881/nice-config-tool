<script setup lang="ts">
import { ref } from 'vue'


/**
 * 	<!-- 注释节点 -->
 */

const node = defineModel<App.Dict.Node>()

</script>

<template>
    <a-alert v-if="node['_']" style="background-color: rgb(244, 249, 247);color: rgb(0, 0, 0);;"
        :message="node['_'][0]['#text']" type="success" />
</template>