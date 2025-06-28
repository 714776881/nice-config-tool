export enum ReptNodeTypeEnum {
  BodyPart = 0,
  ExamItem = 1,
  Category = 2,
  ReportTemplate = 3
}

// 根据key获取目录节点
export function getCatelogNode(data: Api.ReptCatelog.Node[], key: string): Api.ReptCatelog.Node {
  for (const node of data) {
    if (node.key === key) {
      return node
    }
    if (node.children && node.children.length > 0) {
      const result = getCatelogNode(node.children, key)
      if (result) {
        return result // 返回找到的节点
      }
    }
  }
  return null // 如果没有找到，返回 null
}

// 获取目录节点的父节点
export function getParentNode(
  data: Api.ReptCatelog.Node[],
  key: string,
  parentNode?: Api.ReptCatelog.Node
) {
  for (const node of data) {
    if (node.key === key) {
      return parentNode
    }
    if (node.children && node.children.length > 0) {
      const result = getParentNode(node.children, key, node)
      if (result) {
        return result // 返回找到的节点
      }
    }
  }
  return null // 如果没有找到，返回 null
}

// 将目录节点数据扁平化
export function flattenTree(
  data: Api.ReptCatelog.Node[],
  parentKey?: string
): Api.ReptCatelog.FlatNode[] {
  const result = []
  if (!data) return []

  data.forEach((node) => {
    // 创建当前节点的扁平化对象
    const flatNode = {
      key: node.key,
      title: node.title,
      type: node.type,
      isPublic: node.isPublic,
      parentKey // 父节点 key，顶层节点为 undefined
    }

    result.push(flatNode)

    // 如果有子节点，递归处理
    if (node.children && node.children.length > 0) {
      result.push(...flattenTree(node.children, node.key))
    }
  })

  return result
}

/* 判断公有节点是否包含私有节点 */
export const isHavePrivateChildren = (children: Api.ReptCatelog.Node[]): boolean => {
  if (!children || children.length === 0) {
    return false
  }
  if (children.some((node) => node.isPublic === false)) {
    return true
  }

  return children.some((node) => {
    return node.children && isHavePrivateChildren(node.children)
  })
}

export const nodeToPublic = (data: Api.ReptCatelog.Node[], isPublic: boolean) => {
  data.forEach((node) => {
    node.isPublic = isPublic
    if (node.children && node.children.length > 0) {
      nodeToPublic(node.children, isPublic)
    }
  })
}
