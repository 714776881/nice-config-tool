import request from '../request/index'

/**
 * 登录/认证
 *
 * @param userName User name
 * @param password Password
 */
export function fetchLogin(loginToken: string) {
  return request<Api.Auth.UserInfo>({
    url: '/login',
    method: 'post',
    data: { loginToken }
  })
}
