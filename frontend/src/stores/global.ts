import { ref, computed, reactive } from 'vue'
import { defineStore } from 'pinia'
import { useAuthStore } from './auth'

const GlobalID = 'global-store'

export const useGlobalStore = defineStore(GlobalID, () => {
  const count = ref(0)

  return { count }
})
