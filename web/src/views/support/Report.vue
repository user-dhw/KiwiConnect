<template>
	<div class="support-page">
		<div v-title data-title="Information Platform | Report Violation"></div>
		<el-card class="support-card" shadow="hover">
			<el-page-header @back="goBack" content="Report Violation" />
			<el-divider />

			<el-alert
				v-if="hasPrefilledContext"
				type="info"
				show-icon
				:closable="false"
				class="prefill-alert"
			>
				<template #title> Auto-filled from violation link </template>
				<div v-if="form.jubao_user">
					Reported account: {{ form.jubao_user }}
				</div>
				<div v-if="form.jubao_url">
					Violation URL: {{ form.jubao_url }}
				</div>
			</el-alert>

			<el-form
				v-if="step === STEP.FORM"
				:model="form"
				label-width="130px"
				class="section"
				@submit.prevent
			>
				<el-form-item label="Reported Account">
					<el-input
						v-model.trim="form.jubao_user"
						placeholder="Enter account username"
					/>
				</el-form-item>

				<el-form-item label="Content URL">
					<el-input
						v-model.trim="form.jubao_url"
						placeholder="Enter the violating content URL"
					/>
				</el-form-item>

				<el-form-item label="Screenshots">
					<el-upload
						v-model:file-list="uploadFileList"
						list-type="picture-card"
						:http-request="uploadRequest"
						:on-remove="handleRemove"
						:on-preview="handlePreview"
					>
						<el-icon><Plus /></el-icon>
					</el-upload>

					<el-dialog
						v-model="previewVisible"
						title="Preview"
						width="680px"
					>
						<img
							:src="previewImage"
							alt="Report screenshot"
							class="preview-image"
						/>
					</el-dialog>
				</el-form-item>

				<el-form-item label="Description">
					<el-input
						v-model.trim="form.jubao_content"
						type="textarea"
						:rows="8"
						placeholder="Please describe the violation details"
					/>
				</el-form-item>

				<el-form-item>
					<el-button
						type="primary"
						:loading="isSubmitting"
						@click="handleSubmit"
					>
						Submit
					</el-button>
					<el-button @click="resetForm">Cancel</el-button>
				</el-form-item>
			</el-form>

			<support-submit-success
				v-else
				title="Report submitted successfully"
				description="Thank you for your report. We will review it and take proper action."
				:on-back="goBack"
			/>
		</el-card>
	</div>
</template>

<script setup>
import { computed, onMounted, reactive, ref } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { ElMessage } from 'element-plus'
import { Plus } from '@element-plus/icons-vue'
import {
	createReport,
	uploadSupportImage,
	validateSupportImage,
} from '@/api/support'
import SupportSubmitSuccess from '@/components/support/SupportSubmitSuccess.vue'

const STEP = Object.freeze({
	FORM: 0,
	SUCCESS: 1,
})

const router = useRouter()
const route = useRoute()

const step = ref(STEP.FORM)
const isSubmitting = ref(false)
const previewVisible = ref(false)
const previewImage = ref('')
const uploadFileList = ref([])

const form = reactive({
	jubao_content: '',
	jubao_user: '',
	jubao_img: [],
	jubao_url: '',
})

const hasPrefilledContext = computed(() =>
	Boolean(form.jubao_user || form.jubao_url),
)

const goBack = () => {
	router.back()
}

const resetForm = () => {
	form.jubao_content = ''
	form.jubao_user = ''
	form.jubao_img = []
	form.jubao_url = ''
	uploadFileList.value = []
}

const handleRemove = file => {
	form.jubao_img = form.jubao_img.filter(item => item.url !== file.url)
}

const handlePreview = file => {
	previewImage.value = file.url || ''
	previewVisible.value = true
}

const pickQueryValue = value => {
	if (Array.isArray(value)) {
		return typeof value[0] === 'string' ? value[0] : ''
	}
	return typeof value === 'string' ? value : ''
}

const safeDecode = value => {
	if (!value) return ''
	try {
		return decodeURIComponent(value)
	} catch {
		return value
	}
}

const uploadRequest = async uploadOption => {
	try {
		if (!validateSupportImage(uploadOption.file)) {
			throw new Error('Only JPG, JPEG, PNG, and WEBP images are allowed')
		}

		const uploaded = await uploadSupportImage(uploadOption.file)
		if (!uploaded.url) {
			throw new Error('Upload failed')
		}

		const imageItem = { url: uploaded.url }
		form.jubao_img.push(imageItem)
		uploadOption.onSuccess(imageItem)
	} catch (error) {
		ElMessage.error(error.message || 'Image upload failed')
		uploadOption.onError(error)
	}
}

const setRouteDefaults = () => {
	const queryUrl = pickQueryValue(route.query?.url)
	const queryUser = pickQueryValue(route.query?.user)

	if (queryUrl) {
		form.jubao_url = safeDecode(queryUrl)
	}

	if (queryUser) {
		form.jubao_user = queryUser
	}
}

const handleSubmit = async () => {
	if (!form.jubao_user || !form.jubao_url || !form.jubao_content) {
		ElMessage.error('Please complete all required fields')
		return
	}

	isSubmitting.value = true
	try {
		const payload = {
			jubao_user: form.jubao_user,
			jubao_url: form.jubao_url,
			jubao_content: form.jubao_content,
			jubao_img: JSON.stringify(form.jubao_img),
		}

		const res = await createReport(payload)
		if (res.state?.type !== 'SUCCESS') {
			ElMessage.error(res.state?.msg || 'Failed to submit report')
			return
		}

		ElMessage.success('Report submitted successfully')
		step.value = STEP.SUCCESS
	} catch {
		ElMessage.error('Failed to submit report')
	} finally {
		isSubmitting.value = false
	}
}

onMounted(() => {
	setRouteDefaults()
})
</script>

<style scoped>
.support-page {
	padding: 20px;
}

.support-card {
	max-width: 980px;
	margin: 0 auto;
}

.section {
	max-width: 760px;
	margin: 0 auto;
}

.prefill-alert {
	margin-bottom: 16px;
}

.preview-image {
	width: 100%;
	display: block;
}
</style>
