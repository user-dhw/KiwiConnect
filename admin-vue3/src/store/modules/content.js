import { defineStore } from 'pinia'
import { ref, computed } from 'vue'

export const useContentStore = defineStore('content', () => {
  // State
  const commentNum = ref(0)
  const contentId = ref('')
  const contentName = ref('')
  const contentUserId = ref('')
  const baseUrl = ref(import.meta.env.VITE_API_URL || 'http://localhost:8080')

  // Getters (computed properties)
  const hasContent = computed(() => !!contentId.value)
  const contentInfo = computed(() => ({
    contentId: contentId.value,
    contentName: contentName.value,
    contentUserId: contentUserId.value,
  }))

  // Actions
  const setContentId = id => {
    contentId.value = id
  }

  const setCommentNum = num => {
    commentNum.value = num
  }

  const setContentInfo = data => {
    contentName.value = data.contentname || ''
    contentUserId.value = data.contentuserid || ''
  }

  const setContentDetails = data => {
    contentId.value = data.contentId || ''
    contentName.value = data.contentName || ''
    contentUserId.value = data.contentUserId || ''
  }

  const resetContent = () => {
    commentNum.value = 0
    contentId.value = ''
    contentName.value = ''
    contentUserId.value = ''
  }

  return {
    // State
    commentNum,
    contentId,
    contentName,
    contentUserId,
    baseUrl,
    // Getters
    hasContent,
    contentInfo,
    // Actions
    setContentId,
    setCommentNum,
    setContentInfo,
    setContentDetails,
    resetContent,
  }
})
