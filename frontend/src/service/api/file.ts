import request from '../request/index'

// 获取配置文件
export function fetchGetConfigFile(fileName: string) {
  return request<string>({
    url: '/configFile',
    method: 'get',
    params: { fileName }
  })
}

// 更新配置文件
export function fetchPostConfigFile(fileName: string, fileContent: App.Crud.Config) {
  return request<string>({
    url: '/configFile',
    method: 'post',
    params: { fileName },
    data: fileContent
  })
}

// 获取xml节点
export function fetchGetXmlFileNode(fileFullPath: string, nodePath: string) {
  return request<string>({
    url: '/xmlFile',
    method: 'get',
    params: { fileFullPath, nodePath }
  })
}

// 提交xml节点
export function fetchPostXmlFileNode(fileFullPath: string, nodePath: string, nodeValue: string) {
  return request<boolean>({
    url: '/xmlFile',
    method: 'post',
    params: { fileFullPath, nodePath },
    data: { nodeValue: nodeValue }
  })
}
