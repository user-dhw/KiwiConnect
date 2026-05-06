<template>
	<div class="admin-home">
		<div class="summary-card">
			<div class="user-head">
				<img
					:src="avatarUrl"
					@error="handleAvatarError"
					class="avatar"
					alt="avatar"
				/>
				<div>
					<h3>{{ form.nickname || 'Nickname not set' }}</h3>
					<p>
						{{
							Number(form.realstate) === 3
								? 'Verified User'
								: 'Unverified User'
						}}
					</p>
				</div>
			</div>
			<div class="base-info">
				<p><span>Real Name: </span>{{ form.realname || '-' }}</p>
				<p><span>Nick Name: </span>{{ form.nickname || '-' }}</p>
				<p><span>Email: </span>{{ form.mail || '-' }}</p>
				<p><span>Phone: </span>{{ form.phone || '-' }}</p>
				<p><span>Bio: </span>{{ form.synopsis || '-' }}</p>
			</div>
			<el-button
				type="primary"
				plain
				@click="router.push('/admin/myself')"
				>Edit Profile</el-button
			>
		</div>

		<div class="chart-card">
			<chart
				style="width: 100%; height: 320px"
				:option="chartOptions"
				autoresize
			/>
		</div>
	</div>
</template>

<script setup>
import { computed, onMounted, reactive } from 'vue'
import { useRouter } from 'vue-router'
import { getUser, getUserNumbering } from '@/api/account'

const router = useRouter()
const defaultAvatar =
	'https://images.pexels.com/photos/5263577/pexels-photo-5263577.jpeg'

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

const avatarUrl = computed(() => normalizeFileUrl(form.avatar) || defaultAvatar)

const handleAvatarError = event => {
	event.target.src = defaultAvatar
}

const chartOptions = computed(() => ({
	color: ['#4c8ef7'],
	tooltip: {
		trigger: 'axis',
		axisPointer: {
			type: 'shadow',
		},
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
			barWidth: '60%',
			data: [
				countData.help,
				countData.activity,
				countData.article,
				countData.oldstuff,
			],
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
	display: flex;
	gap: 20px;
	flex-wrap: wrap;
}

.summary-card,
.chart-card {
	background: #fff;
	border: 1px solid #e7ebf0;
	border-radius: 8px;
	padding: 20px;
}

.summary-card {
	width: 320px;
	flex: 0 0 320px;
}

.chart-card {
	flex: 1;
	min-width: 420px;
}

.user-head {
	display: flex;
	align-items: center;
	gap: 12px;
	margin-bottom: 16px;
}

.user-head h3 {
	margin: 0;
	font-size: 18px;
}

.user-head p {
	margin: 6px 0 0;
	color: #68707d;
}

.avatar {
	width: 60px;
	height: 60px;
	border-radius: 50%;
	object-fit: cover;
}

.base-info {
	margin-bottom: 18px;
	line-height: 1.9;
	color: #2f3944;
}

.base-info span {
	font-weight: 600;
	color: #0f1723;
}

@media (max-width: 960px) {
	.summary-card,
	.chart-card {
		width: 100%;
		min-width: 0;
		flex: 1 1 100%;
	}
}
</style>
