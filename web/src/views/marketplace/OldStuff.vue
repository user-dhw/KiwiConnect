<template>
	<div class="oldstuff-page">
		<div v-title data-title="Information Platform | Marketplace"></div>

		<div class="page-container">
			<div class="container">
				<div class="row content-grid">
					<div class="span8 page-content">
						<div class="page-header">
							<h1>
								Marketplace
								<small v-if="subtitle">{{ subtitle }}</small>
							</h1>
						</div>

						<div class="filter-section">
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
						</div>

						<section class="widget">
							<div class="items-grid" v-if="itemList.length">
								<router-link
									v-for="item in itemList"
									:key="item.oldstuff_id"
									:to="`/oldstuffcontent/${item.oldstuff_id}`"
									class="item-link"
								>
									<el-card class="item-card" shadow="hover">
										<div class="item-image-wrapper">
											<el-image
												:src="
													normalizeImageUrl(
														item.oldstuff_img,
													)
												"
												fit="cover"
												class="item-image"
											>
												<template #error>
													<div class="image-error">
														<el-icon
															><Picture
														/></el-icon>
													</div>
												</template>
											</el-image>
										</div>
										<div class="item-content">
											<div class="item-price">
												¥{{ item.oldstuff_price }}
											</div>
											<div
												class="item-name"
												:title="item.oldstuff_name"
											>
												{{ item.oldstuff_name }}
											</div>
										</div>
									</el-card>
								</router-link>
							</div>

							<el-empty v-else description="No items available" />
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
						<ActivityWidget />
						<NewsWidget />
					</aside>
				</div>
			</div>
		</div>
	</div>
</template>

<script setup>
import { reactive, ref, onMounted } from 'vue'
import { Picture } from '@element-plus/icons-vue'
import Carousel from '@/components/carousel.vue'
import ActivityWidget from '@/components/activity.vue'
import NewsWidget from '@/components/news.vue'
import { getLabelOptions, getWebOldStuffList } from '@/api/content'

const subtitle = ref('')
const labels = ref([])
const itemList = ref([])

const query = reactive({
	lable: '',
	total: 0,
	pagesize: 12,
	page: 1,
})

const normalizeImageUrl = value => {
	const src = String(value || '').trim()
	if (!src) return ''
	if (/^https?:\/\//i.test(src)) return src
	if (src.startsWith('/api/')) return src
	if (src.startsWith('/uplodes/')) return `/api${src}`
	if (src.startsWith('/')) return `/api${src}`
	return `/api/${src}`
}

const loadItemList = async () => {
	const res = await getWebOldStuffList({
		lable: query.lable,
		pagesize: query.pagesize,
		page: query.page,
	})

	if (res.state?.type !== 'SUCCESS') return

	itemList.value = res.data || []
	query.total = Number(res.count || 0)
}

const loadLabels = async () => {
	labels.value = await getLabelOptions('oldstuff')
}

const changeLabel = async label => {
	query.lable = label
	query.page = 1
	subtitle.value = label
	await loadItemList()
}

const handleCurrentChange = async page => {
	query.page = page
	await loadItemList()
}

onMounted(async () => {
	await loadLabels()
	await loadItemList()
})
</script>

<style scoped>
.oldstuff-page {
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
	min-height: 400px;
}

.items-grid {
	display: grid;
	grid-template-columns: repeat(auto-fill, minmax(220px, 1fr));
	gap: 16px;
}

.item-link {
	text-decoration: none;
	color: inherit;
}

.item-card {
	height: 100%;
	transition: transform 0.3s ease;
	cursor: pointer;
}

.item-card:hover {
	transform: translateY(-4px);
}

.item-card :deep(.el-card__body) {
	padding: 0;
}

.item-image-wrapper {
	width: 100%;
	height: 200px;
	overflow: hidden;
	border-radius: 4px 4px 0 0;
}

.item-image {
	width: 100%;
	height: 100%;
	display: block;
}

.image-error {
	display: flex;
	align-items: center;
	justify-content: center;
	height: 200px;
	background: #f5f7fa;
	color: #909399;
	font-size: 48px;
}

.item-content {
	padding: 12px;
}

.item-price {
	font-size: 20px;
	font-weight: 600;
	color: #f56c6c;
	margin-bottom: 8px;
}

.item-name {
	font-size: 14px;
	color: #303133;
	line-height: 1.5;
	overflow: hidden;
	text-overflow: ellipsis;
	display: -webkit-box;
	-webkit-line-clamp: 2;
	line-clamp: 2;
	-webkit-box-orient: vertical;
	min-height: 42px;
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

@media (max-width: 768px) {
	.items-grid {
		grid-template-columns: repeat(auto-fill, minmax(160px, 1fr));
		gap: 12px;
	}

	.item-image-wrapper {
		height: 160px;
	}

	.image-error {
		height: 160px;
		font-size: 36px;
	}
}
</style>
