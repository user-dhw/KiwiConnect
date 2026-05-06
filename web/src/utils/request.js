import axios from 'axios'
import { ElMessage, ElNotification } from 'element-plus'
import store from '../store'
import router from '../router'

const instance = axios.create({
	baseURL: import.meta.env.VITE_API_URL || '/api',
	timeout: 10000,
	headers: {
		'Content-Type': 'application/json',
	},
})

let hasHandledTokenExpired = false
let hasHandledAccountSuspended = false
let tokenExpireTimer = null

const clearTokenExpireTimer = () => {
	if (tokenExpireTimer) {
		clearTimeout(tokenExpireTimer)
		tokenExpireTimer = null
	}
}

const getTokenFromStoreOrLocal = () =>
	store.state.user?.token || window.localStorage.getItem('luffy_jwt_token')

const getTokenExpireAt = token => {
	try {
		const payload = token?.split('.')?.[1]
		if (!payload) return null

		const base64 = payload.replace(/-/g, '+').replace(/_/g, '/')
		const normalized = base64 + '='.repeat((4 - (base64.length % 4)) % 4)
		const json = decodeURIComponent(
			atob(normalized)
				.split('')
				.map(c => '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2))
				.join(''),
		)

		const data = JSON.parse(json)
		if (!data.exp) return null
		return data.exp * 1000
	} catch {
		return null
	}
}

const handleTokenExpired = () => {
	if (hasHandledTokenExpired) return
	hasHandledTokenExpired = true
	clearTokenExpireTimer()

	ElNotification({
		title: 'Warning',
		message: 'Session expired. Please sign in again.',
		type: 'warning',
	})

	store.dispatch('user/deleteuserinfo')
	store.dispatch('user/close', true)
	router.push('/').catch(() => {})

	setTimeout(() => {
		hasHandledTokenExpired = false
	}, 1000)
}

const handleAccountSuspended = message => {
	if (hasHandledAccountSuspended) return
	hasHandledAccountSuspended = true
	clearTokenExpireTimer()

	ElMessage.error(message || 'Your account is currently suspended')

	store.dispatch('user/deleteuserinfo')
	store.dispatch('user/close', false)
	router.push('/').catch(() => {})

	setTimeout(() => {
		hasHandledAccountSuspended = false
	}, 1000)
}

const scheduleTokenExpireCheck = () => {
	clearTokenExpireTimer()

	const token = getTokenFromStoreOrLocal()
	if (!token) return

	const expireAt = getTokenExpireAt(token)
	if (!expireAt) return

	const delay = expireAt - Date.now()
	if (delay <= 0) {
		handleTokenExpired()
		return
	}

	tokenExpireTimer = setTimeout(() => {
		handleTokenExpired()
	}, delay)
}

// Request interceptor
instance.interceptors.request.use(
	config => {
		scheduleTokenExpireCheck()
		const token = getTokenFromStoreOrLocal()
		if (token) {
			config.headers.Authorization = `Bearer ${token}`
		}
		return config
	},
	error => {
		return Promise.reject(error)
	},
)

// Response interceptor
instance.interceptors.response.use(
	response => {
		const requestUrl = response.config?.url || ''
		const isLoginRequest = requestUrl.includes('/webadmin/login')

		if (response.data && response.data.code) {
			const code = Number.parseInt(response.data.code, 10)
			if (code === 401 && getTokenFromStoreOrLocal()) {
				handleTokenExpired()
			}
			if (code === -1) {
				ElNotification({
					title: 'Warning',
					message: 'Request failed',
					type: 'warning',
				})
			}
		}

		const stateType = response.data?.state?.type
		if (stateType === 'ACCOUNT_SUSPENDED' && !isLoginRequest) {
			handleAccountSuspended(response.data?.state?.msg)
		}
		return response.data
	},
	error => {
		if (error.response) {
			const apiError = error.response.data
			const isSuspended = apiError?.state?.type === 'ACCOUNT_SUSPENDED'
			const isLoginRequest =
				error.config?.url?.includes('/webadmin/login')
			if (isSuspended) {
				if (!isLoginRequest) {
					handleAccountSuspended(apiError.state?.msg)
				}
				return Promise.resolve(apiError)
			}
			if (apiError?.state) {
				switch (error.response.status) {
					case 401:
						if (getTokenFromStoreOrLocal()) {
							handleTokenExpired()
						}
						break
					case 403:
						ElMessage.error(apiError.state?.msg || 'Access denied')
						break
					case 404:
						ElMessage.error(
							apiError.state?.msg ||
								'Requested resource was not found',
						)
						break
					case 409:
						ElMessage.error(
							apiError.state?.msg || 'Request conflict',
						)
						break
					case 500:
						ElMessage.error(
							apiError.state?.msg || 'Internal server error',
						)
						break
					default:
						ElMessage.error(apiError.state?.msg || 'Request failed')
				}

				return Promise.resolve(apiError)
			}

			switch (error.response.status) {
				case 401:
					if (getTokenFromStoreOrLocal()) {
						handleTokenExpired()
					}
					break
				case 403:
					ElMessage.error('Access denied')
					break
				case 404:
					ElMessage.error('Requested URL was not found')
					break
				case 500:
					ElMessage.error('Internal server error')
					break
				default:
					ElMessage.error(
						error.response.data?.message || 'Request failed',
					)
			}
		} else if (error.request) {
			ElNotification({
				title: 'Warning',
				message: 'Server connection failed',
				type: 'warning',
			})
		} else {
			ElMessage.error('Request configuration error')
		}
		return Promise.reject(error)
	},
)

scheduleTokenExpireCheck()

export default instance
