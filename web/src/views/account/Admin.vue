<template>
	<div class="admin-layout">
		<div v-title data-title="Information Platform | Account Center"></div>
		<div class="container">
			<div class="admin-shell">
				<aside class="admin-sidebar">
					<div class="admin-sidebar-head">
						<h1>Account Center</h1>
						<p>Manage your profile, posts, verification, and notifications.</p>
					</div>

					<el-menu router :default-active="$route.path" class="admin-menu">
						<el-menu-item index="/admin">Dashboard</el-menu-item>
						<el-menu-item index="/admin/myself">Account Settings</el-menu-item>
						<el-sub-menu index="content-manage">
							<template #title>Content Management</template>
							<el-menu-item
								index="/admin/createhelplist"
								:disabled="!isVerified"
							>
								Q&amp;A
							</el-menu-item>
							<el-menu-item
								index="/admin/createactivitylist"
								:disabled="!isVerified"
							>
								Activities
							</el-menu-item>
							<el-menu-item
								index="/admin/createoldstufflist"
								:disabled="!isVerified"
							>
								Marketplace
							</el-menu-item>
							<el-menu-item
								index="/admin/articlelist"
								:disabled="!isVerified"
							>
								Articles / News
							</el-menu-item>
						</el-sub-menu>
						<el-menu-item index="/admin/notice">Notifications</el-menu-item>
					</el-menu>
				</aside>

				<main class="admin-main">
					<router-view />
				</main>
			</div>
		</div>
	</div>
</template>

<script setup>
import { computed, onMounted } from 'vue'
import { useStore } from 'vuex'
import { syncCurrentUserProfile } from '@/utils/currentUser'

const store = useStore()
const userinfo = computed(() => store.state.user.userinfo || {})
const isVerified = computed(() => Number(userinfo.value.realstate) === 3)

onMounted(() => {
	syncCurrentUserProfile(store)
})
</script>

<style scoped>
.admin-layout {
	padding: 24px 0 40px;
}

.admin-shell {
	display: grid;
	grid-template-columns: minmax(0, 1fr);
	gap: 24px;
}

.admin-sidebar,
.admin-main {
	background: rgba(255, 255, 255, 0.88);
	border: 1px solid rgba(38, 99, 235, 0.12);
	border-radius: 28px;
	box-shadow: 0 18px 40px rgba(38, 99, 235, 0.08);
	backdrop-filter: blur(12px);
}

.admin-sidebar {
	padding: 20px;
}

.admin-sidebar-head {
	margin-bottom: 18px;
}

.admin-sidebar-head h1 {
	margin: 0 0 8px;
	font-size: 1.4rem;
	font-weight: 800;
	letter-spacing: -0.03em;
}

.admin-sidebar-head p {
	margin: 0;
	color: #667085;
	line-height: 1.65;
}

.admin-main {
	padding: clamp(18px, 3vw, 28px);
	min-height: 720px;
}

.admin-menu {
	border-right: none;
	background: transparent;
}

@media (min-width: 980px) {
	.admin-shell {
		grid-template-columns: 280px minmax(0, 1fr);
		align-items: start;
	}

	.admin-sidebar {
		position: sticky;
		top: 110px;
	}
}
</style>
