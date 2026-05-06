<template>
	<div class="admin-layout">
		<div v-title data-title="Information Platform | Account Center"></div>
		<div class="admin-wrapper">
			<el-container class="admin-container">
				<el-aside width="220px" class="admin-aside">
					<el-menu
						router
						:default-active="$route.path"
						class="admin-menu"
					>
						<el-menu-item index="/admin">Dashboard</el-menu-item>
						<el-menu-item index="/admin/myself"
							>Account Settings</el-menu-item
						>
						<el-sub-menu index="content-manage">
							<template #title>Content Management</template>
							<el-menu-item index="/admin/createhelplist"
								>Q&amp;A</el-menu-item
							>
							<el-menu-item
								index="/admin/createactivitylist"
								:disabled="userinfo.realstate !== 3"
								>Activities</el-menu-item
							>
							<el-menu-item
								index="/admin/createoldstufflist"
								:disabled="userinfo.realstate !== 3"
								>Marketplace</el-menu-item
							>
							<el-menu-item
								index="/admin/articlelist"
								:disabled="userinfo.realstate !== 3"
								>Articles / News</el-menu-item
							>
						</el-sub-menu>
						<el-menu-item index="/admin/notice"
							>Notifications</el-menu-item
						>
					</el-menu>
				</el-aside>

				<el-main class="admin-main">
					<router-view />
				</el-main>
			</el-container>
		</div>
	</div>
</template>

<script setup>
import { computed } from 'vue'
import { useStore } from 'vuex'

const store = useStore()
const userinfo = computed(() => store.state.user.userinfo || {})
</script>

<style scoped>
.admin-layout {
	padding: 16px 0 24px;
}

.admin-wrapper {
	width: min(1180px, 92vw);
	margin: 0 auto;
}

.admin-container {
	border: 1px solid #e8edf2;
	background: #fff;
	min-height: 700px;
}

.admin-aside {
	border-right: 1px solid #edf1f5;
	background: #ffffff;
}

.admin-main {
	padding: 20px;
}

.admin-menu {
	border-right: none;
}
</style>
