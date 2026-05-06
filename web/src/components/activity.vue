<template>
	<div class="activity">
		<div class="row-fluid top-cats">
			<section class="widget">
				<router-link to="/activity" custom v-slot="{ navigate }">
					<div class="page-header section-header" @click="navigate">
						<h3 class="section-title">Campus Activities</h3>
						<h4 class="section-more">More &gt;</h4>
					</div>
				</router-link>

				<ul class="articles" v-loading="isLoading">
					<li
						class="article-entry standard"
						v-for="(activity, idx) in tableData"
						:key="activity.activity_id || idx"
					>
						<h4>
							<router-link
								:to="`/activitycontent/${activity.activity_id}`"
							>
								{{ activity.activity_title }}
							</router-link>
						</h4>
						<span class="article-meta">
							<a class="iconfont">&#xe619;</a>
							{{ formatDate(activity.createtime) }}
							<a class="iconfont" style="margin-left: 50px"
								>&#xe609;</a
							>
							{{ activity.activity_locale }}
						</span>
						<span class="like-count">
							<a class="iconfont" style="color: red">&#xe647;</a>
							{{ activity.activity_read_num }}
						</span>
					</li>

					<li
						v-if="!isLoading && tableData.length === 0"
						class="article-entry standard empty-state"
					>
						No activity data available
					</li>
				</ul>
			</section>
		</div>
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
	pagesize: 5,
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
.section-header {
	display: flex;
	align-items: center;
	justify-content: space-between;
	gap: 12px;
	cursor: pointer;
	margin-bottom: 8px;
}

.section-title {
	margin: 0;
	line-height: 1.25;
}

.section-more {
	margin: 0;
	font-size: 14px;
	font-weight: 500;
	color: #5f6b7a;
	white-space: nowrap;
	transition:
		color 0.2s ease,
		transform 0.2s ease;
}

.section-header:hover .section-more {
	color: #2f6fdd;
	transform: translateX(2px);
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
