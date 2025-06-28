/**
 * 深拷贝数据
 * @param {Object} data
 * @returns {Object}
 */

export function deepClone(data) {
  return JSON.parse(JSON.stringify(data))
}