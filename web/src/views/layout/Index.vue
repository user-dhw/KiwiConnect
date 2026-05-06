<template>
	<div class="index">
		<!-- Start of Header -->
		<div class="header-wrapper">
			<header>
				<div class="container">
					<div class="logo-container">
						<!-- Website Logo -->
						<span style="font-size: 35px; color: white"
							>KiwiConnect</span
						>
						<span class="tag-line">Waikato</span>
					</div>
					<!-- Start of Main Navigation -->
					<nav class="main-nav">
						<div class="menu-top-menu-container">
							<ul id="menu-top-menu" class="clearfix">
								<li>
									<router-link to="/">Home</router-link>
								</li>
								<li>
									<router-link to="/help">Q&A</router-link>
								</li>
								<li>
									<router-link to="/activity"
										>Events</router-link
									>
								</li>
								<li>
									<router-link to="/oldstuff"
										>Marketplace</router-link
									>
								</li>
								<li>
									<router-link to="/news"
										>Articles/News</router-link
									>
								</li>

								<li v-if="avatar === ''">
									<a @click="closein" style="cursor: pointer"
										>Sign In / Register</a
									>
								</li>
								<li v-else class="user-menu-item">
									<el-dropdown>
										<a
											class="user-menu-trigger"
											style="
												color: #c1cad1;
												cursor: pointer;
											"
										>
											<img
												v-if="unread === 0"
												style="height: 20px"
												:src="avatar"
												class="avatar touxiang avatar-60 photo"
												height="20"
												width="20"
											/>
											<el-badge
												v-else
												:value="unread"
												class="item"
											>
												<img
													style="height: 20px"
													:src="avatar"
													class="avatar touxiang avatar-60 photo"
													height="20"
													width="20"
												/>
											</el-badge>
											{{ nickname }}
										</a>
										<template #dropdown>
											<el-dropdown-menu>
												<el-dropdown-item>
													<router-link to="/admin"
														>Profile</router-link
													>
												</el-dropdown-item>
												<el-dropdown-item>
													<router-link
														to="/admin/notice"
													>
														<span
															v-if="unread === 0"
															>Notifications</span
														>
														<el-badge
															v-else
															:value="unread"
															class="item"
														>
															<span
																>Notifications</span
															>
														</el-badge>
													</router-link>
												</el-dropdown-item>
												<el-dropdown-item>
													<a
														@click="logout"
														style="cursor: pointer"
														>Sign Out</a
													>
												</el-dropdown-item>
											</el-dropdown-menu>
										</template>
									</el-dropdown>
								</li>
							</ul>
						</div>
						<select
							v-model="selected"
							@change="changeHref(parseInt(selected))"
							class="responsive-nav"
						>
							<option value="1">Home</option>
							<option value="2">Q&A</option>
							<option value="3">Events</option>
							<option value="5">Marketplace</option>
							<option value="6">Sign In / Register</option>
						</select>
					</nav>
					<!-- End of Main Navigation -->
				</div>
			</header>
		</div>
		<!-- End of Header -->
		<!-- Start of Search Wrapper -->
		<div class="search-area-wrapper">
			<div class="search-area container">
				<h3 class="search-header">information exchange</h3>
				<p class="search-tag-line" style="margin-top: 50px">
					Information sharing and communication platform Makes
					information transfer easier
				</p>

				<form class="search-form clearfix" @submit.prevent="onSubmit">
					<input
						class="search-term required"
						type="text"
						v-model="search"
						placeholder="Type your search terms here"
					/>
					<input
						class="search-btn"
						type="submit"
						@click="searchbtn"
						value="Search"
					/>
					<div id="search-error-container"></div>
				</form>
			</div>
		</div>
		<!-- End of Search Wrapper -->
		<router-view />
		<!-- start of foot -->
		<foot />
		<!-- end of foot -->
		<!-- Modal component -->
		<!-- Sign in/Register modal -->
		<LoginRegisterModal />
	</div>
</template>

<script setup>
import { computed, onMounted, ref } from 'vue'
import { useStore } from 'vuex'
import { useRouter } from 'vue-router'
import { ElMessage } from 'element-plus'
import foot from '@/components/foot.vue'
import LoginRegisterModal from '@/components/auth/LoginRegisterModal.vue'
import { getNotice } from '@/api/auth'

const store = useStore()
const router = useRouter()

// Data
const selected = ref(1)
const search = ref('')

// Computed properties
const avatar = computed(() => store.state.user.userinfo?.avatar || '')
const nickname = computed(() => store.state.user.userinfo?.nickname || '')
const unread = computed(() => store.state.user.unread || 0)

// Methods
const changeHref = sortnum => {
	const routes = {
		1: '/',
		2: '/help',
		3: '/activity',
		5: '/oldstuff',
		6: null,
	}

	if (sortnum === 6) {
		store.dispatch('user/close', true)
	} else if (routes[sortnum]) {
		router.push({ path: routes[sortnum] })
	}
}

const logout = () => {
	store.dispatch('user/deleteuserinfo')
	ElMessage.success('Signed out successfully')
}

const closein = () => {
	store.dispatch('user/close', true)
}

const onSubmit = () => {
	return false
}

const searchbtn = () => {
	if (search.value === '') {
		ElMessage.error('Search keyword cannot be empty')
		return
	}
	router.push({
		path: '/search',
		query: { search: search.value },
	})
}

const getNoticeData = async () => {
	try {
		const res = await getNotice({ num: 1 })
		store.dispatch('user/setunread', res.data.count)
	} catch (e) {
		console.error('Failed to fetch notifications:', e)
	}
}

// Lifecycle
onMounted(() => {
	const token = localStorage.getItem('luffy_jwt_token')
	if (token) {
		getNoticeData()
	}
})
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
.header-btn {
	background-color: #2c696d;
	font-size: 14px;
	line-height: 19px;
	font-weight: 600;
	padding: 14px 30px 15px;
	color: #fff;
	display: table;
	margin: 0 auto;
}

#mask {
	position: fixed;
	z-index: 999;
	top: 0;
	left: 0;
	width: 100%;
	height: 100%;
	background: #000;
	filter: alpha(Opacity=30);
	opacity: 0.2;
	margin: 0;
}

#loginBox {
	position: fixed;
	left: 50%;
	top: 50%;
	transform: translate(-50%, -50%);
	z-index: 1000;
	width: 380px;
	height: 330px;
	border: 1px solid #ccc;
	background-color: #fff;
}

#loginBox h2 {
	height: 40px;
	text-align: center;
	line-height: 40px;
	font-size: 14px;
	letter-spacing: 1px;
	color: #666;
	margin: 0 0 20px 0;
	padding: 0;
	border-bottom: 1px solid #ccc;
}

#loginBox h2 img {
	display: block;
	float: right;
	position: relative;
	top: 10px;
	right: 10px;
	cursor: pointer;
}

#loginBox .user,
#loginBox .pass {
	font-size: 14px;
	color: #666;
	padding: 5px 0;
	text-align: center;
}

#loginBox input.text {
	width: 200px;
	height: 25px;
	font-size: 14px;
	border: 1px solid #ccc;
	background-color: #fff;
}

#loginBox .button {
	text-align: center;
	padding: 10px 0;
}

#loginBox input.submit {
	width: 107px;
	height: 30px;
	background-color: rgb(179, 146, 233);
	border: none;
	cursor: pointer;
}

#loginBox .other {
	text-align: right;
	padding: 15px 10px;
	font-size: 14px;
	color: #666;
	cursor: pointer;
}

.iconfont {
	font-size: 20px;
	color: #000;
	position: absolute;
	right: 10px;
	top: 10px;
}

.user-menu-item {
	display: inline-block;
}

.user-menu-trigger {
	display: inline-flex;
	align-items: center;
	gap: 6px;
}
</style>
