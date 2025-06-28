import './assets/main.css'

import { createApp } from 'vue'
import { createPinia } from 'pinia'

import App from './App.vue'
import router from './router'

import Antd from 'ant-design-vue'
import 'ant-design-vue/dist/reset.css'
import crud from '@/components/crud-table/index.vue'
import dict from '@/components/dict-file/index.vue'
import node from '@/components/dict-file/node/node.vue'
import { message } from 'ant-design-vue'

import * as AntDesignIconsVue from '@ant-design/icons-vue'

import ElementPlus from 'element-plus'
import 'element-plus/dist/index.css'

import lxselect from './components/input/lx-select.vue'
import treeSelect from './components/input/tree-select.vue'
import xpath from './components/input/xpath.vue'
import fileUpload from './components/input/file-upload.vue'
import ExternalUrl from './components/input/externalsystemurl-input.vue'
import dataPicker from './components/input/datePicker.vue'
import reportInput from './components/input/report-input.vue'

import i18n from './locale/locale'

const app = createApp(App)

// 全局注册图标
for (const [key, component] of Object.entries(AntDesignIconsVue)) {
  app.component(key, component)
}

// 全局注册自定义组件
app.component('lxSelect', lxselect)
app.component('treeSelect', treeSelect)
app.component('lxCrud', crud)
app.component('lxDict', dict)
app.component('lxNode', node)
app.component('valueXpath', xpath)
app.component('fileUpload', fileUpload)
app.component('externalUrl', ExternalUrl)
app.component('dataPicker', dataPicker)
app.component('reportInput', reportInput)

// 加载插件
app.use(createPinia())
app.use(router)
app.use(Antd)
app.use(ElementPlus)
app.use(i18n)
app.mount('#app')

// 全局配置Message提示方式
message.config({
  top: `10px`,
  duration: 3,
  maxCount: 4
})