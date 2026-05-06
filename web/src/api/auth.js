import request from '@/utils/request'

const toFormData = data => {
	const params = new URLSearchParams()
	Object.entries(data || {}).forEach(([key, value]) => {
		params.append(key, value ?? '')
	})
	return params
}

/**
 * User registration
 */
export function register(data) {
	return request.post('/webadmin/registered', toFormData(data), {
		headers: {
			'Content-Type': 'application/x-www-form-urlencoded',
		},
	})
}

/**
 * User login
 */
export function login(data) {
	return request.post('/webadmin/login', {
		type: '',
		...data,
	})
}

/**
 * Get notification messages
 */
export function getNotice(params) {
	return request.post('/web/getnotice', toFormData(params), {
		headers: {
			'Content-Type': 'application/x-www-form-urlencoded',
		},
	})
}
