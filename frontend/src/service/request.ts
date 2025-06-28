//进行axios的二次封装：使用请求和响应拦截器
import { localStg } from '@/utils/storage'
import axios from 'axios'

//const serviceBaseURL = import.meta.env.VITE_SERVICE_BASE_URL

const serviceBaseURL = window.config.apiBaseUrl

//创建axios实例
const request = axios.create({
  baseURL: serviceBaseURL,
  timeout: 12000,
  withCredentials: true, //允许携带cookie
})

//请求拦截器 设置Token
request.interceptors.request.use((config) => {
  const { headers } = config
  // set token
  //const token = localStg.get('token')
  //const Authorization = token ? `Bearer ${token}` : null
  //Object.assign(headers, { Authorization })
  return config
})

//响应拦截器 捕获异常
request.interceptors.response.use(
  (response) => {
    if (response.data.code === '200') { // 200: 请求成功
      return response.data
    } else {
      window.$message?.error(response.data.message)
      return Promise.reject(new Error(response.data.message))
    }
  },
  (error) => {
    let message = error.message
    if (error.code === '500') { // 500: 服务器错误
      message = error.response?.data?.message || message
    }
    window.$message?.error(message)
    return Promise.reject(error)
  }
)
export default request
