<template>
	<div class="activity">
		<section class="hub-section">
			<router-link to="/activity" custom v-slot="{ navigate }">
				<div class="section-header" @click="navigate">
					<div>
						<p class="section-eyebrow">Campus Life</p>
						<h3 class="section-title">Campus Activities</h3>
					</div>
					<h4 class="section-more">More</h4>
				</div>
			</router-link>

			<div class="hub-list" v-loading="isLoading">
				<li
					class="hub-item"
					v-for="(activity, idx) in tableData"
					:key="activity.activity_id || idx"
				>
					<div class="hub-item-top">
						<span class="hub-badge">Event</span>
						<span class="hub-views">{{ activity.activity_read_num || 0 }} views</span>
					</div>
					<h4 class="hub-item-title">
						<router-link :to="`/activitycontent/${activity.activity_id}`">
							{{ activity.activity_title }}
						</router-link>
					</h4>
					<span class="hub-meta">
						<span>{{ formatDate(activity.createtime) }}</span>
						<span>{{ activity.activity_locale }}</span>
					</span>
				</li>

				<li
					v-if="!isLoading && tableData.length === 0"
					class="hub-item empty-state"
				>
					No activity data available
				</li>
			</div>
		</section>
	</div>
</template>

<script setup>
import { onMounted, ref } from 'vue'
import moment from 'moment'
import { ElMessage } from 'element-plus'
import { getWebActivityList } from '@/api/content'

const pagelistquery = ref({
	lable: '',
	total: 0,
	pagesize: 3,
	page: 1,
})

const tableData = ref([])
const isLoading = ref(false)

const formatDate = value => {
	return value ? moment(value).format('YYYY-MM-DD HH:mm') : '-'
}

const getActivityList = async () => {
	isLoading.value = true
	try {
		const res = await getWebActivityList(pagelistquery.value)
		if (res.state?.type === 'SUCCESS') {
			tableData.value = Array.isArray(res.data) ? res.data : []
			pagelistquery.value.total = Number(res.count || 0)
			return
		}

		ElMessage.error(res.state?.msg || 'Failed to load activities')
	} catch {
		ElMessage.error('Failed to load activities')
	} finally {
		isLoading.value = false
	}
}

onMounted(() => {
	getActivityList()
})
</script>

<style scoped>
.hub-section {
	height: 100%;
	display: grid;
	grid-template-rows: auto 1fr;
}

.section-header {
	display: flex;
	align-items: flex-start;
	justify-content: space-between;
	gap: 12px;
	cursor: pointer;
	margin-bottom: 16px;
}

.section-eyebrow {
	margin: 0 0 8px;
	font-size: 0.74rem;
	font-weight: 700;
	letter-spacing: 0.12em;
	text-transform: uppercase;
	color: #7f93bb;
}

.section-title {
	margin: 0;
	line-height: 1.25;
	font-size: 1.02rem;
}

.section-more {
	margin: 0;
	font-size: 13px;
	font-weight: 700;
	color: #6281bf;
	white-space: nowrap;
	transition:
		color 0.2s ease,
		transform 0.2s ease;
}

.section-header:hover .section-more {
	color: #2663eb;
	transform: translateX(2px);
}

.hub-list {
	display: grid;
	gap: 12px;
	align-content: start;
}

.hub-item {
	display: grid;
	gap: 10px;
	padding: 16px;
	border-radius: 18px;
	background: linear-gradient(180deg, #ffffff 0%, #f8fbff 100%);
	border: 1px solid rgba(38, 99, 235, 0.1);
	box-shadow: 0 8px 18px rgba(38, 99, 235, 0.05);
}

.hub-item-top {
	display: flex;
	align-items: center;
	justify-content: space-between;
	gap: 10px;
}

.hub-badge {
	display: inline-flex;
	align-items: center;
	padding: 6px 10px;
	border-radius: 999px;
	background: rgba(38, 99, 235, 0.1);
	color: #2663eb;
	font-size: 0.73rem;
	font-weight: 700;
	letter-spacing: 0.06em;
	text-transform: uppercase;
}

.hub-views {
	font-size: 0.8rem;
	font-weight: 700;
	color: #6d7c96;
}

.hub-item-title {
	margin: 0;
	font-size: 1rem;
	line-height: 1.45;
}

.hub-item-title a {
	color: #243047;
	text-decoration: none;
}

.hub-item-title a:hover {
	color: #2663eb;
}

.hub-meta {
	display: flex;
	flex-wrap: wrap;
	gap: 8px 12px;
	font-size: 0.86rem;
	color: #7a869f;
}

.empty-state {
	color: #9aa3ab;
	text-align: center;
}

@media (max-width: 768px) {
	.section-more {
		font-size: 13px;
	}
}
</style>
