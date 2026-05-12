<template>
	<div class="help-detail-page">
		<div v-title data-title="Q&A Details"></div>

		<div class="page-container">
			<div class="container">
				<div class="page-shell detail-shell page-shell--full">
					<div class="page-main">
						<article class="content-article detail-article">
							<h1 class="post-title">{{ content.help_title }}</h1>

							<div class="post-meta">
								<span class="date">{{ formatDateTime(content.createtime) }}</span>
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
									<a href="#" title="comments" @click.prevent>{{ commentnum }} Comments</a>
								</span>
							</div>

							<div
								v-if="helpLead || helpTags.length || content.help_lable"
								class="detail-summary-card"
							>
								<p v-if="content.help_lable" class="detail-summary-eyebrow">
									{{ content.help_lable }}
								</p>
								<p v-if="helpLead" class="detail-summary-text">
									{{ helpLead }}
								</p>
								<div v-if="helpTags.length" class="detail-summary-tags">
									<span
										v-for="(tag, id) in helpTags"
										:key="`summary-${tag}-${id}`"
										class="detail-summary-tag"
									>
										{{ tag }}
									</span>
								</div>
							</div>

							<blockquote v-html="content.help_content"></blockquote>

							<div class="tag-list">
								<span
									v-for="(tag, id) in helpTags"
									:key="`${tag}-${id}`"
									class="tag-chip"
									@click="navigateByTag(tag)"
								>
									{{ tag }}
								</span>
							</div>
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
import { getHelpContent } from '@/api/content'
import { normalizeFileUrl } from '@/utils/currentUser'

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
const helpTags = computed(() => {
	const raw = content.value?.help_tag
	if (Array.isArray(raw)) return raw
	if (typeof raw !== 'string' || !raw.trim()) return []
	return raw
		.split(',')
		.map(item => item.trim())
		.filter(Boolean)
})
const helpLead = computed(() => {
	const raw = String(content.value?.help_content || '')
	const plain = raw.replace(/<[^>]+>/g, ' ').replace(/\s+/g, ' ').trim()
	if (!plain) return ''
	return plain.length > 180 ? `${plain.slice(0, 180)}...` : plain
})

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

const navigateByTag = tag => {
	if (!tag) return
	router.push(`/help/${tag}`)
}

const loadHelpContent = async id => {
	if (!id) return

	try {
		const res = await getHelpContent(id)
		if (res.state?.type !== 'SUCCESS') {
			ElMessage.error(res.state?.msg || 'Failed to load Q&A detail')
			return
		}

		content.value = {
			...(res.data || {}),
			avatar: normalizeFileUrl(res.data?.avatar || ''),
		}
		await store.dispatch('setcontentinfo', {
			contentname: res.data?.help_title || '',
			contentuserid: res.data?.user_id || '',
		})
	} catch {
		ElMessage.error('Failed to load Q&A detail')
	}
}

watch(
	contentId,
	async id => {
		if (!id) return
		await store.dispatch('setcontentid', id)
		await loadHelpContent(id)
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

.detail-summary-card {
	display: grid;
	gap: 12px;
	margin: 0 0 18px;
	padding: 18px 22px;
	border-radius: 20px;
	border: 1px solid rgba(38, 99, 235, 0.1);
	background: linear-gradient(180deg, #f9fbff 0%, #f3f7ff 100%);
}

.detail-summary-eyebrow {
	margin: 0;
	font-size: 0.78rem;
	font-weight: 800;
	letter-spacing: 0.08em;
	text-transform: uppercase;
	color: #2663eb;
}

.detail-summary-text {
	margin: 0;
	font-size: 1.02rem;
	line-height: 1.8;
	color: #56637d;
}

.detail-summary-tags {
	display: flex;
	flex-wrap: wrap;
	gap: 8px;
}

.detail-summary-tag {
	display: inline-flex;
	align-items: center;
	padding: 7px 12px;
	border-radius: 999px;
	background: rgba(38, 99, 235, 0.1);
	color: #2663eb;
	font-size: 0.86rem;
	font-weight: 700;
}

@media (min-width: 900px) {
	.page-shell--full {
		grid-template-columns: minmax(0, 1fr);
	}
}
</style>
