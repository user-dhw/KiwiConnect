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

				<!-- Content Grid - 2 columns -->
				<div class="row">
					<!-- Left Column -->
					<div class="span6 content-col">
						<!-- Help Center -->
						<div class="content-card">
							<Help />
						</div>
						<!-- Old Stuff -->
						<div class="content-card">
							<OldStuff />
						</div>
					</div>

					<!-- Right Column -->
					<div class="span6 content-col">
						<!-- News -->
						<div class="content-card">
							<News />
						</div>
						<!-- Activity -->
						<div class="content-card">
							<Activity />
						</div>
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
	padding: 20px 0;
}

.page-container {
	background: #f5f7fa;
	padding: 20px 0;
}

.container {
	max-width: 1200px;
	margin: 0 auto;
	padding: 0 20px;
}

.row {
	display: flex;
	gap: 20px;
}

.span8 {
	flex: 0 0 66.66%;
}

.span4 {
	flex: 0 0 33.34%;
}

.carousel-section {
	margin-bottom: 30px;
	border-radius: 4px;
	overflow: hidden;
}

.content-col {
	display: flex;
	flex-direction: column;
	gap: 20px;
}

.content-card {
	background: #fff;
	border-radius: 4px;
	padding: 20px;
	box-shadow: 0 1px 3px rgba(0, 0, 0, 0.1);
	transition: box-shadow 0.3s ease;
}

.content-card:hover {
	box-shadow: 0 2px 8px rgba(0, 0, 0, 0.15);
}

.widget {
	margin-bottom: 20px;
}

.guomore {
	float: right;
}

@media (max-width: 768px) {
	.row {
		flex-direction: column;
	}

	.span6 {
		flex: 0 0 100% !important;
	}

	.content-col {
		gap: 15px;
	}

	.content-card {
		padding: 15px;
	}
}
</style>
