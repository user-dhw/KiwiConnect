<template>
	<div class="hello">
		<section class="widget">
			<el-carousel :interval="5000" height="250px" v-loading="isLoading">
				<el-carousel-item v-for="item in carouselItems" :key="item.id">
					<div class="carousel-frame">
						<div class="carousel-title">{{ item.title }}</div>
						<img
							:src="item.image"
							:alt="item.title"
							class="carouselimg"
						/>
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
	},
	{
		id: 2,
		title: 'Share Together, Grow Together',
		image: livingRoomImage,
	},
	{
		id: 3,
		title: 'Community Life Assistant',
		image: livingRoomImage,
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
	overflow: hidden;
}

.carouselimg {
	width: 100%;
	height: 100%;
	object-fit: cover;
	transform: scale(1.01);
}

.carousel-title {
	position: absolute;
	left: 20px;
	right: 20px;
	bottom: 18px;
	padding: 16px 18px;
	border-radius: 18px;
	background: linear-gradient(135deg, rgba(10, 35, 31, 0.72), rgba(33, 85, 73, 0.48));
	backdrop-filter: blur(10px);
	font-size: 22px;
	font-weight: 700;
	color: #fff;
	line-height: 1.3;
	box-shadow: 0 12px 30px rgba(0, 0, 0, 0.18);
}

@media (max-width: 768px) {
	.carousel-title {
		left: 14px;
		right: 14px;
		bottom: 14px;
		padding: 12px 14px;
		font-size: 18px;
	}
}
</style>
