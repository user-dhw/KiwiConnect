import axios from 'axios'
import { ElMessage } from 'element-plus'

// Create axios instance
const instance = axios.create({
	baseURL: 'http://127.0.0.1:3000',
	headers: {
		post: {
			'Content-Type': 'application/x-www-form-urlencoded; charset=UTF-8',
		},
	},
})

// Token expiration handler
let hasHandledTokenExpired = false
let tokenExpireTimer = null

const clearTokenExpireTimer = () => {
	if (tokenExpireTimer) {
		clearTimeout(tokenExpireTimer)
		tokenExpireTimer = null
	}
}

const getTokenExpireAt = token => {
	try {
		const payload = token.split('.')[1]
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
	} catch (err) {
		return null
	}
}

export const setupAxiosInterceptors = router => {
	const handleTokenExpired = () => {
		if (hasHandledTokenExpired) return
		hasHandledTokenExpired = true
		clearTokenExpireTimer()

		ElMessage.warning({
			message: 'Login expired, please log in again',
		})

		localStorage.removeItem('admin_jwt_token')
		router.push('/login')

		setTimeout(() => {
			hasHandledTokenExpired = false
		}, 1000)
	}

	const scheduleTokenExpireCheck = () => {
		clearTokenExpireTimer()
		const token = localStorage.getItem('admin_jwt_token')
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
			const token = localStorage.getItem('admin_jwt_token')
			if (token) {
				config.headers.Authorization = `Bearer ${token}`
			}
			return config
		},
		err => Promise.reject(err),
	)

	// Response interceptor
	instance.interceptors.response.use(
		response => {
			if (response.data && response.data.code) {
				if (parseInt(response.data.code) === 401) {
					handleTokenExpired()
				}
				if (parseInt(response.data.code) === -1) {
					ElMessage.warning({
						message: 'Request failed',
					})
				}
			}
			return response
		},
		error => {
			if (error?.response?.status === 401) {
				handleTokenExpired()
				return Promise.reject(error)
			}

			ElMessage.warning({
				message: 'Server connection failed',
			})
			return Promise.reject(error)
		},
	)

	scheduleTokenExpireCheck()
}

export default instance
