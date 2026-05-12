<template>
  <div class="profile-page">
    <el-main>
      <div class="top">
        <el-breadcrumb separator="/">
          <el-breadcrumb-item :to="{ path: '/' }">Home</el-breadcrumb-item>
          <el-breadcrumb-item>Personal Center</el-breadcrumb-item>
        </el-breadcrumb>
      </div>

      <div class="profile-body">
        <section class="profile-hero">
          <div>
            <p class="profile-eyebrow">Admin Profile</p>
            <h1 class="profile-title">{{ tableData.nickname || tableData.username || 'Administrator' }}</h1>
            <p class="profile-copy">
              Review account details, update your display name, and manage access credentials from one place.
            </p>
          </div>
          <div class="profile-badges">
            <span class="status-badge" :class="String(tableData.user_state) === '1' ? 'is-enabled' : 'is-disabled'">
              {{ String(tableData.user_state) === '1' ? 'Enabled' : 'Disabled' }}
            </span>
            <span class="role-badge">
              {{ tableData.username === 'admin' ? 'Super Admin' : 'Normal Admin' }}
            </span>
          </div>
        </section>

        <section class="profile-grid">
          <article class="profile-card">
            <div class="card-header">
              <div>
                <h2>Account Information</h2>
                <p>Core account details and editable profile fields.</p>
              </div>
            </div>

            <div class="info-list">
              <div class="info-item">
                <span class="info-label">Account</span>
                <strong class="info-value">{{ tableData.username || '--' }}</strong>
              </div>
              <div class="info-item">
                <span class="info-label">Created At</span>
                <strong class="info-value">{{ formatDate(tableData.user_createtime) }}</strong>
              </div>
              <div class="info-item info-item--wide">
                <span class="info-label">Nickname</span>
                <div class="inline-editor">
                  <template v-if="showNameInput">
                    <el-input
                      v-model="tableData.nickname"
                      placeholder="Enter a new nickname"
                      @keyup.enter="changeadminuser"
                    />
                    <div class="inline-actions">
                      <el-button type="primary" @click="changeadminuser">Save</el-button>
                      <el-button @click="cancelNicknameEdit">Cancel</el-button>
                    </div>
                  </template>
                  <template v-else>
                    <strong class="info-value">{{ tableData.nickname || '--' }}</strong>
                    <el-button type="primary" link size="small" @click="showNameInput = true">
                      Edit Nickname
                    </el-button>
                  </template>
                </div>
              </div>
              <div class="info-item info-item--wide">
                <span class="info-label">Password</span>
                <div class="inline-editor">
                  <template v-if="showPasswordInput">
                    <el-input
                      v-model="tableData.newpassword"
                      type="password"
                      show-password
                      placeholder="Enter a new password"
                      @keyup.enter="changeadminuser"
                    />
                    <div class="inline-actions">
                      <el-button type="primary" @click="changeadminuser">Update</el-button>
                      <el-button @click="cancelPasswordEdit">Cancel</el-button>
                    </div>
                  </template>
                  <template v-else>
                    <strong class="info-value">Password protected</strong>
                    <el-button type="primary" link size="small" @click="showPasswordInput = true">
                      Change Password
                    </el-button>
                  </template>
                </div>
              </div>
            </div>
          </article>

          <article class="profile-card">
            <div class="card-header">
              <div>
                <h2>Permissions</h2>
                <p>Modules this account can currently access in the admin system.</p>
              </div>
            </div>

            <div class="permission-list">
              <span class="permission-chip" :class="{ 'is-active': String(tableData.issh) === '1' || tableData.username === 'admin' }">
                Review Center
              </span>
              <span class="permission-chip" :class="{ 'is-active': String(tableData.isyh) === '1' || tableData.username === 'admin' }">
                User Management
              </span>
              <span class="permission-chip" :class="{ 'is-active': String(tableData.isgl) === '1' || tableData.username === 'admin' }">
                Website Management
              </span>
              <span class="permission-chip" :class="{ 'is-active': String(tableData.isfk) === '1' || tableData.username === 'admin' }">
                Feedback Center
              </span>
            </div>

            <div class="permission-note">
              <template v-if="tableData.username === 'admin'">
                Super admin accounts automatically have full access across all modules.
              </template>
              <template v-else>
                Permissions are assigned based on your admin role and can be updated by a super admin.
              </template>
            </div>
          </article>
        </section>

        <section class="profile-card responsibilities-card">
          <div class="card-header">
            <div>
              <h2>Responsibilities</h2>
              <p>Operational areas this account is expected to monitor and maintain.</p>
            </div>
          </div>

          <div class="responsibility-list">
            <div v-if="String(uinfo?.jurisdiction?.issh) === '1' || uinfo?.username === 'admin'" class="responsibility-item">
              <strong>Review Center</strong>
              <span>Review site content, moderate community submissions, and monitor comment activity.</span>
            </div>
            <div v-if="String(uinfo?.jurisdiction?.isyh) === '1' || uinfo?.username === 'admin'" class="responsibility-item">
              <strong>User Management</strong>
              <span>Manage user verification, access state, and account integrity.</span>
            </div>
            <div v-if="String(uinfo?.jurisdiction?.isgl) === '1' || uinfo?.username === 'admin'" class="responsibility-item">
              <strong>Website Management</strong>
              <span>Maintain homepage carousel, labels, and core platform configuration.</span>
            </div>
            <div v-if="String(uinfo?.jurisdiction?.isfk) === '1' || uinfo?.username === 'admin'" class="responsibility-item">
              <strong>Feedback Center</strong>
              <span>Handle feedback, appeals, and reports raised by platform users.</span>
            </div>
            <div v-if="uinfo?.username === 'admin'" class="responsibility-item">
              <strong>Super Admin Oversight</strong>
              <span>Manage admin accounts and intervene in platform-wide moderation when required.</span>
            </div>
            <div v-if="String(uinfo?.user_state) === '0'" class="responsibility-item responsibility-item--warning">
              <strong>Account status alert</strong>
              <span>Your account is currently disabled and may need review before normal operation resumes.</span>
            </div>
          </div>
        </section>
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
import { toFormData } from '../../utils/form'

const userStore = useUserStore()

const showNameInput = ref(false)
const showPasswordInput = ref(false)
const tableData = ref({})

const uinfo = computed(() => userStore.uinfo || {})

const cancelNicknameEdit = () => {
  showNameInput.value = false
  getadmin()
}

const cancelPasswordEdit = () => {
  showPasswordInput.value = false
  tableData.value.newpassword = ''
}

const changeadminuser = async () => {
  try {
    const res = await axios.post('/admin/changeadminuser', toFormData(tableData.value))
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
      tableData.value = { ...(res.data.data || {}), newpassword: '' }
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

.profile-page {
  position: relative;
  width: 100%;
}

.profile-body {
  margin-top: 40px;
  padding: 24px;
  background: #f8fbff;
}

.profile-hero {
  display: flex;
  align-items: flex-start;
  justify-content: space-between;
  gap: 24px;
  padding: 28px 32px;
  border: 1px solid #dbe7ff;
  border-radius: 24px;
  background: linear-gradient(135deg, #ffffff 0%, #edf4ff 100%);
  box-shadow: 0 20px 45px rgba(38, 99, 235, 0.08);
}

.profile-eyebrow {
  margin: 0 0 8px;
  font-size: 13px;
  font-weight: 700;
  letter-spacing: 0.08em;
  text-transform: uppercase;
  color: #2663eb;
}

.profile-title {
  margin: 0;
  font-size: 34px;
  line-height: 1.1;
  color: #1f2a44;
}

.profile-copy {
  max-width: 720px;
  margin: 12px 0 0;
  color: #667085;
  line-height: 1.7;
}

.profile-badges {
  display: flex;
  flex-wrap: wrap;
  gap: 10px;
}

.status-badge,
.role-badge {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  min-height: 36px;
  padding: 0 14px;
  border-radius: 999px;
  font-size: 13px;
  font-weight: 700;
}

.status-badge.is-enabled {
  background: #ecfdf3;
  color: #067647;
}

.status-badge.is-disabled {
  background: #fef3f2;
  color: #b42318;
}

.role-badge {
  background: #eef4ff;
  color: #2663eb;
}

.profile-grid {
  display: grid;
  grid-template-columns: minmax(0, 1.35fr) minmax(320px, 1fr);
  gap: 18px;
  margin-top: 22px;
}

.profile-card {
  padding: 24px;
  border: 1px solid #e3edff;
  border-radius: 22px;
  background: #fff;
  box-shadow: 0 14px 34px rgba(15, 23, 42, 0.05);
}

.card-header h2 {
  margin: 0;
  font-size: 24px;
  color: #1f2a44;
}

.card-header p {
  margin: 8px 0 0;
  color: #667085;
}

.info-list {
  display: grid;
  grid-template-columns: repeat(2, minmax(0, 1fr));
  gap: 18px;
  margin-top: 20px;
}

.info-item {
  padding: 18px;
  border: 1px solid #edf2ff;
  border-radius: 18px;
  background: #f9fbff;
}

.info-item--wide {
  grid-column: 1 / -1;
}

.info-label {
  display: block;
  margin-bottom: 8px;
  font-size: 13px;
  font-weight: 700;
  letter-spacing: 0.04em;
  text-transform: uppercase;
  color: #667085;
}

.info-value {
  color: #1f2a44;
}

.inline-editor {
  display: flex;
  flex-direction: column;
  gap: 10px;
}

.inline-actions {
  display: flex;
  gap: 10px;
  flex-wrap: wrap;
}

.permission-list {
  display: flex;
  flex-wrap: wrap;
  gap: 10px;
  margin-top: 20px;
}

.permission-chip {
  padding: 10px 14px;
  border-radius: 999px;
  background: #f2f4f7;
  color: #98a2b3;
  font-weight: 600;
}

.permission-chip.is-active {
  background: #eef4ff;
  color: #2663eb;
}

.permission-note {
  margin-top: 18px;
  color: #667085;
  line-height: 1.7;
}

.responsibilities-card {
  margin-top: 22px;
}

.responsibility-list {
  display: grid;
  grid-template-columns: repeat(2, minmax(0, 1fr));
  gap: 16px;
  margin-top: 20px;
}

.responsibility-item {
  display: flex;
  flex-direction: column;
  gap: 8px;
  padding: 18px;
  border: 1px solid #edf2ff;
  border-radius: 18px;
  background: #f9fbff;
}

.responsibility-item strong {
  color: #1f2a44;
}

.responsibility-item span {
  color: #667085;
  line-height: 1.7;
}

.responsibility-item--warning {
  border-color: #ffd7d2;
  background: #fff7f6;
}

@media (max-width: 1200px) {
  .profile-grid,
  .responsibility-list,
  .info-list {
    grid-template-columns: 1fr;
  }
}

@media (max-width: 768px) {
  .profile-body {
    padding: 16px;
  }

  .profile-hero {
    flex-direction: column;
    padding: 24px;
  }

  .profile-title {
    font-size: 28px;
  }

  .profile-card {
    padding: 18px;
  }
}
</style>
