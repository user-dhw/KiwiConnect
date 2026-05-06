<template>
	<div class="edit-page">
		<h2>{{ isEdit ? 'Edit Article / News' : 'Create Article / News' }}</h2>
		<el-form :model="article" label-width="130px">
			<el-form-item label="Title">
				<el-input v-model="article.article_title" />
			</el-form-item>
			<el-form-item label="Introduction">
				<el-input
					v-model="article.article_introduction"
					type="textarea"
					:rows="3"
				/>
			</el-form-item>
			<el-form-item label="Category">
				<el-radio-group v-model="article.article_lable">
					<el-radio
						v-for="item in labels"
						:key="item"
						:value="item"
						>{{ item }}</el-radio
					>
				</el-radio-group>
			</el-form-item>
			<el-form-item label="Content">
				<el-input
					v-model="article.article_content"
					type="textarea"
					:rows="14"
				/>
			</el-form-item>
			<el-form-item>
				<el-button type="primary" @click="submit">Save</el-button>
				<el-button @click="router.push('/admin/articlelist')"
					>Cancel</el-button
				>
			</el-form-item>
		</el-form>
	</div>
</template>

<script setup>
import { computed, onMounted, ref } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { ElMessage } from 'element-plus'
import {
	createArticle,
	getArticleDetails,
	getLabelOptions,
	updateArticle,
} from '@/api/content'

const route = useRoute()
const router = useRouter()

const id = computed(() => route.params.id || '')
const isEdit = computed(() => !!id.value)

const labels = ref([])
const article = ref({
	article_title: '',
	article_introduction: '',
	article_lable: '',
	article_content: '',
})

const submit = async () => {
	if (
		!article.value.article_title ||
		!article.value.article_lable ||
		!article.value.article_content
	) {
		ElMessage.error('Title, category, and content are required')
		return
	}
	const res = isEdit.value
		? await updateArticle({ ...article.value, article_id: id.value })
		: await createArticle(article.value)
	if (res.state?.type === 'SUCCESS') {
		if (isEdit.value) {
			ElMessage.success('Saved successfully')
		} else {
			ElMessage.success(
				'Article submitted successfully. It will be visible to others after admin review.',
			)
		}
		router.push('/admin/articlelist')
		return
	}
	ElMessage.error(res.state?.msg || 'Save failed')
}

const loadLabels = async () => {
	labels.value = await getLabelOptions('article')
}

const loadDetails = async () => {
	if (!isEdit.value) return
	const res = await getArticleDetails(id.value)
	if (res.state?.type === 'SUCCESS') {
		article.value = { ...article.value, ...(res.data || {}) }
	}
}

onMounted(() => {
	loadLabels()
	loadDetails()
})
</script>
