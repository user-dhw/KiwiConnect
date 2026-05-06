<template>
	<div class="help">
		<section class="widget">
			<router-link to="/oldstuff" custom v-slot="{ navigate }">
				<div class="page-header section-header" @click="navigate">
					<h3 class="section-title">Second-hand Trading</h3>
					<h4 class="section-more">More &gt;</h4>
				</div>
			</router-link>

			<div class="oldstuff-grid" v-loading="isLoading">
				<div
					class="oldstuff-card"
					v-for="(oldstuff, id) in tableData"
					:key="oldstuff.oldstuff_id || id"
				>
					<router-link
						:to="`/oldstuffcontent/${oldstuff.oldstuff_id}`"
					>
						<div class="thumbnail">
							<img
								data-src="holder.js/100%x200"
								alt="100%x200"
								:src="oldstuff.oldstuff_img"
								data-holder-rendered="true"
								style="
									height: 200px;
									object-fit: cover;
									width: 100%;
									display: block;
								"
							/>
							<div class="caption">
								<h3 style="color: red">
									￥{{ oldstuff.oldstuff_price }}
								</h3>
								<p>{{ oldstuff.oldstuff_name }}</p>
							</div>
						</div>
					</router-link>
				</div>

				<div
					v-if="!isLoading && tableData.length === 0"
					class="empty-state"
				>
					No second-hand listings available
				</div>
			</div>
		</section>
	</div>
</template>

<script setup>
import { onMounted, ref } from 'vue'
import { ElMessage } from 'element-plus'
import { getWebOldStuffList } from '@/api/content'

const pagelistquery = ref({
	lable: '',
	total: 0,
	pagesize: 6,
	page: 1,
})

const tableData = ref([])
const isLoading = ref(false)

const getOldStuffList = async () => {
	isLoading.value = true
	try {
		const res = await getWebOldStuffList(pagelistquery.value)
		if (res.state?.type === 'SUCCESS') {
			tableData.value = Array.isArray(res.data) ? res.data : []
			pagelistquery.value.total = Number(res.count || 0)
			return
		}

		ElMessage.error(res.state?.msg || 'Failed to load second-hand listings')
	} catch {
		ElMessage.error('Failed to load second-hand listings')
	} finally {
		isLoading.value = false
	}
}

onMounted(() => {
	getOldStuffList()
})
</script>

<style scoped>
.help {
	min-height: 200px;
}

.oldstuff-grid {
	display: grid;
	grid-template-columns: repeat(2, 1fr);
	gap: 16px;
	align-items: start;
}

.oldstuff-card {
	min-width: 0;
}

.oldstuff-card .thumbnail {
	margin: 0;
	border-radius: 10px;
	overflow: hidden;
	background: #fff;
	box-shadow: 0 2px 12px rgba(30, 73, 120, 0.08);
	transition:
		transform 0.2s ease,
		box-shadow 0.2s ease;
}

.oldstuff-card .thumbnail:hover {
	transform: translateY(-2px);
	box-shadow: 0 6px 16px rgba(30, 73, 120, 0.13);
}

.oldstuff-card .caption {
	padding: 10px 12px 12px;
}

.oldstuff-card .caption h3 {
	margin: 0 0 6px;
}

.oldstuff-card .caption p {
	margin: 0;
	color: #2f3c48;
	line-height: 1.45;
	word-break: break-word;
}

.section-header {
	display: flex;
	align-items: center;
	justify-content: space-between;
	gap: 12px;
	cursor: pointer;
	margin-bottom: 8px;
}

.section-title {
	margin: 0;
	line-height: 1.25;
}

.section-more {
	margin: 0;
	font-size: 14px;
	font-weight: 500;
	color: #5f6b7a;
	white-space: nowrap;
	transition:
		color 0.2s ease,
		transform 0.2s ease;
}

.section-header:hover .section-more {
	color: #2f6fdd;
	transform: translateX(2px);
}

.empty-state {
	color: #9aa3ab;
	padding: 8px 0;
	text-align: center;
	width: 100%;
	grid-column: 1 / -1;
}

@media (max-width: 768px) {
	.oldstuff-grid {
		grid-template-columns: 1fr;
		gap: 12px;
	}

	.section-more {
		font-size: 13px;
	}
}
</style>
