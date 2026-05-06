<template>
	<el-dialog
		class="auth-dialog"
		:model-value="isclose"
		:title="islogin ? 'Website Login' : 'New User Registration'"
		width="400px"
		@close="handleClose"
		:close-on-click-modal="false"
	>
		<el-form class="auth-form" :model="formData">
			<el-form-item>
				<el-input
					v-model="formData.username"
					placeholder="Enter username"
					:prefix-icon="User"
					clearable
				/>
			</el-form-item>

			<el-form-item>
				<el-input
					v-model="formData.password"
					placeholder="Enter password"
					type="password"
					:prefix-icon="Lock"
					show-password
				/>
			</el-form-item>

			<el-form-item v-if="!islogin">
				<el-input
					v-model="formData.password1"
					placeholder="Confirm password"
					type="password"
					:prefix-icon="Lock"
					show-password
				/>
			</el-form-item>

			<el-button
				class="auth-submit"
				type="primary"
				size="large"
				:loading="loading"
				:disabled="loading"
				@click="islogin ? handleLogin() : handleRegister()"
			>
				{{ islogin ? 'Login' : 'Register' }}
			</el-button>

			<div class="auth-switch-row">
				<el-link type="primary" @click="handleToggleMode">
					{{ islogin ? 'Register a new account' : 'Back to login' }}
				</el-link>
			</div>
		</el-form>
	</el-dialog>
</template>
<script setup>
import { computed, ref } from 'vue'
import { useStore } from 'vuex'
import { ElMessage } from 'element-plus'
import { register, login as apiLogin } from '@/api/auth'
import { User, Lock } from '@element-plus/icons-vue'
const store = useStore()

const formData = ref({
	username: '',
	password: '',
	password1: '',
})

const loading = ref(false)

const isclose = computed(() => store.state.user.isclose)
const islogin = computed(() => store.state.user.islogin)

const resetForm = () => {
	formData.value.username = ''
	formData.value.password = ''
	formData.value.password1 = ''
}

const formatBanTime = timestamp => {
	const date = new Date(Number(timestamp))
	if (Number.isNaN(date.getTime())) {
		return ''
	}
	const pad = num => String(num).padStart(2, '0')
	return `${date.getFullYear()}-${pad(date.getMonth() + 1)}-${pad(date.getDate())} ${pad(date.getHours())}:${pad(date.getMinutes())}`
}

const handleRegister = async () => {
	const userReg = /^[1-9a-zA-Z]{1}[0-9a-zA-Z]{5,9}$/
	const pwdReg = /^[a-zA-Z]\w{5,17}$/

	if (!userReg.test(formData.value.username)) {
		ElMessage.error(
			'Username must be 6-10 characters (letters/numbers), cannot start with 0',
		)
		return
	}

	if (!pwdReg.test(formData.value.password)) {
		ElMessage.error(
			'Password must be 6-18 characters (letters/numbers/underscore), must start with a letter',
		)
		return
	}

	if (formData.value.password !== formData.value.password1) {
		ElMessage.error('Passwords do not match')
		return
	}

	if (loading.value) {
		return
	}

	loading.value = true
	try {
		const res = await register({
			username: formData.value.username,
			password: formData.value.password,
		})

		if (res.state?.type === 'SUCCESS') {
			ElMessage.success('Registration successful, please login')
			store.dispatch('user/join', true)
			formData.value.password = ''
			formData.value.password1 = ''
			return
		}

		if (res.state?.type === 'ERROR_PARAMS_EXIST') {
			ElMessage.error('Username already exists')
			return
		}

		ElMessage.error(res.state?.msg || 'Registration failed')
	} catch {
		ElMessage.error('Registration failed, please try again later')
	} finally {
		loading.value = false
	}
}

const handleLogin = async () => {
	if (!formData.value.username || !formData.value.password) {
		ElMessage.error('Username or password cannot be empty')
		return
	}

	if (loading.value) {
		return
	}

	loading.value = true
	try {
		const res = await apiLogin({
			username: formData.value.username,
			password: formData.value.password,
		})

		if (res.state?.type === 'SUCCESS') {
			store.dispatch('user/setUserInfo', res.data?.userinfo || {})
			store.dispatch('user/setToken', res.data?.token || null)
			store.dispatch('user/changeislog', true)
			store.dispatch('user/close', false)
			resetForm()
			ElMessage.success('Login successful')
			return
		}

		if (res.state?.type === 'ACCOUNT_SUSPENDED') {
			const time = formatBanTime(res.data)
			const message =
				res.state?.msg && time
					? `${res.state.msg} (until ${time})`
					: res.state?.msg ||
						(time
							? `Your account has been suspended until ${time} due to violations`
							: 'Your account is currently suspended')
			ElMessage.error({
				message,
				duration: 5000,
				showClose: true,
			})
			return
		}

		ElMessage.error('Invalid username or password')
	} catch {
		ElMessage.error('Login failed, please try again later')
	} finally {
		loading.value = false
	}
}

const handleToggleMode = () => {
	store.dispatch('user/join')
	formData.value.password = ''
	formData.value.password1 = ''
}

const handleClose = () => {
	store.dispatch('user/close', false)
	resetForm()
}
</script>
<style scoped>
.auth-form {
	padding: 10px 0;
}

.auth-form .el-form-item {
	margin-bottom: 20px;
}

:deep(.el-input__wrapper) {
	border-radius: 4px;
	background: #fff;
	box-shadow: inset 0 0 0 1px #d7dce2;
	min-height: 42px;
	transition: box-shadow 0.2s ease;
}

:deep(.el-input__wrapper:hover) {
	box-shadow: inset 0 0 0 1px #bcc4cd;
}

:deep(.el-input__wrapper.is-focus) {
	background: #ffffff;
	box-shadow:
		inset 0 0 0 1px #395996,
		0 0 0 3px rgba(57, 89, 150, 0.12);
}

:deep(.el-input__inner) {
	font-size: 14px;
	line-height: 1.5;
	color: #3b4348;
	font-weight: 500;
	height: auto;
}

:deep(.el-input__inner::placeholder) {
	color: #9aa3ab;
	font-weight: 400;
}

:deep(.el-input__prefix) {
	font-size: 16px;
	color: #8a949d;
	display: flex;
	align-items: center;
}

:deep(.el-input__wrapper.is-focus .el-input__prefix) {
	color: #395996;
}

:deep(.el-input__suffix) {
	font-size: 16px;
	color: #a0aec0;
	display: flex;
	align-items: center;
}

.auth-submit {
	width: 100%;
	height: 42px;
	margin-top: 12px;
	border-radius: 4px;
	font-size: 15px;
	font-weight: 700;
	letter-spacing: 0.5px;
	background: linear-gradient(to bottom, #3f4b55, #353d44);
	border: 1px solid #2f363c;
	box-shadow: inset 0 1px 0 rgba(255, 255, 255, 0.08);
	transition: all 0.2s ease;
	position: relative;
	overflow: hidden;
}

.auth-submit::before {
	content: none;
}

:deep(.auth-submit.el-button:hover) {
	transform: translateY(-1px);
	background: linear-gradient(to bottom, #4a5c70, #395996);
	border-color: #334f85;
}

:deep(.auth-submit.el-button:active) {
	transform: translateY(0);
}

.auth-switch-row {
	text-align: center;
	margin-top: 20px;
	padding-top: 12px;
	border-top: 1px solid #e8edf2;
}

:deep(.auth-switch-row .el-link) {
	font-size: 14px;
	font-weight: 600;
	color: #395996;
	text-decoration: none;
	transition: all 0.2s;
}

:deep(.auth-switch-row .el-link:hover) {
	color: #2f4e86;
}

:deep(.auth-dialog) {
	--el-dialog-padding-primary: 32px;
}

:deep(.auth-dialog .el-dialog__header) {
	padding: 20px 24px;
	margin: 0;
	border-bottom: 1px solid #2f363c;
	background: #3b4348;
}

:deep(.auth-dialog .el-dialog__title) {
	font-size: 20px;
	font-weight: 700;
	color: #ffffff;
}

:deep(.auth-dialog .el-dialog__headerbtn) {
	top: 20px;
	right: 20px;
	width: 32px;
	height: 32px;
	font-size: 18px;
}

:deep(.auth-dialog .el-dialog__close) {
	color: #c6d0da;
}

:deep(.auth-dialog .el-dialog__headerbtn:hover .el-dialog__close) {
	color: #ffffff;
}

:deep(.auth-dialog .el-dialog__body) {
	padding: 22px 24px 24px;
}

:deep(.auth-dialog .el-dialog) {
	border-radius: 6px;
	overflow: hidden;
	box-shadow: 0 16px 28px rgba(0, 0, 0, 0.25);
	animation: auth-dialog-enter 0.25s ease-out;
}

@keyframes auth-dialog-enter {
	0% {
		opacity: 0;
		transform: translateY(-12px);
	}
	100% {
		opacity: 1;
		transform: translateY(0);
	}
}

:deep(.el-overlay) {
	background-color: rgba(31, 38, 44, 0.55);
}
</style>
