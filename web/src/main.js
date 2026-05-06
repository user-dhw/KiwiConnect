import { createApp } from 'vue'
import App from './App.vue'
import router from './router'
import store from './store'
import moment from 'moment'
import qs from 'qs'
import './assets/css/bootstrap.css'
import './assets/css/green-skin.css'
import './assets/css/main.css'
import './assets/css/responsive.css'
import './assets/css/ali.css'
import './assets/css/element-plus-fixes.css'
import 'element-plus/es/components/message/style/css'
import 'element-plus/es/components/message-box/style/css'
import 'element-plus/es/components/notification/style/css'
// ECharts
import VueECharts from 'vue-echarts'
import { use } from 'echarts/core'
import { CanvasRenderer } from 'echarts/renderers'
import { BarChart } from 'echarts/charts'
import { GridComponent, TooltipComponent } from 'echarts/components'

use([CanvasRenderer, BarChart, GridComponent, TooltipComponent])

const app = createApp(App)

app.use(store)
app.use(router)

// Register ECharts component
app.component('chart', VueECharts)

// Custom directive: update page title
app.directive('title', {
	mounted(el, binding) {
		if (el.dataset.title) {
			document.title = el.dataset.title
		}
	},
})

// Global properties
app.config.globalProperties.$moment = moment
app.config.globalProperties.$qs = qs

// Global helper: date formatting (replacement for Vue 2 filters)
app.config.globalProperties.$formatDate = (
	datestr,
	pattern = 'YYYY-MM-DD HH:mm',
) => {
	return moment(datestr).format(pattern)
}

app.mount('#app')
