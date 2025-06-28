import request from '../request'

// 获取报告模板
export function fetchGetTemplate(reptTempId: string) {
  return request<Api.ReptCatelog.ReptTempVO>({
    url: '/ReptTemp/Get',
    method: 'post',
    params: { reptTempId }
  })
}

// 上传报告模板
export function fetchPostTemplate(reptTempForm: Api.ReptCatelog.ReptTempForm) {
  return request<string>({
    url: '/ReptTemp/Update',
    method: 'post',
    data: reptTempForm
  })
}
