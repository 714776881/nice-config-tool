// 清空节点值
export const clearNodeValue = (node) => {
  if (node == undefined) {
    return
  }

  if (
    node[':@'] != undefined &&
    node[':@'] &&
    node[':@'].value &&
    !node[':@'].value.startsWith('#Array')
  ) {
    node[':@'].value = ''
  }

  if (node.item == undefined) {
    return
  }
  if (node.item.length == undefined) {
    return
  }

  if (node.item.length == 0) {
    return
  }

  if (node[':@'] && node[':@'].value && node[':@'].value.startsWith('#Array')) {
    node.item.splice(1)
  }
  node.item.forEach(clearNodeValue)
}
