// 校检函数
export function useValid(funName) {
  if (funName == 'isValidJSON') {
    return validateJson
  }
}

// 自定义的 JSON 格式验证函数
const validateJson = (rule, value) => {
  if (!value) return Promise.resolve()
  try {
    JSON.parse(value) // 尝试解析 JSON
    return Promise.resolve() // 如果是有效的 JSON 格式，返回成功
  } catch (e) {
    return Promise.reject('请输入有效的 JSON 格式！') // 如果解析失败，返回失败
  }
}
