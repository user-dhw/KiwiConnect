<template>
	<el-result icon="success" :title="title" :sub-title="description">
		<template #extra>
			<el-button type="primary" @click="onBack">Back</el-button>
		</template>
	</el-result>
</template>

<script setup>
defineProps({
	title: {
		type: String,
		default: 'Submitted successfully',
	},
	description: {
		type: String,
		default: '',
	},
	onBack: {
		type: Function,
		required: true,
	},
})
</script>
