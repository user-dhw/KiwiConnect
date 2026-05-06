<template>
	<div class="oldstuff-detail-page">
		<div class="page-container">
			<div class="container">
				<div class="page-shell detail-shell">
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
										<el-popover placement="left" :width="400" trigger="hover">
											<li class="comment even thread-odd thread-alt depth-1" id="li-comment-4">
												<article id="comment-4">
													<img
														:src="content.avatar"
														class="avatar touxiang avatar-60 photo"
														height="60"
														width="60"
													/>
													<div class="comment-meta">
														<h5 class="author">{{ content.nickname }}</h5>
														<p class="date" v-if="Number(content.realstate) === 3">
															Verified User
														</p>
														<p class="date" v-else>Unverified User</p>
													</div>
												</article>
											</li>

											<div class="profile-detail"><strong>Account:</strong> {{ content.username }}</div>
											<div class="profile-detail"><strong>Email:</strong> {{ content.mail }}</div>
											<div class="profile-detail"><strong>Bio:</strong> {{ content.synopsis }}</div>

											<el-button @click="reportUser(content.username)" type="danger" plain>
												Report
											</el-button>

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

					<aside class="page-aside panel-stack">
						<Carousel />
						<OldStuffHot />
					</aside>
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
import Carousel from '@/components/carousel.vue'
import Comment from '@/components/comment.vue'
import OldStuffHot from '@/components/oldstuffhot.vue'
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
</style>
