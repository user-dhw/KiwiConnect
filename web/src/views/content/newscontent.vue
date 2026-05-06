<template>
	<div id="newscontent">
		<div v-title data-title="Article Details"></div>

		<div class="page-container">
			<div class="container">
				<div class="row">
					<div class="span8 page-content">
						<article
							class="type-post format-standard hentry clearfix"
						>
							<h1 class="post-title">
								<a>{{ content.article_title }}</a>
							</h1>

							<div class="post-meta clearfix">
								<span class="date">{{
									formatDateTime(content.article_createtime)
								}}</span>
								<span class="category">
									<el-popover
										placement="right"
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
											@click="
												reportUser(content.username)
											"
											style="margin: 10px 150px"
											type="danger"
											plain
										>
											Report
										</el-button>

										<template #reference>
											<a
												href="#"
												title="author"
												@click.prevent
											>
												{{ content.nickname }}
											</a>
										</template>
									</el-popover>
								</span>
								<span class="comments">
									<a href="#" @click.prevent
										>{{ commentnum }} Comments</a
									>
								</span>
							</div>

							<blockquote
								v-html="content.article_content"
							></blockquote>
						</article>

						<Comment />
					</div>

					<aside class="span4 page-sidebar">
						<Carousel />
						<News />
					</aside>
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
import Carousel from '@/components/carousel.vue'
import Comment from '@/components/comment.vue'
import News from '@/components/news.vue'
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
#newscontent {
	min-height: 200px;
}
</style>
