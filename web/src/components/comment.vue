<template>
	<div class="comment">
		<section id="comments">
			<h3 id="comments-title">({{ commentCount }}) Comments</h3>

			<article
				id="comment-self"
				v-if="hasCurrentUser"
				class="self-user-row"
			>
				<img
					:src="currentAvatar"
					class="avatar touxiang avatar-60 photo"
					style="width: 50px; height: 50px"
				/>
				<h4 class="author" style="display: inline">
					{{ currentNickname }}
				</h4>
			</article>

			<div
				v-if="editorId !== -1"
				@click="openCommentEditor"
				:class="['commenttop', { disabled: !isUserVerified }]"
				:style="{
					cursor: isUserVerified ? 'pointer' : 'not-allowed',
					opacity: isUserVerified ? 1 : 0.6,
				}"
			>
				{{
					isUserVerified
						? 'Add Comment'
						: 'Verify Your Account to Comment'
				}}
			</div>

			<div
				v-if="!isUserVerified && editorId === -1"
				style="
					color: #ff6b6b;
					font-size: 12px;
					padding: 10px;
					background: #ffe6e6;
					border-radius: 4px;
					margin-bottom: 10px;
				"
			>
				You must verify your account before commenting.
			</div>

			<comment-editor
				v-if="editorId === -1"
				v-model="commentContent"
				placeholder="Write your comment"
				:rows="6"
				@submit="submitComment"
			/>

			<ol class="commentlist">
				<li
					v-for="(item, idx) in commentList"
					:key="item.comment_id || idx"
					class="comment even thread-even depth-1"
				>
					<article>
						<el-popover
							placement="top"
							:width="380"
							trigger="hover"
						>
							<template #reference>
								<div class="comment-meta-row">
									<img
										:src="item.avatar"
										class="avatar touxiang avatar-60 photo"
										height="40"
										width="40"
									/>
									<div class="comment-meta">
										<h5 class="author">
											{{ item.nickname }}
										</h5>
										<p class="date">
											{{
												formatDate(
													item.comment_createtime,
												)
											}}
										</p>
									</div>
								</div>
							</template>

							<div class="profile-card">
								<p>
									<strong>Account:</strong>
									{{ item.username || '-' }}
								</p>
								<p>
									<strong>Email:</strong>
									{{ item.mail || '-' }}
								</p>
								<p>
									<strong>Bio:</strong>
									{{ item.synopsis || '-' }}
								</p>
								<el-button
									@click="reportUser(item.username)"
									type="danger"
									plain
								>
									Report
								</el-button>
							</div>
						</el-popover>

						<div
							class="comment-body"
							v-html="item.comment_content"
						></div>

						<div class="comment-footer">
							<p @click="toggleReplyList(item.comment_id, idx)">
								View Replies
							</p>
							<p
								@click="
									isUserVerified
										? openReplyEditor(
												idx,
												item.nickname,
												item.user_id,
												item.comment_id,
											)
										: ElMessage.warning(
												'You must verify your account before replying',
											)
								"
								:style="{
									cursor: isUserVerified
										? 'pointer'
										: 'not-allowed',
									opacity: isUserVerified ? 1 : 0.6,
								}"
							>
								Reply
							</p>
						</div>

						<div class="reply" v-if="idx === editorId">
							<comment-editor
								v-model="commentContent"
								placeholder="Write your reply"
								:rows="5"
								@submit="submitComment"
							/>
						</div>
					</article>

					<ul class="children" v-if="idx === replyListVisibleIndex">
						<li
							v-for="(reply, replyIdx) in replyList"
							:key="reply.reply_id || replyIdx"
							class="comment byuser odd alt depth-2"
						>
							<article>
								<el-popover
									placement="top"
									:width="380"
									trigger="hover"
								>
									<template #reference>
										<div class="comment-meta-row">
											<img
												:src="reply.avatar"
												class="avatar touxiang avatar-60 photo"
												height="40"
												width="40"
											/>
											<div class="comment-meta">
												<h5 class="author">
													{{ reply.nickname }}
													@
													<span class="touser">{{
														reply.tousernickname
													}}</span>
												</h5>
												<p class="date">
													{{
														formatDate(
															reply.createtime,
														)
													}}
												</p>
											</div>
										</div>
									</template>

									<div class="profile-card">
										<p>
											<strong>Account:</strong>
											{{ reply.username || '-' }}
										</p>
										<p>
											<strong>Email:</strong>
											{{ reply.mail || '-' }}
										</p>
										<p>
											<strong>Bio:</strong>
											{{ reply.synopsis || '-' }}
										</p>
										<el-button
											@click="reportUser(reply.username)"
											type="danger"
											plain
										>
											Report
										</el-button>
									</div>
								</el-popover>

								<div
									class="comment-body reply-body"
									v-html="reply.reply_content"
								></div>

								<div class="comment-footer">
									<p
										@click="
											openNestedReplyEditor(
												replyIdx,
												reply.nickname,
												reply.user_id,
												reply.comment_id,
											)
										"
									>
										Reply
									</p>
								</div>

								<div
									class="reply"
									v-if="replyIdx === replyEditorId"
								>
									<comment-editor
										v-model="commentContent"
										placeholder="Write your reply"
										:rows="4"
										@submit="submitComment"
									/>
								</div>
							</article>
						</li>
					</ul>
				</li>
			</ol>
		</section>
	</div>
</template>

<script setup>
import { computed, onMounted, ref } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useStore } from 'vuex'
import { ElMessage } from 'element-plus'
import moment from 'moment'
import CommentEditor from '@/components/comment/CommentEditor.vue'
import {
	createComment,
	createReply,
	getCommentList,
	getReplyList,
} from '@/api/comment'

const props = defineProps({
	contentId: {
		type: String,
		default: '',
	},
	contentName: {
		type: String,
		default: '',
	},
	contentUserId: {
		type: String,
		default: '',
	},
})

const store = useStore()
const router = useRouter()
const route = useRoute()

const editorId = ref(-2)
const replyEditorId = ref(-2)
const replyListVisibleIndex = ref(-1)

const currentCommentId = ref('')
const toUserId = ref('')
const toUserNickname = ref('')

const commentList = ref([])
const replyList = ref([])
const commentContent = ref('')

const contentId = computed(() => props.contentId || store.state.contentid)
const contentName = computed(() => props.contentName || store.state.contentname)
const contentUserId = computed(
	() => props.contentUserId || store.state.contentuserid,
)
const commentCount = computed(() => Number(store.state.commentnum || 0))

const currentAvatar = computed(() => store.state.user?.userinfo?.avatar || '')
const currentNickname = computed(
	() => store.state.user?.userinfo?.nickname || '',
)
const hasCurrentUser = computed(() =>
	Boolean(currentNickname.value || currentAvatar.value),
)

const isUserVerified = computed(() => {
	const userinfo = store.state.user?.userinfo || {}
	return Number(userinfo.realstate) === 3
})

const formatDate = value => {
	return value ? moment(value).format('YYYY-MM-DD HH:mm') : '-'
}

const resetReplyTargets = () => {
	toUserId.value = ''
	toUserNickname.value = ''
	currentCommentId.value = ''
}

const reportUser = username => {
	router.push({
		path: '/report',
		query: { user: username || '', url: window.location.href || '' },
	})
}

const openCommentEditor = () => {
	if (!isUserVerified.value) {
		ElMessage.warning('You must verify your account before commenting')
		return
	}
	editorId.value = -1
	replyEditorId.value = -2
	resetReplyTargets()
}

const openReplyEditor = (index, nickname, userId, commentId) => {
	if (editorId.value === index) {
		editorId.value = -2
		resetReplyTargets()
		return
	}

	editorId.value = index
	replyEditorId.value = -2
	toUserId.value = userId || ''
	toUserNickname.value = nickname || ''
	currentCommentId.value = commentId || ''
}

const openNestedReplyEditor = (index, nickname, userId, commentId) => {
	if (replyEditorId.value === index) {
		replyEditorId.value = -2
		resetReplyTargets()
		return
	}

	replyEditorId.value = index
	editorId.value = -2
	toUserId.value = userId || ''
	toUserNickname.value = nickname || ''
	currentCommentId.value = commentId || ''
}

const loadReplies = async commentId => {
	if (!commentId) {
		replyList.value = []
		return
	}

	try {
		const res = await getReplyList(commentId)
		if (res.state?.type === 'SUCCESS') {
			replyList.value = Array.isArray(res.data) ? res.data : []
			return
		}
		replyList.value = []
	} catch {
		replyList.value = []
	}
}

const toggleReplyList = async (commentId, index) => {
	if (replyListVisibleIndex.value === index) {
		replyListVisibleIndex.value = -1
		replyList.value = []
		return
	}

	replyListVisibleIndex.value = index
	await loadReplies(commentId)
}

const loadComments = async () => {
	if (!contentId.value) {
		commentList.value = []
		store.dispatch('setcommentnum', 0)
		return
	}

	try {
		const res = await getCommentList(contentId.value)
		if (res.state?.type === 'SUCCESS') {
			commentList.value = Array.isArray(res.data) ? res.data : []
			store.dispatch('setcommentnum', Number(res.count || 0))
			return
		}
		commentList.value = []
		store.dispatch('setcommentnum', 0)
	} catch {
		commentList.value = []
		store.dispatch('setcommentnum', 0)
	}
}

const submitComment = async () => {
	if (!isUserVerified.value) {
		ElMessage.error('You must verify your account before commenting')
		return
	}

	if (!commentContent.value.trim()) {
		ElMessage.error('Comment content cannot be empty')
		return
	}

	if (!contentId.value) {
		ElMessage.error('Missing content information')
		return
	}

	const payload = {
		router: route.name || '',
		content_id: contentId.value,
		contentname: contentName.value,
		comment_content: commentContent.value,
	}

	try {
		let res
		if (toUserId.value) {
			res = await createReply({
				...payload,
				to_userid: toUserId.value,
				touserid: toUserId.value,
				tousernickname: toUserNickname.value,
				comment_id: currentCommentId.value,
			})
		} else {
			res = await createComment({
				...payload,
				to_userid: contentUserId.value,
			})
		}

		if (res.State?.Type == 'UNAUTHORIZED') {
			ElMessage.error('Please log in to submit a comment')
			return
		}

		ElMessage.success('Comment submitted successfully')
		commentContent.value = ''
		replyEditorId.value = -2
		editorId.value = -2

		const visibleIndex = replyListVisibleIndex.value
		await loadComments()
		if (visibleIndex >= 0) {
			const comment = commentList.value[visibleIndex]
			await loadReplies(comment?.comment_id)
		}
		resetReplyTargets()
	} catch {
		ElMessage.error('Please log in to submit a comment')
	}
}

onMounted(() => {
	loadComments()
})
</script>

<style scoped>
.comment {
	margin-top: 10px;
	margin-left: 20px;
}

.self-user-row {
	display: flex;
	align-items: center;
	gap: 8px;
	margin-bottom: 12px;
}

.comment-footer {
	min-height: 30px;
	line-height: 30px;
	display: flex;
	gap: 20px;
}

.comment-footer p {
	cursor: pointer;
	font-size: 14px;
	color: #4a5c70;
}

.commentlist {
	list-style: none;
	padding-left: 0;
	margin: 16px 0 0;
}

.commentlist > li {
	padding: 14px 0;
	border-bottom: 1px solid #e7edf6;
}

.commenttop {
	margin-top: 20px;
	font-size: 18px;
	text-align: center;
	padding: 10px;
	background-color: rgb(250, 244, 244);
	cursor: pointer;
}

.touser {
	color: #2f6fdd;
}

.comment-meta-row {
	display: flex;
	align-items: center;
	gap: 8px;
	cursor: pointer;
}

.profile-card p {
	margin: 6px 0;
	font-size: 13px;
	word-break: break-word;
}

.comment-body {
	margin-top: 10px;
	padding: 12px 14px;
	background: #ffffff;
	border: 1px solid #e4ebf5;
	border-radius: 10px;
	line-height: 1.75;
	color: #2f3c48;
	word-break: break-word;
	box-shadow: 0 2px 10px rgba(31, 74, 126, 0.06);
	transition:
		border-color 0.2s ease,
		transform 0.2s ease;
}

.comment-body:hover {
	border-color: #bfd3f6;
	transform: translateY(-1px);
}

.children {
	margin-top: 10px;
	padding-left: 16px;
	border-left: 2px dashed #d6e3f7;
}

.reply-body {
	margin-top: 8px;
	padding: 10px 12px;
	background: linear-gradient(180deg, #f6f9ff 0%, #eef4ff 100%);
	border: 1px solid #d9e6fb;
	border-left: 4px solid #2f6fdd;
	border-radius: 10px;
	box-shadow: none;
}

.reply {
	margin-top: 10px;
}

.empty-note {
	color: #9aa3ab;
	font-size: 13px;
}
</style>
