import { createRouter, createWebHistory } from 'vue-router'
import Index from '@/views/layout/Index.vue'
import Home from '@/views/home/Home.vue'

const routes = [
	{
		path: '/',
		name: 'Index',
		component: Index,
		children: [
			{
				path: '',
				name: 'HomePage',
				component: Home,
			},
			{
				path: 'help',
				name: 'Help',
				component: () => import('@/views/help/Help.vue'),
			},
			{
				path: 'help/:id',
				name: 'HelpContent',
				component: () => import('@/views/content/helpcentent.vue'),
				props: true,
			},
			{
				path: 'helpcontent/:id',
				name: 'HelpContentLegacy',
				component: () => import('@/views/content/helpcentent.vue'),
				props: true,
			},
			{
				path: 'activity',
				name: 'Activity',
				component: () => import('@/views/activity/Activity.vue'),
			},
			{
				path: 'activity/:id',
				name: 'ActivityContent',
				component: () => import('@/views/content/activitycontent.vue'),
				props: true,
			},
			{
				path: 'activitycontent/:id',
				name: 'ActivityContentLegacy',
				component: () => import('@/views/content/activitycontent.vue'),
				props: true,
			},
			{
				path: 'oldstuff',
				name: 'OldStuff',
				component: () => import('@/views/marketplace/OldStuff.vue'),
			},
			{
				path: 'oldstuff/:id',
				name: 'OldStuffContent',
				component: () => import('@/views/content/oldstuffcontent.vue'),
				props: true,
			},
			{
				path: 'oldstuffcontent/:id',
				name: 'OldStuffContentLegacy',
				component: () => import('@/views/content/oldstuffcontent.vue'),
				props: true,
			},
			{
				path: 'news',
				name: 'News',
				component: () => import('@/views/news/News.vue'),
			},
			{
				path: 'news/:id',
				name: 'ArticleContent',
				component: () => import('@/views/content/newscontent.vue'),
				props: true,
			},
			{
				path: 'articlecontent/:id',
				name: 'ArticleContentLegacy',
				component: () => import('@/views/content/newscontent.vue'),
				props: true,
			},
			{
				path: 'newscontent/:id',
				name: 'NewsContentLegacy',
				component: () => import('@/views/content/newscontent.vue'),
				props: true,
			},
			{
				path: 'search',
				name: 'Search',
				component: () => import('@/views/search/Search.vue'),
			},
			{
				path: 'admin',
				name: 'Admin',
				component: () => import('@/views/account/Admin.vue'),
				children: [
					{
						path: '',
						name: 'AdminHome',
						component: () =>
							import('@/views/account/AdminHome.vue'),
					},
					{
						path: 'myself',
						name: 'AdminMyself',
						component: () => import('@/views/account/Myself.vue'),
					},
					{
						path: 'notice',
						name: 'Notice',
						component: () => import('@/views/account/Notice.vue'),
					},
					{
						path: 'createhelplist',
						name: 'CreateHelpList',
						component: () =>
							import('@/views/account/content/help/createhelplist.vue'),
					},
					{
						path: 'createhelp/:id?',
						name: 'CreateHelp',
						component: () =>
							import('@/views/account/content/help/createhelp.vue'),
					},
					{
						path: 'updatehelp/:id',
						name: 'UpdateHelp',
						component: () =>
							import('@/views/account/content/help/createhelp.vue'),
					},
					{
						path: 'createactivitylist',
						name: 'CreateActivityList',
						component: () =>
							import('@/views/account/content/activity/createactivitylist.vue'),
					},
					{
						path: 'createactivity/:id?',
						name: 'CreateActivity',
						component: () =>
							import('@/views/account/content/activity/createactivity.vue'),
					},
					{
						path: 'updateactivity/:id',
						name: 'UpdateActivity',
						component: () =>
							import('@/views/account/content/activity/createactivity.vue'),
					},
					{
						path: 'createoldstufflist',
						name: 'CreateOldStuffList',
						component: () =>
							import('@/views/account/content/oldstuff/createoldstufflist.vue'),
					},
					{
						path: 'createoldstuff/:id?',
						name: 'CreateOldStuff',
						component: () =>
							import('@/views/account/content/oldstuff/createoldstuff.vue'),
					},
					{
						path: 'updateoldstuff/:id',
						name: 'UpdateOldStuff',
						component: () =>
							import('@/views/account/content/oldstuff/createoldstuff.vue'),
					},
					{
						path: 'articlelist',
						name: 'AdminArticleList',
						component: () =>
							import('@/views/account/content/article/articlelist.vue'),
					},
					{
						path: 'createarticle/:id?',
						name: 'CreateArticle',
						component: () =>
							import('@/views/account/content/article/createarticle.vue'),
					},
					{
						path: 'updataarticle/:id',
						name: 'UpdateArticleLegacyTypo',
						component: () =>
							import('@/views/account/content/article/createarticle.vue'),
					},
					{
						path: 'updatearticle/:id',
						name: 'UpdateArticle',
						component: () =>
							import('@/views/account/content/article/createarticle.vue'),
					},
				],
			},
			{
				path: 'feedback',
				name: 'Feedback',
				component: () => import('@/views/support/Feedback.vue'),
			},
			{
				path: 'appeal',
				name: 'Appeal',
				component: () => import('@/views/support/Appeal.vue'),
			},
			{
				path: 'report',
				name: 'Report',
				component: () => import('@/views/support/Report.vue'),
			},
		],
	},
]

const router = createRouter({
	history: createWebHistory(),
	routes,
})

export default router
