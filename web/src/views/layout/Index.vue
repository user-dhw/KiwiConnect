<template>
	<div class="index">
		<div class="header-wrapper">
			<header class="site-header">
				<div class="container site-header-shell">
					<router-link
						to="/"
						class="logo-container"
						aria-label="KiwiConnect Waikato home"
					>
						<div class="brand-mark">KC</div>
						<div class="brand-copy">
							<span class="brand-title">KiwiConnect</span>
							<span class="tag-line"
								>Waikato student community platform</span
							>
						</div>
					</router-link>
					<button
						class="mobile-menu-button"
						type="button"
						:aria-expanded="mobileMenuOpen ? 'true' : 'false'"
						aria-controls="site-nav-panel"
						aria-label="Toggle navigation menu"
						@click="toggleMobileMenu"
					>
						<span></span>
						<span></span>
						<span></span>
					</button>
					<nav
						id="site-nav-panel"
						:class="['main-nav', { open: mobileMenuOpen }]"
						aria-label="Main navigation"
					>
						<div class="menu-top-menu-container">
							<ul id="menu-top-menu" class="menu-list">
								<li v-for="item in navItems" :key="item.to">
									<router-link
										:to="item.to"
										@click="closeMobileMenu"
									>
										{{ item.label }}
									</router-link>
								</li>
								<li v-if="avatar === ''" class="auth-link">
									<button
										type="button"
										class="nav-cta"
										@click="closein"
									>
										Sign In / Register
									</button>
								</li>
								<li v-else class="user-menu-item">
									<el-dropdown>
										<button
											type="button"
											class="user-menu-trigger"
										>
											<img
												v-if="unread === 0"
												:src="displayAvatar"
												class="avatar touxiang avatar-60 photo header-avatar"
												@error="handleAvatarError"
											/>
											<el-badge
												v-else
												:value="unread"
												class="item"
											>
												<img
													:src="displayAvatar"
													class="avatar touxiang avatar-60 photo header-avatar"
													@error="handleAvatarError"
												/>
											</el-badge>
											<span>{{ nickname }}</span>
										</button>
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
													<button
														type="button"
														class="dropdown-action"
														@click="logout"
													>
														Sign Out
													</button>
												</el-dropdown-item>
											</el-dropdown-menu>
										</template>
									</el-dropdown>
								</li>
							</ul>
						</div>
					</nav>
				</div>
			</header>
		</div>
		<div v-if="showHeroSection" class="search-area-wrapper">
			<div class="search-area container">
				<div class="hero-badge">
					Campus hub for students, clubs and everyday life
				</div>
				<h3 class="search-header">
					A softer, smarter community space for KiwiConnect Waikato
				</h3>
				<p class="search-tag-line">
					Find answers, discover events, share updates, and trade
					second-hand items in one modern student-friendly platform.
				</p>
				<form class="search-form" @submit.prevent="onSubmit">
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
				<div class="hero-topics">
					<span class="hero-topic">Questions & Answers</span>
					<span class="hero-topic">Campus Events</span>
					<span class="hero-topic">Marketplace</span>
					<span class="hero-topic">News & Articles</span>
				</div>
			</div>
		</div>
		<router-view />
		<foot />
		<LoginRegisterModal />
	</div>
</template>

<script setup>
import { computed, onMounted, ref, watch } from 'vue'
import { useStore } from 'vuex'
import { useRoute, useRouter } from 'vue-router'
import { ElMessage } from 'element-plus'
import foot from '@/components/foot.vue'
import LoginRegisterModal from '@/components/auth/LoginRegisterModal.vue'
import { getNotice } from '@/api/auth'
import { getStoredAuthToken, syncCurrentUserProfile } from '@/utils/currentUser'

const store = useStore()
const router = useRouter()
const route = useRoute()

const search = ref('')
const mobileMenuOpen = ref(false)
const showHeroSection = computed(() => route.path === '/')

const navItems = [
	{ label: 'Home', to: '/' },
	{ label: 'Q&A', to: '/help' },
	{ label: 'Events', to: '/activity' },
	{ label: 'Marketplace', to: '/oldstuff' },
	{ label: 'Articles/News', to: '/news' },
]

const avatar = computed(() => store.state.user.userinfo?.avatar || '')
const nickname = computed(() => store.state.user.userinfo?.nickname || '')
const unread = computed(() => store.state.user.unread || 0)
const apiBaseUrl = import.meta.env.VITE_API_URL || '/api'
const defaultAvatar = `${apiBaseUrl}/uplodes/avatar.jpg`

const normalizeAvatar = value => {
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

const displayAvatar = computed(
	() => normalizeAvatar(avatar.value) || defaultAvatar,
)

const toggleMobileMenu = () => {
	mobileMenuOpen.value = !mobileMenuOpen.value
}

const closeMobileMenu = () => {
	mobileMenuOpen.value = false
}

const logout = () => {
	store.dispatch('user/deleteuserinfo')
	closeMobileMenu()
	ElMessage.success('Signed out successfully')
}

const closein = () => {
	closeMobileMenu()
	store.dispatch('user/close', true)
}

const handleAvatarError = event => {
	event.target.src = defaultAvatar
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
	closeMobileMenu()
}

const getNoticeData = async () => {
	try {
		const res = await getNotice({ num: 1 })
		store.dispatch('user/setunread', res.data.count)
	} catch (e) {
		console.error('Failed to fetch notifications:', e)
	}
}

watch(
	() => route.fullPath,
	() => {
		closeMobileMenu()
	},
)

onMounted(() => {
	const token = getStoredAuthToken()
	if (token) {
		syncCurrentUserProfile(store).finally(() => {
			getNoticeData()
		})
		return
	}
	store.dispatch('user/close', true)
})
</script>

<style scoped>
.site-header {
	position: relative;
}

.site-header-shell {
	display: flex;
	align-items: center;
	justify-content: space-between;
	gap: 20px;
	padding-top: 16px;
	padding-bottom: 16px;
}

.logo-container {
	display: flex;
	align-items: center;
	gap: 14px;
	padding: 0;
	flex-shrink: 0;
}

.brand-mark {
	width: 46px;
	height: 46px;
	border-radius: 14px;
	display: grid;
	place-items: center;
	background: linear-gradient(135deg, #72a0ff, #2663eb);
	color: #ffffff;
	font-size: 18px;
	font-weight: 800;
	letter-spacing: 0.08em;
	box-shadow: 0 12px 28px rgba(38, 99, 235, 0.28);
}

.brand-copy {
	display: flex;
	flex-direction: column;
}

.brand-title {
	font-size: 30px;
	line-height: 1;
	font-weight: 800;
	color: #2663eb;
	letter-spacing: -0.03em;
}

.tag-line {
	top: 0;
	font-size: 13px;
	color: #7e8ba8;
}

.mobile-menu-button {
	display: none;
	flex-direction: column;
	justify-content: center;
	gap: 5px;
	width: 46px;
	height: 46px;
	padding: 12px;
	border-radius: 14px;
	background: #f4f7ff;
	border: 1px solid rgba(38, 99, 235, 0.12);
	box-shadow: 0 8px 20px rgba(38, 99, 235, 0.08);
}

.mobile-menu-button span {
	display: block;
	height: 2px;
	border-radius: 999px;
	background: #2663eb;
}

.main-nav {
	display: flex;
	align-items: center;
	margin-left: auto;
}

.menu-list {
	display: flex;
	align-items: center;
	gap: 8px;
	margin: 0;
	padding: 0;
	list-style: none;
}

.menu-list > li > :deep(a),
.nav-cta,
.user-menu-trigger {
	display: inline-flex;
	align-items: center;
	justify-content: center;
	gap: 10px;
	min-height: 44px;
	padding: 10px 16px;
	border-radius: 999px;
	color: #4a5568;
	font-weight: 700;
	transition:
		background-color 0.2s ease,
		color 0.2s ease,
		transform 0.2s ease;
}

.menu-list > li > :deep(a:hover),
.menu-list > li > :deep(a.router-link-active),
.nav-cta:hover,
.user-menu-trigger:hover {
	color: #2663eb;
	background: #edf3ff;
}

.nav-cta {
	background: linear-gradient(135deg, #2663eb, #547ff6);
	color: #fff;
	box-shadow: 0 14px 28px rgba(38, 99, 235, 0.18);
}

.nav-cta:hover {
	color: #fff;
	background: linear-gradient(135deg, #1d4fc2, #2663eb);
}

.dropdown-action {
	width: 100%;
	text-align: left;
	color: inherit;
	cursor: pointer;
}

.hero-badge {
	display: inline-flex;
	align-items: center;
	justify-content: center;
	padding: 8px 16px;
	margin-bottom: 18px;
	border-radius: 999px;
	background: rgba(255, 255, 255, 0.26);
	border: 1px solid rgba(255, 255, 255, 0.3);
	color: #eff4ff;
	font-size: 12px;
	font-weight: 700;
	letter-spacing: 0.08em;
	text-transform: uppercase;
	backdrop-filter: blur(12px);
}

.hero-topics {
	display: flex;
	flex-wrap: wrap;
	justify-content: center;
	gap: 10px;
	margin-top: 22px;
}

.hero-topic {
	padding: 8px 14px;
	border-radius: 999px;
	background: rgba(255, 255, 255, 0.16);
	color: #eff6ff;
	font-size: 13px;
	font-weight: 600;
	border: 1px solid rgba(255, 255, 255, 0.12);
}

.user-menu-item {
	display: flex;
	align-items: center;
}

.user-menu-trigger {
	display: inline-flex;
	align-items: center;
	gap: 8px;
	cursor: pointer;
}

.header-avatar {
	width: 32px;
	height: 32px;
	border-radius: 50%;
	object-fit: cover;
	display: inline-block;
	border: 1px solid rgba(255, 255, 255, 0.3);
}

@media (max-width: 768px) {
	.site-header-shell {
		display: grid;
		grid-template-columns: minmax(0, 1fr) auto;
		align-items: center;
		padding-top: 14px;
		padding-bottom: 14px;
	}

	.logo-container {
		min-width: 0;
	}

	.brand-copy {
		min-width: 0;
	}

	.brand-title {
		font-size: 23px;
	}

	.tag-line {
		font-size: 12px;
	}

	.mobile-menu-button {
		display: inline-flex;
	}

	.main-nav {
		grid-column: 1 / -1;
		margin-left: 0;
		width: 100%;
		display: block;
		max-height: 0;
		overflow: hidden;
		opacity: 0;
		pointer-events: none;
		transition:
			max-height 0.25s ease,
			opacity 0.2s ease,
			margin-top 0.2s ease;
	}

	.main-nav.open {
		max-height: 560px;
		opacity: 1;
		pointer-events: auto;
		margin-top: 14px;
	}

	.menu-top-menu-container,
	.menu-list {
		width: 100%;
	}

	.main-nav > .menu-top-menu-container {
		display: block !important;
	}

	.menu-list {
		flex-direction: column;
		align-items: stretch;
		padding: 14px;
		border-radius: 22px;
		background: rgba(255, 255, 255, 0.95);
		border: 1px solid rgba(38, 99, 235, 0.1);
		box-shadow: 0 16px 40px rgba(38, 99, 235, 0.08);
	}

	.menu-list > li > :deep(a),
	.nav-cta,
	.user-menu-trigger {
		justify-content: flex-start;
		width: 100%;
	}

	.hero-badge {
		margin-top: 8px;
	}
}

@media (max-width: 560px) {
	.brand-mark {
		width: 42px;
		height: 42px;
		border-radius: 12px;
		font-size: 16px;
	}

	.brand-title {
		font-size: 20px;
	}

	.search-header {
		font-size: 2rem;
	}
}
</style>
