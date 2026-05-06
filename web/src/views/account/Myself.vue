<template>
	<div class="myself">
		<h2>Profile Editor</h2>
		<el-tabs type="border-card">
			<el-tab-pane label="Profile Information">
				<el-form :model="form" label-width="120px">
					<el-form-item label="Avatar">
						<el-upload
							:action="uploadAction"
							:accept="IMAGE_UPLOAD_ACCEPT"
							:before-upload="beforeImageUpload"
							:headers="uploadHeaders"
							:show-file-list="false"
							:on-success="uploadAvatarSuccess"
							class="avatar-uploader"
						>
							<img
								v-if="form.avatar"
								:src="form.avatar"
								class="avatar-preview"
							/>
							<el-icon v-else class="avatar-uploader-icon"
								><Plus
							/></el-icon>
						</el-upload>
					</el-form-item>
					<el-form-item label="Nickname">
						<el-input v-model="form.nickname" />
					</el-form-item>
					<el-form-item label="Email">
						<el-input v-model="form.mail" />
					</el-form-item>
					<el-form-item label="QQ">
						<el-input v-model="form.qq" />
					</el-form-item>
					<el-form-item label="Phone">
						<el-input v-model="form.phone" />
					</el-form-item>
					<el-form-item label="Bio">
						<el-input
							v-model="form.synopsis"
							type="textarea"
							:rows="4"
						/>
					</el-form-item>
					<el-form-item>
						<el-button
							type="primary"
							:loading="savingUser"
							@click="submitUserInfo"
							>Save</el-button
						>
					</el-form-item>
				</el-form>
			</el-tab-pane>

			<el-tab-pane label="Identity Verification">
				<el-steps :active="Number(form.realstate)" align-center>
					<el-step title="Submit Information" />
					<el-step title="Under Review" />
					<el-step title="Verified" />
				</el-steps>

				<el-form
					:model="student"
					label-width="120px"
					style="margin-top: 20px"
				>
					<el-form-item label="Full Name">
						<el-input
							v-model="student.realname"
							:disabled="studentLocked"
						/>
					</el-form-item>
					<el-form-item label="Student ID">
						<el-input
							v-model="student.studentid"
							:disabled="studentLocked"
						/>
					</el-form-item>
					<el-form-item label="Student Card">
						<el-upload
							:action="uploadAction"
							:accept="IMAGE_UPLOAD_ACCEPT"
							:before-upload="beforeImageUpload"
							:headers="uploadHeaders"
							list-type="picture-card"
							:file-list="student.studentcard"
							:on-success="uploadStudentCardSuccess"
							:on-remove="removeStudentCard"
							:disabled="studentLocked"
						>
							<el-icon><Plus /></el-icon>
						</el-upload>
					</el-form-item>
					<el-form-item>
						<el-button
							type="primary"
							:disabled="studentLocked"
							:loading="savingStudent"
							@click="submitStudentInfo"
						>
							Submit Verification
						</el-button>
					</el-form-item>
				</el-form>
			</el-tab-pane>
		</el-tabs>
	</div>
</template>

<script setup>
import { computed, onMounted, reactive, ref } from 'vue'
import { useStore } from 'vuex'
import { ElMessage } from 'element-plus'
import { Plus } from '@element-plus/icons-vue'
import { getUser, updateUser } from '@/api/account'
import { IMAGE_UPLOAD_ACCEPT, validateImageFile } from '@/api/content'
import { isValidEmail, normalizeEmail } from '@/utils/validators'

const store = useStore()
const savingUser = ref(false)
const savingStudent = ref(false)

const form = reactive({
	m: 'user',
	avatar: '',
	nickname: '',
	qq: '',
	mail: '',
	phone: '',
	synopsis: '',
	realstate: 0,
	realname: '',
	studentid: '',
	studentcard: '',
})

const student = reactive({
	m: 'student',
	realname: '',
	studentid: '',
	studentcard: [],
})

const studentLocked = computed(
	() => Number(form.realstate) === 2 || Number(form.realstate) === 3,
)

const uploadAction = `${import.meta.env.VITE_API_URL || '/api'}/uplod`

const normalizeFileUrl = value => {
	if (!value || typeof value !== 'string') return ''
	if (/^https?:\/\//i.test(value)) return value
	if (value.startsWith('/api/')) return value
	if (value.startsWith('/uplodes/')) return `/api${value}`
	return `/api/uplodes/${value.replace(/^\/+/, '')}`
}

const uploadHeaders = computed(() => {
	const token =
		store.state.user?.token ||
		window.localStorage.getItem('luffy_jwt_token')
	return token ? { Authorization: `Bearer ${token}` } : {}
})

const beforeImageUpload = rawFile => {
	if (validateImageFile(rawFile)) {
		return true
	}

	ElMessage.error('Only JPG, JPEG, PNG, and WEBP images are allowed')
	return false
}

const loadUser = async () => {
	const res = await getUser()
	if (res.state?.type !== 'SUCCESS') {
		ElMessage.error('Failed to load profile data')
		return
	}

	Object.assign(form, {
		m: 'user',
		...res.data,
		avatar: normalizeFileUrl(res.data?.avatar || ''),
		realstate: Number(res.data?.realstate || 0),
	})

	let parsedCards = []
	try {
		const cards = res.data?.studentcard
		if (cards) {
			const list = typeof cards === 'string' ? JSON.parse(cards) : cards
			parsedCards = Array.isArray(list)
				? list
						.map(item => {
							if (typeof item === 'string') {
								const url = normalizeFileUrl(item)
								return url ? { url } : null
							}
							if (item?.url) {
								return {
									...item,
									url: normalizeFileUrl(item.url),
								}
							}
							return null
						})
						.filter(Boolean)
				: []
		}
	} catch {
		parsedCards = []
	}

	Object.assign(student, {
		m: 'student',
		realname: res.data?.realname || '',
		studentid: res.data?.studentid || '',
		studentcard: parsedCards,
	})
}

const uploadAvatarSuccess = res => {
	form.avatar = normalizeFileUrl(res.url)
}

const uploadStudentCardSuccess = res => {
	student.studentcard.push({ url: normalizeFileUrl(res.url) })
}

const removeStudentCard = file => {
	const index = student.studentcard.findIndex(item => item.url === file.url)
	if (index >= 0) {
		student.studentcard.splice(index, 1)
	}
}

const submitUserInfo = async () => {
	if (form.mail && !isValidEmail(form.mail)) {
		ElMessage.error('Please enter a valid email address')
		return
	}

	savingUser.value = true
	try {
		const payload = {
			m: 'user',
			avatar: form.avatar,
			nickname: form.nickname,
			mail: normalizeEmail(form.mail),
			qq: form.qq,
			phone: form.phone,
			synopsis: form.synopsis,
		}
		const res = await updateUser(payload)
		if (res.state?.type !== 'SUCCESS') {
			ElMessage.error(res.state?.msg || 'Failed to save profile')
			return
		}
		if (res.data?.userinfo) {
			store.dispatch('user/setUserInfo', res.data.userinfo)
		}
		ElMessage.success('Profile saved successfully')
		await loadUser()
	} finally {
		savingUser.value = false
	}
}

const submitStudentInfo = async () => {
	savingStudent.value = true
	try {
		const payload = {
			m: 'student',
			realname: student.realname,
			studentid: student.studentid,
			studentcard: JSON.stringify(student.studentcard),
		}
		const res = await updateUser(payload)
		if (res.state?.type === 'SUCCESS') {
			ElMessage.success('Verification submitted successfully')
			await loadUser()
			return
		}
		ElMessage.error(res.state?.msg || 'Failed to submit verification')
	} finally {
		savingStudent.value = false
	}
}

onMounted(() => {
	loadUser()
})
</script>

<style scoped>
.myself h2 {
	margin: 0 0 16px;
}

.avatar-uploader :deep(.el-upload) {
	border: 1px dashed #d9d9d9;
	border-radius: 6px;
	cursor: pointer;
	overflow: hidden;
}

.avatar-uploader :deep(.el-upload:hover) {
	border-color: #409eff;
}

.avatar-uploader-icon {
	font-size: 28px;
	color: #8c939d;
	width: 120px;
	height: 120px;
	line-height: 120px;
	text-align: center;
}

.avatar-preview {
	width: 120px;
	height: 120px;
	display: block;
	object-fit: cover;
}
</style>
