<template>
	<div class="activity-page">
		<div v-title data-title="Information Platform | Activities"></div>

		<div class="page-container">
			<div class="container">
				<div class="page-shell">
					<div class="page-main">
						<div class="page-heading">
							<h1 class="page-title">
								Activity Center
								<span v-if="subtitle" class="page-highlight">/ {{ subtitle }}</span>
							</h1>
							<p class="page-subtitle">
								Explore campus events, club meetups, and student activities in a
								cleaner mobile-first event feed.
							</p>
						</div>

						<div class="filter-bar">
							<span class="filter-label">Category:</span>
							<el-tag
								:type="!query.lable ? 'primary' : 'info'"
								:effect="!query.lable ? 'dark' : 'plain'"
								@click="changeLabel('')"
								class="filter-tag"
								size="large"
							>
								All
							</el-tag>
							<el-tag
								v-for="label in labels"
								:key="label"
								:type="query.lable === label ? 'primary' : 'info'"
								:effect="query.lable === label ? 'dark' : 'plain'"
								@click="changeLabel(label)"
								class="filter-tag"
								size="large"
							>
								{{ label }}
							</el-tag>
						</div>

						<section class="section-card widget">
							<ul class="articles" v-if="activityList.length">
								<li
									v-for="item in activityList"
									:key="item.activity_id"
									class="article-entry standard"
								>
									<h4>
										<router-link :to="`/activitycontent/${item.activity_id}`">
											{{ item.activity_title }}
										</router-link>
									</h4>
									<span class="article-meta">
										<span>{{ formatDate(item.createtime) }}</span>
										<span v-if="item.activity_locale" class="meta-gap">
											<el-icon><LocationFilled /></el-icon>
											{{ item.activity_locale }}
										</span>
									</span>
									<span class="like-count">Views: {{ item.activity_read_num }}</span>
								</li>
							</ul>

							<el-empty v-else description="No activities found" />
						</section>

						<el-pagination
							class="pagination"
							@current-change="handleCurrentChange"
							layout="prev, pager, next"
							:total="query.total"
							:page-size="query.pagesize"
							:current-page="query.page"
						/>
					</div>

					<aside class="page-aside panel-stack">
						<Carousel />
					</aside>
				</div>
			</div>
		</div>
	</div>
</template>

<script setup>
import { reactive, ref, onMounted } from 'vue'
import { LocationFilled } from '@element-plus/icons-vue'
import moment from 'moment'
import Carousel from '@/components/carousel.vue'
import { getLabelOptions, getWebActivityList } from '@/api/content'

const subtitle = ref('')
const labels = ref([])
const activityList = ref([])

const query = reactive({
	lable: '',
	total: 0,
	pagesize: 10,
	page: 1,
})

const formatDate = value => moment(value).format('YYYY-MM-DD HH:mm')

const loadActivityList = async () => {
	const res = await getWebActivityList({
		lable: query.lable,
		pagesize: query.pagesize,
		page: query.page,
	})

	if (res.state?.type !== 'SUCCESS') return

	activityList.value = res.data || []
	query.total = Number(res.count || 0)
}

const loadLabels = async () => {
	labels.value = await getLabelOptions('activity')
}

const changeLabel = async label => {
	query.lable = label
	query.page = 1
	subtitle.value = label
	await loadActivityList()
}

const handleCurrentChange = async page => {
	query.page = page
	await loadActivityList()
}

onMounted(async () => {
	await loadLabels()
	await loadActivityList()
})
</script>

<style scoped>
.filter-tag {
	cursor: pointer;
	transition: all 0.3s ease;
	user-select: none;
}

.filter-tag:hover {
	transform: translateY(-2px);
	box-shadow: 0 2px 8px rgba(0, 0, 0, 0.15);
}

.article-entry a {
	color: #1f2d3d;
	text-decoration: none;
}

.article-entry a:hover {
	color: #1989fa;
}

.meta-gap {
	display: inline-flex;
	align-items: center;
	gap: 6px;
	margin-left: 24px;
}

.pagination {
	margin-top: 24px;
	display: flex;
	justify-content: center;
}
</style>
