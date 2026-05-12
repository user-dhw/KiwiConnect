<template>
  <div class="useruser">
    <el-main>
      <div class="top">
        <el-breadcrumb separator="/">
          <el-breadcrumb-item :to="{ path: '/' }">Home</el-breadcrumb-item>
          <el-breadcrumb-item>User Management</el-breadcrumb-item>
          <el-breadcrumb-item>Regular User Management</el-breadcrumb-item>
        </el-breadcrumb>
      </div>

      <div class="main">
        <div class="page-header">
          <h2 class="page-title">Regular User Management</h2>
          <p class="page-copy">
            Review verification status, manage account access, and handle routine moderation actions for platform users.
          </p>
        </div>

        <div class="search">
            <el-form :inline="true" :model="filterForm" class="demo-form-inline">
              <el-form-item>
                <el-input v-model="filterForm.user" placeholder="Search account" />
              </el-form-item>
              <el-form-item>
                <el-select
                  v-model="filterForm.realstate"
                  :teleported="false"
                  style="width: 200px"
                  placeholder="Real-name verification"
                >
                  <el-option label="All" value="all" />
                  <el-option label="Not verified" value="1" />
                  <el-option label="Pending" value="2" />
                  <el-option label="Verified" value="3" />
                  <el-option label="Rejected" value="0" />
                </el-select>
              </el-form-item>
              <el-form-item>
                <el-button type="primary" @click="applyFiltersAndSearch">Search</el-button>
              </el-form-item>
            </el-form>
          </div>

          <el-table
            v-loading="loading"
            element-loading-text="Loading..."
            :data="tableData"
            border
            style="width: 100%; min-height: 500px"
          >
            <el-table-column prop="username" label="Account" />
            <el-table-column prop="nickname" label="Nickname" />
            <el-table-column prop="realstate" label="Real-name verification">
              <template #default="scope">
                <el-button
                  type="primary"
                  link
                  :disabled="!canManageUsers"
                  @click="changerealstatedialog(scope.row)"
                >
                  {{ userStateLabel(scope.row.realstate) }}
                </el-button>
              </template>
            </el-table-column>

            <el-table-column prop="activationdate" label="Access status" width="180">
              <template #default="scope">
                <span
                  :class="[
                    'access-status',
                    isUserBanned(scope.row.activationdate) ? 'is-banned' : 'is-active',
                  ]"
                >
                  {{ formatAccessStatus(scope.row.activationdate) }}
                </span>
              </template>
            </el-table-column>
            <el-table-column fixed="right" label="Actions" width="280">
              <template #default="scope">
                <el-button
                  type="warning"
                  link
                  size="small"
                  :disabled="!canBanUsers"
                  @click="activationdate(scope.row)"
                >
                  Ban/Unban
                </el-button>
                <el-button
                  type="danger"
                  link
                  size="small"
                  :disabled="!canManageUsers"
                  @click="deleteuser(scope.row)"
                >
                  Delete
                </el-button>
                <el-button
                  type="primary"
                  link
                  size="small"
                  :disabled="scope.row.username === 'admin' || !canChangeUserPassword"
                  @click="changepw(scope.row)"
                >
                  Change Password
                </el-button>
              </template>
            </el-table-column>
          </el-table>

          <el-dialog v-model="dialogpw" title="Set new password" width="500px">
            <el-form :model="changepassword">
              <el-form-item label="Account" label-width="100px">
                {{ changepassword.username }}
              </el-form-item>
              <el-form-item label="New password" label-width="100px">
                <el-input v-model="changepassword.newpassword" autocomplete="off" />
              </el-form-item>
            </el-form>
            <template #footer>
              <div class="dialog-footer">
                <el-button @click="dialogpw = false">Cancel</el-button>
                <el-button type="primary" @click="change">Confirm</el-button>
              </div>
            </template>
          </el-dialog>

          <el-dialog v-model="dialogstudent" title="Real-name verification">
            <div v-if="Number(changerealstateuser.realstate) === 1">
              <img src="../../assets/img/noinfo.png" width="100%" alt="No info" />
              <div style="width: 100%; text-align: center">
                This user has not submitted verification details.
              </div>
            </div>
            <el-form v-else>
              <el-form-item label="Student ID">
                {{ changerealstateuser.studentid }}
              </el-form-item>
              <el-form-item label="Name">
                {{ changerealstateuser.realname }}
              </el-form-item>
              <el-form-item label="Student card">
                <img
                  v-for="(img, id) in changerealstateuser.studentcard"
                  :key="id"
                  :src="img.url"
                  style="width: 40%; margin: 20px"
                  alt="Student card"
                />
              </el-form-item>
            </el-form>
            <template #footer>
              <div v-if="Number(changerealstateuser.realstate) === 1" class="dialog-footer">
                <el-button @click="dialogstudent = false">Close</el-button>
              </div>
              <div v-else class="dialog-footer">
                <el-button
                  v-if="Number(changerealstateuser.realstate) !== 3"
                  type="primary"
                  @click="changestate('realstate', 3, changerealstateuser.user_id)"
                >
                  Approve
                </el-button>
                <el-button
                  type="danger"
                  @click="changestate('realstate', 0, changerealstateuser.user_id)"
                >
                  Reject
                </el-button>
                <el-button @click="dialogstudent = false">Cancel</el-button>
              </div>
            </template>
          </el-dialog>

          <el-dialog v-model="activationttime.dialog" title="Ban duration (days)" width="500px">
            <el-form :model="activationttime">
              <el-form-item>
                <el-input v-model="activationttime.time" />
              </el-form-item>
            </el-form>
            <template #footer>
              <div class="dialog-footer">
                <el-button @click="activationttime.dialog = false">Cancel</el-button>
                <el-button type="primary" @click="changeactivationdate">Confirm</el-button>
              </div>
            </template>
          </el-dialog>

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
  import { useRoute } from 'vue-router'
  import { ElMessage } from 'element-plus'
  import axios from '../../utils/axios'
  import { formatDate } from '../../utils/dateFormat'
  import { useUserStore } from '../../store/modules/user'

  const route = useRoute()
  const userStore = useUserStore()

  const changepassword = reactive({
    username: '',
    newpassword: '',
    type: 'adminuser',
  })
  const changerealstateuser = reactive({
    realstate: 1,
    realname: '',
    studentid: '',
    studentcard: [],
  })

  const dialogstudent = ref(false)
  const dialogpw = ref(false)
  const loading = ref(false)
  const tableData = ref([])

  const filterForm = reactive({
    realstate: 'all',
    user: '',
  })

  const pagelistquery = reactive({
    realstate: '',
    total: 0,
    page: 1,
    pagesize: 10,
    user: '',
  })

  const activationttime = reactive({
    time: 0,
    userid: '',
    jubao_id: '',
    dialog: false,
  })

  const isSuperAdmin = computed(() => userStore.uinfo?.username === 'admin')
  const jurisdiction = computed(() => userStore.uinfo?.jurisdiction || {})
  const canManageUsers = computed(
    () => isSuperAdmin.value || String(jurisdiction.value.isyh) === '1'
  )
  const canBanUsers = computed(() => isSuperAdmin.value || String(jurisdiction.value.isfk) === '1')
  const canChangeUserPassword = computed(
    () => isSuperAdmin.value || String(jurisdiction.value.isgl) === '1'
  )

  const toFormBody = data => {
    const body = new URLSearchParams()
    Object.keys(data).forEach(key => {
      const value = data[key]
      body.append(key, value === undefined || value === null ? '' : String(value))
    })
    return body
  }

  const userStateLabel = state => {
    switch (Number(state)) {
      case 3:
        return 'Verified'
      case 2:
        return 'Pending review'
      case 1:
        return 'Not verified'
      case 0:
        return 'Rejected'
      default:
        return 'Unknown'
    }
  }

  const isUserBanned = activationdate => {
    if (!activationdate) return false
    const time = Number(activationdate)
    return Number.isFinite(time) && time > Date.now()
  }

  const formatAccessStatus = activationdate => {
    if (!activationdate) return 'Active'
    return isUserBanned(activationdate) ? `Banned until ${formatDate(activationdate)}` : 'Active'
  }

  const parseImageList = value => {
    if (Array.isArray(value)) return value
    if (!value) return []
    try {
      const parsed = typeof value === 'string' ? JSON.parse(value) : value
      return Array.isArray(parsed) ? parsed : []
    } catch (error) {
      return []
    }
  }

  const activationdate = row => {
    activationttime.userid = row.user_id
    activationttime.dialog = true
  }

  const changeactivationdate = async () => {
    const payload = {
      time: activationttime.time,
      userid: activationttime.userid,
      jubao_id: activationttime.jubao_id,
    }
    const res = await axios.post('/admin/changeactivationdate', toFormBody(payload))
    if (res.data.state.type === 'SUCCESS') {
      ElMessage.success('Operation successful')
      activationttime.dialog = false
      getuserlist()
    }
  }

  const changestate = async (type, state, userid) => {
    const payload = {
      type,
      state,
      user_id: userid,
    }
    const res = await axios.post('/admin/changeuserstate', toFormBody(payload))
    if (res.data.state.type === 'SUCCESS') {
      ElMessage.success('Operation successful')
      dialogstudent.value = false
      getuserlist()
    }
  }

  const changepw = row => {
    Object.assign(changepassword, row)
    changepassword.newpassword = ''
    changepassword.type = 'adminuser'
    dialogpw.value = true
  }

  const change = async () => {
    changepassword.type = 'adminuser'
    const res = await axios.post('/admin/changepassword', toFormBody(changepassword))
    if (res.data.state.type === 'SUCCESS') {
      ElMessage.success('Password updated successfully')
      dialogpw.value = false
    }
  }

  const deleteuser = async row => {
    const payload = {
      user_id: row.user_id,
      usertype: 'user',
    }
    const res = await axios.post('/admin/deleteuser', toFormBody(payload))
    if (res.data.state.type === 'SUCCESS') {
      ElMessage.success('User deleted successfully')
      getuserlist()
    }
  }

  const changerealstatedialog = user => {
    const cloned = JSON.parse(JSON.stringify(user))
    Object.assign(changerealstateuser, cloned)
    changerealstateuser.studentcard = parseImageList(cloned.studentcard)
    dialogstudent.value = true
  }

  const handleSizeChange = val => {
    pagelistquery.pagesize = val
    getuserlist()
  }

  const handleCurrentChange = val => {
    pagelistquery.page = val
    getuserlist()
  }

  const applyFiltersAndSearch = () => {
    pagelistquery.user = filterForm.user
    pagelistquery.realstate = filterForm.realstate
    pagelistquery.page = 1
    getuserlist()
  }

  const getuserlist = async () => {
    loading.value = true
    try {
      const payload = {
        ...pagelistquery,
        realstate: pagelistquery.realstate === 'all' ? '' : pagelistquery.realstate,
      }

      const res = await axios.post('/admin/getuserlist', toFormBody(payload))
      if (res.data.state.type === 'SUCCESS') {
        tableData.value = res.data.data
        pagelistquery.total = res.data.count
      } else {
        ElMessage.error(res.data.state.msg || 'Failed to load user list')
      }
    } catch (error) {
      ElMessage.error('Failed to load user list')
    } finally {
      loading.value = false
    }
  }

  const jubao = () => {
    const user = Array.isArray(route.query.user) ? route.query.user[0] : route.query.user
    const jubaoId = Array.isArray(route.query.jubao_id)
      ? route.query.jubao_id[0]
      : route.query.jubao_id

    pagelistquery.user = user || ''
    filterForm.user = user || ''
    activationttime.jubao_id = jubaoId || ''
  }

  onMounted(() => {
    filterForm.realstate = 'all'
    if (route.query.user) {
      jubao()
    }
    getuserlist()
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

  .access-status {
    display: inline-flex;
    align-items: center;
    min-height: 28px;
    padding: 4px 10px;
    border-radius: 999px;
    font-size: 13px;
    font-weight: 600;
    line-height: 1.4;
  }

  .access-status.is-active {
    color: #067647;
    background: #ecfdf3;
  }

  .access-status.is-banned {
    color: #b42318;
    background: #fef3f2;
  }
</style>
