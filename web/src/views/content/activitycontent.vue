<template>
	<div class="activity-detail-page">
		<div v-title data-title="Activity Details"></div>

		<div class="page-container">
			<div class="container">
				<div class="page-shell detail-shell page-shell--full">
					<div class="page-main">
						<div class="detail-back">
							<el-page-header @back="goBack" content="Activity Details" />
						</div>

						<article class="content-article detail-article">
							<h1 class="post-title">{{ content.activity_title }}</h1>

							<div class="detail-facts">
								<div class="detail-fact">
									<div class="detail-fact-icon">#</div>
									<div class="detail-fact-copy">
										<span class="detail-fact-label">Activity Name</span>
										<span class="detail-fact-value">{{ content.activity_title || '-' }}</span>
									</div>
								</div>

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
												<span class="detail-fact-label">Organizer</span>
												<span class="detail-fact-value">{{ content.nickname || '-' }}</span>
											</div>
										</div>
									</template>
								</el-popover>

								<div class="detail-fact">
									<div class="detail-fact-icon">T</div>
									<div class="detail-fact-copy">
										<span class="detail-fact-label">Activity Time</span>
										<span class="detail-fact-value">
											{{ formatDateTime(content.activity_statetime) }} -
											{{ formatDateTime(content.activity_endtime) }}
										</span>
									</div>
								</div>

								<div class="detail-fact">
									<div class="detail-fact-icon">P</div>
									<div class="detail-fact-copy">
										<span class="detail-fact-label">Participants</span>
										<span class="detail-fact-value">{{ content.activity_num || '-' }}</span>
									</div>
								</div>

								<div class="detail-fact">
									<div class="detail-fact-icon">L</div>
									<div class="detail-fact-copy">
										<span class="detail-fact-label">Location</span>
										<span class="detail-fact-value">{{ content.activity_locale || '-' }}</span>
									</div>
								</div>
							</div>

							<div class="detail-actions">
								<el-button
									type="primary"
									@click="joinActivity"
									:loading="isJoining"
									:disabled="!isUserVerified"
									:title="
										isUserVerified
											? ''
											: 'You must verify your account before joining activities'
									"
								>
									Join Activity
								</el-button>
								<p v-if="!isUserVerified" class="detail-note">
									You must verify your account before joining activities.
								</p>
							</div>

							<h3 class="detail-section-title">Activity Description</h3>
							<blockquote v-html="content.activity_content"></blockquote>
						</article>

						<section class="section-card timeline-card">
							<h3 class="detail-section-title">Announcements</h3>
							<el-timeline>
								<el-timeline-item
									v-for="(item, id) in announcementList"
									:key="item.announcement_id || id"
									:timestamp="formatDateTime(item.announcement_createtime)"
									placement="top"
								>
									<el-card>
										<h4>{{ item.announcement_name }}</h4>
										<p>{{ item.announcement_content }}</p>
									</el-card>
								</el-timeline-item>
							</el-timeline>
						</section>

						<Comment />
					</div>

				</div>
			</div>
		</div>
	</div>
</template>

<script setup>
import { computed, onMounted, ref, watch } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useStore } from 'vuex'
import moment from 'moment'
import { ElMessage } from 'element-plus'
import Comment from '@/components/comment.vue'
import { getActivityContent, getAnnouncementList, setJoin } from '@/api/content'

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
const announcementList = ref([])
const isJoining = ref(false)

const contentId = computed(() => String(props.id || route.params.id || ''))
const contentUserId = computed(() => store.state.contentuserid)
const isUserVerified = computed(() => {
	const userinfo = store.state.user?.userinfo || {}
	return Number(userinfo.realstate) === 3
})

const formatDateTime = value => {
	if (!value) return '-'
	return moment(value).format('YYYY-MM-DD HH:mm')
}

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

const loadActivityContent = async id => {
	if (!id) return

	try {
		const res = await getActivityContent(id)
		if (res.state?.type !== 'SUCCESS') {
			ElMessage.error(res.state?.msg || 'Failed to load activity details')
			return
		}

		content.value = res.data || {}
		await store.dispatch('setcontentinfo', {
			contentname: res.data?.activity_title || '',
			contentuserid: res.data?.user_id || '',
		})
	} catch {
		ElMessage.error('Failed to load activity details')
	}
}

const loadAnnouncementList = async id => {
	if (!id) return

	try {
		const res = await getAnnouncementList(id)
		if (res.state?.type !== 'SUCCESS') {
			announcementList.value = []
			return
		}

		announcementList.value = Array.isArray(res.data) ? res.data : []
	} catch {
		announcementList.value = []
	}
}

const joinActivity = async () => {
	if (!content.value?.activity_id) {
		ElMessage.error('Activity information is not ready')
		return
	}

	isJoining.value = true
	try {
		const payload = {
			type: route.name || 'ActivityContent',
			name: '',
			describe: '',
			content_id: content.value.activity_id,
			contentname: content.value.activity_title || '',
			to_userid: contentUserId.value || '',
		}

		const res = await setJoin(payload)
		if (res.state?.type === 'SUCCESS') {
			ElMessage.success('Joined successfully')
			return
		}

		ElMessage.error(res.state?.msg || 'You have already joined this activity')
	} catch {
		ElMessage.error('Failed to join this activity')
	} finally {
		isJoining.value = false
	}
}

watch(
	contentId,
	async id => {
		if (!id) return
		await store.dispatch('setcontentid', id)
		await Promise.all([loadActivityContent(id), loadAnnouncementList(id)])
	},
	{ immediate: true },
)

onMounted(async () => {
	if (contentId.value) {
		await store.dispatch('setcontentid', contentId.value)
	}
})
</script>

<style scoped>
.detail-fact-clickable {
	cursor: pointer;
}

.detail-section-title {
	margin: 22px 0 12px;
	font-size: 1.18rem;
	font-weight: 800;
}

.timeline-card {
	margin-top: 24px;
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
