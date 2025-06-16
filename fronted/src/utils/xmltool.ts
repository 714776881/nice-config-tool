import { XMLParser, XMLBuilder } from 'fast-xml-parser'

export function xmlToObject(xmlString) {
  const parser = new XMLParser({
    ignoreAttributes: false,
    attributeNamePrefix: '',
    preserveOrder: true,
    commentPropName: '_',
    isArray: (name, jpath, isLeafNode, isAttribute) => {
      return false // 其他节点不解析为数组
    }
  })

  const result = parser.parse(xmlString)
  return result
}

export function objectToXml(obj) {
  // 创建 XMLBuilder 实例
  const builder = new XMLBuilder({
    attributeNamePrefix: '', // 属性名前缀
    commentPropName: '_',
    preserveOrder: true,
    ignoreAttributes: false, // 不忽略属性
    format: true, // 格式化输出
    indentBy: '\t', // 使用两个空格进行缩进
    suppressEmptyNode: true
  })

  // 将对象转换为 XML 字符串
  const xmlString = builder.build(obj)

  return xmlString
}
