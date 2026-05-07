<template>
	<div class="page-container">
		<div class="container">
			<div class="page-narrow">
				<section class="section-card search-card">
					<div class="page-heading">
						<h1 class="page-title">Search</h1>
						<p class="page-subtitle">
							Search across Q&amp;A, campus events, marketplace items, and
							articles from the student community.
						</p>
					</div>

					<div class="search-query-chip">Keyword: {{ searchQuery }}</div>

					<div class="search-feedback" v-if="loading">Searching...</div>
					<div class="search-feedback error" v-else-if="errorMessage">
						{{ errorMessage }}
					</div>
					<div class="search-feedback" v-else-if="!hasKeyword">
						Please enter a keyword to start searching.
					</div>
					<div class="search-feedback" v-else-if="totalResults === 0">
						No matching content found.
					</div>

					<div v-else class="search-groups">
						<section
							v-for="group in resultGroups"
							:key="group.key"
							class="result-group"
						>
							<div class="result-group-head">
								<h2>{{ group.title }}</h2>
								<span>{{ group.items.length }}</span>
							</div>

							<div class="result-list">
								<router-link
									v-for="item in group.items"
									:key="item.id"
									:to="item.to"
									class="result-item"
								>
									<div class="result-item-top">
										<h3>{{ item.title }}</h3>
										<span class="result-tag">{{ group.shortLabel }}</span>
									</div>
									<p v-if="item.summary" class="result-summary">
										{{ item.summary }}
									</p>
									<div class="result-meta">
										<span>{{ item.author }}</span>
										<span v-if="item.date">{{ item.date }}</span>
									</div>
								</router-link>
							</div>
						</section>
					</div>
				</section>
			</div>
		</div>
	</div>
</template>

<script setup>
import { useRoute } from 'vue-router'
import { computed, ref, watch } from 'vue'
import moment from 'moment'
import { searchContent } from '@/api/content'
import { ElMessage } from 'element-plus'

const route = useRoute()
const searchQuery = computed(() => String(route.query.search || '').trim())
const hasKeyword = computed(() => searchQuery.value.length > 0)

const loading = ref(false)
const errorMessage = ref('')
const rawResults = ref({
	help: [],
	activity: [],
	oldstuff: [],
	article: [],
})

const formatDate = value => {
	if (!value) return ''
	const num = Number(value)
	if (Number.isFinite(num) && num > 0) {
		return moment(num).format('YYYY-MM-DD HH:mm')
	}
	return moment(value).isValid() ? moment(value).format('YYYY-MM-DD HH:mm') : ''
}

const trimText = (value, fallback = '') => {
	const text = String(value || '').replace(/<[^>]+>/g, ' ').replace(/\s+/g, ' ').trim()
	return text || fallback
}

const resultGroups = computed(() => [
	{
		key: 'help',
		title: 'Q&A',
		shortLabel: 'Q&A',
		items: (rawResults.value.help || []).map(item => ({
			id: item.help_id,
			to: `/helpcontent/${item.help_id}`,
			title: trimText(item.help_title, 'Untitled Q&A'),
			summary: trimText(item.help_tag || item.help_content, ''),
			author: trimText(item.nickname, 'Unknown author'),
			date: formatDate(item.createtime),
		})),
	},
	{
		key: 'activity',
		title: 'Events',
		shortLabel: 'Event',
		items: (rawResults.value.activity || []).map(item => ({
			id: item.activity_id,
			to: `/activitycontent/${item.activity_id}`,
			title: trimText(item.activity_title, 'Untitled event'),
			summary: trimText(item.activity_locale || item.activity_content, ''),
			author: trimText(item.nickname || 'Campus event'),
			date: formatDate(item.createtime),
		})),
	},
	{
		key: 'oldstuff',
		title: 'Marketplace',
		shortLabel: 'Market',
		items: (rawResults.value.oldstuff || []).map(item => ({
			id: item.oldstuff_id,
			to: `/oldstuffcontent/${item.oldstuff_id}`,
			title: trimText(item.oldstuff_name, 'Untitled listing'),
			summary: trimText(item.oldstuff_content, ''),
			author: item.oldstuff_price ? `NZ$ ${item.oldstuff_price}` : 'Marketplace',
			date: formatDate(item.createtime),
		})),
	},
	{
		key: 'article',
		title: 'Articles / News',
		shortLabel: 'News',
		items: (rawResults.value.article || []).map(item => ({
			id: item.article_id,
			to: `/newscontent/${item.article_id}`,
			title: trimText(item.article_title, 'Untitled article'),
			summary: trimText(item.article_introduction || item.article_content, ''),
			author: trimText(item.nickname, 'Unknown author'),
			date: formatDate(item.article_createtime),
		})),
	},
].filter(group => group.items.length > 0))

const totalResults = computed(() =>
	resultGroups.value.reduce((sum, group) => sum + group.items.length, 0),
)

const loadSearchResults = async keyword => {
	if (!keyword) {
		rawResults.value = { help: [], activity: [], oldstuff: [], article: [] }
		errorMessage.value = ''
		return
	}

	loading.value = true
	errorMessage.value = ''
	try {
		const res = await searchContent(keyword)
		if (res.state?.type !== 'SUCCESS') {
			errorMessage.value = res.state?.msg || 'Search failed'
			rawResults.value = { help: [], activity: [], oldstuff: [], article: [] }
			return
		}

		rawResults.value = {
			help: Array.isArray(res.data?.help) ? res.data.help : [],
			activity: Array.isArray(res.data?.activity) ? res.data.activity : [],
			oldstuff: Array.isArray(res.data?.oldstuff) ? res.data.oldstuff : [],
			article: Array.isArray(res.data?.article) ? res.data.article : [],
		}
	} catch {
		errorMessage.value = 'Failed to load search results'
		rawResults.value = { help: [], activity: [], oldstuff: [], article: [] }
		ElMessage.error('Failed to load search results')
	} finally {
		loading.value = false
	}
}

watch(
	() => searchQuery.value,
	keyword => {
		loadSearchResults(keyword)
	},
	{ immediate: true },
)
</script>

<style scoped>
.search-card {
	display: grid;
	gap: 18px;
}

.search-query-chip {
	display: inline-flex;
	align-items: center;
	width: fit-content;
	padding: 10px 14px;
	border-radius: 999px;
	background: #edf3ff;
	color: #2663eb;
	font-weight: 700;
}

.search-feedback {
	margin: 0;
	color: #667085;
}

.search-feedback.error {
	color: #d14343;
}

.search-groups {
	display: grid;
	gap: 18px;
}

.result-group {
	display: grid;
	gap: 14px;
	padding: 20px;
	border-radius: 22px;
	border: 1px solid rgba(38, 99, 235, 0.12);
	background: linear-gradient(180deg, #ffffff 0%, #f9fbff 100%);
}

.result-group-head {
	display: flex;
	align-items: center;
	justify-content: space-between;
	gap: 12px;
}

.result-group-head h2 {
	margin: 0;
	font-size: 1.05rem;
	font-weight: 800;
	color: #1f2a37;
}

.result-group-head span {
	padding: 6px 10px;
	border-radius: 999px;
	background: #edf3ff;
	color: #2663eb;
	font-size: 13px;
	font-weight: 700;
}

.result-list {
	display: grid;
	gap: 12px;
}

.result-item {
	display: grid;
	gap: 8px;
	padding: 16px 18px;
	border-radius: 18px;
	border: 1px solid rgba(38, 99, 235, 0.1);
	background: #fff;
	transition:
		transform 0.2s ease,
		box-shadow 0.2s ease,
		border-color 0.2s ease;
}

.result-item:hover {
	transform: translateY(-2px);
	border-color: rgba(38, 99, 235, 0.2);
	box-shadow: 0 14px 30px rgba(38, 99, 235, 0.08);
}

.result-item-top {
	display: flex;
	align-items: flex-start;
	justify-content: space-between;
	gap: 12px;
}

.result-item-top h3 {
	margin: 0;
	font-size: 1rem;
	font-weight: 800;
	color: #243247;
}

.result-tag {
	flex-shrink: 0;
	padding: 5px 9px;
	border-radius: 999px;
	background: #f3f6ff;
	color: #5d75d6;
	font-size: 12px;
	font-weight: 700;
}

.result-summary {
	margin: 0;
	color: #526071;
	line-height: 1.65;
}

.result-meta {
	display: flex;
	flex-wrap: wrap;
	gap: 12px;
	color: #7a8699;
	font-size: 13px;
}
</style>
