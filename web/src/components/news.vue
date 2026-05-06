<template>
	<div class="help">
		<div>
			<router-link to="/news" custom v-slot="{ navigate }">
				<div class="page-header section-header" @click="navigate">
					<h3 class="section-title">News / Articles</h3>
					<h4 class="section-more">More &gt;</h4>
				</div>
			</router-link>

			<article
				class="format-standard type-post hentry clearfix"
				v-for="(item, id) in tableData"
				:key="item.article_id || id"
			>
				<header class="clearfix">
					<h3 class="post-title">
						<router-link :to="`/newscontent/${item.article_id}`">
							{{ item.article_title }}
						</router-link>
					</h3>

					<div class="post-meta clearfix">
						<span class="date">{{
							formatDate(item.article_createtime)
						}}</span>
						<span class="category">
							<a href="#" @click.prevent>{{
								item.nickname || 'Information Platform'
							}}</a>
						</span>
						<span class="comments">Campus update</span>
						<span class="like-count">Views {{ item.article_read_num }}</span>
					</div>
				</header>
			</article>

			<div
				v-if="!isLoading && tableData.length === 0"
				class="empty-state"
			>
				No articles available
			</div>
		</div>
	</div>
</template>

<script setup>
import { onMounted, ref } from 'vue'
import moment from 'moment'
import { ElMessage } from 'element-plus'
import { getArticleList } from '@/api/content'

const pagelistquery = ref({
	lable: '',
	tag: '',
	total: 0,
	pagesize: 3,
	page: 1,
})

const tableData = ref([])
const isLoading = ref(false)

const formatDate = value => {
	return value ? moment(value).format('YYYY-MM-DD HH:mm') : '-'
}

const getArticleData = async () => {
	isLoading.value = true
	try {
		const res = await getArticleList(pagelistquery.value)
		if (res.state?.type === 'SUCCESS') {
			tableData.value = Array.isArray(res.data) ? res.data : []
			pagelistquery.value.total = Number(res.count || 0)
			return
		}

		ElMessage.error(res.state?.msg || 'Failed to load articles')
	} catch {
		ElMessage.error('Failed to load articles')
	} finally {
		isLoading.value = false
	}
}

onMounted(() => {
	getArticleData()
})
</script>

<style scoped>
.help {
	min-height: 200px;
}

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
	padding: 8px 0;
}

@media (max-width: 768px) {
	.section-more {
		font-size: 13px;
	}
}
</style>
