<template>
	<div class="manage-page">
		<div class="page-heading">
			<h1 class="page-title">Activity Management</h1>
			<p class="page-subtitle">Manage the events you publish and the activities you join.</p>
		</div>

		<el-tabs type="border-card" class="admin-tabs">
			<el-tab-pane label="My Published Activities">
				<div class="admin-panel admin-table">
					<div class="admin-page-head">
						<div></div>
						<el-button type="primary" @click="router.push('/admin/createactivity')">
							Create Activity
						</el-button>
					</div>
					<el-table :data="tableData" border v-loading="loading" style="width: 100%">
						<el-table-column label="Date" width="140">
							<template #default="scope">{{ formatDate(scope.row.createtime) }}</template>
						</el-table-column>
						<el-table-column prop="activity_title" label="Title" min-width="280" />
						<el-table-column prop="activity_lable" label="Category" width="100" />
						<el-table-column label="Actions" width="170" fixed="right">
							<template #default="scope">
								<el-button
									text
									type="primary"
									@click="router.push(`/admin/updateactivity/${scope.row.activity_id}`)"
								>
									Edit
								</el-button>
								<el-button text type="danger" @click="removeItem(scope.row.activity_id)">
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
				</div>
			</el-tab-pane>

			<el-tab-pane label="My Joined Activities">
				<div class="admin-panel admin-table">
					<el-table :data="joinTableData" border v-loading="joinLoading" style="width: 100%">
						<el-table-column label="Joined At" width="180">
							<template #default="scope">{{ formatDate(scope.row.joins_createtime) }}</template>
						</el-table-column>
						<el-table-column prop="activity_title" label="Title" min-width="280" />
						<el-table-column prop="activity_lable" label="Category" width="180" />
						<el-table-column label="Actions" width="180" fixed="right">
							<template #default="scope">
								<el-link
									:href="getActivityDetailHref(scope.row.activity_id)"
									target="_blank"
									type="primary"
								>
									View
								</el-link>
								<el-button text type="danger" @click="removeJoin(scope.row.join_id)">
									Cancel Join
								</el-button>
							</template>
						</el-table-column>
					</el-table>
				</div>
			</el-tab-pane>
		</el-tabs>
	</div>
</template>

<script setup>
import { onMounted, reactive, ref } from 'vue'
import { useRouter } from 'vue-router'
import { ElMessage, ElMessageBox } from 'element-plus'
import moment from 'moment'
import {
	deleteActivity,
	deleteJoin,
	getActivityManageList,
	getJoinsList,
} from '@/api/content'

const router = useRouter()

const loading = ref(false)
const joinLoading = ref(false)
const tableData = ref([])
const joinTableData = ref([])

const query = reactive({
	total: 0,
	page: 1,
	pagesize: 10,
})

const formatDate = value => moment(value).format('YYYY-MM-DD HH:mm')

const getActivityDetailHref = activityId => {
	if (!activityId) return '#'
	return router.resolve({ path: `/activitycontent/${activityId}` }).href
}

const loadList = async () => {
	loading.value = true
	try {
		const res = await getActivityManageList(query.page, query.pagesize)
		if (res.state?.type === 'SUCCESS') {
			tableData.value = res.data || []
			query.total = res.count || 0
			return
		}
		ElMessage.error(res.state?.msg || 'Failed to load activity list')
	} catch {
		ElMessage.error('Failed to load activity list')
	} finally {
		loading.value = false
	}
}

const loadJoinList = async () => {
	joinLoading.value = true
	try {
		const res = await getJoinsList('activitycontent')
		if (res.state?.type === 'SUCCESS') {
			joinTableData.value = res.data || []
			return
		}
		ElMessage.error(res.state?.msg || 'Failed to load joined activity list')
	} catch {
		ElMessage.error('Failed to load joined activity list')
	} finally {
		joinLoading.value = false
	}
}

const removeItem = async activityId => {
	try {
		await ElMessageBox.confirm('Delete this activity permanently?', 'Confirm Deletion', {
			type: 'warning',
		})
		const res = await deleteActivity(activityId)
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

const removeJoin = async joinId => {
	try {
		await ElMessageBox.confirm('Cancel this join record?', 'Confirm', {
			type: 'warning',
		})
		const res = await deleteJoin(joinId)
		if (res.state?.type === 'SUCCESS') {
			ElMessage.success('Cancelled successfully')
			loadJoinList()
			return
		}
		ElMessage.error(res.state?.msg || 'Cancel failed')
	} catch {
		// User cancelled or request failed.
	}
}

onMounted(() => {
	loadList()
	loadJoinList()
})
</script>
