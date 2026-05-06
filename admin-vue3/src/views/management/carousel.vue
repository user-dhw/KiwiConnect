<template>
  <div class="contentexamine">
    <el-main>
      <div class="top">
        <el-breadcrumb separator="/">
          <el-breadcrumb-item :to="{ path: '/' }">Home</el-breadcrumb-item>
          <el-breadcrumb-item>Website Management</el-breadcrumb-item>
          <el-breadcrumb-item>Carousel Management</el-breadcrumb-item>
        </el-breadcrumb>
      </div>
      <div class="main" style="margin-top: 40px">
        <el-button
          type="primary"
          :icon="Plus"
          style="margin-left: 40px; margin-top: 20px"
          circle
          @click="add"
        />
        <span style="margin-left: 50px; color: red">
          Tip: Fill in the title and click Save to create or update.
        </span>
      </div>
      <div v-for="(item, id) in carouselList" :key="id" class="main" style="position: relative">
        <el-form :model="item" style="margin-top: 40px; width: 80%" label-width="80px">
          <el-form-item label="Image">
            <el-upload
              :action="uploadAction"
              accept=".jpg,.jpeg,.png,.webp,image/jpeg,image/png,image/webp"
              :show-file-list="false"
              :before-upload="beforeUpload"
              :on-success="value => uplogsuccess(value, id)"
              class="avatar-uploader"
            >
              <img v-if="item.carousel_img" :src="item.carousel_img" class="avatar" />
              <el-icon v-else class="avatar-uploader-icon">
                <Plus />
              </el-icon>
            </el-upload>
          </el-form-item>
          <el-form-item label="Title">
            <el-input v-model="item.carousel_title" />
          </el-form-item>
        </el-form>
        <el-button type="success" :icon="Check" class="save" circle @click="changecarousel(id)" />
        <el-button type="danger" :icon="Delete" class="del" circle @click="del(id)" />
      </div>
    </el-main>
  </div>
</template>

<script setup>
  import { computed, onMounted, ref } from 'vue'
  import { ElMessage } from 'element-plus'
  import { Check, Delete, Plus } from '@element-plus/icons-vue'
  import axios from '../../utils/axios'

  const carouselList = ref([])
  const MAX_FILE_SIZE = 5 * 1024 * 1024
  const ALLOWED_EXTENSIONS = new Set(['.jpg', '.jpeg', '.png', '.webp'])
  const ALLOWED_MIME_TYPES = new Set(['image/jpeg', 'image/png', 'image/webp'])

  const uploadAction = computed(() => `${axios.defaults.baseURL}/uplod`)

  const beforeUpload = file => {
    const fileName = file?.name || ''
    const extension = fileName.includes('.')
      ? fileName.slice(fileName.lastIndexOf('.')).toLowerCase()
      : ''
    const mimeType = (file?.type || '').toLowerCase()

    if (!ALLOWED_EXTENSIONS.has(extension)) {
      ElMessage.error('Only JPG, JPEG, PNG, and WEBP files are allowed')
      return false
    }

    if (!ALLOWED_MIME_TYPES.has(mimeType)) {
      ElMessage.error('Invalid image content type')
      return false
    }

    if ((file?.size || 0) > MAX_FILE_SIZE) {
      ElMessage.error('File size cannot exceed 5 MB')
      return false
    }

    return true
  }

  const toFormData = payload => {
    const params = new URLSearchParams()
    Object.entries(payload).forEach(([key, value]) => {
      if (value !== undefined && value !== null) {
        params.append(key, String(value))
      }
    })
    return params
  }

  const isComplete = item => item.carousel_img !== '' && item.carousel_title !== ''

  const changecarousel = async id => {
    const item = carouselList.value[id]
    if (!item) return

    if (!isComplete(item)) {
      if (item.carousel_id === '') {
        carouselList.value.splice(id, 1)
      }
      return
    }

    try {
      const isEdit = item.carousel_id !== ''
      const res = await axios.post('/admin/changecarousel', toFormData(item))
      if (res.data?.state?.type === 'SUCCESS') {
        ElMessage.success(isEdit ? 'Updated successfully' : 'Added successfully')
        await carousellist()
        return
      }
      ElMessage.error(res.data?.state?.msg || 'Save failed')
    } catch {
      ElMessage.error('Save failed')
    }
  }

  const del = id => {
    const item = carouselList.value[id]
    if (!item) return

    if (isComplete(item) && item.carousel_id !== '') {
      deletecarouse(id)
      return
    }

    if (item.carousel_id === '') {
      carouselList.value.splice(id, 1)
    }
  }

  const add = () => {
    carouselList.value.unshift({
      carousel_img: '',
      carousel_id: '',
      carousel_title: '',
    })
  }

  const uplogsuccess = (res, id) => {
    const item = carouselList.value[id]
    if (!item) return

    item.carousel_img = res?.url || ''
  }

  const deletecarouse = async id => {
    const item = carouselList.value[id]
    if (!item) return

    try {
      const res = await axios.post('/admin/deletecarouse', toFormData(item))
      if (res.data?.state?.type === 'SUCCESS') {
        ElMessage.success('Deleted successfully')
        await carousellist()
        return
      }
      ElMessage.error(res.data?.state?.msg || 'Delete failed')
    } catch {
      ElMessage.error('Delete failed')
    }
  }

  const carousellist = async () => {
    try {
      const res = await axios.post('/admin/carousellist', toFormData({ lable_name: '' }))
      if (res.data?.state?.type === 'SUCCESS') {
        carouselList.value = res.data.data || []
        return
      }
      ElMessage.error(res.data?.state?.msg || 'Failed to load carousel list')
    } catch {
      ElMessage.error('Failed to load carousel list')
    }
  }

  onMounted(() => {
    carousellist()
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
    margin-top: 20px;
    padding: 5px 40px 20px 40px;
    background-color: #fff;
  }
  .avatar-uploader .el-upload {
    border: 1px dashed #d9d9d9;
    border-radius: 6px;
    cursor: pointer;
    position: relative;
    overflow: hidden;
  }
  .avatar-uploader .el-upload:hover {
    border-color: #409eff;
  }
  .avatar-uploader-icon {
    font-size: 28px;
    color: #8c939d;
    width: 100px;
    height: 100px;
    line-height: 178px;
    text-align: center;
  }
  .avatar {
    min-width: 100px;
    height: 100px;
    display: block;
  }
  .del {
    position: absolute;

    right: 100px;
    top: 100px;
  }
  .save {
    position: absolute;
    right: 150px;
    top: 100px;
  }
</style>
