<template>
  <div class="fankui">
    <el-main>
      <div class="top">
        <el-breadcrumb separator="/">
          <el-breadcrumb-item :to="{ path: '/' }">
            Home
          </el-breadcrumb-item>
          <el-breadcrumb-item>Support Center</el-breadcrumb-item>
          <el-breadcrumb-item>Account Appeals</el-breadcrumb-item>
        </el-breadcrumb>
      </div>

      <div class="main">
        <section class="workspace-hero">
          <div>
            <p class="workspace-eyebrow">Appeal Workflow</p>
            <h1 class="workspace-title">Account Appeals</h1>
            <p class="workspace-copy">
              Review appeal submissions, trace original report reasons, and keep account recovery decisions organized.
            </p>
          </div>
        </section>

        <section class="workspace-card">
        <div class="search workspace-filters">
          <el-form
            :inline="true"
            :model="filterForm"
            class="demo-form-inline"
          >
            <el-form-item>
              <el-select
                v-model="filterForm.state"
                :teleported="false"
                style="width: 180px"
                placeholder="Account Status"
              >
                <el-option
                  label="All"
                  value="all"
                />
                <el-option
                  label="Processed"
                  value="1"
                />
                <el-option
                  label="Unprocessed"
                  value="0"
                />
                <el-option
                  label="Flagged"
                  value="2"
                />
              </el-select>
            </el-form-item>
            <el-form-item>
              <el-button
                type="primary"
                @click="applyFiltersAndSearch"
              >
                Search
              </el-button>
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
          <el-table-column
            prop="createtime"
            label="Created At"
          >
            <template #default="scope">
              {{
                formatDate(scope.row.shensu_createtime)
              }}
            </template>
          </el-table-column>
          <el-table-column
            prop="shensu_user"
            label="Appeal Account"
          />
          <el-table-column
            prop="shensu_content"
            label="Appeal Content"
          />
          <el-table-column
            prop="shensu_jubao_id"
            label="Ban Reason"
          >
            <template #default="scope">
              <el-button
                type="primary"
                link
                size="small"
                @click="tojubao(scope.row)"
              >
                View Ban Reason
              </el-button>
            </template>
          </el-table-column>

          <el-table-column
            label="Status"
            prop="jubao_state"
          >
            <template #default="scope">
              <span
                v-if="scope.row.shensu_state == 0"
                style="color: #409eff"
              >Unprocessed</span>
              <span
                v-if="scope.row.shensu_state == 1"
                style="color: #6cbb7a"
              >Processed</span>
              <span
                v-if="scope.row.shensu_state == 2"
                style="color: #f60c6c"
              >Flagged</span>
            </template>
          </el-table-column>
          <el-table-column
            prop="admin"
            label="Admin"
          />
          <el-table-column
            prop="nickname"
            fixed="right"
            label="Actions"
            width="200"
          >
            <template #default="scope">
              <el-button
                type="primary"
                link
                size="small"
                @click="touser(scope.row)"
              >
                Open User
              </el-button>
              <el-button
                type="primary"
                link
                size="small"
                @click="changestate(scope.row, 2)"
              >
                Flag
              </el-button>
              <el-button
                type="primary"
                link
                size="small"
                @click="changestate(scope.row, 1)"
              >
                Processed
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
        </section>
      </div>
    </el-main>
  </div>
</template>

<script setup>
import { onMounted, reactive, ref } from 'vue'
import { useRouter } from 'vue-router'
import { ElMessage } from 'element-plus'
import axios from '../../utils/axios'
import { formatDate } from '../../utils/dateFormat'
import { toFormData } from '../../utils/form'

const router = useRouter()

const loading = ref(false)
const tableData = ref([])

const filterForm = reactive({
	state: '0',
})

const pagelistquery = reactive({
	total: 0,
	page: 1,
	pagesize: 10,
	kefu_type: 'shensu',
	state: '0',
	id: '',
})

const changestate = async (row, state) => {
	const data = {
		kefu_id: row.shensu_id,
		kefu_state: state,
		type: 'shensu',
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
		query: { user: row.shensu_user, jubao_id: '' },
	})
}

const tojubao = row => {
	router.push({
		path: '/jubao',
		query: { jubao_id: row.shensu_jubao_id },
	})
}

const deletkefu = async row => {
	try {
		const res = await axios.post(
			'/admin/deletekefu',
			toFormData({ id: row.shensu_id, type: 'shensu' }),
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
			tableData.value = res.data.data || []
			pagelistquery.total = res.data.count || 0
			return
		}
		ElMessage.error(res.data?.state?.msg || 'Failed to load appeals')
	} catch {
		ElMessage.error('Failed to load appeals')
	} finally {
		loading.value = false
	}
}

onMounted(() => {
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
  padding: 24px;
  background-color: #f8fbff;
}

.workspace-hero,
.workspace-card {
  border: 1px solid #e3edff;
  border-radius: 24px;
  background: #fff;
  box-shadow: 0 14px 34px rgba(15, 23, 42, 0.05);
}

.workspace-hero {
  padding: 28px 32px;
  background: linear-gradient(135deg, #ffffff 0%, #edf4ff 100%);
}

.workspace-eyebrow {
  margin: 0 0 8px;
  font-size: 13px;
  font-weight: 700;
  letter-spacing: 0.08em;
  text-transform: uppercase;
  color: #2663eb;
}

.workspace-title {
  margin: 0;
  font-size: 34px;
  line-height: 1.1;
  color: #1f2a44;
}

.workspace-copy {
  max-width: 760px;
  margin: 12px 0 0;
  color: #667085;
  line-height: 1.7;
}

.workspace-card {
  margin-top: 22px;
  padding: 24px;
}

.workspace-filters {
  padding: 18px 20px 2px;
  border: 1px solid #edf2ff;
  border-radius: 20px;
  background: #f9fbff;
}
</style>
