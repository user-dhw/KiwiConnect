<template>
	<div class="oldstuff-detail-page">
		<div class="page-container">
			<div class="container">
				<div class="page-shell detail-shell page-shell--full">
					<div class="page-main">
						<div class="detail-back">
							<el-page-header @back="goBack" content="Item Details" />
						</div>

						<article class="content-article detail-article">
							<div class="detail-grid">
								<div class="detail-media-card">
									<img :src="content.oldstuff_img" alt="Item photo" />
								</div>

								<div class="detail-info-card">
									<h1 class="post-title">{{ content.oldstuff_name }}</h1>
									<div class="detail-price">¥{{ content.oldstuff_price }}</div>
									<p class="detail-original-price">
										Original price: {{ content.oldstuff_price }}
									</p>

									<div class="detail-facts">
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
												<div class="detail-fact detail-fact-clickable">
													<div class="detail-fact-icon">@</div>
													<div class="detail-fact-copy">
														<span class="detail-fact-label">Seller</span>
														<span class="detail-fact-value">{{ content.nickname || '-' }}</span>
													</div>
												</div>
											</template>
										</el-popover>

										<div class="detail-fact">
											<div class="detail-fact-icon">P</div>
											<div class="detail-fact-copy">
												<span class="detail-fact-label">Phone</span>
												<span class="detail-fact-value">{{ content.phone || '-' }}</span>
											</div>
										</div>
									</div>

									<div class="detail-actions">
										<el-button
											type="primary"
											@click="
												isUserVerified
													? (dialogFormVisible = true)
													: ElMessage.warning(
															'You must verify your account to express interest',
														)
											"
											:disabled="!isUserVerified"
										>
											{{ isUserVerified ? 'Interested in buying' : 'Verify Account to Buy' }}
										</el-button>
										<p v-if="!isUserVerified" class="detail-note">
											You must verify your account before expressing purchase
											interest.
										</p>
									</div>

									<div class="detail-summary-card">
										<p v-if="content.oldstuff_lable" class="detail-summary-eyebrow">
											{{ content.oldstuff_lable }}
										</p>
										<p v-if="listingLead" class="detail-summary-text">
											{{ listingLead }}
										</p>
										<div class="detail-summary-tags">
											<span v-if="content.oldstuff_price" class="detail-summary-tag">
												¥{{ content.oldstuff_price }}
											</span>
											<span v-if="content.nickname" class="detail-summary-tag">
												Sold by {{ content.nickname }}
											</span>
										</div>
									</div>

									<el-dialog
										v-model="dialogFormVisible"
										title="Purchase Intent"
										width="min(92vw, 520px)"
									>
										<el-form :model="intentForm" size="default">
											<el-form-item label="Contact info">
												<el-input autocomplete="off" v-model="intentForm.describe" />
											</el-form-item>
											<el-form-item label="Intended price">
												<el-input autocomplete="off" v-model="intentForm.name" />
											</el-form-item>
										</el-form>

										<template #footer>
											<el-button @click="dialogFormVisible = false">Cancel</el-button>
											<el-button type="primary" :loading="isJoining" @click="submitJoin">
												Confirm
											</el-button>
										</template>
									</el-dialog>
								</div>
							</div>

							<h3 class="detail-section-title">Item Description</h3>
							<blockquote v-html="content.oldstuff_content"></blockquote>
						</article>

						<Comment />
					</div>

				</div>
			</div>
		</div>
	</div>
</template>

<script setup>
import { computed, reactive, ref, watch } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useStore } from 'vuex'
import { ElMessage } from 'element-plus'
import Comment from '@/components/comment.vue'
import { getOldStuffContent, setJoin } from '@/api/content'

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
const dialogFormVisible = ref(false)
const isJoining = ref(false)

const intentForm = reactive({
	name: '',
	describe: '',
})

const contentId = computed(() => String(props.id || route.params.id || ''))
const contentUserId = computed(() => store.state.contentuserid)
const isUserVerified = computed(() => {
	const userinfo = store.state.user?.userinfo || {}
	return Number(userinfo.realstate) === 3
})
const listingLead = computed(() => {
	const raw = String(content.value?.oldstuff_content || '')
	const plain = raw.replace(/<[^>]+>/g, ' ').replace(/\s+/g, ' ').trim()
	if (!plain) return ''
	return plain.length > 180 ? `${plain.slice(0, 180)}...` : plain
})

const goBack = () => {
	router.back()
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

const apiBaseUrl = import.meta.env.VITE_API_URL || '/api'

const normalizeFileUrl = value => {
	if (!value || typeof value !== 'string') return ''
	if (value.startsWith('http://127.0.0.1:3000')) {
		return value.replace('http://127.0.0.1:3000', apiBaseUrl)
	}
	if (value.startsWith('http://localhost:3000')) {
		return value.replace('http://localhost:3000', apiBaseUrl)
	}
	if (/^https?:\/\//i.test(value)) return value
	if (value.startsWith('/api/')) return value
	if (value.startsWith('/uplodes/')) return `${apiBaseUrl}${value}`
	return `${apiBaseUrl}/uplodes/${value.replace(/^\/+/, '')}`
}

const loadOldStuffContent = async id => {
	if (!id) return

	try {
		const res = await getOldStuffContent(id)
		if (res.state?.type !== 'SUCCESS') {
			ElMessage.error(res.state?.msg || 'Failed to load item details')
			return
		}

		content.value = {
			...(res.data || {}),
			oldstuff_img: normalizeFileUrl(res.data?.oldstuff_img || ''),
			avatar: normalizeFileUrl(res.data?.avatar || ''),
		}
		await store.dispatch('setcontentinfo', {
			contentname: res.data?.oldstuff_name || '',
			contentuserid: res.data?.user_id || '',
		})
	} catch {
		ElMessage.error('Failed to load item details')
	}
}

const submitJoin = async () => {
	if (!isUserVerified.value) {
		ElMessage.error('You must verify your account to express purchase interest')
		return
	}

	if (!content.value?.oldstuff_id) {
		ElMessage.error('Item information is not ready')
		return
	}

	isJoining.value = true
	try {
		const payload = {
			type: route.name || 'OldStuffContent',
			name: intentForm.name,
			describe: intentForm.describe,
			content_id: content.value.oldstuff_id,
			contentname: content.value.oldstuff_name || '',
			to_userid: contentUserId.value || '',
		}

		const res = await setJoin(payload)
		if (res.state?.type === 'SUCCESS') {
			ElMessage.success('Purchase intent submitted successfully')
			dialogFormVisible.value = false
			intentForm.name = ''
			intentForm.describe = ''
			return
		}

		ElMessage.error(
			res.state?.msg || 'You have already submitted an intent for this item',
		)
	} catch {
		ElMessage.error('Failed to submit purchase intent')
	} finally {
		isJoining.value = false
	}
}

watch(
	contentId,
	async id => {
		if (!id) return
		await store.dispatch('setcontentid', id)
		await loadOldStuffContent(id)
	},
	{ immediate: true },
)
</script>

<style scoped>
.detail-fact-clickable {
	cursor: pointer;
}

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

.detail-summary-card {
	display: grid;
	gap: 12px;
	margin: 18px 0 0;
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

<style scoped>
.detail-price {
	margin-top: 6px;
	font-size: clamp(1.7rem, 3vw, 2.2rem);
	font-weight: 800;
	color: #d93b52;
}

.detail-original-price {
	margin: 8px 0 0;
	color: #7d8799;
}

.detail-fact-clickable {
	cursor: pointer;
}

.detail-section-title {
	margin: 22px 0 12px;
	font-size: 1.18rem;
	font-weight: 800;
}

.profile-detail {
	margin-bottom: 10px;
	color: #252b37;
}

@media (min-width: 900px) {
	.page-shell--full {
		grid-template-columns: minmax(0, 1fr);
	}
}
</style>
