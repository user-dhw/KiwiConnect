<template>
	<div class="edit-page">
		<div class="page-heading">
			<h1 class="page-title">{{ isEdit ? 'Edit Listing' : 'Create Listing' }}</h1>
			<p class="page-subtitle">Publish a second-hand item with image, pricing, and description.</p>
		</div>

		<el-tabs type="border-card" class="admin-tabs">
			<el-tab-pane label="Listing Details">
				<section class="admin-form-card">
					<form @submit.prevent="submit">
						<el-form :model="form" label-width="130px" class="admin-form">
						<el-form-item label="Item Image">
							<div class="image-preview-wrap">
								<el-image
									v-if="previewItemImg"
									:src="previewItemImg"
									:preview-src-list="[previewItemImg]"
									fit="cover"
									class="image-preview"
								/>
								<div v-else class="image-placeholder">No image uploaded</div>
							</div>
						</el-form-item>
						<el-form-item label="Or Upload Image">
							<el-upload
								:auto-upload="false"
								:accept="IMAGE_UPLOAD_ACCEPT"
								:before-upload="beforeImageUpload"
								:show-file-list="false"
								:on-change="handleUploadChange"
							>
								<el-button>Upload</el-button>
							</el-upload>
						</el-form-item>
						<el-form-item label="Item Name">
							<el-input v-model="form.oldstuff_name" />
						</el-form-item>
						<el-form-item label="Price">
							<el-input v-model="form.oldstuff_price" />
						</el-form-item>
						<el-form-item label="Category">
							<el-radio-group v-model="form.oldstuff_lable">
								<el-radio v-for="item in labels" :key="item" :value="item">
									{{ item }}
								</el-radio>
							</el-radio-group>
						</el-form-item>
						<el-form-item label="Description">
							<el-input v-model="form.oldstuff_content" type="textarea" :rows="10" />
						</el-form-item>
						<el-form-item>
							<div class="inline-actions">
								<el-button type="primary" native-type="submit">Save</el-button>
								<el-button @click="router.push('/admin/createoldstufflist')">Cancel</el-button>
							</div>
						</el-form-item>
						</el-form>
					</form>
				</section>
			</el-tab-pane>

			<el-tab-pane label="Purchase Interests" v-if="isEdit">
				<section class="admin-panel admin-table">
					<el-table :data="joinTableData" border style="width: 100%">
						<el-table-column label="Created At" width="180">
							<template #default="scope">{{ formatDate(scope.row.joins_createtime) }}</template>
						</el-table-column>
						<el-table-column prop="describe" label="Contact Info" min-width="240" />
						<el-table-column prop="name" label="Offer" width="180" />
					</el-table>
				</section>
			</el-tab-pane>
		</el-tabs>
	</div>
</template>

<script setup>
import { computed, onMounted, ref } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { ElMessage } from 'element-plus'
import moment from 'moment'
import {
	createOldStuff,
	getLabelOptions,
	getOldStuffDetails,
	getWebJoinsList,
	IMAGE_UPLOAD_ACCEPT,
	uploadFile,
	updateOldStuff,
	validateImageFile,
} from '@/api/content'

const route = useRoute()
const router = useRouter()

const id = computed(() => route.params.id || '')
const isEdit = computed(() => !!id.value)
const labels = ref([])
const joinTableData = ref([])

const form = ref({
	oldstuff_img: '',
	oldstuff_name: '',
	oldstuff_price: '',
	oldstuff_content: '',
	oldstuff_lable: '',
})

const normalizeImageUrl = value => {
	const src = String(value || '').trim()
	if (!src) return ''
	if (/^https?:\/\//i.test(src)) return src
	if (src.startsWith('/api/')) return src
	if (src.startsWith('/uplodes/')) return `/api${src}`
	if (src.startsWith('/')) return `/api${src}`
	return `/api/${src}`
}

const previewItemImg = computed(() => normalizeImageUrl(form.value.oldstuff_img))

const formatDate = value => moment(value).format('YYYY-MM-DD HH:mm')

const beforeImageUpload = rawFile => {
	if (validateImageFile(rawFile)) {
		return true
	}

	ElMessage.error('Only JPG, JPEG, PNG, and WEBP images are allowed')
	return false
}

const handleUploadChange = async uploadFileItem => {
	try {
		const res = await uploadFile(uploadFileItem.raw)
		if (res?.url) {
			form.value.oldstuff_img = res.url
			ElMessage.success('Image uploaded')
		}
	} catch {
		ElMessage.error('Image upload failed')
	}
}

const submit = async () => {
	if (!form.value.oldstuff_name || !form.value.oldstuff_price || !form.value.oldstuff_lable) {
		ElMessage.error('Name, price, and category are required')
		return
	}
	const res = isEdit.value
		? await updateOldStuff({ ...form.value, oldstuff_id: id.value })
		: await createOldStuff(form.value)
	if (res.state?.type === 'SUCCESS') {
		if (isEdit.value) {
			ElMessage.success('Saved successfully')
		} else {
			ElMessage.success(
				'Item submitted successfully. It will be visible to others after admin review.',
			)
		}
		router.push('/admin/createoldstufflist')
		return
	}
	ElMessage.error(res.state?.msg || 'Save failed')
}

const loadLabels = async () => {
	labels.value = await getLabelOptions('oldstuff')
}

const loadDetails = async () => {
	if (!isEdit.value) return
	const res = await getOldStuffDetails(id.value)
	if (res.state?.type === 'SUCCESS') {
		form.value = { ...form.value, ...(res.data || {}) }
	}
}

const loadJoins = async () => {
	if (!isEdit.value) return
	const res = await getWebJoinsList(id.value)
	if (res.state?.type === 'SUCCESS') {
		joinTableData.value = res.data || []
	}
}

onMounted(() => {
	loadLabels()
	loadDetails()
	loadJoins()
})
</script>

<style scoped>
.image-preview-wrap {
	width: 220px;
	height: 140px;
	border: 1px dashed #dcdfe6;
	border-radius: 16px;
	overflow: hidden;
	background: #fafafa;
}

.image-preview {
	width: 100%;
	height: 100%;
	display: block;
}

.image-placeholder {
	width: 100%;
	height: 100%;
	display: flex;
	align-items: center;
	justify-content: center;
	color: #909399;
	font-size: 13px;
}
</style>
