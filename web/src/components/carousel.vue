<template>
	<div class="hello">
		<section class="widget">
			<el-carousel :interval="5000" height="250px" v-loading="isLoading">
				<el-carousel-item v-for="item in carouselItems" :key="item.id">
					<div class="carousel-frame">
						<a
							:href="item.url || '#'"
							target="_blank"
							rel="noopener noreferrer"
						>
							<div class="carousel-title">{{ item.title }}</div>
							<img
								:src="item.image"
								:alt="item.title"
								class="carouselimg"
							/>
						</a>
					</div>
				</el-carousel-item>
			</el-carousel>
		</section>
	</div>
</template>

<script setup>
import { onMounted, ref } from 'vue'
import { getCarouselList } from '@/api/content'
import livingRoomImage from '@/assets/images/temp/living-room-770x501.jpg'

const defaultItems = [
	{
		id: 1,
		title: 'Information Exchange Platform',
		image: livingRoomImage,
		url: '#',
	},
	{
		id: 2,
		title: 'Share Together, Grow Together',
		image: livingRoomImage,
		url: '#',
	},
	{
		id: 3,
		title: 'Community Life Assistant',
		image: livingRoomImage,
		url: '#',
	},
]

const carouselItems = ref([...defaultItems])
const isLoading = ref(false)

const apiBaseUrl = import.meta.env.VITE_API_URL || ''

const normalizeImageUrl = value => {
	if (!value) return value
	if (value.startsWith('http://127.0.0.1:3000')) {
		return value.replace('http://127.0.0.1:3000', apiBaseUrl)
	}
	if (value.startsWith('http://localhost:3000')) {
		return value.replace('http://localhost:3000', apiBaseUrl)
	}
	if (value.startsWith('/uplodes/')) {
		return `${apiBaseUrl}${value}`
	}
	return value
}

const normalizeCarouselItems = rows => {
	if (!Array.isArray(rows)) return []

	return rows
		.map((row, idx) => ({
			id: row.carousel_id || idx,
			title: row.carousel_title || 'Information Exchange Platform',
			image: normalizeImageUrl(row.carousel_img) || livingRoomImage,
			url: row.carousel_url || '#',
		}))
		.filter(item => item.image)
}

const loadCarousel = async () => {
	isLoading.value = true
	try {
		const res = await getCarouselList()
		if (res.state?.type === 'SUCCESS') {
			const list = normalizeCarouselItems(res.data)
			carouselItems.value = list.length > 0 ? list : [...defaultItems]
			return
		}

		carouselItems.value = [...defaultItems]
	} catch {
		carouselItems.value = [...defaultItems]
	} finally {
		isLoading.value = false
	}
}

onMounted(() => {
	loadCarousel()
})
</script>

<style scoped>
.carousel-frame {
	position: relative;
	width: 100%;
	height: 250px;
}

.carouselimg {
	width: 100%;
	height: 100%;
	object-fit: cover;
}

.carousel-title {
	position: absolute;
	left: 0;
	bottom: 4px;
	width: 100%;
	height: 30px;
	background-color: aliceblue;
	font-size: 18px;
	opacity: 0.45;
	font-weight: 700;
	color: #111;
	line-height: 30px;
	padding-left: 8px;
}
</style>
