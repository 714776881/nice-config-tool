import { useAuthStore } from '@/stores/auth'
import { replaceSql } from '@/utils/tool'

// 校检函数
export function useValid(funName) {
  if (funName == 'isValidJSON') {
    return validateJson
  }
}

// 自定义的 JSON 格式验证函数
const validateJson = (rule, value) => {
  if (!value) return Promise.resolve()
  try {
    JSON.parse(value) // 尝试解析 JSON
    return Promise.resolve() // 如果是有效的 JSON 格式，返回成功
  } catch (e) {
    return Promise.reject('请输入有效的 JSON 格式！') // 如果解析失败，返回失败
  }
}

export const replaceSqlInGlobalState = (sqlTemp, state) => {
  const authStore = useAuthStore()
  // 区域ID、院区ID、科室ID； 先替换全局状态，再替换表单状态
  const sql = replaceSql(sqlTemp, authStore.deptState, 'GLOBAL')

  // 替换表单状态
  const sql2 = replaceSql(sql, state)

  // 去除{}和{}中的内容
  //sql = sql.replace(/{.*?}/g, '')
  return sql2
}

export function useCrudSql2() {
  // bug 直接获取数据，无法复用查询条件，需要通过父子组件传参基础，使子组件根据父组件的传参来获取数据
  const authStore = useAuthStore()
  // 将全局状态和表单状态注入SQL语句
  const replaceSqlInGlobalState = (sqlTemp, state) => {
    // 区域ID、院区ID、科室ID； 先替换全局状态，再替换表单状态
    const sql = replaceSql(sqlTemp, authStore.deptState, 'GLOBAL')

    // 替换表单状态
    const sql2 = replaceSql(sql, state)

    // 去除{}和{}中的内容
    //sql = sql.replace(/{.*?}/g, '')

    return sql2
  }

  return { replaceSqlInGlobalState }
}