<template>
  <div class="dashboard-page">
    <el-main>
      <div class="top">
        <el-breadcrumb separator="/">
          <el-breadcrumb-item :to="{ path: '/' }">
            Home
          </el-breadcrumb-item>
          <el-breadcrumb-item>Data Center</el-breadcrumb-item>
        </el-breadcrumb>
      </div>

      <div class="dashboard-body">
        <div class="page-header">
          <h2 class="page-title">Data Center</h2>
          <p class="page-copy">
            Track users, content volume, and discussion activity from one central dashboard.
          </p>
        </div>

        <section class="stats-grid">
          <article class="metric-card">
            <span class="metric-label">Content Items</span>
            <strong class="metric-value">{{ contentCount }}</strong>
            <span class="metric-note">Q&amp;A, events, listings, and articles</span>
          </article>
          <article class="metric-card">
            <span class="metric-label">Comments</span>
            <strong class="metric-value">{{ commentCount }}</strong>
            <span class="metric-note">Community discussions and replies</span>
          </article>
          <article class="metric-card">
            <span class="metric-label">Users</span>
            <strong class="metric-value">{{ userCount }}</strong>
            <span class="metric-note">Registered user accounts</span>
          </article>
        </section>

        <section class="chart-panel">
          <div class="chart-panel__header">
            <div>
              <h2>Insights</h2>
              <p>Switch between growth trends and content distribution without losing chart scale.</p>
            </div>
            <el-button type="primary" plain @click="numbering">
              Refresh data
            </el-button>
          </div>

          <el-tabs v-model="activeTab" class="dashboard-tabs">
            <el-tab-pane label="New User Trend" name="first">
              <div class="chart-card chart-card--single">
                <div ref="userTrendChartRef" class="chart-box trend-chart-box" />
              </div>
            </el-tab-pane>
            <el-tab-pane label="Content Data" name="second">
              <div class="chart-grid">
                <div class="chart-card">
                  <div ref="contentBarChartRef" class="chart-box" />
                </div>
                <div class="chart-card">
                  <div ref="contentPieChartRef" class="chart-box" />
                </div>
              </div>
            </el-tab-pane>
          </el-tabs>
        </section>
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
let resizeObserver = null

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

const syncChartLayout = async () => {
  await nextTick()
  await renderCharts()
  resizeCharts()
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
    const userRows = Array.isArray(responseData.user) ? responseData.user : []
    const countRows = Array.isArray(responseData.count) ? responseData.count : []

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
      oldstuff: 'Marketplace',
      article: 'Articles',
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

  await syncChartLayout()
}

onMounted(() => {
  numbering()
  window.addEventListener('resize', resizeCharts)

  if (typeof ResizeObserver !== 'undefined') {
    resizeObserver = new ResizeObserver(() => {
      resizeCharts()
    })
    if (userTrendChartRef.value) resizeObserver.observe(userTrendChartRef.value)
    if (contentBarChartRef.value) resizeObserver.observe(contentBarChartRef.value)
    if (contentPieChartRef.value) resizeObserver.observe(contentPieChartRef.value)
  }
})

watch(activeTab, async () => {
  await syncChartLayout()
})

onBeforeUnmount(() => {
  window.removeEventListener('resize', resizeCharts)
  resizeObserver?.disconnect()
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

.dashboard-page {
  position: relative;
  width: 100%;
}

.dashboard-body {
  margin-top: 40px;
  padding: 20px;
  background: #fff;
}

.page-header {
  margin-bottom: 20px;
}

.page-title {
  margin: 0;
  font-size: 28px;
  color: #1f2a44;
}

.page-copy {
  max-width: 720px;
  margin: 10px 0 0;
  color: #667085;
  line-height: 1.7;
}

.stats-grid {
  display: grid;
  grid-template-columns: repeat(3, minmax(0, 1fr));
  gap: 18px;
  margin-top: 0;
}

.metric-card {
  display: flex;
  flex-direction: column;
  gap: 8px;
  padding: 24px;
  border: 1px solid #e3edff;
  border-radius: 22px;
  background: #fff;
  box-shadow: 0 14px 34px rgba(15, 23, 42, 0.05);
}

.metric-label {
  font-size: 14px;
  font-weight: 700;
  color: #667085;
  text-transform: uppercase;
  letter-spacing: 0.04em;
}

.metric-value {
  font-size: 38px;
  line-height: 1;
  color: #1f2a44;
}

.metric-note {
  color: #8a94a6;
  line-height: 1.6;
}

.chart-panel {
  margin-top: 22px;
  padding: 0;
  border: 0;
  background: #fff;
  box-shadow: none;
}

.chart-panel__header h2 {
  margin: 0;
  font-size: 24px;
  color: #1f2a44;
}

.chart-panel__header {
  display: flex;
  align-items: flex-start;
  justify-content: space-between;
  gap: 16px;
}

.chart-panel__header p {
  margin: 8px 0 0;
  color: #667085;
}

.dashboard-tabs {
  margin-top: 18px;
}

.chart-grid {
  display: grid;
  grid-template-columns: minmax(0, 1.5fr) minmax(320px, 1fr);
  gap: 18px;
}

.chart-card {
  min-height: 420px;
  padding: 16px;
  border: 1px solid #edf2ff;
  border-radius: 20px;
  background: #f9fbff;
}

.chart-card--single {
  min-height: 540px;
}

.chart-box {
  width: 100%;
  height: 100%;
  min-height: 380px;
}

.trend-chart-box {
  min-height: 500px;
}

@media (max-width: 1200px) {
  .stats-grid,
  .chart-grid {
    grid-template-columns: 1fr;
  }
}

@media (max-width: 768px) {
  .dashboard-body {
    padding: 20px;
  }

  .chart-panel__header {
    flex-direction: column;
  }

  .metric-value {
    font-size: 32px;
  }

  .chart-panel,
  .chart-card {
    padding: 16px;
  }
}
</style>
