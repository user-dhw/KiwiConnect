<template>
	<div class="help">
		<section class="hub-section">
			<router-link to="/oldstuff" custom v-slot="{ navigate }">
				<div class="section-header" @click="navigate">
					<div>
						<p class="section-eyebrow">Market Board</p>
						<h3 class="section-title">Second-hand Trading</h3>
					</div>
					<h4 class="section-more">More</h4>
				</div>
			</router-link>

			<div class="hub-list" v-loading="isLoading">
				<div
					class="hub-item oldstuff-item"
					v-for="(oldstuff, id) in tableData"
					:key="oldstuff.oldstuff_id || id"
				>
					<router-link :to="`/oldstuffcontent/${oldstuff.oldstuff_id}`" class="oldstuff-link">
						<div class="oldstuff-thumb">
							<img alt="Item photo" :src="oldstuff.oldstuff_img" />
						</div>
						<div class="oldstuff-copy">
							<div class="hub-item-top">
								<span class="hub-badge">Listing</span>
								<span class="oldstuff-price">￥{{ oldstuff.oldstuff_price }}</span>
							</div>
							<h4 class="hub-item-title">{{ oldstuff.oldstuff_name }}</h4>
							<p class="oldstuff-note">Student marketplace pick</p>
						</div>
					</router-link>
				</div>

				<div
					v-if="!isLoading && tableData.length === 0"
					class="hub-item empty-state"
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
	pagesize: 2,
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

.hub-section {
	height: 100%;
	display: grid;
	grid-template-rows: auto 1fr;
}

.hub-list {
	display: grid;
	gap: 12px;
	align-content: start;
}

.hub-item {
	display: grid;
	gap: 10px;
	padding: 16px;
	border-radius: 18px;
	background: linear-gradient(180deg, #ffffff 0%, #f8fbff 100%);
	border: 1px solid rgba(38, 99, 235, 0.1);
	box-shadow: 0 8px 18px rgba(38, 99, 235, 0.05);
}

.oldstuff-link {
	display: grid;
	grid-template-columns: 92px minmax(0, 1fr);
	gap: 14px;
	align-items: center;
	color: inherit;
	text-decoration: none;
}

.oldstuff-thumb {
	width: 92px;
	height: 92px;
	border-radius: 18px;
	overflow: hidden;
	background: #eef4ff;
}

.oldstuff-thumb img {
	width: 100%;
	height: 100%;
	object-fit: cover;
	display: block;
}

.oldstuff-copy {
	display: grid;
	gap: 10px;
	min-width: 0;
}

.oldstuff-price {
	font-size: 0.95rem;
	font-weight: 800;
	color: #eb3b3b;
}

.oldstuff-note {
	margin: 0;
	font-size: 0.85rem;
	color: #7a869f;
}

.section-header {
	display: flex;
	align-items: flex-start;
	justify-content: space-between;
	gap: 12px;
	cursor: pointer;
	margin-bottom: 16px;
}

.section-eyebrow {
	margin: 0 0 8px;
	font-size: 0.74rem;
	font-weight: 700;
	letter-spacing: 0.12em;
	text-transform: uppercase;
	color: #7f93bb;
}

.section-title {
	margin: 0;
	line-height: 1.25;
	font-size: 1.02rem;
}

.section-more {
	margin: 0;
	font-size: 13px;
	font-weight: 700;
	color: #6281bf;
	white-space: nowrap;
	transition:
		color 0.2s ease,
		transform 0.2s ease;
}

.section-header:hover .section-more {
	color: #2663eb;
	transform: translateX(2px);
}

.hub-item-top {
	display: flex;
	align-items: center;
	justify-content: space-between;
	gap: 10px;
}

.hub-badge {
	display: inline-flex;
	align-items: center;
	padding: 6px 10px;
	border-radius: 999px;
	background: rgba(38, 99, 235, 0.1);
	color: #2663eb;
	font-size: 0.73rem;
	font-weight: 700;
	letter-spacing: 0.06em;
	text-transform: uppercase;
}

.hub-item-title {
	margin: 0;
	font-size: 1rem;
	line-height: 1.45;
	color: #243047;
	word-break: break-word;
}

.empty-state {
	color: #9aa3ab;
	padding: 16px;
	text-align: center;
}

@media (max-width: 768px) {
	.section-more {
		font-size: 13px;
	}

	.oldstuff-link {
		grid-template-columns: 76px minmax(0, 1fr);
	}

	.oldstuff-thumb {
		width: 76px;
		height: 76px;
	}
}
</style>
