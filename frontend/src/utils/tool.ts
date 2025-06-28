import { stringType } from 'ant-design-vue/es/_util/type'
import { logger } from './logger'
import { useGlobalStore } from '@/stores/global'

// 获取字典标签
export function toLabel(value: string, options: [App.Crud.Option]) {
  const option = options.find((item) => item.value === value)
  if (option) {
    return option.label
  } else {
    return value
  }
}

// 从pink red orange green cyan blue purple颜色中随机选择一个
export function getRandomColor() {
  const colors = ['pink', 'red', 'orange', 'green', 'cyan', 'blue', 'purple']
  return colors[Math.floor(Math.random() * colors.length)]
}

// 深度拷贝对象，避免引用
export function deepClone<T>(obj: T): T {
  if (obj == undefined) {
    return null
  }

  return JSON.parse(JSON.stringify(obj))
}

// 替换sql中的变量
export function replaceSql(sql: string, state: any, prefix: string = ''): string {
  Object.keys(state).forEach((key) => {
    if (prefix == '') {
      sql = replaceAll(sql, `{${key}}`, state[key])
    } else {
      sql = replaceAll(sql, `{${prefix}}{${key}}`, state[key])
    }
  })

  return sql
}

// 添加where条件
export function addWhereSql(sql: string, state: any): string {
  if (!sql || sql == '') {
    return sql
  }

  if (Object.keys(state).length > 0) {
    sql = 'select * from (' + sql + ') where 1=1'
  } else {
    return sql
  }

  Object.keys(state).forEach((key) => {
    const isHaveKey = sql.indexOf(key) > -1
    const isNullState = state[key] == undefined || state[key] == null || state[key] == ''

    if (isHaveKey == false && isNullState == false) {
      sql += ` and ${key} like '${state[key]}%' `
    } else if (isHaveKey == true && isNullState == true) {
      const key_begin_index = sql.indexOf(key)
      const key_end_index = sql.indexOf('and', key_begin_index)
      sql = sql.replace(sql.substring(key_begin_index, key_end_index) + 'and', '')
    }
  })

  return sql
}

function replaceAll(str, search, replacement) {
  if (!str || !search) {
    return str; // 如果输入字符串或搜索字符串为空，直接返回原字符串
  }
 
  // 创建一个全局正则表达式，用于匹配所有出现的 search 字符串
  // 使用 RegExp 构造函数和适当的转义来避免特殊字符的问题
  const searchRegex = new RegExp(search.replace(/[.*+?^${}()|[\]\\]/g, '\\$&'), 'g');
  if(!replacement)
  {
    replacement = ""
  }
 
  return str.replace(searchRegex, replacement);
}

// 产生UUID
export function uuid2() {
  return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
    const r = (Math.random() * 16) | 0,
      v = c == 'x' ? r : (r & 0x3) | 0x8
    return v.toString(16)
  })
}

/**
 * 生成一个永不重复的ID
 * @param randomLength 随机id长度 0 - 32
 */
export function uuid(randomLength = 32): string {
  const characters = 'abcdefghijklmnopqrstuvwxyz0123456789';
  let uuid = '';

  for (let i = 0; i < randomLength; i++) {
      const randomIndex = Math.floor(Math.random() * characters.length);
      uuid += characters[randomIndex];
  }
  return uuid;
}
