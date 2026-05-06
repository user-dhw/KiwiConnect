<template>
  <div
    id="login"
    class="login-page"
  >
    <div class="bg-orb orb-a" />
    <div class="bg-orb orb-b" />

    <div class="login-shell">
      <section class="auth-panel">
        <div class="auth-card">
          <div class="auth-header">
            <div class="auth-logo">
              Admin Login
            </div>
            <h2>Welcome back</h2>
            <p>
              Use your administrator account to access the
              management workspace.
            </p>
          </div>

          <form
            class="auth-form"
            @submit.prevent="handleLogin"
          >
            <label
              for="username"
              class="field-label"
            >Username</label>
            <input
              id="username"
              v-model="username"
              type="text"
              autocomplete="username"
              placeholder="Enter your username"
            >

            <label
              for="password"
              class="field-label"
            >Password</label>
            <input
              id="password"
              v-model="password"
              type="password"
              autocomplete="current-password"
              placeholder="Enter your password"
            >

            <button
              class="submit-btn"
              type="submit"
              :disabled="loading"
            >
              {{
                loading
                  ? 'Signing in...'
                  : 'Sign In to Dashboard'
              }}
            </button>
          </form>

          <p class="auth-hint">
            Demo account: admin / admin
          </p>
        </div>
      </section>
    </div>

    <footer class="page-footer">
      Copyright © {{ currentYear }} KiwiConnect. All rights reserved.
    </footer>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { useUserStore } from '@/store/modules/user'
import { ElMessage } from 'element-plus'
import axios from '@/utils/axios'
import qs from 'qs'

const router = useRouter()
const userStore = useUserStore()

// State
const username = ref('')
const password = ref('')
const loading = ref(false)
const currentYear = new Date().getFullYear()

// Methods
const handleLogin = async () => {
	if (loading.value) {
		return
	}

	// Validation
	if (!username.value || !password.value) {
		ElMessage.error('Username or password is empty')
		return
	}

	try {
		loading.value = true

		const credentials = {
			username: username.value,
			password: password.value,
		}

		const response = await axios.post(
			'/admin/login',
			qs.stringify(credentials),
		)

		if (response.data?.state?.type === 'SUCCESS') {
			const { userinfo, token } = response.data.data
			ElMessage.success('Login successful')

			// Update user store
			userStore.setUserInfo(userinfo)
			userStore.setToken(token)

			// Navigate to dashboard
			router.push('/')
		} else {
			ElMessage.error(
				response.data?.state?.msg || 'Invalid username or password',
			)
		}
	} catch (error) {
		ElMessage.error('Network error: ' + (error?.message || 'Unknown error'))
	} finally {
		loading.value = false
	}
}
</script>

<style scoped>
.login-page {
	position: relative;
	min-height: 100vh;
	overflow: hidden;
	padding: 28px;
	box-sizing: border-box;
	background: linear-gradient(125deg, #eef5ff 0%, #ddeff6 45%, #e7f8ef 100%);
	animation: pageGlow 14s ease-in-out infinite alternate;
}

.bg-orb {
	position: absolute;
	border-radius: 999px;
	filter: blur(6px);
	opacity: 0.6;
	pointer-events: none;
}

.orb-a {
	width: 360px;
	height: 360px;
	left: -120px;
	top: -80px;
	background: radial-gradient(
		circle at 40% 40%,
		#75a8ff 0%,
		#4f7ed8 65%,
		transparent 100%
	);
}

.orb-b {
	width: 420px;
	height: 420px;
	right: -140px;
	bottom: -120px;
	background: radial-gradient(
		circle at 50% 50%,
		#66d7b3 0%,
		#3fa785 60%,
		transparent 100%
	);
}

.login-shell {
	position: relative;
	z-index: 2;
	max-width: 520px;
	min-height: calc(100vh - 120px);
	margin: 0 auto;
	border-radius: 28px;
	overflow: hidden;
	display: flex;
	align-items: center;
	justify-content: center;
	background: rgba(255, 255, 255, 0.56);
	border: 1px solid rgba(255, 255, 255, 0.7);
	box-shadow: 0 28px 70px rgba(28, 54, 97, 0.18);
	backdrop-filter: blur(10px);
}

.auth-panel {
	width: 100%;
	padding: 34px;
	display: flex;
	align-items: center;
	justify-content: center;
	background: linear-gradient(
		180deg,
		rgba(255, 255, 255, 0.56) 0%,
		rgba(244, 249, 255, 0.78) 100%
	);
}

.auth-card {
	width: min(420px, 100%);
	padding: 30px;
	border-radius: 20px;
	background: rgba(255, 255, 255, 0.72);
	border: 1px solid rgba(255, 255, 255, 0.9);
	box-shadow: 0 20px 54px rgba(42, 80, 138, 0.2);
	backdrop-filter: blur(16px);
}

.auth-header h2 {
	margin: 10px 0 8px;
	font-size: 30px;
	line-height: 1.1;
	color: #1f3349;
}

.auth-logo {
	display: inline-block;
	font-size: 12px;
	padding: 5px 11px;
	border-radius: 999px;
	color: #2d5f9b;
	background: #e8f0ff;
}

.auth-header p {
	margin: 0;
	font-size: 14px;
	line-height: 1.6;
	color: #56687d;
}

.auth-form {
	margin-top: 22px;
	display: flex;
	flex-direction: column;
	gap: 10px;
}

.field-label {
	font-size: 13px;
	font-weight: 600;
	color: #34495e;
}

.auth-form input {
	height: 46px;
	padding: 0 14px;
	border-radius: 12px;
	border: 1px solid #d8e1ee;
	background: rgba(255, 255, 255, 0.92);
	font-size: 14px;
	color: #1f3349;
	outline: none;
	transition:
		border-color 0.2s ease,
		box-shadow 0.2s ease;
}

.auth-form input:focus {
	border-color: #4a85cf;
	box-shadow: 0 0 0 3px rgba(74, 133, 207, 0.14);
}

.submit-btn {
	height: 48px;
	border: 0;
	border-radius: 12px;
	margin-top: 8px;
	font-size: 15px;
	font-weight: 600;
	color: #fff;
	background: linear-gradient(135deg, #2d6bc2 0%, #2483bc 55%, #1d9a8e 100%);
	cursor: pointer;
	transition:
		transform 0.2s ease,
		box-shadow 0.2s ease,
		opacity 0.2s ease;
}

.submit-btn:hover {
	transform: translateY(-1px);
	box-shadow: 0 12px 24px rgba(37, 111, 172, 0.3);
}

.submit-btn:disabled {
	opacity: 0.75;
	cursor: not-allowed;
	transform: none;
	box-shadow: none;
}

.auth-hint {
	margin: 14px 2px 0;
	font-size: 12px;
	color: #6e7f93;
}

.page-footer {
	position: relative;
	z-index: 2;
	margin-top: 16px;
	text-align: center;
	font-size: 12px;
	color: #5f7086;
}

@keyframes pageGlow {
	0% {
		background-position: 0% 0%;
	}
	100% {
		background-position: 100% 100%;
	}
}

@media (max-width: 980px) {
	.login-page {
		padding: 14px;
	}

	.login-shell {
		max-width: 100%;
		min-height: auto;
	}

	.auth-panel {
		padding: 22px;
	}
}

@media (max-width: 560px) {
	.auth-card {
		padding: 22px;
	}

	.auth-header h2 {
		font-size: 26px;
	}
}
</style>
