import { defineConfig } from 'vite'
import vue from '@vitejs/plugin-vue'
import path from 'path'
import AutoImport from 'unplugin-auto-import/vite'
import Components from 'unplugin-vue-components/vite'
import { ElementPlusResolver } from 'unplugin-vue-components/resolvers'

export default defineConfig(({ mode }) => ({
	plugins: [
		vue(),
		AutoImport({
			imports: ['vue', 'vue-router', 'vuex'],
			resolvers: [ElementPlusResolver()],
			dts: 'src/auto-imports.d.ts',
		}),
		Components({
			resolvers: [ElementPlusResolver({ importStyle: 'css' })],
			dts: 'src/components.d.ts',
		}),
	],
	resolve: {
		alias: {
			'@': path.resolve(__dirname, './src'),
		},
	},
	server: {
		port: 5050,
		strictPort: false,
		open: true,
		proxy: {
			'/api': {
				target: 'http://localhost:3000',
				changeOrigin: true,
				rewrite: path => path.replace(/^\/api/, ''),
			},
		},
	},
	build: {
		outDir: 'dist',
		sourcemap: mode !== 'production',
		rollupOptions: {
			output: {
				manualChunks(id) {
					if (id.includes('node_modules')) {
						if (id.includes('element-plus'))
							return 'vendor-element-plus'
						if (
							id.includes('echarts') ||
							id.includes('vue-echarts')
						)
							return 'vendor-echarts'
						if (id.includes('vue-router'))
							return 'vendor-vue-router'
						if (id.includes('/vuex/')) return 'vendor-vuex'
						if (id.includes('/axios/')) return 'vendor-axios'
						if (id.includes('/moment/')) return 'vendor-moment'
						if (id.includes('/vue-cookies/'))
							return 'vendor-cookies'
						if (id.includes('/qs/')) return 'vendor-qs'
						if (id.includes('/vue/')) return 'vendor-vue'
					}
				},
			},
		},
	},
}))
