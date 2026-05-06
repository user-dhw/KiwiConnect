<template>
	<div class="notice-page">
		<h3>Unread Messages: {{ unread }}/{{ total }}</h3>
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

		<el-divider />

		<div v-if="total === 0" class="empty-wrap">
			<img
				src="@/assets/images/noinfo.png"
				alt="No notifications"
				class="empty-image"
			/>
			<div class="empty-text">No notifications yet</div>
		</div>

		<div v-else>
			<div
				class="notice-item"
				v-for="notice in notices"
				:key="notice.notice_id"
			>
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
					<span
						class="state-dot"
						:class="`state-${notice.state}`"
					></span>
					<span>{{ formatDate(notice.createtime) }}</span>
					<span>{{ notice.nickname }}</span>
					<span>{{ notice.action }}</span>
					<span>
						From {{ formatPlatform(notice.router) }}
						<span class="notice-link">
							{{ notice.content_name }}
						</span>
					</span>
					<el-button
						text
						type="danger"
						@click.stop
						@click="updateNotice('delete', notice.notice_id)"
					>
						Delete
					</el-button>
				</div>
				<el-divider />
			</div>
		</div>
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
	// Fire and forget so link navigation is not blocked by the API request.
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
	width: 100%;
}

.toolbar {
	margin: 12px 0;
	position: relative;
	min-height: 30px;
	display: flex;
	justify-content: flex-end;
	gap: 8px;
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
	width: 420px;
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
	padding: 8px 10px;
	border-radius: 8px;
	transition:
		background-color 0.18s ease,
		color 0.18s ease;
}

.notice-row.clickable {
	cursor: pointer;
}

.notice-row.clickable:hover {
	background: #f3f7ff;
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
