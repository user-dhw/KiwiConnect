<template>
	<div class="support-page">
		<div v-title data-title="Information Platform | Account Appeal"></div>

		<el-card class="support-card" shadow="hover">
			<div class="page-heading">
				<h1 class="page-title">Account Appeal</h1>
				<p class="page-subtitle">
					Check restriction status and submit an appeal request through a clearer,
					step-based support flow.
				</p>
			</div>
			<el-page-header @back="goBack" content="Back" />
			<el-divider />

			<form v-if="currentStep === STEP.LOGIN" @submit.prevent="handleLogin">
				<el-form
					:model="loginForm"
					label-width="90px"
					class="section"
				>
				<el-form-item label="Username">
					<el-input
						v-model.trim="loginForm.username"
						placeholder="Enter your account username"
					/>
				</el-form-item>
				<el-form-item label="Password">
					<el-input
						v-model="loginForm.password"
						type="password"
						show-password
						placeholder="Enter your password"
					/>
				</el-form-item>
				<el-form-item>
					<el-button type="primary" native-type="submit" :loading="isLoginLoading">
						Sign In
					</el-button>
				</el-form-item>
				</el-form>
			</form>

			<div v-else-if="currentStep === STEP.STATUS" class="section">
				<appeal-status-panel
					:restricted="isRestricted"
					:unlock-time="unlockTimeText"
					:report="reportDetail"
					:loading="isReportLoading"
				/>

				<div v-if="isRestricted" class="action-row">
					<el-button type="primary" @click="currentStep = STEP.APPEAL">
						Proceed to Appeal
					</el-button>
				</div>
			</div>

			<form v-else-if="currentStep === STEP.APPEAL" @submit.prevent="submitAppealForm">
				<el-form
					:model="appealForm"
					label-width="120px"
					class="section"
				>
				<el-form-item label="Appeal Description">
					<el-input
						v-model.trim="appealForm.shensu_content"
						type="textarea"
						:rows="8"
						placeholder="Describe your appeal details"
					/>
				</el-form-item>
				<el-form-item>
					<el-button
						type="primary"
						native-type="submit"
						:loading="isSubmitLoading"
					>
						Submit Appeal
					</el-button>
					<el-button @click="currentStep = STEP.STATUS">Cancel</el-button>
				</el-form-item>
				</el-form>
			</form>

			<el-result
				v-else
				icon="success"
				title="Appeal submitted successfully"
				sub-title="We will review your request as soon as possible."
			>
				<template #extra>
					<el-button type="primary" @click="goBack">Back</el-button>
				</template>
			</el-result>
		</el-card>
	</div>
</template>

<script setup>
import { computed, reactive, ref } from 'vue'
import { useRouter } from 'vue-router'
import { ElMessage } from 'element-plus'
import { getReportContent, loginForAppeal, submitAppeal } from '@/api/support'
import AppealStatusPanel from '@/components/support/AppealStatusPanel.vue'

const STEP = Object.freeze({
	LOGIN: 0,
	STATUS: 1,
	APPEAL: 2,
	SUCCESS: 3,
})

const router = useRouter()
const currentStep = ref(STEP.LOGIN)
const isLoginLoading = ref(false)
const isReportLoading = ref(false)
const isSubmitLoading = ref(false)

const loginForm = reactive({
	username: '',
	password: '',
})

const appealForm = reactive({
	shensu_content: '',
})

const accountData = ref(null)
const reportDetail = ref(null)

const isRestricted = computed(() => {
	const activation = Number(accountData.value?.shensu?.activationdate || 0)
	return activation > Date.now()
})

const unlockTimeText = computed(() => {
	const activation = Number(accountData.value?.shensu?.activationdate || 0)
	if (!activation) return '-'

	return new Intl.DateTimeFormat('en-NZ', {
		year: 'numeric',
		month: '2-digit',
		day: '2-digit',
		hour: '2-digit',
		minute: '2-digit',
		hour12: false,
	}).format(new Date(activation))
})

const normalizeReportImages = raw => {
	if (!raw) return []
	if (Array.isArray(raw)) return raw

	if (typeof raw === 'string') {
		try {
			const parsed = JSON.parse(raw)
			return Array.isArray(parsed) ? parsed : []
		} catch {
			return []
		}
	}

	return []
}

const normalizeReport = data => {
	const images = normalizeReportImages(data?.jubao_img)
	return {
		...(data || {}),
		jubao_img: images.map((item, index) => {
			if (typeof item === 'string') {
				return { id: String(index), url: item }
			}
			return {
				id: item?.id || String(index),
				url: item?.url || '',
			}
		}),
	}
}

const goBack = () => {
	router.back()
}

const loadReportDetail = async () => {
	const reportId = accountData.value?.shensu?.jubao_id
	if (!reportId) {
		reportDetail.value = null
		return
	}

	isReportLoading.value = true
	try {
		const res = await getReportContent(reportId)
		if (res.state?.type !== 'SUCCESS') {
			ElMessage.error(res.state?.msg || 'Failed to load report details')
			return
		}

		reportDetail.value = normalizeReport(res.data)
	} catch {
		ElMessage.error('Failed to load report details')
	} finally {
		isReportLoading.value = false
	}
}

const handleLogin = async () => {
	if (!loginForm.username || !loginForm.password) {
		ElMessage.error('Username and password are required')
		return
	}

	isLoginLoading.value = true
	try {
		const res = await loginForAppeal({
			username: loginForm.username,
			password: loginForm.password,
		})

		if (res.state?.type !== 'SUCCESS') {
			ElMessage.error(res.state?.msg || 'Invalid username or password')
			return
		}

		accountData.value = res.data || null
		currentStep.value = STEP.STATUS

		if (isRestricted.value) {
			await loadReportDetail()
		}
	} catch {
		ElMessage.error('Login failed, please try again later')
	} finally {
		isLoginLoading.value = false
	}
}

const submitAppealForm = async () => {
	if (!appealForm.shensu_content) {
		ElMessage.error('Please enter an appeal description')
		return
	}

	const shensuUser = accountData.value?.shensu?.username
	const shensuJubaoId = accountData.value?.shensu?.jubao_id

	if (!shensuUser || !shensuJubaoId) {
		ElMessage.error('Missing appeal account information')
		return
	}

	isSubmitLoading.value = true
	try {
		const res = await submitAppeal({
			shensu_content: appealForm.shensu_content,
			shensu_user: shensuUser,
			shensu_jubao_id: shensuJubaoId,
		})

		if (res.state?.type !== 'SUCCESS') {
			ElMessage.error(res.state?.msg || 'Failed to submit appeal')
			return
		}

		ElMessage.success('Appeal submitted successfully')
		currentStep.value = STEP.SUCCESS
	} catch {
		ElMessage.error('Failed to submit appeal')
	} finally {
		isSubmitLoading.value = false
	}
}
</script>

<style scoped>
.action-row {
	margin-top: 18px;
}
</style>
