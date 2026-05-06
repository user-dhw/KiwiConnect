<template>
  <div class="contentexamine">
    <el-main>
      <div class="top">
        <el-breadcrumb separator="/">
          <el-breadcrumb-item :to="{ path: '/' }">
            Home
          </el-breadcrumb-item>
          <el-breadcrumb-item>Data Center</el-breadcrumb-item>
        </el-breadcrumb>
      </div>
      <div>
        <div class="main card">
          <p>Content Items</p>
          <h2 style="margin-top: -7px">
            {{ contentCount }}
          </h2>
        </div>
        <div
          class="card"
          style="width: 2%; height: 20px"
        />
        <div class="main card">
          <p>Comments</p>
          <h2 style="margin-top: -7px">
            {{ commentCount }}
          </h2>
        </div>
        <div
          class="card"
          style="width: 2%; height: 20px"
        />
        <div class="main card">
          <p>Users</p>
          <h2 style="margin-top: -7px">
            {{ userCount }}
          </h2>
        </div>
        <div style="clear: both" />
      </div>

      <div class="main tags">
        <el-tabs v-model="activeTab">
          <el-tab-pane
            label="New User Trend"
            name="first"
          >
            <div
              ref="userTrendChartRef"
              class="chart-box trend-chart-box"
            />
          </el-tab-pane>
          <el-tab-pane
            label="Content Data"
            name="second"
          >
            <div style="overflow: auto">
              <div style="float: left; width: 55%; height: 400px">
                <div
                  ref="contentBarChartRef"
                  class="chart-box"
                />
              </div>
              <div style="float: left; width: 40%; height: 400px">
                <div
                  ref="contentPieChartRef"
                  class="chart-box"
                />
              </div>
            </div>
          </el-tab-pane>
        </el-tabs>
      </div>
    </el-main>
  </div>
</template>

<script setup>
import { nextTick, onBeforeUnmount, onMounted, ref, watch } from 'vue'
import { ElMessage } from 'element-plus'
import axios from '@/utils/axios'

const commentCount = ref(0)
const contentCount = ref(0)
const userCount = ref(0)
const activeTab = ref('first')
const userTrendChartRef = ref(null)
const contentBarChartRef = ref(null)
const contentPieChartRef = ref(null)

let userTrendChart = null
let contentBarChart = null
let contentPieChart = null
let echartsRef = null
let hasTrendModules = false
let hasContentModules = false

const loadTrendEcharts = async () => {
	if (echartsRef) {
		if (hasTrendModules) return echartsRef
	} else {
		echartsRef = await import('echarts/core')
	}

	const [renderers, charts, components] = await Promise.all([
		import('echarts/renderers'),
		import('echarts/charts'),
		import('echarts/components'),
	])

	const { CanvasRenderer } = renderers
	const { LineChart } = charts
	const { GridComponent, TooltipComponent } = components

	echartsRef.use([CanvasRenderer, LineChart, GridComponent, TooltipComponent])
	hasTrendModules = true

	return echartsRef
}

const loadContentEcharts = async () => {
	const echarts = await loadTrendEcharts()
	if (hasContentModules) {
		return echarts
	}

	const [charts, components] = await Promise.all([
		import('echarts/charts'),
		import('echarts/components'),
	])

	const { BarChart, PieChart } = charts
	const { LegendComponent, TitleComponent } = components

	echarts.use([BarChart, PieChart, LegendComponent, TitleComponent])
	hasContentModules = true

	return echarts
}

const contentBarOptions = ref({
	color: ['#3398DB'],
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
			data: [],
			axisTick: { alignWithLabel: true },
		},
	],
	yAxis: [{ type: 'value' }],
	series: [
		{
			name: 'Content Count',
			type: 'bar',
			barWidth: '60%',
			data: [],
		},
	],
})

const contentPieOptions = ref({
	title: {
		text: 'Content Distribution',
		subtext: 'By Type',
		left: 'center',
	},
	legend: {
		orient: 'vertical',
		left: 'left',
		data: [],
	},
	series: [
		{
			name: 'Content Type',
			type: 'pie',
			radius: '55%',
			center: ['50%', '60%'],
			data: [],
			emphasis: {
				itemStyle: {
					shadowBlur: 10,
					shadowOffsetX: 0,
					shadowColor: 'rgba(0, 0, 0, 0.5)',
				},
			},
		},
	],
})

const userTrendOptions = ref({
	xAxis: { type: 'category', data: [] },
	yAxis: { type: 'value' },
	series: [{ data: [], type: 'line' }],
})

const ensureUserTrendChart = async () => {
	if (!userTrendChart && userTrendChartRef.value) {
		const echarts = await loadTrendEcharts()
		userTrendChart = echarts.init(userTrendChartRef.value)
	}
}

const ensureContentCharts = async () => {
	if (!contentBarChart && contentBarChartRef.value) {
		const echarts = await loadContentEcharts()
		contentBarChart = echarts.init(contentBarChartRef.value)
	}
	if (!contentPieChart && contentPieChartRef.value) {
		const echarts = await loadContentEcharts()
		contentPieChart = echarts.init(contentPieChartRef.value)
	}
}

const renderCharts = async () => {
	await ensureUserTrendChart()
	userTrendChart?.setOption(userTrendOptions.value, true)

	if (activeTab.value === 'second') {
		await ensureContentCharts()
		contentBarChart?.setOption(contentBarOptions.value, true)
		contentPieChart?.setOption(contentPieOptions.value, true)
	}
}

const resizeCharts = () => {
	userTrendChart?.resize()
	contentBarChart?.resize()
	contentPieChart?.resize()
}

const getPreviousMonth = monthLabel => {
	const [year, month] = monthLabel.split('-')
	const prevMonth =
		month === '01' ? '12' : String(parseInt(month, 10) - 1).padStart(2, '0')
	const prevYear = month === '01' ? String(parseInt(year, 10) - 1) : year
	return `${prevYear}-${prevMonth}`
}

const numbering = async () => {
	try {
		const res = await axios.post('/admin/numbering')
		if (res.data?.state?.type !== 'SUCCESS') {
			ElMessage.error(res.data?.state?.msg || 'Failed to load statistics')
			return
		}

		const responseData = res.data.data || {}
		const userRows = Array.isArray(responseData.user)
			? responseData.user
			: []
		const countRows = Array.isArray(responseData.count)
			? responseData.count
			: []

		const userDataArr = []
		const userTimeArr = []
		userRows.forEach(item => {
			userDataArr.push(item.num)
			userTimeArr.push(item.time)
		})

		if (userDataArr.length === 1 && userTimeArr.length === 1) {
			userTimeArr.unshift(getPreviousMonth(userTimeArr[0]))
			userDataArr.unshift(0)
		}

		if (userDataArr.length === 0) {
			userTimeArr.push('No Data')
			userDataArr.push(0)
		}

		userTrendOptions.value = {
			tooltip: { trigger: 'axis' },
			xAxis: { type: 'category', data: userTimeArr },
			yAxis: { type: 'value', min: 0 },
			series: [
				{
					data: userDataArr,
					type: 'line',
					smooth: true,
					symbol: 'circle',
					symbolSize: 10,
					lineStyle: { width: 4 },
					label: { show: true, position: 'top' },
					areaStyle: { opacity: 0.25 },
				},
			],
		}

		const contentDataArr = []
		const contentNameArr = []
		const contentPieArr = []
		let totalContentCount = 0
		let commentTotalCount = 0

		const contentNameMap = {
			help: 'Q&A',
			activity: 'Activity',
			oldstuff: 'Secondhand',
			article: 'Article News',
		}

		countRows.forEach(item => {
			const tableName = item.tablE_NAME
			const rows = item.tablE_ROWS

			if (contentNameMap[tableName]) {
				const name = contentNameMap[tableName]
				contentDataArr.push(rows)
				contentNameArr.push(name)
				contentPieArr.push({ name, value: rows })
				totalContentCount += rows
			}

			if (tableName === 'comment' || tableName === 'reply') {
				commentTotalCount += rows
			}
		})

		contentCount.value = totalContentCount
		commentCount.value = commentTotalCount
		userCount.value = userRows.reduce((sum, item) => sum + item.num, 0)

		if (contentNameArr.length === 0) {
			contentNameArr.push('No Data')
			contentDataArr.push(0)
			contentPieArr.push({ name: 'No Data', value: 0 })
		}

		contentBarOptions.value = {
			color: ['#3398DB'],
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
					data: contentNameArr,
					axisTick: { alignWithLabel: true },
				},
			],
			yAxis: [{ type: 'value' }],
			series: [
				{
					name: 'Content Count',
					type: 'bar',
					barWidth: '60%',
					data: contentDataArr,
				},
			],
		}

		contentPieOptions.value = {
			title: {
				text: 'Content Distribution',
				subtext: 'By Type',
				left: 'center',
			},
			legend: {
				orient: 'vertical',
				left: 'left',
				data: contentNameArr,
			},
			series: [
				{
					name: 'Content Type',
					type: 'pie',
					radius: '55%',
					center: ['50%', '60%'],
					data: contentPieArr,
					emphasis: {
						itemStyle: {
							shadowBlur: 10,
							shadowOffsetX: 0,
							shadowColor: 'rgba(0, 0, 0, 0.5)',
						},
					},
				},
			],
		}
	} catch {
		ElMessage.error('Failed to load statistics')
	}

	await nextTick()
	await renderCharts()
}

onMounted(() => {
	numbering()
	window.addEventListener('resize', resizeCharts)
})

watch(activeTab, async value => {
	if (value === 'second') {
		await nextTick()
		await renderCharts()
		resizeCharts()
	}
})

onBeforeUnmount(() => {
	window.removeEventListener('resize', resizeCharts)
	userTrendChart?.dispose()
	contentBarChart?.dispose()
	contentPieChart?.dispose()
})
</script>
<style scoped>
.top {
	position: absolute;
	top: 0;
	left: 0;
	width: calc(100% - 35px);
	padding: 12px 16px;
	background-color: #fff;
}
.contentexamine {
	position: relative;
	width: 100%;
}
.main {
	margin-top: 40px;
	padding: 5px 40px 20px 40px;
	background-color: #fff;
}
.card {
	width: 25%;
	float: left;
}

.chart-box {
	width: 100%;
	height: 100%;
	min-height: 400px;
}

.trend-chart-box {
	min-height: 520px;
}
</style>
