<template>
  <div class="myself">
    <el-main>
      <div class="top">
        <el-breadcrumb separator="/">
          <el-breadcrumb-item :to="{ path: '/' }">
            Home
          </el-breadcrumb-item>
          <el-breadcrumb-item>User Management</el-breadcrumb-item>
          <el-breadcrumb-item>Profile Center</el-breadcrumb-item>
        </el-breadcrumb>
      </div>

      <div class="main">
        <div class="user-info clearfix">
          <h4>Account Information</h4>
          <el-divider />
          <ul class="first-userInfo">
            <li class="user-li">
              Account:
              <span
                class="user-name"
                title="726647774@qq.com"
              >{{
                tableData.username
              }}</span>
            </li>
            <li>
              Nickname:
              <span>{{ tableData.nickname }}</span>
              <el-button
                type="primary"
                link
                size="small"
                @click="showNameInput = true"
              >
                Edit Nickname
              </el-button>
              <el-input
                v-if="showNameInput"
                v-model="tableData.nickname"
                @keyup.enter="changeadminuser"
              />
            </li>
            <li>
              Created At:
              <span>{{
                formatDate(tableData.user_createtime)
              }}</span>
            </li>
          </ul>
          <ul>
            <li>
              Account Status:
              <span v-if="String(tableData.user_state) === '1'">Enabled</span>
              <span v-else>Disabled</span>
            </li>
            <li>
              Role:
              <span
                v-if="tableData.username === 'admin'"
                style="margin-left: 20px"
              >Super Admin</span>
              <span
                v-else
                style="margin-left: 20px"
              >Normal Admin</span>
            </li>
            <li>
              Permissions:
              <span
                v-if="
                  String(tableData.issh) === '1' &&
                    tableData.username !== 'admin'
                "
                style="margin-left: 20px"
              >Review Center</span>
              <span
                v-if="
                  String(tableData.isyh) === '1' &&
                    tableData.username !== 'admin'
                "
                style="margin-left: 20px"
              >User Center</span>
              <span
                v-if="
                  String(tableData.isgl) === '1' &&
                    tableData.username !== 'admin'
                "
                style="margin-left: 20px"
              >Platform Management</span>
              <span
                v-if="
                  String(tableData.isfk) === '1' &&
                    tableData.username !== 'admin'
                "
                style="margin-left: 20px"
              >Feedback Center</span>
              <span
                v-if="tableData.username === 'admin'"
                style="margin-left: 20px"
              >Super admin has full permissions</span>
            </li>
          </ul>
          <ul>
            <li>
              Password:
              <el-button
                type="primary"
                link
                size="small"
                @click="showPasswordInput = true"
              >
                Change Password
              </el-button>
              <el-input
                v-if="showPasswordInput"
                v-model="tableData.newpassword"
                @keyup.enter="changeadminuser"
              />
            </li>
          </ul>
          <div style="clear: both" />
        </div>
        <div class="renwu">
          <h4>Responsibilities</h4>
          <el-divider />
          <div
            v-if="
              String(uinfo?.jurisdiction?.issh) === '1' ||
                uinfo?.username === 'admin'
            "
          >
            <span>Review site content and user comments.</span>
            <el-divider />
          </div>
          <div
            v-if="
              String(uinfo?.jurisdiction?.isyh) === '1' ||
                uinfo?.username === 'admin'
            "
          >
            <span>Manage user verification and account status.</span>
            <el-divider />
          </div>
          <div
            v-if="
              String(uinfo?.jurisdiction?.isgl) === '1' ||
                uinfo?.username === 'admin'
            "
          >
            <span>Manage homepage carousel and module
              categories.</span>
            <el-divider />
          </div>
          <div
            v-if="
              String(uinfo?.jurisdiction?.isfk) === '1' ||
                uinfo?.username === 'admin'
            "
          >
            <span>Manage user feedback and violation reports.</span>
            <el-divider />
          </div>
          <div v-if="uinfo?.username === 'admin'">
            <span>Manage admin accounts and remove site content when
              needed.</span>
            <el-divider />
          </div>
          <div v-if="String(uinfo?.user_state) === '0'">
            <span>Your account is currently disabled.</span>
            <el-divider />
          </div>
        </div>
      </div>
    </el-main>
  </div>
</template>

<script setup>
import { computed, onMounted, ref } from 'vue'
import { ElMessage } from 'element-plus'
import { useUserStore } from '../../store/modules/user'
import axios from '../../utils/axios'
import { formatDate } from '../../utils/dateFormat'

const userStore = useUserStore()

const showNameInput = ref(false)
const showPasswordInput = ref(false)
const tableData = ref({})

const uinfo = computed(() => userStore.uinfo || {})

const toFormData = payload => {
	const params = new URLSearchParams()
	Object.entries(payload).forEach(([key, value]) => {
		if (value !== undefined && value !== null) {
			params.append(key, String(value))
		}
	})
	return params
}

const changeadminuser = async () => {
	try {
		const res = await axios.post(
			'/admin/changeadminuser',
			toFormData(tableData.value),
		)
		if (res.data?.state?.type === 'SUCCESS') {
			ElMessage.success('Operation successful')
			await getadmin()
			showNameInput.value = false
			showPasswordInput.value = false
			return
		}
		ElMessage.error(res.data?.state?.msg || 'Operation failed')
	} catch {
		ElMessage.error('Operation failed')
	}
}

const getadmin = async () => {
	try {
		const res = await axios.post('/admin/getadmin')
		if (res.data?.state?.type === 'SUCCESS') {
			tableData.value = res.data.data || {}
			return
		}
		ElMessage.error(res.data?.state?.msg || 'Failed to load profile')
	} catch {
		ElMessage.error('Failed to load profile')
	}
}

onMounted(() => {
	getadmin()
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
.myself {
	position: relative;
	width: 100%;
}
.main {
	margin-top: 40px;
	padding: 2px 20px 20px 20px;
	background-color: #fff;
}
.first-userInfo {
	padding-left: 20px;
}
.user-info ul {
	float: left;
	width: 30%;
	margin-top: -10px;
}
.user-info li {
	margin-top: 10px;
}
</style>
