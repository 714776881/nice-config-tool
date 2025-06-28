import { ref, computed, reactive } from 'vue'
import { defineStore } from 'pinia'
import { localStg } from '@/utils/storage'

const AuthStoreId = 'auth-store'

export const useAuthStore = defineStore(AuthStoreId, () => {
  const loginToken = ref<Api.Auth.LoginToken>({ token: '', refreshToken: '' })
  const isAuthenticated = ref(false)
  const userInfo = ref<Api.Auth.UserInfo>()

  const deptState = computed(() => {
    if (userInfo.value == undefined) {
      return {}
    }
    return {
      REGIONID: userInfo.value.regionId,
      HOSPITALID: userInfo.value.hospitalId,
      DEPARTMENTID: userInfo.value.departmentId
    }
  })

  function login(user) {
    isAuthenticated.value = true
    userInfo.value = user
  }

  function logout() {
    userInfo.value = null
    isAuthenticated.value = false
  }

  return { isAuthenticated, loginToken, userInfo, deptState, login, logout }
})
