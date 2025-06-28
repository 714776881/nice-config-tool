import request from '../request'

/**
 * 执行select sql获取数据，后面移到后端
 */
export function fetchCrudData(sql: string, values: any = {}) {
  return request<Api.Crud.Data>({
    url: '/Crud/Select',
    method: 'post',
    headers:{
      'Content-Type': 'multipart/form-data'
    },
    data: {
      sql: sql,
      values: values,
    }
  })
}

// 执行sql
export function fetchCrudExeSql(sql: string, values: any = {}, file?: File) {
  return request<Api.Crud.EexResult>({
    url: '/Crud/ExeSql',
    method: 'post',
    headers:{
      'Content-Type': 'multipart/form-data'
    },
    data: {
      sql: sql,
      values: values,
      file: file
    }
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
