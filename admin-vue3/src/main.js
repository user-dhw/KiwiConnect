import { createApp } from 'vue'
import App from './App.vue'
import router from './router'
import pinia from './store'

// Utilities
import { setupAxiosInterceptors } from './utils/axios'
import { formatDate } from './utils/dateFormat'

const app = createApp(App)

// Use plugins
app.use(router)
app.use(pinia)

// Setup global utilities
setupAxiosInterceptors(router)

// Global properties
app.config.globalProperties.$filters = {
	dataFormat: formatDate,
}

app.mount('#app')
