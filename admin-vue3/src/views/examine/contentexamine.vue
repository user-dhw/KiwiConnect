<template>
  <div id="contentexamine" class="contentexamine">
    <el-main>
      <div class="top">
        <el-breadcrumb separator="/">
          <el-breadcrumb-item :to="{ path: '/' }">Home</el-breadcrumb-item>
          <el-breadcrumb-item>Review Center</el-breadcrumb-item>
          <el-breadcrumb-item>Content Review</el-breadcrumb-item>
        </el-breadcrumb>
      </div>

      <div class="main">
        <div class="page-header">
          <h2 class="page-title">Content Review</h2>
          <p class="page-copy">
            Review submissions across Q&amp;A, activities, marketplace listings, and articles from one moderation workspace.
          </p>
        </div>
        <div class="search">
            <el-form :inline="true" :model="filterForm" class="demo-form-inline">
              <el-form-item>
                <el-input v-model="filterForm.user" placeholder="User" />
              </el-form-item>
              <el-form-item>
                <el-input v-model="filterForm.admin" placeholder="Reviewer" />
              </el-form-item>
              <el-form-item>
                <el-input v-model="filterForm.search" placeholder="Keyword" />
              </el-form-item>
              <el-form-item>
                <el-select
                  v-model="filterForm.state"
                  :teleported="false"
                  style="width: 180px"
                  placeholder="Status"
                >
                  <el-option label="All" value="all" />
                  <el-option label="Pending" value="0" />
                  <el-option label="Approved" value="1" />
                  <el-option label="Rejected" value="-1" />
                </el-select>
              </el-form-item>
              <el-form-item>
                <el-select
                  v-model="filterForm.type"
                  :teleported="false"
                  style="width: 180px"
                  placeholder="Module"
                >
                  <el-option label="Q&A" value="help" />
                  <el-option label="Campus Activity" value="activity" />
                  <el-option label="Marketplace" value="oldstuff" />
                  <el-option label="Article" value="article" />
                </el-select>
              </el-form-item>
              <el-form-item>
                <el-button type="primary" @click="applyFiltersAndSearch">Search</el-button>
              </el-form-item>
            </el-form>
          </div>

          <el-dialog v-model="dialogVisible" title="Details">
            <el-form v-if="activeType === 'help'">
              <el-form-item label="Title">
                {{ currentContent.help_title }}
              </el-form-item>
              <el-form-item label="Category">
                {{ currentContent.help_lable }}
              </el-form-item>
              <el-form-item label="Content">
                <span v-html="currentContent.help_content" />
              </el-form-item>
            </el-form>

            <el-form v-else-if="activeType === 'activity'">
              <el-form-item label="Title">
                {{ currentContent.activity_title }}
              </el-form-item>
              <el-form-item label="Category">
                {{ currentContent.activity_lable }}
              </el-form-item>
              <el-form-item label="Location">
                {{ currentContent.activity_locale }}
              </el-form-item>
              <el-form-item label="Time">
                {{ formatDate(currentContent.activity_statetime) }}
                -
                {{ formatDate(currentContent.activity_endtime) }}
              </el-form-item>
              <el-form-item label="Content">
                <span v-html="currentContent.activity_content" />
              </el-form-item>
            </el-form>

            <el-form v-else-if="activeType === 'job'">
              <el-form-item label="Salary">
                {{ currentContent.job_salary }}
              </el-form-item>
              <el-form-item label="Headcount">
                {{ currentContent.job_num }}
              </el-form-item>
              <el-form-item label="Category">
                {{ currentContent.job_lable }}
              </el-form-item>
              <el-form-item label="Content">
                <span v-html="currentContent.job_content" />
              </el-form-item>
            </el-form>

            <el-form v-else-if="activeType === 'article'">
              <el-form-item label="Title">
                {{ currentContent.article_title }}
              </el-form-item>
              <el-form-item label="Category">
                {{ currentContent.article_lable }}
              </el-form-item>
              <el-form-item label="Introduction">
                {{ currentContent.article_introduction }}
              </el-form-item>
              <el-form-item label="Content">
                <span v-html="currentContent.article_content" />
              </el-form-item>
            </el-form>

            <el-form v-else-if="activeType === 'oldstuff'">
              <el-form-item label="Item">
                <img :src="currentContent.oldstuff_img" alt="Item image" width="300" />
              </el-form-item>
              <el-form-item label="Name">
                {{ currentContent.oldstuff_name }}
              </el-form-item>
              <el-form-item label="Price">
                {{ currentContent.oldstuff_price }}
              </el-form-item>
              <el-form-item label="Category">
                {{ currentContent.oldstuff_lable }}
              </el-form-item>
              <el-form-item label="Description">
                <span v-html="currentContent.oldstuff_content" />
              </el-form-item>
            </el-form>

            <template #footer>
              <div class="dialog-footer">
                <el-button type="primary" @click="changestate(1)">Approve</el-button>
                <el-button type="danger" @click="changestate(-1)">Reject</el-button>
                <el-button @click="dialogVisible = false">Cancel</el-button>
              </div>
            </template>
          </el-dialog>

          <el-table
            v-loading="loading"
            :data="tableData"
            border
            style="width: 100%; min-height: 500px"
            element-loading-text="Loading..."
          >
            <el-table-column fixed prop="createtime" label="Created At">
              <template #default="scope">
                {{ formatDate(getCreatedAt(scope.row)) }}
              </template>
            </el-table-column>

            <el-table-column prop="nickname" label="Author" />

            <el-table-column v-if="activeType === 'help'" prop="help_title" label="Title" />
            <el-table-column v-if="activeType === 'help'" prop="help_lable" label="Category" />

            <el-table-column
              v-if="activeType === 'activity'"
              prop="activity_title"
              label="Activity Title"
            />
            <el-table-column
              v-if="activeType === 'activity'"
              prop="activity_locale"
              label="Location"
            />
            <el-table-column
              v-if="activeType === 'activity'"
              prop="activity_lable"
              label="Category"
            />

            <el-table-column v-if="activeType === 'oldstuff'" label="Image">
              <template #default="scope">
                <img :src="scope.row.oldstuff_img" alt="Item image" height="100" />
              </template>
            </el-table-column>
            <el-table-column v-if="activeType === 'oldstuff'" prop="oldstuff_name" label="Name" />
            <el-table-column
              v-if="activeType === 'oldstuff'"
              prop="oldstuff_lable"
              label="Category"
            />

            <el-table-column v-if="activeType === 'article'" prop="article_title" label="Title" />
            <el-table-column
              v-if="activeType === 'article'"
              prop="article_lable"
              label="Category"
            />

            <el-table-column v-if="activeType === 'job'" prop="job_salary" label="Salary" />
            <el-table-column v-if="activeType === 'job'" prop="job_lable" label="Category" />

            <el-table-column prop="ispublic" label="Status">
              <template #default="scope">
                <span
                  :style="{
                    color: getStatusColor(scope.row.ispublic),
                  }"
                >
                  {{ getStatusText(scope.row.ispublic) }}
                </span>
              </template>
            </el-table-column>

            <el-table-column prop="admin" label="Reviewer" />

            <el-table-column fixed="right" label="Actions" width="170">
              <template #default="scope">
                <el-button type="primary" link size="small" @click="openReview(scope.row)">
                  Review
                </el-button>
                <el-button
                  type="danger"
                  link
                  size="small"
                  :disabled="!isSuperAdmin"
                  @click="del(scope.row)"
                >
                  Delete
                </el-button>
              </template>
            </el-table-column>
          </el-table>

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
  import { computed, onMounted, reactive, ref } from 'vue'
  import { ElMessage } from 'element-plus'
  import { useUserStore } from '../../store/modules/user'
  import axios from '../../utils/axios'
  import { formatDate } from '../../utils/dateFormat'
  import { toFormData } from '../../utils/form'

  const userStore = useUserStore()

  const loading = ref(false)
  const dialogVisible = ref(false)
  const tableData = ref([])
  const currentContent = ref({})

  const filterForm = reactive({
    user: '',
    admin: '',
    state: '0',
    search: '',
    type: 'help',
  })

  const pagelistquery = reactive({
    user: '',
    admin: '',
    state: '0',
    search: '',
    type: 'help',
    total: 0,
    page: 1,
    pagesize: 10,
  })

  const activeType = computed(() => pagelistquery.type)
  const isSuperAdmin = computed(() => userStore.uinfo?.username === 'admin')

  const getStatusText = status => {
    if (status === 1) return 'Approved'
    if (status === -1) return 'Rejected'
    return 'Pending'
  }

  const getStatusColor = status => {
    if (status === 1) return '#6cbb7a'
    if (status === -1) return '#f60c6c'
    return '#409eff'
  }

  const getCreatedAt = row => {
    if (activeType.value === 'activity') return row.activity_statetime
    if (activeType.value === 'article') return row.article_createtime
    return row.createtime
  }

  const getContentId = row => {
    if (activeType.value === 'help') return row.help_id
    if (activeType.value === 'activity') return row.activity_id
    if (activeType.value === 'job') return row.job_id
    if (activeType.value === 'oldstuff') return row.oldstuff_id
    if (activeType.value === 'article') return row.article_id
    return null
  }

  const openReview = row => {
    currentContent.value = row
    dialogVisible.value = true
  }

  const changestate = async state => {
    const id = getContentId(currentContent.value)
    if (!id) {
      ElMessage.error('Invalid content id')
      return
    }

    try {
      const res = await axios.post(
        '/admin/changecontentstate',
        toFormData({ id, type: activeType.value, state })
      )

      if (res.data?.state?.type === 'SUCCESS') {
        ElMessage.success('Operation successful')
        dialogVisible.value = false
        await getcontentlist()
        return
      }

      ElMessage.error(res.data?.state?.msg || 'Operation failed')
    } catch {
      ElMessage.error('Operation failed')
    }
  }

  const del = async row => {
    const id = getContentId(row)
    if (!id) {
      ElMessage.error('Invalid content id')
      return
    }

    try {
      const res = await axios.post('/admin/admindelete', toFormData({ id, type: activeType.value }))
      if (res.data?.state?.type === 'SUCCESS') {
        ElMessage.success('Deleted successfully')
        await getcontentlist()
        return
      }
      ElMessage.error(res.data?.state?.msg || 'Delete failed')
    } catch {
      ElMessage.error('Delete failed')
    }
  }

  const handleSizeChange = val => {
    pagelistquery.pagesize = val
    getcontentlist()
  }

  const handleCurrentChange = val => {
    pagelistquery.page = val
    getcontentlist()
  }

  const applyFiltersAndSearch = () => {
    pagelistquery.user = filterForm.user
    pagelistquery.admin = filterForm.admin
    pagelistquery.search = filterForm.search
    pagelistquery.type = filterForm.type
    pagelistquery.state = filterForm.state
    pagelistquery.page = 1
    getcontentlist()
  }

  const getcontentlist = async () => {
    loading.value = true
    try {
      const payload = {
        ...pagelistquery,
        state: pagelistquery.state === 'all' ? '' : pagelistquery.state,
      }

      const res = await axios.post('/admin/contentexamine', toFormData(payload))

      if (res.data?.state?.type === 'SUCCESS') {
        tableData.value = res.data.data || []
        pagelistquery.total = res.data.count || 0
        return
      }

      ElMessage.error(res.data?.state?.msg || 'Failed to load data')
    } catch {
      ElMessage.error('Failed to load data')
    } finally {
      loading.value = false
    }
  }

  onMounted(() => {
    getcontentlist()
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
    padding: 20px;
    background-color: #fff;
  }

  .page-header {
    margin-bottom: 20px;
  }

  .page-title {
    margin: 0;
    font-size: 28px;
    color: #1f2a44;
  }

  .page-copy {
    max-width: 760px;
    margin: 10px 0 0;
    color: #667085;
    line-height: 1.7;
  }
</style>
