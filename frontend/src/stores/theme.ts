import { ref, computed, reactive } from 'vue'
import { defineStore } from 'pinia'
import { fetchGetConfigFile, fetchPostConfigFile } from '@/service/api/file'
import { localStg } from '@/utils/storage'

const ThemeStoreId = 'theme-store'
const fileName = 'menuConfig.json'

export const useTheme = defineStore(ThemeStoreId, () => {
  const menuConfig = ref([])

  const LoadMenuConfig = async () => {
    await fetchGetConfigFile(fileName).then((res) => {
      menuConfig.value = JSON.parse(res.data)
    })
  }

  const PostMenuConfig = async (fileName, fileContent) => {
    await fetchPostConfigFile(fileName, fileContent).then((res) => {
      menuConfig.value = fileContent
    })
  }

  const _currentMenuItem = ref(localStg.get('currentMenu') || null)
  const currentMenuItem = computed({
    get: () => {
      return _currentMenuItem.value
    },
    set: (value) => {
      _currentMenuItem.value = value
      localStg.set('currentMenu', value)
    }
  })

  return {
    menuConfig,
    LoadMenuConfig,
    PostMenuConfig,
    currentMenuItem
  }
})
