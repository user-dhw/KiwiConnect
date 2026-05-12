<template>
  <div class="main">
    <el-container class="layout">
      <el-aside class="aside">
        <div class="aside-top">
          <div
            v-if="!isCollapse"
            class="aside-title"
          >
            Admin Panel
          </div>
          <el-icon
            class="collapse-btn"
            @click="toggleCollapse"
          >
            <Fold v-if="!isCollapse" />
            <Expand v-else />
          </el-icon>
        </div>

        <el-menu
          router
          :default-active="activePath"
          class="el-menu-vertical-demo"
          background-color="#545c64"
          text-color="#fff"
          active-text-color="#ffd04b"
          :collapse="isCollapse"
          :collapse-transition="true"
          :unique-opened="true"
        >
          <template
            v-for="menu in visibleMenus"
            :key="menu.index"
          >
            <el-menu-item
              v-if="menu.type === 'item'"
              :index="menu.index"
            >
              <el-icon><component :is="menu.icon" /></el-icon>
              <template #title>
                {{ menu.title }}
              </template>
            </el-menu-item>

            <el-sub-menu
              v-else
              :index="menu.index"
            >
              <template #title>
                <el-icon><component :is="menu.icon" /></el-icon>
                <span>{{ menu.title }}</span>
              </template>
              <el-menu-item
                v-for="child in menu.children"
                :key="child.index"
                :index="child.index"
                :disabled="child.adminOnly && !isAdmin"
              >
                {{ child.title }}
              </el-menu-item>
            </el-sub-menu>
          </template>
        </el-menu>
      </el-aside>

      <el-container class="content-layout">
        <el-header class="header">
          <div class="header-title">
            Information Platform Data Center
          </div>
          <div class="header-user">
            <span class="nickname">{{ userStore.uinfo.nickname }}</span>
            <span
              class="logout"
              @click="handleLogout"
            >Logout</span>
          </div>
        </el-header>

        <el-main class="main-content">
          <router-view />
        </el-main>
      </el-container>
    </el-container>
  </div>
</template>

<script setup>
import { computed, ref } from 'vue'
import { useRouter } from 'vue-router'
import { useRoute } from 'vue-router'
import { useUserStore } from '@/store/modules/user'
import {
	Expand,
	Fold,
	FolderChecked,
	Message,
	PieChart,
	Setting,
	User,
	UserFilled,
} from '@element-plus/icons-vue'

const router = useRouter()
const route = useRoute()
const userStore = useUserStore()

// State
const isCollapse = ref(true)
const asideWidth = computed(() => (isCollapse.value ? '64px' : '240px'))
const isAdmin = computed(() => userStore.uinfo.username === 'admin')
const activePath = computed(() => route.path)

const menus = [
	{ type: 'item', index: '/', title: 'Data Center', icon: PieChart },
	{
		type: 'item',
		index: '/myself',
		title: 'Personal Center',
		icon: UserFilled,
	},
	{
		type: 'sub',
		index: 'review-center',
		title: 'Review Center',
		icon: FolderChecked,
		permissionKey: 'issh',
		children: [
			{ index: '/contentexamine', title: 'Content Review' },
			{ index: '/comment', title: 'Comment Moderation' },
		],
	},
	{
		type: 'sub',
		index: 'user-management',
		title: 'User Management',
		icon: User,
		permissionKey: 'isyh',
		children: [
			{ index: '/useruser', title: 'User Management' },
			{ index: '/useradmin', title: 'Administrator', adminOnly: true },
		],
	},
	{
		type: 'sub',
		index: 'website-management',
		title: 'Website Management',
		icon: Setting,
		permissionKey: 'isgl',
		children: [
			{ index: '/managementlable', title: 'Label Management' },
			{ index: '/carousel', title: 'Carousel' },
		],
	},
	{
		type: 'sub',
		index: 'feedback-center',
		title: 'Feedback Center',
		icon: Message,
		permissionKey: 'isfk',
		children: [
			{ index: '/fankui', title: 'Feedback' },
			{ index: '/jubao', title: 'Report' },
			{ index: '/shensu', title: 'Appeal' },
		],
	},
]

const hasPermission = (permissionKey) => {
	if (isAdmin.value) {
		return true
	}
	return userStore.uinfo.jurisdiction?.[permissionKey] === '1'
}

const visibleMenus = computed(() => {
	return menus.filter((menu) => {
		if (menu.type === 'item') {
			return true
		}
		return hasPermission(menu.permissionKey)
	})
})

// Methods
const toggleCollapse = () => {
	isCollapse.value = !isCollapse.value
}

const handleLogout = () => {
	userStore.deleteUserInfo()
	router.push('/login')
}
</script>

<style scoped>
.layout {
	height: 100vh;
}

.el-menu-vertical-demo {
	width: 100%;
}

.el-menu {
	border-right: 0;
}

.aside {
	width: v-bind(asideWidth);
	background-color: #545c64;
	color: white;
	flex-shrink: 0;
	overflow-x: hidden;
	overflow-y: auto;
	transition: width 0.28s cubic-bezier(0.4, 0, 0.2, 1);
}


.aside-top {
	height: 56px;
	padding: 0 16px;
	display: flex;
	align-items: center;
	justify-content: space-between;
	box-sizing: border-box;
	color: #fff;
}

.aside-title {
	font-size: 16px;
	font-weight: 600;
	white-space: nowrap;
}

.collapse-btn {
	font-size: 18px;
	cursor: pointer;
}

.content-layout {
	min-width: 0;
}

.header {
	height: 60px;
	padding: 0 24px;
	background-color: #545c64;
	color: #fff;
	display: flex;
	align-items: center;
	justify-content: space-between;
}

.header-title {
	font-size: 20px;
	font-weight: 500;
}

.header-user {
	font-size: 13px;
	display: flex;
	align-items: center;
	gap: 18px;
}

.nickname {
	opacity: 0.92;
}

.logout {
	cursor: pointer;
}

.logout:hover {
	color: #ffd04b;
}

.main-content {
	padding: 0;
	background-color: #f7f7f7;
}

:deep(.el-menu-item),
:deep(.el-sub-menu__title) {
	height: 56px;
	line-height: 56px;
	font-size: 15px;
}

:deep(.el-sub-menu__title) {
	display: flex;
	align-items: center;
	padding-right: 40px;
}

:deep(.el-sub-menu__title span) {
	display: inline-block;
	white-space: nowrap;
}

:deep(.el-sub-menu__icon-arrow) {
	right: 16px;
	margin-top: -3px;
}

:deep(.el-menu--collapse .el-sub-menu__icon-arrow) {
	display: none;
}
</style>
