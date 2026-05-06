<template>
	<div class="comment-editor">
		<el-input
			:model-value="modelValue"
			type="textarea"
			:rows="rows"
			:placeholder="placeholder"
			@update:model-value="value => $emit('update:modelValue', value)"
		/>

		<div class="editor-actions">
			<input
				ref="fileInputRef"
				type="file"
				accept=".jpg,.jpeg,.png,.webp"
				class="hidden-input"
				@change="handleUpload"
			/>
			<el-button size="small" @click="triggerFilePicker"
				>Insert Image</el-button
			>
			<el-button type="info" size="small" @click="$emit('submit')"
				>Submit</el-button
			>
		</div>
	</div>
</template>

<script setup>
import { ref } from 'vue'
import { ElMessage } from 'element-plus'
import { uploadCommentImage } from '@/api/comment'

const props = defineProps({
	modelValue: {
		type: String,
		default: '',
	},
	placeholder: {
		type: String,
		default: 'Write a comment...',
	},
	rows: {
		type: Number,
		default: 5,
	},
})

const emit = defineEmits(['update:modelValue', 'submit'])
const fileInputRef = ref(null)

const triggerFilePicker = () => {
	fileInputRef.value?.click()
}

const handleUpload = async event => {
	const file = event.target?.files?.[0]
	if (!file) return

	try {
		const imageUrl = await uploadCommentImage(file)
		const nextValue = `${props.modelValue || ''}<p><img src="${imageUrl}" alt="uploaded-image" /></p>`
		emit('update:modelValue', nextValue)
	} catch (error) {
		ElMessage.error(error.message || 'Failed to upload image')
	} finally {
		event.target.value = ''
	}
}
</script>

<style scoped>
.editor-actions {
	margin-top: 10px;
	display: flex;
	gap: 8px;
}

.hidden-input {
	display: none;
}
</style>
