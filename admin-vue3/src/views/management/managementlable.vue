<template>
  <div class="contentexamine">
    <el-main>
      <div class="top">
        <el-breadcrumb separator="/">
          <el-breadcrumb-item :to="{ path: '/' }">
            Home
          </el-breadcrumb-item>
          <el-breadcrumb-item>Website Management</el-breadcrumb-item>
          <el-breadcrumb-item>Category Management</el-breadcrumb-item>
        </el-breadcrumb>
      </div>

      <div
        v-for="(item, id) in labelList"
        :key="id"
        class="main"
      >
        <h3>{{ item.lable_name }}</h3>
        <div>
          <el-tag
            v-for="tag in item.lable"
            :key="tag"
            effect="dark"
            type="success"
            closable
            :disable-transitions="false"
            @close="handleClose(tag, id)"
          >
            {{ tag }}
          </el-tag>
          <el-input
            v-if="item.inputshow === 1"
            v-model="inputValue"
            class="input-new-tag"
            size="small"
            @keyup.enter="handleInputConfirm(id)"
            @blur="handleInputConfirm(id)"
          />
          <el-button
            v-else
            class="button-new-tag"
            size="small"
            @click="showInput(id)"
          >
            + Add New Category
          </el-button>
        </div>
      </div>
    </el-main>
  </div>
</template>

<script setup>
import { onMounted, ref } from 'vue'
import { ElMessage } from 'element-plus'
import axios from '../../utils/axios'
import { toFormData } from '../../utils/form'

const labelList = ref([])
const inputValue = ref('')

const parseLabelArray = value => {
	if (Array.isArray(value)) return value
	if (typeof value !== 'string' || !value.trim()) return []
	try {
		const parsed = JSON.parse(value)
		return Array.isArray(parsed) ? parsed : []
	} catch {
		return []
	}
}

const handleClose = (tag, id) => {
	const item = labelList.value[id]
	if (!item) return

	const index = item.lable.indexOf(tag)
	if (index >= 0) {
		item.lable.splice(index, 1)
		change(id)
	}
}

const showInput = id => {
	const item = labelList.value[id]
	if (!item) return
	item.inputshow = 1
}

const handleInputConfirm = id => {
	const item = labelList.value[id]
	if (!item) return

	const value = inputValue.value.trim()
	if (value) {
		item.lable.push(value)
		change(id)
	}

	item.inputshow = 0
	inputValue.value = ''
}

const change = async id => {
	const item = labelList.value[id]
	if (!item) return

	const changeLabel = {
		...item,
		lable: JSON.stringify(item.lable),
	}

	try {
		const res = await axios.post(
			'/admin/changelable',
			toFormData(changeLabel),
		)
		if (res.data?.state?.type === 'SUCCESS') {
			ElMessage.success('Updated successfully')
			await lablelist()
			return
		}
		ElMessage.error(res.data?.state?.msg || 'Update failed')
	} catch {
		ElMessage.error('Update failed')
	}
}

const lablelist = async () => {
	try {
		const res = await axios.post(
			'/admin/lablelist',
			toFormData({ lable_name: '' }),
		)
		if (res.data?.state?.type === 'SUCCESS') {
			labelList.value = (res.data.data || []).map(item => ({
				...item,
				lable: parseLabelArray(item.lable),
				inputshow: Number(item.inputshow || 0),
			}))
			return
		}
		ElMessage.error(res.data?.state?.msg || 'Failed to load categories')
	} catch {
		ElMessage.error('Failed to load categories')
	}
}

onMounted(() => {
	lablelist()
})
</script>
<style scoped>
.top {
	position: absolute;
	top: 0;
	left: 0;
	width: calc(100% - 35px);
	padding: 12px 16px;
	background-color: #fff;
}
.contentexamine {
	position: relative;
	width: 100%;
}
.main {
	margin-top: 40px;
	padding: 5px 40px 20px 40px;
	background-color: #fff;
}

.el-tag + .el-tag {
	margin-left: 10px;
}
.button-new-tag {
	margin-left: 10px;
	height: 32px;
	line-height: 30px;
	padding-top: 0;
	padding-bottom: 0;
}
.input-new-tag {
	width: 90px;
	margin-left: 10px;
	vertical-align: bottom;
}
</style>
