<template>
	<div class="manage-page">
		<div class="admin-heading">
			<div class="page-heading">
				<h1 class="page-title">Marketplace Management</h1>
				<p class="page-subtitle">Manage your listings and track purchase interest from other users.</p>
			</div>

			<div class="admin-overview">
				<div class="admin-stat-card">
					<span class="admin-stat-label">Listings</span>
					<span class="admin-stat-value">{{ query.total }}</span>
					<span class="admin-stat-note">Items currently managed by you</span>
				</div>
				<div class="admin-stat-card">
					<span class="admin-stat-label">Interests</span>
					<span class="admin-stat-value">{{ joinTableData.length }}</span>
					<span class="admin-stat-note">Purchase enquiries from the marketplace</span>
				</div>
			</div>
		</div>

		<el-tabs type="border-card" class="admin-tabs">
			<el-tab-pane label="My Listings">
				<section class="admin-panel admin-table">
					<div class="admin-workspace">
						<div class="admin-workspace-head">
							<div class="admin-workspace-copy">
								<h2 class="admin-workspace-title">Marketplace Listings</h2>
								<p class="admin-workspace-note">
									Manage product details, pricing, and what buyers will see first.
								</p>
							</div>
							<div class="admin-workspace-actions">
								<el-button type="primary" @click="router.push('/admin/createoldstuff')">
									Create Listing
								</el-button>
							</div>
						</div>
						<el-table :data="tableData" border v-loading="loading" style="width: 100%">
							<el-table-column label="Date" width="140">
								<template #default="scope">{{ formatDate(scope.row.createtime) }}</template>
							</el-table-column>
							<el-table-column prop="oldstuff_name" label="Item" min-width="220" />
							<el-table-column prop="oldstuff_lable" label="Category" width="100" />
							<el-table-column prop="oldstuff_price" label="Price" width="100" />
							<el-table-column label="Actions" width="170" fixed="right">
								<template #default="scope">
									<el-button
										text
										type="primary"
										@click="router.push(`/admin/updateoldstuff/${scope.row.oldstuff_id}`)"
									>
										Edit
									</el-button>
									<el-button text type="danger" @click="removeItem(scope.row.oldstuff_id)">
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
				</section>
			</el-tab-pane>

			<el-tab-pane label="My Purchase Interests">
				<section class="admin-panel admin-table">
					<div class="admin-workspace">
						<div class="admin-workspace-copy">
							<h2 class="admin-workspace-title">Purchase Interests</h2>
							<p class="admin-workspace-note">
								See which items you expressed interest in and cancel outdated requests.
							</p>
						</div>
						<el-table :data="joinTableData" border v-loading="joinLoading" style="width: 100%">
							<el-table-column label="Added At" width="180">
								<template #default="scope">{{ formatDate(scope.row.joins_createtime) }}</template>
							</el-table-column>
							<el-table-column prop="oldstuff_name" label="Item" min-width="220" />
							<el-table-column prop="oldstuff_price" label="Price" width="100" />
							<el-table-column prop="name" label="My Offer" width="160" />
							<el-table-column label="Actions" width="190" fixed="right">
								<template #default="scope">
									<el-link
										:href="getOldStuffDetailHref(scope.row.oldstuff_id)"
										target="_blank"
										type="primary"
									>
										View
									</el-link>
									<el-button text type="danger" @click="removeJoin(scope.row.join_id)">
										Cancel Interest
									</el-button>
								</template>
							</el-table-column>
						</el-table>
					</div>
				</section>
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
	deleteJoin,
	deleteOldStuff,
	getJoinsList,
	getOldStuffManageList,
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

const getOldStuffDetailHref = oldstuffId => {
	if (!oldstuffId) return '#'
	return router.resolve({ path: `/oldstuffcontent/${oldstuffId}` }).href
}

const loadList = async () => {
	loading.value = true
	try {
		const res = await getOldStuffManageList(query.page, query.pagesize)
		if (res.state?.type === 'SUCCESS') {
			tableData.value = res.data || []
			query.total = res.count || 0
			return
		}
		ElMessage.error(res.state?.msg || 'Failed to load listing data')
	} catch {
		ElMessage.error('Failed to load listing data')
	} finally {
		loading.value = false
	}
}

const loadJoinList = async () => {
	joinLoading.value = true
	try {
		const res = await getJoinsList('oldstuffcontent')
		if (res.state?.type === 'SUCCESS') {
			joinTableData.value = res.data || []
			return
		}
		ElMessage.error(res.state?.msg || 'Failed to load purchase interests')
	} catch {
		ElMessage.error('Failed to load purchase interests')
	} finally {
		joinLoading.value = false
	}
}

const removeItem = async oldstuffId => {
	try {
		await ElMessageBox.confirm('Delete this listing permanently?', 'Confirm Deletion', {
			type: 'warning',
		})
		const res = await deleteOldStuff(oldstuffId)
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
		await ElMessageBox.confirm('Cancel this purchase interest?', 'Confirm', {
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
