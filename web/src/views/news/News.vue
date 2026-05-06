<template>
	<div class="news-page">
		<div v-title data-title="Information Platform | News"></div>

		<div class="page-container">
			<div class="container">
				<div class="page-shell">
					<div class="page-main">
						<div class="page-heading">
							<h1 class="page-title">
								Articles & News
								<span v-if="subtitle" class="page-highlight">/ {{ subtitle }}</span>
							</h1>
							<p class="page-subtitle">
								Read student stories, platform updates, and useful campus news in
								a cleaner, softer reading experience.
							</p>
						</div>

						<div class="filter-bar">
							<span class="filter-label">Category:</span>
							<el-tag
								:type="!query.lable && !query.tag ? 'primary' : 'info'"
								:effect="!query.lable && !query.tag ? 'dark' : 'plain'"
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

						<section class="section-card widget">
							<div v-if="articleList.length">
								<article
									v-for="article in articleList"
									:key="article.article_id"
									class="article-item"
								>
									<header class="article-header">
										<h3 class="article-title">
											<router-link :to="`/newscontent/${article.article_id}`">
												{{ article.article_title }}
											</router-link>
										</h3>

										<div class="article-meta">
											<span class="meta-date">
												<el-icon><Calendar /></el-icon>
												{{ formatDate(article.article_createtime) }}
											</span>
											<span class="meta-author">
												<el-icon><User /></el-icon>
												{{ article.nickname }}
											</span>
											<span class="meta-views">
												<el-icon><View /></el-icon>
												{{ article.article_read_num }} views
											</span>
										</div>
									</header>

									<div class="article-intro">
										{{ article.article_introduction }}
										<router-link
											:to="`/newscontent/${article.article_id}`"
											class="read-more"
										>
											... Read more
										</router-link>
									</div>
								</article>
							</div>

							<el-empty v-else description="No articles found" />
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
import { Calendar, User, View } from '@element-plus/icons-vue'
import moment from 'moment'
import Carousel from '@/components/carousel.vue'
import OldStuffWidget from '@/components/oldstuff.vue'
import { getLabelOptions, getArticleList } from '@/api/content'

const route = useRoute()

const subtitle = ref('')
const labels = ref([])
const articleList = ref([])

const query = reactive({
	lable: '',
	tag: '',
	total: 0,
	pagesize: 5,
	page: 1,
})

const formatDate = value => moment(value).format('YYYY-MM-DD HH:mm')

const loadArticleList = async () => {
	const res = await getArticleList({
		lable: query.lable,
		tag: query.tag,
		pagesize: query.pagesize,
		page: query.page,
	})

	if (res.state?.type !== 'SUCCESS') return

	articleList.value = res.data || []
	query.total = Number(res.count || 0)
}

const loadLabels = async () => {
	labels.value = await getLabelOptions('article')
}

const changeLabel = async label => {
	query.lable = label
	query.tag = ''
	query.page = 1
	subtitle.value = label
	await loadArticleList()
}

const clearTagFilter = async () => {
	query.tag = ''
	query.page = 1
	subtitle.value = query.lable
	await loadArticleList()
}

const applyTagFilter = async tag => {
	const nextTag = String(tag || '').trim()
	query.tag = nextTag
	query.lable = ''
	query.page = 1
	subtitle.value = nextTag
	await loadArticleList()
}

const handleCurrentChange = async page => {
	query.page = page
	await loadArticleList()
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
	await loadArticleList()
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

.article-header {
	margin-bottom: 12px;
}

.article-title a {
	color: #1f2d3d;
	text-decoration: none;
	transition: color 0.3s ease;
}

.article-title a:hover {
	color: #2663eb;
}

.article-meta > span {
	display: inline-flex;
	align-items: center;
	gap: 4px;
}

.pagination {
	margin-top: 18px;
	display: flex;
	justify-content: center;
}
</style>
