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

export function getUser() {
	return request.post('/webadmin/getuser')
}

export function getUserNumbering() {
	return request.post('/webadmin/getusernumbering')
}

export function updateUser(data) {
	return request.post('/webadmin/updatauser', toFormData(data), formConfig)
}

export function getNoticeList(num = '') {
	return request.post('/web/getnotice', toFormData({ num }), formConfig)
}

export function changeNotice(change, notice_id) {
	return request.post(
		'/web/changenotice',
		toFormData({ change, notice_id }),
		formConfig,
	)
}
