<template>
	<div class="manage-page">
		<div class="page-header">
			<h2>Q&A Management</h2>
			<el-button type="primary" @click="router.push('/admin/createhelp')">
				Create Q&A
			</el-button>
		</div>

		<el-table
			:data="tableData"
			border
			v-loading="loading"
			style="width: 100%"
		>
			<el-table-column prop="createtime" label="Date" width="140">
				<template #default="scope">
					{{ formatDate(scope.row.createtime) }}
				</template>
			</el-table-column>
			<el-table-column prop="help_title" label="Title" min-width="260" />
			<el-table-column prop="help_lable" label="Category" width="100" />
			<el-table-column label="Status" width="140">
				<template #default="scope">
					{{ mapStatus(scope.row.ispublic) }}
				</template>
			</el-table-column>
			<el-table-column label="Actions" width="170" fixed="right">
				<template #default="scope">
					<el-button
						text
						type="primary"
						@click="
							router.push(
								`/admin/updatehelp/${scope.row.help_id}`,
							)
						"
					>
						Edit
					</el-button>
					<el-button
						text
						type="danger"
						@click="removeItem(scope.row.help_id)"
					>
						Delete
					</el-button>
				</template>
			</el-table-column>
		</el-table>

		<div class="pager-wrap">
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
	</div>
</template>

<script setup>
import { onMounted, reactive, ref } from 'vue'
import { useRouter } from 'vue-router'
import { ElMessage, ElMessageBox } from 'element-plus'
import moment from 'moment'
import { deleteHelp, getHelpManageList } from '@/api/content'

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
		const res = await getHelpManageList(query.page, query.pagesize)
		if (res.state?.type === 'SUCCESS') {
			tableData.value = res.data || []
			query.total = res.count || 0
			return
		}
		ElMessage.error(res.state?.msg || 'Failed to load Q&A list')
	} catch {
		ElMessage.error('Failed to load Q&A list')
	} finally {
		loading.value = false
	}
}

const removeItem = async helpId => {
	try {
		await ElMessageBox.confirm(
			'Delete this Q&A item permanently?',
			'Confirm Deletion',
			{ type: 'warning' },
		)
		const res = await deleteHelp(helpId)
		if (res.state?.type === 'SUCCESS') {
			ElMessage.success('Deleted successfully')
			loadList()
			return
		}
		ElMessage.error(res.state?.msg || 'Delete failed')
	} catch {
		// User cancelled or request failed.
	}
}

onMounted(() => {
	loadList()
})
</script>

<style scoped>
.page-header {
	display: flex;
	justify-content: space-between;
	align-items: center;
	margin-bottom: 12px;
}

.pager-wrap {
	margin-top: 14px;
	display: flex;
	justify-content: flex-end;
}
</style>
