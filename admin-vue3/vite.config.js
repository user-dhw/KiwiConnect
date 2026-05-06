import { defineConfig } from 'vite'
import vue from '@vitejs/plugin-vue'
import { fileURLToPath, URL } from 'node:url'
import AutoImport from 'unplugin-auto-import/vite'
import Components from 'unplugin-vue-components/vite'
import { ElementPlusResolver } from 'unplugin-vue-components/resolvers'

// https://vite.dev/config/
export default defineConfig(({ mode }) => ({
	base: mode === 'production' ? '/admin/' : '/',
	plugins: [
		vue(),
		AutoImport({
			imports: ['vue', 'vue-router', 'pinia'],
			resolvers: [ElementPlusResolver()],
			dts: 'src/auto-imports.d.ts',
			eslintrc: {
				enabled: true,
				filepath: './.eslintrc-auto-import.json',
				globalsPropValue: true,
			},
		}),
		Components({
			resolvers: [ElementPlusResolver({ importStyle: 'css' })],
			dts: 'src/components.d.ts',
		}),
	],
	resolve: {
		alias: {
			'@': fileURLToPath(new URL('./src', import.meta.url)),
		},
	},
	server: {
		host: '0.0.0.0',
		port: 8080,
		allowedHosts: true,
		headers: {
			'Access-Control-Allow-Origin': '*',
		},
	},
	build: {
		rollupOptions: {
			output: {
				manualChunks(id) {
					if (id.includes('node_modules')) {
						if (id.includes('element-plus'))
							return 'vendor-element-plus'
						if (id.includes('vue-router'))
							return 'vendor-vue-router'
						if (id.includes('/pinia/')) return 'vendor-pinia'
						if (id.includes('/axios/')) return 'vendor-axios'
						if (id.includes('/qs/')) return 'vendor-qs'
						if (id.includes('/dayjs/')) return 'vendor-dayjs'
						if (id.includes('/vue/')) return 'vendor-vue'
					}
				},
			},
		},
	},
}))
