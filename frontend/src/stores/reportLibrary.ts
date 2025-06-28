import { ref, computed, reactive } from 'vue'
import { defineStore } from 'pinia'
import { useAuthStore } from './auth'

const ReportLibraryID = 'reportLibrary-store'

export const useReportLibraryStore = defineStore(ReportLibraryID, () => {})
