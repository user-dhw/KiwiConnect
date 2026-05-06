import { createRouter, createWebHistory } from 'vue-router'

// Lazy load views for code splitting
const Main = () => import('../views/main.vue')
const Login = () => import('../views/login.vue')

// Examine module
const ContentExamine = () => import('../views/examine/contentexamine.vue')
const Comment = () => import('../views/examine/comment.vue')

// User management module
const UserUser = () => import('../views/user/useruser.vue')
const UserAdmin = () => import('../views/user/useradmin.vue')

// Management module
const Carousel = () => import('../views/management/carousel.vue')
const ManagementLabel = () => import('../views/management/managementlable.vue')

// Other modules
const Numbering = () => import('../views/numbering/numbering.vue')
const Myself = () => import('../views/myself/myself.vue')
const Fankui = () => import('../views/kefu/fankui.vue')
const Jubao = () => import('../views/kefu/jubao.vue')
const Shensu = () => import('../views/kefu/shensu.vue')

// Route definitions
const routes = [
	{
		path: '/login',
		name: 'login',
		component: Login,
		meta: { isPublic: true, title: 'Login' },
	},
	{
		path: '/',
		name: 'main',
		component: Main,
		redirect: '/numbering',
		children: [
			{
				path: 'numbering',
				name: 'numbering',
				component: Numbering,
				meta: { title: 'Numbering' },
			},
			{
				path: 'myself',
				name: 'myself',
				component: Myself,
				meta: { title: 'My Profile' },
			},
			{
				path: 'contentexamine',
				name: 'contentexamine',
				component: ContentExamine,
				meta: { title: 'Content Examine' },
			},
			{
				path: 'comment',
				name: 'comment',
				component: Comment,
				meta: { title: 'Comments' },
			},
			{
				path: 'useruser',
				name: 'useruser',
				component: UserUser,
				meta: { title: 'Users' },
			},
			{
				path: 'useradmin',
				name: 'useradmin',
				component: UserAdmin,
				meta: { title: 'User Admin' },
			},
			{
				path: 'managementlable',
				name: 'managementlable',
				component: ManagementLabel,
				meta: { title: 'Management Label' },
			},
			{
				path: 'carousel',
				name: 'carousel',
				component: Carousel,
				meta: { title: 'Carousel' },
			},
			{
				path: 'fankui',
				name: 'fankui',
				component: Fankui,
				meta: { title: 'Feedback' },
			},
			{
				path: 'jubao',
				name: 'jubao',
				component: Jubao,
				meta: { title: 'Report' },
			},
			{
				path: 'shensu',
				name: 'shensu',
				component: Shensu,
				meta: { title: 'Appeal' },
			},
		],
	},
	{
		path: '/:pathMatch(.*)*',
		redirect: '/',
	},
]

// Create router instance
const router = createRouter({
	history: createWebHistory(),
	routes,
})

// Navigation guard for authentication
router.beforeEach((to, from, next) => {
	const isAuthenticated = !!localStorage.getItem('admin_jwt_token')

	// Set document title
	document.title = to.meta.title || 'Admin Dashboard'

	// Check if route requires authentication
	if (!to.meta.isPublic && !isAuthenticated) {
		return next('/login')
	}

	// Redirect already logged in users from login page
	if (to.path === '/login' && isAuthenticated) {
		return next('/')
	}

	next()
})

export default router
