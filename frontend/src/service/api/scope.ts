import request from '../request'

/**
 * 获取作用域字典数据，区域、院区、科室
 *
 * @param userName User name
 * @param password Password
 */
export function fetchScopeOptions() {
  return request<Api.Scope.Option[]>({
    url: '/Scope',
    method: 'get'
  })
}
