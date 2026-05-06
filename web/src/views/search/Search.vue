<template>
	<div class="search-page">
		<el-card class="box-card">
			<template #header>
				<span>Search Results: {{ searchQuery }}</span>
			</template>
			<p>Search results list</p>
		</el-card>
	</div>
</template>

<script setup>
import { useRoute } from 'vue-router'
import { computed } from 'vue'

const route = useRoute()
const searchQuery = computed(() => route.query.search || '')
</script>

<style scoped>
.search-page {
	padding: 20px;
}

.box-card {
	width: 100%;
}
</style>
