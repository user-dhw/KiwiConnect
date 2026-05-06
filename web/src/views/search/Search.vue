<template>
	<div class="page-container">
		<div class="container">
			<div class="page-narrow">
				<section class="section-card search-card">
					<div class="page-heading">
						<h1 class="page-title">Search</h1>
						<p class="page-subtitle">
							Your search term has been captured. The search result experience can
							be extended next without affecting the existing backend flow.
						</p>
					</div>

					<div class="search-query-chip">Keyword: {{ searchQuery }}</div>
					<p class="search-placeholder">Search results list</p>
				</section>
			</div>
		</div>
	</div>
</template>

<script setup>
import { useRoute } from 'vue-router'
import { computed } from 'vue'

const route = useRoute()
const searchQuery = computed(() => route.query.search || '')
</script>

<style scoped>
.search-card {
	display: grid;
	gap: 18px;
}

.search-query-chip {
	display: inline-flex;
	align-items: center;
	width: fit-content;
	padding: 10px 14px;
	border-radius: 999px;
	background: #edf3ff;
	color: #2663eb;
	font-weight: 700;
}

.search-placeholder {
	margin: 0;
	color: #667085;
}
</style>
