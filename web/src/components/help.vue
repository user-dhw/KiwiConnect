<template>
	<div class="hoothelp">
		<section class="widget">
			<router-link to="/help" custom v-slot="{ navigate }">
				<div class="page-header section-header" @click="navigate">
					<h3 class="section-title">Discussion / Q&A</h3>
					<h4 class="section-more">More &gt;</h4>
				</div>
			</router-link>

			<ul class="articles" v-loading="isLoading">
				<li
					class="article-entry standard"
					v-for="(item, id) in tableData"
					:key="item.help_id || id"
				>
					<h4>
						<router-link :to="`/helpcontent/${item.help_id}`">
							{{ item.help_title }}
						</router-link>
					</h4>
					<span class="article-meta">
						<span>{{ formatDate(item.createtime) }}</span>
						<span class="meta-separator">•</span>
						<span>Information Platform</span>
					</span>
					<span class="like-count">Views {{ item.help_read_num }}</span>
				</li>

				<li
					v-if="!isLoading && tableData.length === 0"
					class="article-entry standard empty-state"
				>
					No Q&A data available
				</li>
			</ul>
		</section>
	</div>
</template>

<script setup>
import { onMounted, ref } from 'vue'
import moment from 'moment'
import { ElMessage } from 'element-plus'
import { getWebHelpList } from '@/api/content'

const pagelistquery = ref({
	lable: '',
	tag: '',
	pagesize: 3,
	page: 1,
	total: 0,
})

const tableData = ref([])
const isLoading = ref(false)

const formatDate = value => {
	return value ? moment(value).format('YYYY-MM-DD HH:mm') : '-'
}

const getHelpList = async () => {
	isLoading.value = true
	try {
		const res = await getWebHelpList(pagelistquery.value)
		if (res.state?.type === 'SUCCESS') {
			tableData.value = Array.isArray(res.data) ? res.data : []
			pagelistquery.value.total = Number(res.count || 0)
			return
		}

		ElMessage.error(res.state?.msg || 'Failed to load Q&A list')
	} catch {
		ElMessage.error('Failed to load Q&A list')
	} finally {
		isLoading.value = false
	}
}

onMounted(() => {
	getHelpList()
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

.meta-separator {
	color: #c0c7d0;
}

@media (max-width: 768px) {
	.section-more {
		font-size: 13px;
	}
}
</style>
