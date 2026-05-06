import { useUserStore } from '../store/modules/user'
import { useRouter } from 'vue-router'

/**
 * Composable for authentication utilities
 */
export const useAuth = () => {
	const userStore = useUserStore()
	const router = useRouter()

	const logout = () => {
		userStore.deleteUserInfo()
		router.push('/login')
	}

	const isLoggedIn = () => {
		return !!localStorage.getItem('admin_jwt_token')
	}

	const getToken = () => {
		return localStorage.getItem('admin_jwt_token')
	}

	const setToken = token => {
		localStorage.setItem('admin_jwt_token', token)
	}

	return {
		logout,
		isLoggedIn,
		getToken,
		setToken,
		userStore,
	}
}
