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
	padding: 28px 0 36px;
}

.page-container {
	background:
		radial-gradient(circle at top left, rgba(109, 200, 170, 0.12), transparent 26%),
		linear-gradient(180deg, #eef5f2 0%, #f7faf8 38%, #eef4f1 100%);
	padding: 24px 0 40px;
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
	gap: 24px;
	align-items: stretch;
}

.carousel-section {
	margin-bottom: 30px;
	border-radius: 28px;
	overflow: hidden;
	box-shadow: 0 22px 60px rgba(21, 64, 54, 0.16);
}

.content-card {
	background: rgba(255, 255, 255, 0.84);
	border-radius: 24px;
	padding: 22px;
	border: 1px solid rgba(110, 148, 134, 0.14);
	box-shadow: 0 18px 38px rgba(23, 68, 58, 0.08);
	backdrop-filter: blur(10px);
	transition: box-shadow 0.3s ease;
	min-height: 100%;
	height: 100%;
}

.content-card:hover {
	box-shadow: 0 22px 44px rgba(23, 68, 58, 0.12);
}

.widget {
	margin-bottom: 20px;
}

.guomore {
	float: right;
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
