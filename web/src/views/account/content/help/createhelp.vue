<template>
	<div class="edit-page">
		<div class="page-heading">
			<h1 class="page-title">{{ isEdit ? 'Edit Q&A' : 'Create Q&A' }}</h1>
			<p class="page-subtitle">Publish a question, guide, or discussion topic for the campus community.</p>
		</div>

		<section class="admin-form-card">
			<el-form :model="form" label-width="120px" class="admin-form">
				<el-form-item label="Title">
					<el-input v-model="form.help_title" />
				</el-form-item>
				<el-form-item label="Category">
					<el-radio-group v-model="form.help_lable">
						<el-radio v-for="item in labels" :key="item" :value="item">
							{{ item }}
						</el-radio>
					</el-radio-group>
				</el-form-item>
				<el-form-item label="Tags">
					<el-tag v-for="tag in dynamicTags" :key="tag" closable @close="removeTag(tag)">
						{{ tag }}
					</el-tag>
					<el-input
						v-if="inputVisible"
						ref="tagInput"
						v-model="inputValue"
						class="input-new-tag"
						@keyup.enter="confirmTag"
						@blur="confirmTag"
					/>
					<el-button v-else class="button-new-tag" @click="showTagInput">
						+ Add Tag
					</el-button>
				</el-form-item>
				<el-form-item label="Content">
					<el-input v-model="form.help_content" type="textarea" :rows="12" />
				</el-form-item>
				<el-form-item>
					<div class="inline-actions">
						<el-button type="primary" @click="submit">Save</el-button>
						<el-button @click="router.push('/admin/createhelplist')">Cancel</el-button>
					</div>
				</el-form-item>
			</el-form>
		</section>
	</div>
</template>

<script setup>
import { computed, nextTick, onMounted, ref } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { ElMessage } from 'element-plus'
import {
	createHelp,
	getHelpDetails,
	getLabelOptions,
	updateHelp,
} from '@/api/content'

const route = useRoute()
const router = useRouter()

const id = computed(() => route.params.id || '')
const isEdit = computed(() => !!id.value)

const labels = ref([])
const dynamicTags = ref([])
const inputVisible = ref(false)
const inputValue = ref('')
const tagInput = ref(null)

const form = ref({
	help_tag: '',
	help_title: '',
	help_lable: '',
	help_content: '',
})

const removeTag = tag => {
	dynamicTags.value = dynamicTags.value.filter(item => item !== tag)
}

const showTagInput = async () => {
	inputVisible.value = true
	await nextTick()
	tagInput.value?.focus()
}

const confirmTag = () => {
	const val = inputValue.value.trim()
	if (val) dynamicTags.value.push(val)
	inputValue.value = ''
	inputVisible.value = false
}

const submit = async () => {
	if (!form.value.help_title || !form.value.help_lable || !form.value.help_content) {
		ElMessage.error('Title, category, and content are required')
		return
	}
	form.value.help_tag = dynamicTags.value.join(',')

	try {
		const payload = { ...form.value }
		const res = isEdit.value
			? await updateHelp({ ...payload, id: id.value })
			: await createHelp(payload)
		if (res.state?.type === 'SUCCESS') {
			if (isEdit.value) {
				ElMessage.success('Saved successfully')
			} else {
				ElMessage.success(
					'Q&A submitted successfully. It will be visible to others after admin review.',
				)
			}
			router.push('/admin/createhelplist')
			return
		}
		ElMessage.error(res.state?.msg || 'Save failed')
	} catch {
		ElMessage.error('Save failed')
	}
}

const loadLabels = async () => {
	labels.value = await getLabelOptions('help')
}

const loadDetails = async () => {
	if (!isEdit.value) return
	const res = await getHelpDetails(id.value)
	if (res.state?.type !== 'SUCCESS') return
	const data = res.data || {}
	form.value.help_title = data.help_title || ''
	form.value.help_lable = data.help_lable || ''
	form.value.help_content = data.help_content || ''
	dynamicTags.value = data.help_tag ? String(data.help_tag).split(',').filter(Boolean) : []
}

onMounted(() => {
	loadLabels()
	loadDetails()
})
</script>

<style scoped>
.button-new-tag {
	margin-left: 10px;
}

.input-new-tag {
	width: 140px;
	margin-left: 10px;
}
</style>
