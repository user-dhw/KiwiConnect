<template>
	<div class="notice-page">
		<div class="page-heading">
			<h1 class="page-title">Notifications</h1>
			<p class="page-subtitle">
				Review updates from comments, replies, and content interactions across the
				platform.
			</p>
		</div>

		<section class="notice-summary">
			<div class="notice-metric">
				<span class="notice-metric-label">Unread</span>
				<strong>{{ unread }}</strong>
			</div>
			<div class="notice-metric">
				<span class="notice-metric-label">Total</span>
				<strong>{{ total }}</strong>
			</div>
			<div class="toolbar">
				<el-button
					type="primary"
					plain
					size="small"
					v-if="unread > 0"
					@click="updateNotice('changeall', 'a')"
				>
					Mark All as Read
				</el-button>
				<el-button
					type="danger"
					plain
					size="small"
					v-if="total > 0"
					@click="updateNotice('deleteall', 'a')"
				>
					Delete All
				</el-button>
			</div>
		</section>

		<section class="notice-panel">
			<div v-if="total === 0" class="empty-wrap">
				<img
					src="@/assets/images/noinfo.png"
					alt="No notifications"
					class="empty-image"
				/>
				<div class="empty-text">No notifications yet</div>
			</div>

			<div v-else class="notice-list">
				<div class="notice-item" v-for="notice in notices" :key="notice.notice_id">
					<div
						class="notice-row"
						:class="{
							clickable: hasNoticeTarget(notice),
							unread: String(notice.state) === '0',
						}"
						role="button"
						tabindex="0"
						@click="openNotice(notice)"
						@keydown.enter.prevent="openNotice(notice)"
						@keydown.space.prevent="openNotice(notice)"
					>
						<span class="state-dot" :class="`state-${notice.state}`"></span>
						<span>{{ formatDate(notice.createtime) }}</span>
						<span>{{ notice.nickname }}</span>
						<span>{{ notice.action }}</span>
						<span>
							From {{ formatPlatform(notice.router) }}
							<span class="notice-link">{{ notice.content_name }}</span>
						</span>
						<el-button text type="danger" @click.stop @click="updateNotice('delete', notice.notice_id)">
							Delete
						</el-button>
					</div>
				</div>
			</div>
		</section>
	</div>
</template>

<script setup>
import { computed, onMounted, ref } from 'vue'
import { useRouter } from 'vue-router'
import { useStore } from 'vuex'
import moment from 'moment'
import { changeNotice, getNoticeList } from '@/api/account'

const router = useRouter()
const store = useStore()
const notices = ref([])
const total = ref(0)

const unread = computed(() => store.state.user.unread || 0)

const normalizeContentRoute = routeKey => {
	const key = String(routeKey || '').toLowerCase()
	if (key.includes('help')) return 'helpcontent'
	if (key.includes('oldstuff')) return 'oldstuffcontent'
	if (key.includes('activity')) return 'activitycontent'
	if (key.includes('article') || key.includes('news')) return 'articlecontent'
	return ''
}

const formatPlatform = routeKey => {
	const normalized = normalizeContentRoute(routeKey)
	if (normalized === 'helpcontent') return 'Q&A'
	if (normalized === 'oldstuffcontent') return 'Marketplace'
	if (normalized === 'activitycontent') return 'Activities'
	if (normalized === 'articlecontent') return 'Articles / News'
	return 'Content'
}

const formatDate = value => moment(value).format('YYYY-MM-DD HH:mm')

const getNoticeHref = notice => {
	const routeKey = String(notice?.router || '').trim()
	const contentId = String(notice?.content_id || '').trim()
	if (!contentId) return '#'

	if (routeKey && router.hasRoute(routeKey)) {
		return router.resolve({
			name: routeKey,
			params: { id: contentId },
		}).href
	}

	const normalized = normalizeContentRoute(routeKey)
	if (!normalized) return '#'
	return router.resolve({ path: `/${normalized}/${contentId}` }).href
}

const loadNotices = async () => {
	const res = await getNoticeList('')
	if (res.state?.type !== 'SUCCESS') return
	store.dispatch('user/setunread', res.data?.count || 0)
	notices.value = res.data?.list || []
	total.value = res.data?.num || 0
}

const updateNotice = async (change, noticeId) => {
	const res = await changeNotice(change, noticeId)
	if (res.state?.type === 'SUCCESS') {
		await loadNotices()
	}
}

const markAsRead = noticeId => {
	changeNotice('change', noticeId).then(res => {
		if (res.state?.type === 'SUCCESS') {
			loadNotices()
		}
	})
}

const hasNoticeTarget = notice => getNoticeHref(notice) !== '#'

const openNotice = notice => {
	const href = getNoticeHref(notice)
	if (href === '#') return
	markAsRead(notice.notice_id)
	window.open(href, '_blank', 'noopener,noreferrer')
}

onMounted(() => {
	loadNotices()
})
</script>

<style scoped>
.notice-page {
	display: grid;
	gap: 22px;
}

.notice-summary,
.notice-panel {
	background: linear-gradient(180deg, #ffffff 0%, #fbfcff 100%);
	border: 1px solid rgba(38, 99, 235, 0.12);
	border-radius: 24px;
	box-shadow: 0 14px 34px rgba(38, 99, 235, 0.07);
}

.notice-summary {
	display: grid;
	grid-template-columns: repeat(auto-fit, minmax(140px, auto));
	align-items: center;
	gap: 16px;
	padding: 20px 22px;
}

.notice-metric-label {
	display: block;
	margin-bottom: 8px;
	color: #667085;
	font-weight: 700;
}

.notice-metric strong {
	font-size: 1.8rem;
	letter-spacing: -0.04em;
}

.toolbar {
	display: flex;
	flex-wrap: wrap;
	justify-content: flex-end;
	gap: 8px;
	margin-left: auto;
}

.notice-panel {
	padding: 18px;
}

.notice-list {
	display: grid;
	gap: 12px;
}

.notice-item {
	font-size: 14px;
	color: #2d3640;
}

.empty-wrap {
	text-align: center;
}

.empty-image {
	max-width: 100%;
	width: 360px;
	margin: 0 auto;
}

.empty-text {
	margin-top: 8px;
	color: #6b7280;
}

.notice-row {
	display: flex;
	align-items: center;
	gap: 14px;
	flex-wrap: wrap;
	padding: 14px 16px;
	border-radius: 18px;
	border: 1px solid rgba(38, 99, 235, 0.08);
	background: #fff;
	transition:
		background-color 0.18s ease,
		color 0.18s ease,
		transform 0.18s ease;
}

.notice-row.clickable {
	cursor: pointer;
}

.notice-row.clickable:hover {
	background: #f3f7ff;
	transform: translateY(-1px);
}

.notice-row.clickable:active {
	background: #e6efff;
}

.notice-row.unread {
	background: #fff8f8;
}

.notice-row.clickable:focus-visible {
	outline: 2px solid #409eff;
	outline-offset: 2px;
}

.state-dot {
	width: 10px;
	height: 10px;
	border-radius: 50%;
	display: inline-block;
}

.state-0 {
	background: #f56c6c;
}

.state-1 {
	background: #67c23a;
}

.notice-link {
	margin-left: 6px;
	font-size: 15px;
	color: #2a6bdb;
	text-decoration: underline;
}

.notice-row.clickable:hover .notice-link {
	color: #1e4fa8;
}
</style>
