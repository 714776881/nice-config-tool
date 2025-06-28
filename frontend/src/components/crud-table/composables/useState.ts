import { useAuthStore } from '@/stores/auth'


// SQL语句处理函数
// 该函数用于处理SQL语句中的状态变量替换和条件构建
export function userState() {
    const buildWhereSql = (sql, state) => {
        if (!sql || sql == '') {
            return sql
        }

        if (Object.keys(state).length > 0) {
            sql = 'select * from (' + sql + ') where 1=1'
        } 
        else {
            return sql
        }

        Object.keys(state).forEach((key) => {
            const isHaveKey = sql.indexOf(key) > -1
            const isNullState = state[key] == undefined || state[key] == null || state[key] == ''

            if (isHaveKey == false && isNullState == false) {
                sql += ` and ${key} like '${state[key]}%' `
            } 
            else if (isHaveKey == true && isNullState == true) {
                // 如果sql中有这个key，但是state中没有值，则删除这个key
                // 例如：sql = "select * from table where 1=1 and name like '张三%' and age = 18"
                const key_index = sql.indexOf(key)
                const key_begin_index = sql.lastIndexOf('and', key_index) + 3
                const key_end_index = sql.indexOf('and', key_index)
                sql = sql.replace(sql.substring(key_begin_index, key_end_index) + 'and', '')
            }
        })
        return sql
    }

    const replaceSqlByState = (sqlTemp, state) => {
        const authStore = useAuthStore()

        // 区域ID、院区ID、科室ID； 先替换全局状态，再替换表单状态
        const sql = replaceSql(sqlTemp, authStore.scopeState, 'GLOBAL')

        // 替换表单状态
        const sql2 = replaceSql(sql, state)

        // 去除{}和{}中的内容
        //sql = sql.replace(/{.*?}/g, '')
        return sql2
    }

    const replaceSql = (sql: string, state: any, prefix: string = ''): string => {
        // 替换sql中的{}中的内容
        Object.keys(state).forEach((key) => {
            if (prefix == '') {
                sql = replaceAll(sql, `{${key}}`, state[key])
            } else {
                sql = replaceAll(sql, `{${prefix}}{${key}}`, state[key])
            }
        })

        return sql
    }

    const replaceAll = (str, search, replacement) => {
        if (!str || !search) {
            return str;
        }

        // 创建一个全局正则表达式，用于匹配所有出现的 search 字符串
        // 使用 RegExp 构造函数和适当的转义来避免特殊字符的问题
        const searchRegex = new RegExp(search.replace(/[.*+?^${}()|[\]\\]/g, '\\$&'), 'g');
        if (!replacement) {
            replacement = ""
        }

        return str.replace(searchRegex, replacement);
    }

    const buildSql = (sql:string, state: any, superState = null, whereState:any = null) => {
        if(whereState){
            // 替换父组件中的状态变量
            sql = buildWhereSql(sql, whereState)
        }
        if(superState) {
            // 替换父组件中的状态变量
            sql = replaceSql(sql, superState, "SUPER")
        }
        // 替换sql中的状态变量
        sql = replaceSqlByState(sql, state)
        return sql
    }
    
    return { buildSql}
}