import request from '../request'

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
export function fetchGetXmlFileNode(filePath: string, nodePath: string) {
  return request<string>({
    url: '/xmlFile',
    method: 'get',
    params: { filePath, nodePath }
  })
}

// 提交xml节点
export function fetchPostXmlFileNode(filePath: string, nodePath: string, nodeValue: string) {
  return request<boolean>({
    url: '/xmlFile',
    method: 'post',
    params: { filePath, nodePath },
    data: { nodeValue: nodeValue }
  })
}
