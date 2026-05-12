import { ElMessage } from 'element-plus'
import request from '@/utils/request'

export const IMAGE_UPLOAD_ACCEPT = '.jpg,.jpeg,.png,.webp'

const ALLOWED_IMAGE_TYPES = ['image/jpeg', 'image/png', 'image/webp']

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

export function validateImageFile(file) {
	if (!file) return false

	const fileName = String(file.name || '')
	const extension = fileName.includes('.')
		? `.${fileName.split('.').pop().toLowerCase()}`
		: ''

	if (!IMAGE_UPLOAD_ACCEPT.split(',').includes(extension)) {
		return false
	}

	return ALLOWED_IMAGE_TYPES.includes(file.type)
}

export function getActivityContent(id) {
	return request.post(
		'/web/getactivitycontent',
		toFormData({ id }),
		formConfig,
	)
}

export function getHelpContent(id) {
	return request.post('/web/gethelpcontent', toFormData({ id }), formConfig)
}

export function getWebHelpList(params) {
	return request.post(
		'/web/webgetwebhelplist',
		toFormData(params),
		formConfig,
	)
}

export function getWebActivityList(params) {
	return request.post(
		'/web/webgetwebactivitylist',
		toFormData(params),
		formConfig,
	)
}

export function getWebOldStuffList(params) {
	return request.post(
		'/web/webgetweboldstufflist',
		toFormData(params),
		formConfig,
	)
}

export function getArticleList(params) {
	return request.post('/web/getarticlelist', toFormData(params), formConfig)
}

export function searchContent(search) {
	return request.post('/web/search', toFormData({ search }), formConfig)
}

export function getArticleContent(id) {
	return request.post(
		'/web/getarticlecontent',
		toFormData({ id }),
		formConfig,
	)
}

export function getOldStuffContent(id) {
	return request.post(
		'/web/getoldstuffcontent',
		toFormData({ id }),
		formConfig,
	)
}

export function setJoin(data) {
	return request.post('/web/setjoin', toFormData(data), formConfig)
}

export function getCarouselList() {
	return request.post('/web/carousellist', toFormData({}), formConfig)
}

export function getAnnouncementList(contentId) {
	return request.post(
		'/web/announcementlist',
		toFormData({ content_id: contentId }),
		formConfig,
	)
}

export function uploadFile(file) {
	if (!validateImageFile(file)) {
		return Promise.reject(
			new Error('Only JPG, JPEG, PNG, and WEBP images are allowed'),
		)
	}

	const formData = new FormData()
	formData.append('file', file)
	return request
		.post('/uplod', formData, {
			headers: {
				'Content-Type': 'multipart/form-data',
			},
		})
		.then(res => {
			ElMessage.success('Image uploaded successfully')
			return {
				...res,
				url: res?.url || res?.Url || '',
			}
		})
}

export function getLabelList(lableName) {
	return request.post(
		'/web/getlablelist',
		toFormData({ lable_name: lableName }),
		formConfig,
	)
}

const parseLabelArray = row => {
	const raw = row?.lable
	if (!raw) return []
	if (Array.isArray(raw)) return raw
	try {
		const parsed = JSON.parse(raw)
		return Array.isArray(parsed) ? parsed : []
	} catch {
		return []
	}
}

const LABEL_GROUP_INDEX = {
	help: 0,
	activity: 1,
	oldstuff: 2,
	article: 3,
}

export async function getLabelOptions(type) {
	const res = await getLabelList('')
	if (res.state?.type !== 'SUCCESS') return []

	const rows = [...(res.data || [])].sort((a, b) => {
		const aid = Number(a?.lable_id || 0)
		const bid = Number(b?.lable_id || 0)
		return aid - bid
	})

	const groups = rows
		.map(row => parseLabelArray(row))
		.filter(group => group.length > 0)

	const idx = LABEL_GROUP_INDEX[type]
	if (Number.isInteger(idx) && groups[idx]) {
		return groups[idx]
	}

	return groups[0] || []
}

export function getHelpManageList(page, pagesize) {
	return request.post(
		'/webadmin/getwebhelplist',
		toFormData({ page, pagesize }),
		formConfig,
	)
}

export function getHelpDetails(id) {
	return request.post(
		'/webadmin/gethelpdetails',
		toFormData({ id }),
		formConfig,
	)
}

export function createHelp(data) {
	return request.post('/webadmin/createhelp', toFormData(data), formConfig)
}

export function updateHelp(data) {
	return request.post('/webadmin/updateehelp', toFormData(data), formConfig)
}

export function deleteHelp(helpId) {
	return request.post(
		'/webadmin/deletehelp',
		toFormData({ help_id: helpId }),
		formConfig,
	)
}

export function getActivityManageList(page, pagesize) {
	return request.post(
		'/webadmin/getwebactivitylist',
		toFormData({ page, pagesize }),
		formConfig,
	)
}

export function getActivityDetails(id) {
	return request.post(
		'/webadmin/getactivitydetails',
		toFormData({ id }),
		formConfig,
	)
}

export function createActivity(data) {
	return request.post(
		'/webadmin/createactivity',
		toFormData(data),
		formConfig,
	)
}

export function updateActivity(data) {
	return request.post(
		'/webadmin/updateactivity',
		toFormData(data),
		formConfig,
	)
}

export function deleteActivity(activityId) {
	return request.post(
		'/webadmin/deleteactivity',
		toFormData({ activity_id: activityId }),
		formConfig,
	)
}

export function getOldStuffManageList(page, pagesize) {
	return request.post(
		'/webadmin/getweboldstufflist',
		toFormData({ page, pagesize }),
		formConfig,
	)
}

export function getOldStuffDetails(id) {
	return request.post(
		'/webadmin/getoldstuffdetails',
		toFormData({ id }),
		formConfig,
	)
}

export function createOldStuff(data) {
	return request.post(
		'/webadmin/createoldstuff',
		toFormData(data),
		formConfig,
	)
}

export function updateOldStuff(data) {
	return request.post(
		'/webadmin/updateoldstuff',
		toFormData(data),
		formConfig,
	)
}

export function deleteOldStuff(oldstuffId) {
	return request.post(
		'/webadmin/deleteoldstuff',
		toFormData({ oldstuff_id: oldstuffId }),
		formConfig,
	)
}

export function getArticleManageList(page, pagesize) {
	return request.post(
		'/webadmin/articlelist',
		toFormData({ page, pagesize }),
		formConfig,
	)
}

export function getArticleDetails(id) {
	return request.post(
		'/webadmin/getarticledetails',
		toFormData({ id }),
		formConfig,
	)
}

export function createArticle(data) {
	return request.post('/webadmin/createarticle', toFormData(data), formConfig)
}

export function updateArticle(data) {
	return request.post('/webadmin/updatearticle', toFormData(data), formConfig)
}

export function deleteArticle(articleId) {
	return request.post(
		'/webadmin/deletearticle',
		toFormData({ article_id: articleId }),
		formConfig,
	)
}

export function getJoinsList(type) {
	return request.post('/webadmin/joinslist', toFormData({ type }), formConfig)
}

export function getWebJoinsList(id) {
	return request.post(
		'/webadmin/getwebjoinslist',
		toFormData({ id }),
		formConfig,
	)
}

export function deleteJoin(id) {
	return request.post('/webadmin/deletejoin', toFormData({ id }), formConfig)
}

export function setAnnouncement(data) {
	return request.post('/webadmin/setannouncement', toFormData(data), formConfig)
}

export function updateAnnouncement(data) {
	return request.post(
		'/webadmin/updateannouncement',
		toFormData(data),
		formConfig,
	)
}

export function deleteAnnouncement(announcementId) {
	return request.post(
		'/webadmin/deleteannouncement',
		toFormData({ announcement_id: announcementId }),
		formConfig,
	)
}
