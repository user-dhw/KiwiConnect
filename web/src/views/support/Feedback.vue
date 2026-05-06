<template>
	<div class="support-page">
		<div v-title data-title="Information Platform | Feedback"></div>
		<el-card class="support-card" shadow="hover">
			<div class="page-heading">
				<h1 class="page-title">Feedback</h1>
				<p class="page-subtitle">
					Share ideas, report UX friction, or suggest improvements for the student
					community platform.
				</p>
			</div>
			<el-page-header @back="goBack" content="Back" />
			<el-divider />

			<el-form
				v-if="step === STEP.FORM"
				:model="form"
				label-width="90px"
				class="section"
				@submit.prevent
			>
				<el-form-item label="Email">
					<el-input
						v-model.trim="form.fankui_user"
						placeholder="Enter your email"
					/>
				</el-form-item>
				<el-form-item label="Feedback">
					<el-input
						v-model.trim="form.fankui_content"
						type="textarea"
						:rows="8"
						placeholder="Please describe your feedback"
					/>
				</el-form-item>
				<el-form-item>
					<el-button type="primary" :loading="isSubmitting" @click="handleSubmit">
						Submit
					</el-button>
					<el-button @click="resetForm">Cancel</el-button>
				</el-form-item>
			</el-form>

			<support-submit-success
				v-else
				title="Feedback submitted successfully"
				description="Thank you for your feedback. We will review and improve the platform accordingly."
				:on-back="goBack"
			/>
		</el-card>
	</div>
</template>

<script setup>
import { reactive, ref } from 'vue'
import { useRouter } from 'vue-router'
import { useStore } from 'vuex'
import { ElMessage } from 'element-plus'
import { createFeedback } from '@/api/support'
import SupportSubmitSuccess from '@/components/support/SupportSubmitSuccess.vue'
import { isValidEmail, normalizeEmail } from '@/utils/validators'

const STEP = Object.freeze({
	FORM: 0,
	SUCCESS: 1,
})

const router = useRouter()
const store = useStore()
const step = ref(STEP.FORM)
const isSubmitting = ref(false)

const form = reactive({
	fankui_content: '',
	fankui_user: '',
})

const goBack = () => {
	router.back()
}

const openLoginDialog = () => {
	store.dispatch('user/join', true)
	store.dispatch('user/close', true)
}

const resetForm = () => {
	form.fankui_content = ''
	form.fankui_user = ''
}

const handleSubmit = async () => {
	if (!form.fankui_user || !form.fankui_content) {
		ElMessage.error('Email and feedback content are required')
		return
	}

	if (!isValidEmail(form.fankui_user)) {
		ElMessage.error('Please enter a valid email address')
		return
	}

	isSubmitting.value = true
	try {
		const res = await createFeedback({
			...form,
			fankui_user: normalizeEmail(form.fankui_user),
			fankui_content: form.fankui_content.trim(),
		})
		if (res.state?.type !== 'SUCCESS') {
			ElMessage.error(res.state?.msg || 'You have to login to submit feedback')
			openLoginDialog()
			return
		}

		ElMessage.success('Feedback submitted successfully')
		step.value = STEP.SUCCESS
	} catch {
		ElMessage.error('You have to login to submit feedback')
		openLoginDialog()
	} finally {
		isSubmitting.value = false
	}
}
</script>
