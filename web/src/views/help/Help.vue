<template>
	<div class="help-page">
		<div v-title data-title="Information Platform | Q&A"></div>

		<div class="page-container">
			<div class="container">
				<div class="row content-grid">
					<div class="span8 page-content">
						<div class="page-header">
							<h1>
								Q&A Center
								<small v-if="subtitle">{{ subtitle }}</small>
							</h1>
						</div>

						<div class="filter-section">
							<span class="filter-label">Category:</span>
							<el-tag
								:type="
									!query.lable && !query.tag
										? 'primary'
										: 'info'
								"
								:effect="
									!query.lable && !query.tag
										? 'dark'
										: 'plain'
								"
								@click="changeLabel('')"
								class="filter-tag"
								size="large"
							>
								All
							</el-tag>
							<el-tag
								v-for="label in labels"
								:key="label"
								:type="
									query.lable === label ? 'primary' : 'info'
								"
								:effect="
									query.lable === label ? 'dark' : 'plain'
								"
								@click="changeLabel(label)"
								class="filter-tag"
								size="large"
							>
								{{ label }}
							</el-tag>
							<el-tag
								v-if="query.tag"
								type="warning"
								effect="dark"
								@click="clearTagFilter"
								class="filter-tag"
								size="large"
								closable
							>
								Tag: {{ query.tag }}
							</el-tag>
						</div>

						<section class="widget">
							<ul class="articles" v-if="helpList.length">
								<li
									class="article-entry standard"
									v-for="item in helpList"
									:key="item.help_id"
								>
									<h4>
										<router-link
											:to="`/helpcontent/${item.help_id}`"
										>
											{{ item.help_title }}
										</router-link>
									</h4>
									<span class="article-meta">
										<span>{{
											formatDate(item.createtime)
										}}</span>
										<span class="meta-gap"
											>Author: {{ item.nickname }}</span
										>
									</span>
									<span class="like-count"
										>Views: {{ item.help_read_num }}</span
									>
								</li>
							</ul>

							<el-empty v-else description="No Q&A items found" />
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

					<aside class="span4 page-sidebar">
						<Carousel />
						<OldStuffWidget />
					</aside>
				</div>
			</div>
		</div>
	</div>
</template>

<script setup>
import { reactive, ref, onMounted, watch } from 'vue'
import { useRoute } from 'vue-router'
import moment from 'moment'
import Carousel from '@/components/carousel.vue'
import OldStuffWidget from '@/components/oldstuff.vue'
import { getLabelOptions, getWebHelpList } from '@/api/content'

const route = useRoute()

const subtitle = ref('')
const labels = ref([])
const helpList = ref([])

const query = reactive({
	lable: '',
	tag: '',
	total: 0,
	pagesize: 10,
	page: 1,
})

const formatDate = value => moment(value).format('YYYY-MM-DD HH:mm')

const loadHelpList = async () => {
	const res = await getWebHelpList({
		lable: query.lable,
		tag: query.tag,
		pagesize: query.pagesize,
		page: query.page,
	})

	if (res.state?.type !== 'SUCCESS') return

	helpList.value = res.data || []
	query.total = Number(res.count || 0)
}

const loadLabels = async () => {
	labels.value = await getLabelOptions('help')
}

const changeLabel = async label => {
	query.lable = label
	query.tag = ''
	query.page = 1
	subtitle.value = label
	await loadHelpList()
}

const clearTagFilter = async () => {
	query.tag = ''
	query.page = 1
	subtitle.value = query.lable
	await loadHelpList()
}

const applyTagFilter = async tag => {
	const nextTag = String(tag || '').trim()
	query.tag = nextTag
	query.lable = ''
	query.page = 1
	subtitle.value = nextTag
	await loadHelpList()
}

const handleCurrentChange = async page => {
	query.page = page
	await loadHelpList()
}

watch(
	() => route.query.tag,
	async tag => {
		const normalized = String(tag || '').trim()
		if (!normalized) return
		await applyTagFilter(normalized)
	},
	{ immediate: false },
)

onMounted(async () => {
	await loadLabels()
	const initialTag = String(route.query.tag || '').trim()
	if (initialTag) {
		await applyTagFilter(initialTag)
		return
	}
	await loadHelpList()
})
</script>

<style scoped>
.help-page {
	min-height: 200px;
}

.page-container {
	padding: 20px 0;
}

.container {
	max-width: 1200px;
	margin: 0 auto;
	padding: 0 20px;
}

.content-grid {
	display: flex;
	gap: 24px;
}

.page-content {
	flex: 1;
	min-width: 0;
}

.page-sidebar {
	width: 340px;
}

.page-header h1 {
	margin: 0 0 14px;
	font-size: 28px;
}

.page-header small {
	margin-left: 8px;
	font-size: 16px;
	color: #409eff;
	font-weight: normal;
}

.filter-section {
	display: flex;
	align-items: center;
	flex-wrap: wrap;
	gap: 10px;
	margin-bottom: 20px;
	padding: 16px;
	background: #f5f7fa;
	border-radius: 8px;
}

.filter-label {
	font-size: 14px;
	font-weight: 600;
	color: #606266;
	margin-right: 4px;
}

.filter-tag {
	cursor: pointer;
	transition: all 0.3s ease;
	user-select: none;
}

.filter-tag:hover {
	transform: translateY(-2px);
	box-shadow: 0 2px 8px rgba(0, 0, 0, 0.15);
}

.widget {
	background: #fff;
	border: 1px solid #eef2f6;
	border-radius: 10px;
	padding: 16px;
}

.articles {
	list-style: none;
	padding: 0;
	margin: 0;
}

.article-entry {
	padding: 14px 0;
	border-bottom: 1px solid #eef2f6;
}

.article-entry:last-child {
	border-bottom: none;
}

.article-entry h4 {
	margin: 0 0 8px;
	font-size: 18px;
}

.article-entry a {
	color: #1f2d3d;
	text-decoration: none;
}

.article-entry a:hover {
	color: #1989fa;
}

.article-meta {
	color: #6b7785;
	font-size: 13px;
	display: inline-flex;
	align-items: center;
}

.meta-gap {
	margin-left: 24px;
}

.like-count {
	float: right;
	font-size: 13px;
	color: #6b7785;
}

.pagination {
	margin-top: 18px;
	display: flex;
	justify-content: center;
}

@media (max-width: 992px) {
	.content-grid {
		flex-direction: column;
	}

	.page-sidebar {
		width: 100%;
	}
}
</style>
