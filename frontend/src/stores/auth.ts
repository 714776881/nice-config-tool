import { ref, computed, reactive } from 'vue'
import { defineStore } from 'pinia'
import { localStg } from '@/utils/storage'

const AuthStoreId = 'auth-store'

export const useAuthStore = defineStore(AuthStoreId, () => {
  const isAuthenticated = ref(false)
  const userInfo = ref<Api.Auth.UserInfo>()

  const scopeState = ref({
    REGIONID: "",
    HOSPITALID: "",
    DEPARTMENTID: ""
  })

  function login(user) {
    isAuthenticated.value = true
    userInfo.value = user

    scopeState.value.REGIONID = user.regionId;
    scopeState.value.HOSPITALID = user.hospitalId;
    scopeState.value.DEPARTMENTID = user.departmentId;
  }

  function logout() {
    userInfo.value = null
    isAuthenticated.value = false
  }

  const changeScopeState = (regionId,hospitalId,departmentId)=>{
    if(regionId != "")
    {
      scopeState.value.REGIONID = regionId;
    }
    if(hospitalId != "")
    {
      scopeState.value.HOSPITALID = hospitalId;
    }
    if(departmentId != "")
    {
      scopeState.value.DEPARTMENTID = departmentId;
    }
  }

  return { isAuthenticated, userInfo, scopeState, login, logout, changeScopeState }
})