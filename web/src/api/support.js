import { ElMessage } from 'element-plus'
import request from '@/utils/request'

const toFormData = data => {
	const params = new URLSearchParams()
	Object.entries(data || {}).forEach(([key, value]) => {
		params.append(key, value ?? '')
	})
	return params
}

const formConfig = {
	headers: {
		'Content-Type': 'application/x-www-form-urlencoded',
	},
}

export function loginForAppeal({ username, password }) {
	return request.post('/webadmin/login', {
		type: 'shensu',
		username,
		password,
	})
}

export function getReportContent(id) {
	return request.post(
		'/webadmin/jubaocontent',
		toFormData({ id }),
		formConfig,
	)
}

export function submitAppeal(data) {
	return request.post('/webadmin/createshensu', toFormData(data), formConfig)
}

export function createFeedback(data) {
	return request.post('/webadmin/createfankui', toFormData(data), formConfig)
}

export function createReport(data) {
	return request.post('/webadmin/createjubao', toFormData(data), formConfig)
}

const ALLOWED_IMAGE_TYPES = ['image/jpeg', 'image/png', 'image/webp']

export function validateSupportImage(file) {
	if (!file) return false
	return ALLOWED_IMAGE_TYPES.includes(file.type)
}

export async function uploadSupportImage(file) {
	if (!validateSupportImage(file)) {
		throw new Error('Only JPG, JPEG, PNG, and WEBP images are allowed')
	}

	const formData = new FormData()
	formData.append('file', file)

	const res = await request.post('/uplod', formData, {
		headers: {
			'Content-Type': 'multipart/form-data',
		},
	})

	ElMessage.success('Image uploaded successfully')

	return {
		url: res?.url || res?.Url || '',
		storedFileName: res?.storedFileName || res?.StoredFileName || '',
	}
}
