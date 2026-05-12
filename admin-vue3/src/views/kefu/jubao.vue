<template>
  <div class="fankui">
    <el-main>
      <div class="top">
        <el-breadcrumb separator="/">
          <el-breadcrumb-item :to="{ path: '/' }">Home</el-breadcrumb-item>
          <el-breadcrumb-item>Support Center</el-breadcrumb-item>
          <el-breadcrumb-item>Violation Reports</el-breadcrumb-item>
        </el-breadcrumb>
      </div>

      <div class="main">
        <div class="search">
          <el-form :inline="true" :model="filterForm" class="demo-form-inline">
            <el-form-item>
              <el-select
                v-model="filterForm.state"
                :teleported="false"
                style="width: 180px"
                placeholder="Account Status"
              >
                <el-option label="All" value="all" />
                <el-option label="Processed" value="1" />
                <el-option label="Unprocessed" value="0" />
                <el-option label="Flagged" value="2" />
              </el-select>
            </el-form-item>
            <el-form-item>
              <el-button type="primary" @click="applyFiltersAndSearch">Search</el-button>
            </el-form-item>
          </el-form>
        </div>

        <el-table
          v-loading="loading"
          :data="tableData"
          border
          style="width: 100%; min-height: 500px"
          element-loading-text="Loading..."
        >
          <el-table-column prop="createtime" label="Created At">
            <template #default="scope">
              {{ formatDate(scope.row.jubao_createtime) }}
            </template>
          </el-table-column>
          <el-table-column prop="jubao_user" label="Reported Account" />
          <el-table-column prop="jubao_url" label="Violation Link">
            <template #default="scope">
              <a :href="scope.row.jubao_url" target="_blank">Open Link</a>
            </template>
          </el-table-column>
          <el-table-column prop="jubao_content" label="Details">
            <template #default="scope">
              <el-popover placement="right" width="400" trigger="hover">
                <span>
                  <el-form status-icon label-width="100px">
                    <el-form-item label="Report Content">
                      {{ scope.row.jubao_content }}
                    </el-form-item>
                    <el-form-item label="Screenshots">
                      <div>
                        <img
                          v-for="(img, id) in scope.row.jubao_img"
                          :key="id"
                          style="width: 250px; margin-top: 5px"
                          :src="img.url"
                          alt="report screenshot"
                        />
                      </div>
                    </el-form-item>
                    <el-form-item label="Result">{{ scope.row.result }}</el-form-item>
                  </el-form>
                </span>
                <template #reference>
                  <el-button type="primary" link>View Details</el-button>
                </template>
              </el-popover>
            </template>
          </el-table-column>
          <el-table-column label="Status" prop="jubao_state">
            <template #default="scope">
              <span v-if="scope.row.jubao_state == 0" style="color: #409eff">Unprocessed</span>
              <span v-if="scope.row.jubao_state == 1" style="color: #6cbb7a">Processed</span>
              <span v-if="scope.row.jubao_state == 2" style="color: #f60c6c">Flagged</span>
            </template>
          </el-table-column>
          <el-table-column prop="admin" label="Admin" />
          <el-table-column prop="nickname" fixed="right" label="Actions" width="300">
            <template #default="scope">
              <el-button type="primary" link size="small" @click="touser(scope.row)">
                Open User
              </el-button>
              <el-button type="primary" link size="small" @click="changestate(scope.row, 2)">
                Flag
              </el-button>
              <el-button type="primary" link size="small" @click="jieguo(scope.row)">
                Mark Processed
              </el-button>

              <el-button
                type="danger"
                link
                size="small"
                :disabled="scope.row.username == 'admin'"
                @click="deletkefu(scope.row)"
              >
                Delete
              </el-button>
            </template>
          </el-table-column>
        </el-table>
        <el-dialog v-model="dialog" title="Processing Result" width="500px">
          <el-form>
            <el-form-item>
              <el-input v-model="result" type="textarea" />
            </el-form-item>
          </el-form>
          <template #footer>
            <div class="dialog-footer">
              <el-button @click="dialog = false">Cancel</el-button>
              <el-button type="primary" @click="setjieguo">Confirm</el-button>
            </div>
          </template>
        </el-dialog>
        <!-- Pagination -->
        <el-pagination
          :current-page="pagelistquery.page"
          :page-sizes="[10, 20, 50, 100]"
          :page-size="pagelistquery.pagesize"
          layout="total, sizes, prev, pager, next, jumper"
          :total="pagelistquery.total"
          style="margin-top: 20px"
          @size-change="handleSizeChange"
          @current-change="handleCurrentChange"
        />
      </div>
    </el-main>
  </div>
</template>

<script setup>
  import { onMounted, reactive, ref } from 'vue'
  import { useRoute, useRouter } from 'vue-router'
  import { ElMessage } from 'element-plus'
  import axios from '../../utils/axios'
  import { formatDate } from '../../utils/dateFormat'
  import { toFormData } from '../../utils/form'

  const router = useRouter()
  const route = useRoute()

  const result = ref('')
  const dialog = ref(false)
  const loading = ref(false)
  const selectedRow = ref({})
  const tableData = ref([])

  const filterForm = reactive({
    state: '0',
  })

  const pagelistquery = reactive({
    total: 0,
    page: 1,
    pagesize: 10,
    kefu_type: 'jubao',
    state: '0',
    id: '',
  })

  const parseImages = images => {
    if (Array.isArray(images)) return images
    if (typeof images !== 'string' || !images.trim()) return []
    try {
      const parsed = JSON.parse(images)
      return Array.isArray(parsed) ? parsed : []
    } catch {
      return []
    }
  }

  const jieguo = row => {
    selectedRow.value = row
    result.value = row.result || ''
    dialog.value = true
  }

  const setjieguo = async () => {
    const data = {
      jubao_id: selectedRow.value.jubao_id,
      result: result.value,
    }

    try {
      const res = await axios.post('/admin/changresult', toFormData(data))
      if (res.data?.state?.type === 'SUCCESS') {
        ElMessage.success('Saved successfully')
        dialog.value = false
        await kefulist()
        return
      }
      ElMessage.error(res.data?.state?.msg || 'Save failed')
    } catch {
      ElMessage.error('Save failed')
    }
  }

  const changestate = async (row, state) => {
    const data = {
      kefu_id: row.jubao_id,
      kefu_state: state,
      type: 'jubao',
    }

    try {
      const res = await axios.post('/admin/changkefustate', toFormData(data))
      if (res.data?.state?.type === 'SUCCESS') {
        ElMessage.success('Status updated successfully')
        await kefulist()
        return
      }
      ElMessage.error(res.data?.state?.msg || 'Failed to update status')
    } catch {
      ElMessage.error('Failed to update status')
    }
  }

  const touser = row => {
    router.push({
      path: '/useruser',
      query: { user: row.jubao_user, jubao_id: row.jubao_id },
    })
  }

  const deletkefu = async row => {
    try {
      const res = await axios.post(
        '/admin/deletekefu',
        toFormData({ id: row.jubao_id, type: 'jubao' })
      )
      if (res.data?.state?.type === 'SUCCESS') {
        ElMessage.success('Deleted successfully')
        await kefulist()
        return
      }
      ElMessage.error(res.data?.state?.msg || 'Delete failed')
    } catch {
      ElMessage.error('Delete failed')
    }
  }

  const handleSizeChange = val => {
    pagelistquery.pagesize = val
    kefulist()
  }

  const handleCurrentChange = val => {
    pagelistquery.page = val
    kefulist()
  }

  const applyFiltersAndSearch = () => {
    pagelistquery.state = filterForm.state
    pagelistquery.page = 1
    kefulist()
  }

  const kefulist = async () => {
    loading.value = true
    try {
      const payload = {
        ...pagelistquery,
        state: pagelistquery.state === 'all' ? '' : pagelistquery.state,
      }

      const res = await axios.post('/admin/kefullist', toFormData(payload))
      if (res.data?.state?.type === 'SUCCESS') {
        tableData.value = (res.data.data || []).map(item => ({
          ...item,
          jubao_img: parseImages(item.jubao_img),
        }))
        pagelistquery.total = res.data.count || 0
        return
      }
      ElMessage.error(res.data?.state?.msg || 'Failed to load reports')
    } catch {
      ElMessage.error('Failed to load reports')
    } finally {
      loading.value = false
    }
  }

  const shensu = () => {
    const jubaoId = route.query?.jubao_id
    if (jubaoId) {
      pagelistquery.id = String(jubaoId)
      pagelistquery.state = 'all'
      filterForm.state = 'all'
    }
  }

  onMounted(() => {
    shensu()
    kefulist()
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
  .fankui {
    position: relative;
    width: 100%;
  }
  .main {
    margin-top: 40px;
    padding: 20px;
    background-color: #fff;
  }
</style>
