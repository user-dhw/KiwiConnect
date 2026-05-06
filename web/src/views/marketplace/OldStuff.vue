<template>
	<div class="oldstuff-page">
		<div v-title data-title="Information Platform | Marketplace"></div>

		<div class="page-container">
			<div class="container">
				<div class="page-shell">
					<div class="page-main">
						<div class="page-heading">
							<h1 class="page-title">
								Marketplace
								<span v-if="subtitle" class="page-highlight">/ {{ subtitle }}</span>
							</h1>
							<p class="page-subtitle">
								Trade pre-loved campus essentials with softer spacing, clearer
								pricing, and a smoother mobile layout.
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
												:src="normalizeImageUrl(item.oldstuff_img)"
												fit="cover"
												class="item-image"
											>
												<template #error>
													<div class="image-error">
														<el-icon><Picture /></el-icon>
													</div>
												</template>
											</el-image>
										</div>
										<div class="item-content">
											<div class="item-price">¥{{ item.oldstuff_price }}</div>
											<div class="item-name" :title="item.oldstuff_name">
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

					<aside class="page-aside panel-stack">
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

const apiBaseUrl = import.meta.env.VITE_API_URL || '/api'

const normalizeImageUrl = value => {
	const src = String(value || '').trim()
	if (!src) return ''
	if (src.startsWith('http://127.0.0.1:3000')) {
		return src.replace('http://127.0.0.1:3000', apiBaseUrl)
	}
	if (src.startsWith('http://localhost:3000')) {
		return src.replace('http://localhost:3000', apiBaseUrl)
	}
	if (/^https?:\/\//i.test(src)) return src
	if (src.startsWith('/api/')) return src
	if (src.startsWith('/uplodes/')) return `${apiBaseUrl}${src}`
	if (src.startsWith('/')) return `${apiBaseUrl}${src}`
	return `${apiBaseUrl}/${src}`
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
.filter-tag {
	cursor: pointer;
	transition: all 0.3s ease;
	user-select: none;
}

.filter-tag:hover {
	transform: translateY(-2px);
	box-shadow: 0 2px 8px rgba(0, 0, 0, 0.15);
}

.item-card :deep(.el-card__body) {
	padding: 0;
}

.item-image-wrapper {
	height: 200px;
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

.item-name {
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

@media (max-width: 768px) {
	.item-image-wrapper {
		height: 160px;
	}

	.image-error {
		height: 160px;
		font-size: 36px;
	}
}
</style>
