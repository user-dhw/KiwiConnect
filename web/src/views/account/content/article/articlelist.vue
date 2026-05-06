<template>
	<div class="manage-page">
		<div class="admin-page-head">
			<div class="page-heading">
				<h1 class="page-title">Articles / News Management</h1>
				<p class="page-subtitle">Manage published articles, review states, and edit news content.</p>
			</div>
			<el-button type="primary" @click="router.push('/admin/createarticle')">
				Create Article
			</el-button>
		</div>

		<section class="admin-panel admin-table">
			<el-table :data="tableData" border v-loading="loading" style="width: 100%">
				<el-table-column label="Date" width="150">
					<template #default="scope">{{ formatDate(scope.row.article_createtime) }}</template>
				</el-table-column>
				<el-table-column prop="article_title" label="Title" min-width="280" />
				<el-table-column prop="article_lable" label="Category" width="100" />
				<el-table-column label="Status" width="140">
					<template #default="scope">{{ mapStatus(scope.row.ispublic) }}</template>
				</el-table-column>
				<el-table-column label="Actions" width="170" fixed="right">
					<template #default="scope">
						<el-button
							text
							type="primary"
							@click="router.push(`/admin/updataarticle/${scope.row.article_id}`)"
						>
							Edit
						</el-button>
						<el-button text type="danger" @click="handleDelete(scope.row)">
							Delete
						</el-button>
					</template>
				</el-table-column>
			</el-table>

			<div class="admin-pagination">
				<el-pagination
					v-model:current-page="query.page"
					v-model:page-size="query.pagesize"
					:page-sizes="[10, 20, 50, 100]"
					layout="total, sizes, prev, pager, next, jumper"
					:total="query.total"
					@size-change="loadList"
					@current-change="loadList"
				/>
			</div>
		</section>
	</div>
</template>

<script setup>
import { onMounted, reactive, ref } from 'vue'
import { useRouter } from 'vue-router'
import { ElMessage, ElMessageBox } from 'element-plus'
import moment from 'moment'
import { deleteArticle, getArticleManageList } from '@/api/content'

const router = useRouter()
const loading = ref(false)
const tableData = ref([])

const query = reactive({
	total: 0,
	page: 1,
	pagesize: 10,
})

const formatDate = value => moment(value).format('YYYY-MM-DD HH:mm')

const mapStatus = state => {
	if (String(state) === '1') return 'Approved'
	if (String(state) === '0') return 'Pending Review'
	if (String(state) === '-1') return 'Rejected'
	return 'Unknown'
}

const loadList = async () => {
	loading.value = true
	try {
		const res = await getArticleManageList(query.page, query.pagesize)
		if (res.state?.type === 'SUCCESS') {
			tableData.value = res.data || []
			query.total = res.count || 0
			return
		}
		ElMessage.error(res.state?.msg || 'Failed to load article list')
	} catch {
		ElMessage.error('Failed to load article list')
	} finally {
		loading.value = false
	}
}

const handleDelete = async row => {
	const articleId = row?.article_id
	if (!articleId) return

	try {
		await ElMessageBox.confirm(
			'This action will permanently delete the article. Continue?',
			'Confirm Deletion',
			{
				confirmButtonText: 'Delete',
				cancelButtonText: 'Cancel',
				type: 'warning',
			},
		)
	} catch {
		return
	}

	try {
		const res = await deleteArticle(articleId)
		if (res.state?.type !== 'SUCCESS') {
			ElMessage.error(res.state?.msg || 'Failed to delete article')
			return
		}

		ElMessage.success('Article deleted')
		if (tableData.value.length === 1 && query.page > 1) {
			query.page -= 1
		}
		await loadList()
	} catch {
		ElMessage.error('Failed to delete article')
	}
}

onMounted(() => {
	loadList()
})
</script>
