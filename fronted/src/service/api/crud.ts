import request from '../request/index'

/**
 * 执行select sql获取数据，后面移到后端
 */
export function fetchCrudData(sql: string) {
  return request<Api.Crud.Data>({
    url: '/Crud',
    method: 'get',
    params: { sql }
  })
}

// 执行sql
export function fetchCrudExeSql(sql: string) {
  return request<Api.Crud.EexResult>({
    url: '/Crud/ExeSql',
    method: 'post',
    params: { sql }
  })
}
// 批量执行SQL
export function fetchCrudExeBatchSql(sqls: string[]) {
  return request<Api.Crud.EexResult>({
    url: '/Crud/ExeBatchSql',
    method: 'post',
    data: { sqls }
  })
}
