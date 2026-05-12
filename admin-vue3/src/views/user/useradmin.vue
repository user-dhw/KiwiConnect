<template>
  <div class="useruser">
    <el-main>
      <div class="top">
        <el-breadcrumb separator="/">
          <el-breadcrumb-item :to="{ path: '/' }">
            Home
          </el-breadcrumb-item>
          <el-breadcrumb-item>User Management</el-breadcrumb-item>
          <el-breadcrumb-item>
            Admin User Management
          </el-breadcrumb-item>
        </el-breadcrumb>
      </div>

      <div class="main">
        <div class="search">
          <el-form
            :inline="true"
            :model="filterForm"
            class="demo-form-inline"
          >
            <el-form-item>
              <el-input
                v-model="filterForm.user"
                placeholder="Fuzzy search by account"
              />
            </el-form-item>
            <el-form-item>
              <el-select
                v-model="filterForm.state"
                :teleported="false"
                style="width: 180px"
                placeholder="Account status"
              >
                <el-option
                  label="All"
                  value="all"
                />
                <el-option
                  label="Enabled"
                  value="1"
                />
                <el-option
                  label="Disabled"
                  value="0"
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
            <el-form-item>
              <el-button
                type="primary"
                @click="dialogFormVisibleadd = true"
              >
                Add Account
              </el-button>
            </el-form-item>
          </el-form>
        </div>
        <el-dialog
          v-model="dialogFormVisibleadd"
          title="Add New Account"
          width="500px"
        >
          <el-form>
            <el-form-item
              label="Account"
              label-width="100px"
            >
              <el-input
                v-model="user.username"
                autocomplete="off"
              />
            </el-form-item>
            <el-form-item
              label="Password"
              label-width="100px"
            >
              <el-input
                v-model="user.password"
                type="password"
                autocomplete="off"
              />
            </el-form-item>
            <el-form-item
              label="Confirm Password"
              label-width="100px"
            >
              <el-input
                v-model="user.password1"
                type="password"
                autocomplete="off"
              />
            </el-form-item>
          </el-form>
          <template #footer>
            <div class="dialog-footer">
              <el-button @click="dialogFormVisibleadd = false">
                Cancel
              </el-button>
              <el-button
                type="primary"
                @click="registered"
              >
                Confirm
              </el-button>
            </div>
          </template>
        </el-dialog>

        <el-dialog
          v-model="dialogpw"
          title="Set New Password"
          width="500px"
        >
          <el-form :model="changepassword">
            <el-form-item
              label="Account"
              label-width="100px"
            >
              {{
                changepassword.username
              }}
            </el-form-item>
            <el-form-item
              label="New Password"
              label-width="100px"
            >
              <el-input
                v-model="changepassword.newpassword"
                autocomplete="off"
              />
            </el-form-item>
          </el-form>
          <template #footer>
            <div class="dialog-footer">
              <el-button @click="dialogpw = false">
                Cancel
              </el-button>
              <el-button
                type="primary"
                @click="change"
              >
                Confirm
              </el-button>
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
          <el-table-column
            prop="username"
            label="Account"
          />
          <el-table-column
            prop="nickname"
            label="Nickname"
          />
          <el-table-column
            prop="nickname"
            label="Review Center"
          >
            <template #default="scope">
              <el-switch
                v-model="scope.row.issh"
                active-color="#13ce66"
                inactive-color="#ff4949"
                active-value="1"
                inactive-value="0"
                @change="changestate(scope.row)"
              />
            </template>
          </el-table-column>

          <el-table-column
            prop="nickname"
            label="User Management"
          >
            <template #default="scope">
              <el-switch
                v-model="scope.row.isyh"
                active-color="#13ce66"
                inactive-color="#ff4949"
                active-value="1"
                inactive-value="0"
                @change="changestate(scope.row)"
              />
            </template>
          </el-table-column>

          <el-table-column
            prop="nickname"
            label="Site Management"
          >
            <template #default="scope">
              <el-switch
                v-model="scope.row.isgl"
                active-color="#13ce66"
                inactive-color="#ff4949"
                active-value="1"
                inactive-value="0"
                @change="changestate(scope.row)"
              />
            </template>
          </el-table-column>

          <el-table-column
            prop="nickname"
            label="Feedback Center"
          >
            <template #default="scope">
              <el-switch
                v-model="scope.row.isfk"
                active-color="#13ce66"
                inactive-color="#ff4949"
                active-value="1"
                inactive-value="0"
                @change="changestate(scope.row)"
              />
            </template>
          </el-table-column>

          <el-table-column
            prop="nickname"
            label="Account Status"
          >
            <template #default="scope">
              <el-switch
                v-model="scope.row.user_state"
                active-color="#13ce66"
                inactive-color="#ff4949"
                active-value="1"
                inactive-value="0"
                @change="changestate(scope.row)"
              />
            </template>
          </el-table-column>

          <el-table-column
            prop="nickname"
            fixed="right"
            label="Actions"
            width="200"
          >
            <template #default="scope">
              <el-button
                type="danger"
                link
                size="small"
                :disabled="scope.row.username === 'admin'"
                @click="deleteuser(scope.row)"
              >
                Delete
              </el-button>
              <el-button
                type="primary"
                link
                :disabled="scope.row.username === 'admin'"
                size="small"
                @click="changepw(scope.row)"
              >
                Change Password
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
      </div>
    </el-main>
  </div>
</template>

<script setup>
import { onMounted, reactive, ref } from 'vue'
import { ElMessage } from 'element-plus'
import axios from '../../utils/axios'
import { toFormData } from '../../utils/form'

const changepassword = ref({})
const dialogpw = ref(false)
const loading = ref(false)
const dialogFormVisibleadd = ref(false)
const user = reactive({
	username: '',
	password: '',
	password1: '',
})
const filterForm = reactive({
	user: '',
	state: 'all',
})
const pagelistquery = reactive({
	total: 0,
	page: 1,
	pagesize: 10,
	user: '',
	state: '',
})
const tableData = ref([])

const resetUserForm = () => {
	user.username = ''
	user.password = ''
	user.password1 = ''
}

const registered = async () => {
	const userReg = /^[1-9a-zA-Z]{1}[0-9a-zA-Z]{5,9}$/
	const pwdReg = /^[a-zA-Z]\w{5,17}$/

	if (!userReg.test(user.username)) {
		ElMessage.error('Account must be 6-10 letters and numbers')
		return
	}
	if (!pwdReg.test(user.password)) {
		ElMessage.error(
			'Password must be 6-18 characters and start with a letter',
		)
		return
	}
	if (user.password !== user.password1) {
		ElMessage.error('The two passwords do not match')
		return
	}

	try {
		const res = await axios.post('/admin/registered', toFormData(user))
		const data = res.data
		if (data?.state?.type !== 'SUCCESS') {
			if (data?.state?.type === 'ERROR_PARAMS_EXIST') {
				ElMessage.error('Account name already exists')
			} else {
				ElMessage.error('Failed to add account')
			}
			return
		}

		ElMessage.success('Account added successfully')
		dialogFormVisibleadd.value = false
		resetUserForm()
		await getadminlist()
	} catch {
		ElMessage.error('Failed to add account')
	}
}

const changestate = async row => {
	try {
		const res = await axios.post('/admin/changeadminstate', toFormData(row))
		if (res.data?.state?.type === 'SUCCESS') {
			ElMessage.success('Authorization updated successfully')
			return
		}
		ElMessage.error(
			res.data?.state?.msg || 'Failed to update authorization',
		)
	} catch {
		ElMessage.error('Failed to update authorization')
	}
}

const changepw = row => {
	dialogpw.value = true
	changepassword.value = { ...row, newpassword: '' }
}

const change = async () => {
	try {
		const payload = {
			...changepassword.value,
			type: 'adminadmin',
		}
		const res = await axios.post(
			'/admin/changepassword',
			toFormData(payload),
		)
		if (res.data?.state?.type === 'SUCCESS') {
			ElMessage.success('Updated successfully')
			dialogpw.value = false
			return
		}
		ElMessage.error(res.data?.state?.msg || 'Failed to update password')
	} catch {
		ElMessage.error('Failed to update password')
	}
}

const deleteuser = async row => {
	const data = {
		user_id: row.user_id,
		usertype: 'admin',
	}

	try {
		const res = await axios.post('/admin/deleteuser', toFormData(data))
		if (res.data?.state?.type === 'SUCCESS') {
			ElMessage.success('Deleted successfully')
			await getadminlist()
			return
		}
		ElMessage.error(res.data?.state?.msg || 'Delete failed')
	} catch {
		ElMessage.error('Delete failed')
	}
}

const handleSizeChange = val => {
	pagelistquery.pagesize = val
	getadminlist()
}

const handleCurrentChange = val => {
	pagelistquery.page = val
	getadminlist()
}

const applyFiltersAndSearch = () => {
	pagelistquery.user = filterForm.user
	pagelistquery.state = filterForm.state
	pagelistquery.page = 1
	getadminlist()
}

const getadminlist = async () => {
	loading.value = true
	try {
		const payload = {
			...pagelistquery,
			state: pagelistquery.state === 'all' ? '' : pagelistquery.state,
		}

		const res = await axios.post('/admin/getadminlist', toFormData(payload))
		if (res.data?.state?.type === 'SUCCESS') {
			tableData.value = res.data.data || []
			pagelistquery.total = res.data.count || 0
			return
		}
		ElMessage.error(res.data?.state?.msg || 'Failed to load admin list')
	} catch {
		ElMessage.error('Failed to load admin list')
	} finally {
		loading.value = false
	}
}

onMounted(() => {
	getadminlist()
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
.useruser {
	position: relative;
	width: 100%;
}
.main {
	margin-top: 40px;
	padding: 20px;
	background-color: #fff;
}
</style>
