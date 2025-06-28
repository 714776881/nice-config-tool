import request from '../request'

/**
 * 登录/认证 通过token
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

/**
 * 登录/认证 通过用户名和密码
 *
 * @param userCode 用户名
 * @param password 密码
 */
export function fetchLoginWithUP(userCode: string,password: string) {
  return request<Api.Auth.UserInfo>({
    url: '/login',
    method: 'get',
    params: { userCode, password }
  })
}
