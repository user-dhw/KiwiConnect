import { useUserStore } from '../store/modules/user'
import { useRouter } from 'vue-router'

/**
 * Composable for route guard utilities
 */
export const useRouteGuard = () => {
	const userStore = useUserStore()
	const router = useRouter()

	/**
	 * Check if user is authenticated
	 */
	const isAuthenticated = () => {
		return !!localStorage.getItem('admin_jwt_token') && userStore.token
	}

	/**
	 * Require authentication for accessing a route
	 */
	const requireAuth = async to => {
		if (!isAuthenticated()) {
			return router.push('/login')
		}
	}

	/**
	 * Require admin access
	 */
	const requireAdmin = async to => {
		if (!isAuthenticated()) {
			return router.push('/login')
		}
		// Add admin role check if needed
		// if (!userStore.isAdmin) return router.push('/')
	}

	/**
	 * Prevent authenticated users from accessing login page
	 */
	const preventAuthenticatedAccess = async to => {
		if (isAuthenticated()) {
			return router.push('/')
		}
	}

	return {
		isAuthenticated,
		requireAuth,
		requireAdmin,
		preventAuthenticatedAccess,
	}
}
