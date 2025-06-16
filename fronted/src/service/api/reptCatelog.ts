import request from '../request/index'

// 获取模态
export function fetchGetModality() {
  return request<App.Common.Dictionary>({
    url: '/Dictionary/GetModality',
    method: 'get'
  })
}

// 获取报告目录树
export function fetchGetCatelog(modality: string, state: Api.ReptCatelog.ReptLibStateForm) {
  return request<Api.ReptCatelog.ReptCatelogVO>({
    url: '/ReptCatelog/GetCatelog',
    method: 'post',
    params: { modality },
    data: state
  })
}

// 新增目录节点
export function fetchAddCatelogNode(
  parentNode: Api.ReptCatelog.Node,
  node: Api.ReptCatelog.Node,
  state: Api.ReptCatelog.ReptLibStateForm
) {
  return request<string>({
    url: '/ReptCatelog/AddCatelogNode',
    method: 'post',
    data: { ParentNode: parentNode, AddNode: node, State: state }
  })
}

// 更新目录节点
export function fetchUpdateCatelogNode(node: Api.ReptCatelog.Node) {
  return request<boolean>({
    url: '/ReptCatelog/UpdateCatelogNode',
    method: 'post',
    data: { Node: node }
  })
}

// 删除目录节点
export function fetchDeleteCatelogNode(node: Api.ReptCatelog.Node) {
  return request<boolean>({
    url: '/ReptCatelog/DeleteCatelogNode',
    method: 'post',
    data: { Node: node }
  })
}

// 复制目录节点
export function fetchCopyCatelogNode(
  node: Api.ReptCatelog.Node,
  targetNode?: Api.ReptCatelog.Node,
  state?: Api.ReptCatelog.ReptLibStateForm
) {
  return request<string>({
    url: '/ReptCatelog/CopyCatelogNode',
    method: 'post',
    data: { Node: node, TargetNode: targetNode, State: state }
  })
}
// 移动目录节点
export function fetchMoveCatelogNode(
  node: Api.ReptCatelog.Node,
  targetNode?: Api.ReptCatelog.Node,
  state?: Api.ReptCatelog.ReptLibStateForm
) {
  return request<string>({
    url: '/ReptCatelog/MoveCatelogNode',
    method: 'post',
    data: { Node: node, TargetNode: targetNode, State: state }
  })
}

// 获取检查项目
export function fetchExamItems(bodypartId: string) {
  return request<App.Common.Dictionary>({
    url: '/Dictionary/GetExamItems',
    method: 'get',
    params: { bodypartId }
  })
}
// 获取检查项目分组
export function fetchGetExamItemCategory(
  modality: string,
  state: Api.ReptCatelog.ReptLibStateForm,
  isPublic: boolean
) {
  return request<string[]>({
    url: '/Category/GetExamItemCategory',
    method: 'post',
    params: { modality, isPublic },
    data: state
  })
}
export function fetchGetExamItemCategoryByBodypartId(
  bodyPartId: string,
  state: Api.ReptCatelog.ReptLibStateForm,
  isPublic: boolean
) {
  return request<string[]>({
    url: '/Category/GetExamItemCategoryByBodypartId',
    method: 'post',
    params: { bodyPartId, isPublic },
    data: state
  })
}

export function fetchAddExamItemCategory(
  bodyPartId: string,
  examItemIds: string[],
  state: Api.ReptCatelog.ReptLibStateForm,
  isPublic: boolean
) {
  return request<Boolean>({
    url: '/Category/AddExamItemCategory',
    method: 'post',
    params: { bodyPartId, isPublic },
    data: { examItemIds, state }
  })
}
