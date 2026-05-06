import { defineStore } from 'pinia'
import { ref, computed } from 'vue'

export const useUserStore = defineStore('user', () => {
	// State
	const uinfo = ref(JSON.parse(localStorage.getItem('uinfo')) || {})
	const token = ref(localStorage.getItem('admin_jwt_token') || '')
	const islog = ref(true)
	const islogin = ref(true)
	const isclose = ref(false)
	const unread = ref(0)

	// Getters (computed properties)
	const isAuthenticated = computed(() => !!token.value && !!uinfo.value.id)
	const hasUserInfo = computed(() => Object.keys(uinfo.value).length > 0)
	const userId = computed(() => uinfo.value?.id || null)
	const userName = computed(() => uinfo.value?.username || '')

	// Actions
	const setUserInfo = data => {
		uinfo.value = data
		localStorage.setItem('uinfo', JSON.stringify(data))
	}

	const setToken = data => {
		token.value = data
		localStorage.setItem('admin_jwt_token', data)
	}

	const deleteUserInfo = () => {
		uinfo.value = {}
		token.value = ''
		localStorage.removeItem('uinfo')
		localStorage.removeItem('admin_jwt_token')
	}

	const toggleIslog = () => {
		islog.value = !islog.value
	}

	const toggleIslogin = () => {
		islogin.value = !islogin.value
	}

	const toggleIsclose = () => {
		isclose.value = !isclose.value
	}

	const setUnread = num => {
		unread.value = num
	}

	const setIslog = value => {
		islog.value = value
	}

	const setIslogin = value => {
		islogin.value = value
	}

	const setIsclose = value => {
		isclose.value = value
	}

	const clearUnread = () => {
		unread.value = 0
	}

	const incrementUnread = (count = 1) => {
		unread.value += count
	}

	return {
		// State
		uinfo,
		token,
		islog,
		islogin,
		isclose,
		unread,
		// Getters
		isAuthenticated,
		hasUserInfo,
		userId,
		userName,
		// Actions
		setUserInfo,
		setToken,
		deleteUserInfo,
		toggleIslog,
		toggleIslogin,
		toggleIsclose,
		setUnread,
		setIslog,
		setIslogin,
		setIsclose,
		clearUnread,
		incrementUnread,
	}
})
