<template>
	<div class="admin-home">
		<div class="page-heading">
			<h1 class="page-title">Dashboard</h1>
			<p class="page-subtitle">
				A quick overview of your profile and content activity across KiwiConnect
				Waikato.
			</p>
		</div>

		<div class="overview-grid">
			<section class="summary-card">
				<div class="user-head">
					<img :src="avatarUrl" @error="handleAvatarError" class="avatar" alt="avatar" />
					<div>
						<h3>{{ form.nickname || 'Nickname not set' }}</h3>
						<p>
							{{ Number(form.realstate) === 3 ? 'Verified User' : 'Unverified User' }}
						</p>
					</div>
				</div>
				<div class="base-info">
					<p><span>Real Name:</span> {{ form.realname || '-' }}</p>
					<p><span>Nick Name:</span> {{ form.nickname || '-' }}</p>
					<p><span>Email:</span> {{ form.mail || '-' }}</p>
					<p><span>Phone:</span> {{ form.phone || '-' }}</p>
					<p><span>Bio:</span> {{ form.synopsis || '-' }}</p>
				</div>
				<el-button type="primary" @click="router.push('/admin/myself')">
					Edit Profile
				</el-button>
			</section>

			<section class="stats-grid">
				<div class="metric-card">
					<span class="metric-label">Q&A</span>
					<strong>{{ countData.help }}</strong>
				</div>
				<div class="metric-card">
					<span class="metric-label">Activities</span>
					<strong>{{ countData.activity }}</strong>
				</div>
				<div class="metric-card">
					<span class="metric-label">Articles</span>
					<strong>{{ countData.article }}</strong>
				</div>
				<div class="metric-card">
					<span class="metric-label">Marketplace</span>
					<strong>{{ countData.oldstuff }}</strong>
				</div>
			</section>
		</div>

		<section class="chart-card">
			<chart style="width: 100%; height: 320px" :option="chartOptions" autoresize />
		</section>
	</div>
</template>

<script setup>
import { computed, onMounted, reactive } from 'vue'
import { useRouter } from 'vue-router'
import { getUser, getUserNumbering } from '@/api/account'

const router = useRouter()
const apiBaseUrl = import.meta.env.VITE_API_URL || '/api'
const defaultAvatar = `${apiBaseUrl}/uplodes/avatar.jpg`

const form = reactive({
	avatar: '',
	nickname: '',
	realstate: 0,
	mail: '',
	synopsis: '',
})

const countData = reactive({
	help: 0,
	activity: 0,
	article: 0,
	oldstuff: 0,
})

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

const avatarUrl = computed(() => normalizeFileUrl(form.avatar) || defaultAvatar)

const handleAvatarError = event => {
	event.target.src = defaultAvatar
}

const chartOptions = computed(() => ({
	color: ['#2663eb'],
	tooltip: {
		trigger: 'axis',
		axisPointer: { type: 'shadow' },
	},
	grid: {
		left: '3%',
		right: '4%',
		bottom: '3%',
		containLabel: true,
	},
	xAxis: [
		{
			type: 'category',
			data: ['Q&A', 'Activities', 'Articles', 'Marketplace'],
			axisTick: { alignWithLabel: true },
		},
	],
	yAxis: [{ type: 'value' }],
	series: [
		{
			name: 'Published Count',
			type: 'bar',
			barWidth: '56%',
			itemStyle: {
				borderRadius: [10, 10, 0, 0],
			},
			data: [countData.help, countData.activity, countData.article, countData.oldstuff],
		},
	],
}))

const loadUser = async () => {
	const res = await getUser()
	if (res.state?.type !== 'SUCCESS') return
	Object.assign(form, {
		...(res.data || {}),
		avatar: normalizeFileUrl(res.data?.avatar || ''),
	})
}

const loadCounts = async () => {
	const res = await getUserNumbering()
	if (res.state?.type !== 'SUCCESS') return
	Object.assign(countData, res.data || {})
}

onMounted(() => {
	loadUser()
	loadCounts()
})
</script>

<style scoped>
.admin-home {
	display: grid;
	gap: 24px;
}

.overview-grid {
	display: grid;
	grid-template-columns: minmax(0, 1fr);
	gap: 24px;
}

.summary-card,
.chart-card,
.metric-card {
	background: linear-gradient(180deg, #ffffff 0%, #fbfcff 100%);
	border: 1px solid rgba(38, 99, 235, 0.12);
	border-radius: 24px;
	box-shadow: 0 14px 34px rgba(38, 99, 235, 0.07);
}

.summary-card {
	padding: 24px;
}

.chart-card {
	padding: 18px 22px;
}

.stats-grid {
	display: grid;
	grid-template-columns: repeat(auto-fit, minmax(160px, 1fr));
	gap: 16px;
}

.metric-card {
	padding: 20px;
}

.metric-label {
	display: block;
	color: #667085;
	font-weight: 700;
	margin-bottom: 12px;
}

.metric-card strong {
	font-size: 2rem;
	line-height: 1;
	letter-spacing: -0.04em;
}

.user-head {
	display: flex;
	align-items: center;
	gap: 14px;
	margin-bottom: 18px;
}

.user-head h3 {
	margin: 0;
	font-size: 1.1rem;
	font-weight: 800;
}

.user-head p {
	margin: 6px 0 0;
	color: #68707d;
}

.avatar {
	width: 64px;
	height: 64px;
	border-radius: 50%;
	object-fit: cover;
	border: 3px solid #edf3ff;
}

.base-info {
	margin-bottom: 18px;
	line-height: 1.9;
	color: #2f3944;
}

.base-info p {
	margin: 0;
}

.base-info span {
	font-weight: 700;
	color: #0f1723;
}

@media (min-width: 980px) {
	.overview-grid {
		grid-template-columns: 340px minmax(0, 1fr);
	}
}
</style>
