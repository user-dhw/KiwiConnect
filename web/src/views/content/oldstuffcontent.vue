<template>
	<div class="help">
		<div class="page-container">
			<div class="container">
				<div class="row">
					<div class="span8 page-content">
						<el-page-header @back="goBack" content="Item Details" />

						<article
							class="type-post format-standard hentry clearfix"
						>
							<div class="oldstuff-detail-layout">
								<div class="oldstuffcontent">
									<img
										:src="content.oldstuff_img"
										alt="Item photo"
									/>
								</div>

								<div class="oldstuffcontent">
									<h3>{{ content.oldstuff_name }}</h3>

									<div class="prize_bar">
										<div class="show_prize fl">
											￥<em>{{
												content.oldstuff_price
											}}</em>
										</div>
										<div style="margin-top: 10px">
											Original price:
											{{ content.oldstuff_price }}
										</div>
										<div
											style="
												margin-top: 10px;
												font-size: 10px;
											"
										>
											Seller info ———————————————
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
															{{
																content.nickname
															}}
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
												<p style="color: #000">
													Account:
												</p>
												<p>{{ content.username }}</p>
											</div>
											<div class="xinxi">
												<p style="color: #000">
													Email:
												</p>
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
												<div class="show_unit fl">
													<a class="iconfont ic"
														>&#xe622;</a
													>
													{{ content.nickname }}
												</div>
											</template>
										</el-popover>

										<div class="show_unit fl">
											<a class="iconfont ic">&#xe66a;</a>
											{{ content.qq }}
										</div>
										<div class="show_unit fl">
											<a class="iconfont ic">&#xe62a;</a>
											{{ content.phone }}
										</div>
									</div>

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
										{{
											isUserVerified
												? 'Interested in buying'
												: 'Verify Account to Buy'
										}}
									</el-button>
									<p
										v-if="!isUserVerified"
										style="
											color: #ff6b6b;
											font-size: 12px;
											margin-top: 8px;
										"
									>
										You must verify your account before
										expressing purchase interest
									</p>

									<el-dialog
										v-model="dialogFormVisible"
										title="Purchase Intent"
										width="30%"
									>
										<el-form
											:model="intentForm"
											size="default"
										>
											<el-form-item label="Contact info">
												<el-input
													autocomplete="off"
													v-model="
														intentForm.describe
													"
												/>
											</el-form-item>
											<el-form-item
												label="Intended price"
											>
												<el-input
													autocomplete="off"
													v-model="intentForm.name"
												/>
											</el-form-item>
										</el-form>

										<template #footer>
											<el-button
												@click="
													dialogFormVisible = false
												"
												>Cancel</el-button
											>
											<el-button
												type="primary"
												:loading="isJoining"
												@click="submitJoin"
											>
												Confirm
											</el-button>
										</template>
									</el-dialog>
								</div>

								<div style="clear: both"></div>
							</div>

							<h3>Item Description</h3>
							<blockquote
								v-html="content.oldstuff_content"
							></blockquote>
						</article>

						<Comment />
					</div>

					<aside class="span4 page-sidebar">
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
		ElMessage.error(
			'You must verify your account to express purchase interest',
		)
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
			res.state?.msg ||
				'You have already submitted an intent for this item',
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
.help {
	min-height: 200px;
}

.label {
	margin-left: 15px;
}

.oldstuff-detail-layout {
	display: grid;
	grid-template-columns: minmax(320px, 1.1fr) minmax(320px, 1fr);
	gap: 20px;
	align-items: start;
}

.oldstuffcontent {
	width: 100%;
	min-height: 200px;
	padding: 0;
}

.oldstuffcontent img {
	max-width: 100%;
	max-height: 420px;
	width: 100%;
	object-fit: contain;
	display: block;
}

.show_prize {
	font-size: 20px;
	color: #ff3e3e;
	padding-left: 20px;
}

.show_unit {
	margin-bottom: 10px;
	height: 50px;
	line-height: 50px;
}

.ic {
	color: #409eff;
	margin-right: 30px;
	font-size: 30px;
}

@media (max-width: 900px) {
	.oldstuff-detail-layout {
		grid-template-columns: 1fr;
		gap: 12px;
	}
}
</style>
