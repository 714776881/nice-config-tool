import { ref, computed, reactive } from 'vue'
import { defineStore } from 'pinia'
import { fetchGetConfigFile, fetchPostConfigFile } from '@/service/api/file'

const crudStoreId = 'crud-store'

/**
 * 元数据文件
 */
export const useConfig = defineStore(crudStoreId, () => {
  const files = ref<Api.Crud.File[]>([])

  const setConfig = (fileName: string, fileContent: any) => {
    files.value.push({ fileName, fileContent })
  }

  const getConfig = async (fileName: string) => {
    let fileItem = files.value.find((item) => item.fileName === fileName)

    if (!fileItem) {
      const res = await fetchGetConfigFile(fileName)
      const fileContent = JSON.parse(res.data)

      files.value.push({ fileName, fileContent })
      fileItem = files.value.find((item) => item.fileName === fileName)
    }
    return fileItem
  }

  return { getConfig, setConfig }
})
