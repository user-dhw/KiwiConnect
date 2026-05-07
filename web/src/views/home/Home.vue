<template>
	<div class="home">
		<div v-title data-title="Information Platform | Home"></div>

		<!-- Start of Page Container -->
		<div class="page-container">
			<div class="container">
				<!-- Carousel Section - Full Width -->
				<div class="carousel-section">
					<Carousel />
				</div>

				<!-- Content Grid -->
				<div class="row">
					<div class="content-card">
						<Help />
					</div>
					<div class="content-card">
						<News />
					</div>
					<div class="content-card">
						<OldStuff />
					</div>
					<div class="content-card">
						<Activity />
					</div>
				</div>
			</div>
		</div>
		<!-- End of Page Container -->
	</div>
</template>

<script setup>
import Carousel from '@/components/carousel.vue'
import Activity from '@/components/activity.vue'
import News from '@/components/news.vue'
import Help from '@/components/help.vue'
import OldStuff from '@/components/oldstuff.vue'
</script>

<style scoped>
.home {
	padding: 24px 0 32px;
}

.page-container {
	background:
		radial-gradient(circle at top left, rgba(38, 99, 235, 0.08), transparent 26%),
		radial-gradient(circle at top right, rgba(144, 168, 255, 0.08), transparent 24%),
		linear-gradient(180deg, #f4f8ff 0%, #f8fbff 38%, #f1f6ff 100%);
	padding: 20px 0 36px;
}

.container {
	max-width: 1200px;
	margin: 0 auto;
	padding: 0 24px;
}

.row {
	display: grid;
	grid-template-columns: repeat(2, minmax(0, 1fr));
	grid-auto-rows: 1fr;
	gap: 20px;
	align-items: stretch;
}

.carousel-section {
	margin-bottom: 24px;
	border-radius: 28px;
	overflow: hidden;
	box-shadow: 0 24px 60px rgba(38, 99, 235, 0.14);
}

.content-card {
	background: linear-gradient(180deg, rgba(255, 255, 255, 0.96) 0%, #fbfdff 100%);
	border-radius: 26px;
	padding: 22px;
	border: 1px solid rgba(38, 99, 235, 0.1);
	box-shadow: 0 18px 44px rgba(38, 99, 235, 0.08);
	backdrop-filter: blur(14px);
	transition:
		box-shadow 0.3s ease,
		transform 0.3s ease;
	min-height: 100%;
	height: 100%;
}

.content-card:hover {
	transform: translateY(-2px);
	box-shadow: 0 24px 50px rgba(38, 99, 235, 0.12);
}

@media (max-width: 768px) {
	.row {
		grid-template-columns: 1fr;
		gap: 16px;
	}

	.content-card {
		padding: 18px;
		border-radius: 20px;
	}
}
</style>
