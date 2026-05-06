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

const ALLOWED_IMAGE_TYPES = ['image/jpeg', 'image/png', 'image/webp']

export function getCommentList(contentId) {
	return request.post(
		'/web/getcomment',
		toFormData({ content_id: contentId }),
		formConfig,
	)
}

export function getReplyList(commentId) {
	return request.post(
		'/web/getreply',
		toFormData({ comment_id: commentId }),
		formConfig,
	)
}

export function createComment(data) {
	return request.post('/web/setcomment', toFormData(data), formConfig)
}

export function createReply(data) {
	return request.post('/web/setreply', toFormData(data), formConfig)
}

export async function uploadCommentImage(file) {
	if (!file || !ALLOWED_IMAGE_TYPES.includes(file.type)) {
		throw new Error('Only JPG, JPEG, PNG, and WEBP images are allowed')
	}

	const formData = new FormData()
	formData.append('file', file)

	const res = await request.post('/uplod', formData, {
		headers: {
			'Content-Type': 'multipart/form-data',
		},
	})

	const url = res?.url || res?.Url || ''
	if (!url) {
		throw new Error('Image upload failed')
	}

	ElMessage.success('Image uploaded successfully')

	return url
}
