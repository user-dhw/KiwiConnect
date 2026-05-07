import { getUser } from '@/api/account'

const apiBaseUrl = import.meta.env.VITE_API_URL || '/api'

export const getStoredAuthToken = () =>
	window.localStorage.getItem('luffy_jwt_token') || ''

export const normalizeFileUrl = value => {
	if (!value || typeof value !== 'string') return ''
	if (value.startsWith('http://127.0.0.1:3000')) {
		return value.replace('http://127.0.0.1:3000', apiBaseUrl)
	}
	if (value.startsWith('http://localhost:3000')) {
		return value.replace('http://localhost:3000', apiBaseUrl)
	}
	if (/^https?:\/\//i.test(value)) return value
	if (value.startsWith('/api/')) return value
	if (value.startsWith('/uplodes/')) return `${apiBaseUrl}${value}`
	return `${apiBaseUrl}/uplodes/${value.replace(/^\/+/, '')}`
}

export const mapServerUserToStoreUserInfo = user => ({
	uid: user?.user_id || user?.uid || user?.id || '',
	nickname: user?.nickname || '',
	avatar: normalizeFileUrl(user?.avatar || ''),
	realstate: Number(user?.realstate || 0),
})

export async function syncCurrentUserProfile(store) {
	const token = getStoredAuthToken()
	if (!token) return null

	try {
		const res = await getUser()
		if (res.state?.type !== 'SUCCESS') {
			return null
		}

		const userinfo = mapServerUserToStoreUserInfo(res.data)
		store.dispatch('user/setUserInfo', userinfo)
		return userinfo
	} catch {
		return null
	}
}
