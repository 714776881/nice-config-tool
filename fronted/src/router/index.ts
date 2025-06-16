import { createRouter, createWebHistory,createWebHashHistory, type RouteRecordRaw } from 'vue-router'
import HomeView from '../views/HomeView.vue'
import View404 from '../views/error/404.vue'
import View401 from '../views/error/401.vue'
import ViewTest from '../views/error/test.vue'
import ViewLogin from '../views/login/index.vue'
import { useAuthStore } from '@/stores/auth'
import { logger } from '@/utils/logger'

// 动态加载 views 目录下的所有 index.vue 文件
const views = import.meta.glob('../views/**/index.vue')

const routes = Object.keys(views).map((path) => {
  // 获取所有页面路径，通过index.vue定位
  const routePath = path.replace('../views/', '').replace('/index.vue', '')

  const meta = { requiresAuth: true }

  return {
    path: `/${routePath}`, // 生成路由路径
    name: routePath,
    component: views[path], // 使用动态导入的组件
    meta: meta
  }
})

const router = createRouter({
  // 路由Hash模式
  history: createWebHistory(import.meta.env.VITE_BASE_URL),
  routes: [
    {
      path: '/',
      name: 'home',
      component: HomeView
    },
    {
      path: '/404',
      name: '404',
      component: View404
    },
    {
      path: '/401',
      name: '401',
      component: View401
    },
    {
      path: '/test',
      name: 'test',
      component: ViewTest
    },
    {
      path: '/login',
      name: 'login',
      component: ViewLogin
    },
    ...routes
  ]
})

// 路由守卫
router.beforeEach((to, from, next) => {
  const anthStore = useAuthStore()

  // 若未登录，跳转到登录页面
  if (to.meta.requiresAuth && !anthStore.isAuthenticated) {
    console.log('登录失败')
    next({ name: 'home' })
  } else {
    next()
  }
})

export default router
