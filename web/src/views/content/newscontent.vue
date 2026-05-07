<template>
	<div class="news-detail-page">
		<div v-title data-title="Article Details"></div>

		<div class="page-container">
			<div class="container">
				<div class="page-shell detail-shell page-shell--full">
					<div class="page-main">
						<article class="content-article detail-article">
							<h1 class="post-title">{{ content.article_title }}</h1>

							<div class="post-meta">
								<span class="date">{{ formatDateTime(content.article_createtime) }}</span>
								<span class="category">
									<el-popover
										placement="top-end"
										:width="320"
										trigger="hover"
										popper-class="author-popover"
									>
										<div class="author-popover-card">
											<div class="author-popover-header">
												<img
													:src="content.avatar"
													class="author-popover-avatar"
													height="60"
													width="60"
												/>

												<div class="author-popover-meta">
													<h5 class="author-popover-name">{{ content.nickname }}</h5>
													<p
														class="author-popover-status"
														:class="{ 'is-verified': Number(content.realstate) === 3 }"
													>
														{{
															Number(content.realstate) === 3
																? 'Verified User'
																: 'Unverified User'
														}}
													</p>
												</div>
											</div>

											<div class="author-popover-body">
												<div class="profile-detail">
													<strong>Account</strong>
													<span>{{ content.username || '-' }}</span>
												</div>
												<div class="profile-detail">
													<strong>Email</strong>
													<span>{{ content.mail || '-' }}</span>
												</div>
												<div class="profile-detail">
													<strong>Bio</strong>
													<span>{{ content.synopsis || 'No bio yet' }}</span>
												</div>
											</div>

											<el-button @click="reportUser(content.username)" type="danger" plain>
												Report
											</el-button>
										</div>

										<template #reference>
											<a href="#" title="author" @click.prevent>{{ content.nickname }}</a>
										</template>
									</el-popover>
								</span>
								<span class="comments">
									<a href="#" @click.prevent>{{ commentnum }} Comments</a>
								</span>
							</div>

							<blockquote v-html="content.article_content"></blockquote>
						</article>

						<Comment />
					</div>

				</div>
			</div>
		</div>
	</div>
</template>

<script setup>
import { computed, ref, watch } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useStore } from 'vuex'
import moment from 'moment'
import { ElMessage } from 'element-plus'
import Comment from '@/components/comment.vue'
import { getArticleContent } from '@/api/content'

const props = defineProps({
	id: {
		type: [String, Number],
		default: '',
	},
})

const route = useRoute()
const router = useRouter()
const store = useStore()

const content = ref({})

const contentId = computed(() => String(props.id || route.params.id || ''))
const commentnum = computed(() => store.state.commentnum)

const formatDateTime = value => {
	if (!value) return '-'
	return moment(value).format('YYYY-MM-DD HH:mm')
}

const reportUser = username => {
	const currentUrl =
		typeof window !== 'undefined' ? encodeURIComponent(window.location.href) : ''
	router.push({
		path: '/report',
		query: {
			user: username || '',
			url: currentUrl,
		},
	})
}

const loadArticleContent = async id => {
	if (!id) return

	try {
		const res = await getArticleContent(id)
		if (res.state?.type !== 'SUCCESS') {
			ElMessage.error(res.state?.msg || 'Failed to load article detail')
			return
		}

		content.value = res.data || {}
		await store.dispatch('setcontentinfo', {
			contentname: res.data?.article_title || '',
			contentuserid: res.data?.user_id || '',
		})
	} catch {
		ElMessage.error('Failed to load article detail')
	}
}

watch(
	contentId,
	async id => {
		if (!id) return
		await store.dispatch('setcontentid', id)
		await loadArticleContent(id)
	},
	{ immediate: true },
)
</script>

<style scoped>
.profile-detail {
	display: grid;
	gap: 4px;
	color: #252b37;
}

.profile-detail strong {
	font-size: 0.78rem;
	letter-spacing: 0.04em;
	text-transform: uppercase;
	color: #7a869f;
}

.profile-detail span {
	word-break: break-word;
}

.author-popover-card {
	display: grid;
	gap: 16px;
}

.author-popover-header {
	display: flex;
	align-items: center;
	gap: 14px;
}

.author-popover-avatar {
	flex: 0 0 auto;
	width: 60px;
	height: 60px;
	border-radius: 18px;
	object-fit: cover;
	border: 1px solid rgba(38, 99, 235, 0.12);
}

.author-popover-meta {
	display: grid;
	gap: 4px;
}

.author-popover-name {
	margin: 0;
	font-size: 1.4rem;
	font-weight: 800;
	color: #252b37;
}

.author-popover-status {
	margin: 0;
	font-size: 0.95rem;
	color: #7a869f;
}

.author-popover-status.is-verified {
	color: #2663eb;
	font-weight: 600;
}

.author-popover-body {
	display: grid;
	gap: 12px;
}

:deep(.author-popover) {
	padding: 18px;
	border-radius: 24px;
	border: 1px solid rgba(38, 99, 235, 0.12);
	box-shadow: 0 22px 54px rgba(38, 99, 235, 0.14);
}

@media (min-width: 900px) {
	.page-shell--full {
		grid-template-columns: minmax(0, 1fr);
	}
}
</style>
