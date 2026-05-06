<template>
	<div class="activitycontent">
		<div v-title data-title="Activity Details"></div>

		<div class="page-container">
			<div class="container">
				<div class="row">
					<div class="span8 page-content">
						<el-page-header
							@back="goBack"
							content="Activity Details"
						/>
						<article
							class="type-post format-standard hentry clearfix"
						>
							<h3>{{ content.activity_title }}</h3>
							<div>
								<div class="show_unit fl ativity">
									<a class="iconfont ic">&#xe622;</a>
									<a class="tagname">Activity Name:</a>
									{{ content.activity_title }}
								</div>

								<el-popover
									placement="left"
									:width="400"
									trigger="hover"
								>
									<li
										class="comment even thread-odd thread-alt depth-1"
										id="li-comment-4"
									>
										<article id="comment-4">
											<img
												:src="content.avatar"
												class="avatar touxiang avatar-60 photo"
												height="60"
												width="60"
											/>

											<div class="comment-meta">
												<h5 class="author">
													{{ content.nickname }}
												</h5>
												<p
													class="date"
													v-if="
														Number(
															content.realstate,
														) === 3
													"
												>
													Verified User
												</p>
												<p class="date" v-else>
													Unverified User
												</p>
											</div>
										</article>
									</li>

									<div class="xinxi">
										<p style="color: #000">Account:</p>
										<p>{{ content.username }}</p>
									</div>
									<div class="xinxi">
										<p style="color: #000">Email:</p>
										<p>{{ content.mail }}</p>
									</div>
									<div class="xinxi">
										<p style="color: #000">QQ：</p>
										<p>{{ content.qq }}</p>
									</div>
									<div class="xinxi">
										<p style="color: #000">Bio:</p>
										<p>{{ content.synopsis }}</p>
									</div>

									<el-button
										@click="reportUser(content.username)"
										style="margin: 10px 150px"
										type="danger"
										plain
									>
										Report
									</el-button>

									<template #reference>
										<div class="show_unit fl ativity">
											<a class="iconfont ic">&#xe66a;</a>
											<a class="tagname">Organizer:</a>
											{{ content.nickname }}
										</div>
									</template>
								</el-popover>

								<div class="show_unit fl ativity">
									<a class="iconfont ic">&#xe62a;</a>
									<a class="tagname">Activity Time:</a>
									{{
										formatDateTime(
											content.activity_statetime,
										)
									}}
									-
									{{
										formatDateTime(content.activity_endtime)
									}}
								</div>
								<div class="show_unit fl ativity">
									<a class="iconfont ic">&#xe62a;</a>
									<a class="tagname">Participants:</a>
									{{ content.activity_num }}
								</div>
								<div class="show_unit fl ativity">
									<a class="iconfont ic">&#xe62a;</a>
									<a class="tagname">Location:</a>
									{{ content.activity_locale }}
								</div>
								<div style="clear: both"></div>
							</div>

							<h3>Activity Description</h3>
							<blockquote
								v-html="content.activity_content"
							></blockquote>
						</article>

						<el-button
							type="primary"
							@click="joinActivity"
							:loading="isJoining"
							:disabled="!isUserVerified"
							style="width: 80px; margin: 0 auto; display: block"
							:title="
								isUserVerified
									? ''
									: 'You must verify your account before joining activities'
							"
						>
							Join
						</el-button>
						<p
							v-if="!isUserVerified"
							style="
								color: #ff6b6b;
								font-size: 12px;
								margin-top: 8px;
								text-align: center;
							"
						>
							You must verify your account before joining
							activities
						</p>

						<div class="block">
							<h3>Announcements</h3>
							<el-timeline>
								<el-timeline-item
									v-for="(item, id) in announcementList"
									:key="item.announcement_id || id"
									:timestamp="
										formatDateTime(
											item.announcement_createtime,
										)
									"
									placement="top"
								>
									<el-card>
										<h4>{{ item.announcement_name }}</h4>
										<p>{{ item.announcement_content }}</p>
									</el-card>
								</el-timeline-item>
							</el-timeline>
						</div>

						<Comment />
					</div>

					<aside class="span4 page-sidebar">
						<Carousel />
						<Activity />
					</aside>
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
import Carousel from '@/components/carousel.vue'
import Comment from '@/components/comment.vue'
import Activity from '@/components/activity.vue'
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
		typeof window !== 'undefined'
			? encodeURIComponent(window.location.href)
			: ''
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

		ElMessage.error(
			res.state?.msg || 'You have already joined this activity',
		)
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
.activitycontent {
	min-height: 200px;
}

.tagname {
	margin-right: 16px;
	font-size: 18px;
}
</style>
