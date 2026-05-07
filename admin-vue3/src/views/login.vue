<template>
  <div
    id="login"
    class="login-page"
  >
    <div class="bg-grid" />
    <div class="bg-glow glow-a" />
    <div class="bg-glow glow-b" />

    <div class="login-shell">
      <section class="brand-panel">
        <div class="brand-badge">
          KC
        </div>

        <div class="brand-copy">
          <p class="brand-eyebrow">
            KiwiConnect Admin
          </p>
          <h1>
            Admin workspace
          </h1>
          <p class="brand-description">
            Secure access for moderation, content management, and platform operations.
          </p>
        </div>
      </section>

      <section class="auth-panel">
        <div class="auth-card">
          <div class="auth-header">
            <div class="auth-logo">
              Admin Portal
            </div>
            <h2>Sign in</h2>
            <p>
              Use your administrator credentials to enter the control center.
            </p>
          </div>

          <form
            class="auth-form"
            @submit.prevent="handleLogin"
          >
            <div class="field-group">
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
            </div>

            <div class="field-group">
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
            </div>

            <button
              class="submit-btn"
              type="submit"
              :disabled="loading"
            >
              {{
                loading
                  ? 'Signing in...'
                  : 'Enter Admin Dashboard'
              }}
            </button>
          </form>

          <div class="auth-footer">
            <p class="auth-hint">
              Demo account: admin / admin
            </p>
            <p class="auth-note">
              Authorized staff only.
            </p>
          </div>
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
	padding: 20px;
	box-sizing: border-box;
	background:
		radial-gradient(circle at top left, rgba(72, 108, 148, 0.22), transparent 24%),
		radial-gradient(circle at bottom right, rgba(36, 86, 163, 0.18), transparent 22%),
		linear-gradient(135deg, #eef3f8 0%, #e8eef7 42%, #eef2f7 100%);
	display: flex;
	flex-direction: column;
}

.bg-grid {
	position: absolute;
	inset: 0;
	background-image:
		linear-gradient(rgba(84, 92, 100, 0.05) 1px, transparent 1px),
		linear-gradient(90deg, rgba(84, 92, 100, 0.05) 1px, transparent 1px);
	background-size: 26px 26px;
	pointer-events: none;
}

.bg-glow {
	position: absolute;
	border-radius: 999px;
	filter: blur(20px);
	opacity: 0.7;
	pointer-events: none;
}

.glow-a {
	width: 360px;
	height: 360px;
	left: -120px;
	top: -120px;
	background: radial-gradient(circle, rgba(63, 119, 204, 0.3) 0%, transparent 72%);
}

.glow-b {
	width: 400px;
	height: 400px;
	right: -150px;
	bottom: -140px;
	background: radial-gradient(circle, rgba(84, 92, 100, 0.26) 0%, transparent 70%);
}

.login-shell {
	position: relative;
	z-index: 2;
	max-width: 1160px;
	width: 100%;
	min-height: min(700px, calc(100vh - 84px));
	margin: auto;
	border-radius: 32px;
	overflow: hidden;
	display: grid;
	grid-template-columns: minmax(0, 1.08fr) minmax(380px, 0.92fr);
	background: rgba(255, 255, 255, 0.7);
	border: 1px solid rgba(255, 255, 255, 0.8);
	box-shadow: 0 28px 70px rgba(35, 56, 88, 0.16);
	backdrop-filter: blur(14px);
}

.brand-panel {
	width: 100%;
	display: flex;
	flex-direction: column;
	justify-content: center;
	padding: 40px 44px;
	background:
		linear-gradient(180deg, rgba(84, 92, 100, 0.98) 0%, rgba(69, 78, 89, 0.96) 100%);
	color: #eaf1fb;
}

.brand-badge {
	width: 52px;
	height: 52px;
	border-radius: 16px;
	display: grid;
	place-items: center;
	background: linear-gradient(135deg, #ffffff, #d8e4f7);
	color: #3c5876;
	font-size: 20px;
	font-weight: 800;
	letter-spacing: 0.08em;
	box-shadow: 0 12px 28px rgba(15, 22, 33, 0.24);
}

.brand-copy {
	display: grid;
	gap: 14px;
	margin-top: 28px;
	max-width: 420px;
}

.brand-eyebrow {
	margin: 0;
	font-size: 0.8rem;
	font-weight: 700;
	letter-spacing: 0.16em;
	text-transform: uppercase;
	color: rgba(216, 228, 247, 0.76);
}

.brand-copy h1 {
	margin: 0;
	max-width: 7ch;
	font-size: clamp(2.1rem, 3.8vw, 3.35rem);
	line-height: 1;
	letter-spacing: -0.05em;
	color: #ffffff;
}

.brand-description {
	margin: 0;
	max-width: 30ch;
	font-size: 0.95rem;
	line-height: 1.65;
	color: rgba(233, 241, 251, 0.78);
}

.auth-panel {
	width: 100%;
	padding: 0;
	display: flex;
	align-items: center;
	justify-content: center;
	background: linear-gradient(180deg, rgba(250, 252, 255, 0.82) 0%, rgba(243, 247, 252, 0.96) 100%);
}

.auth-card {
	width: 100%;
	max-width: 440px;
	padding: 34px 36px;
	border-radius: 28px;
	background: rgba(255, 255, 255, 0.96);
	border: 1px solid rgba(103, 123, 148, 0.12);
	box-shadow: 0 20px 42px rgba(48, 71, 104, 0.1);
}

.auth-header h2 {
	margin: 12px 0 8px;
	font-size: 1.85rem;
	line-height: 1.05;
	color: #213246;
}

.auth-logo {
	display: inline-block;
	font-size: 12px;
	font-weight: 700;
	padding: 6px 12px;
	border-radius: 999px;
	color: #4d647f;
	background: #edf2f8;
}

.auth-header p {
	margin: 0;
	font-size: 14px;
	line-height: 1.55;
	color: #677b92;
}

.auth-form {
	margin-top: 22px;
	display: flex;
	flex-direction: column;
	gap: 14px;
}

.field-group {
	display: grid;
	gap: 8px;
}

.field-label {
	font-size: 13px;
	font-weight: 700;
	color: #3f5266;
}

.auth-form input {
	height: 48px;
	padding: 0 15px;
	border-radius: 14px;
	border: 1px solid #d6deea;
	background: #fdfefe;
	font-size: 14px;
	color: #213246;
	outline: none;
	transition:
		border-color 0.2s ease,
		box-shadow 0.2s ease,
		background 0.2s ease;
}

.auth-form input:focus {
	border-color: #61778f;
	background: #fff;
	box-shadow: 0 0 0 4px rgba(84, 92, 100, 0.12);
}

.submit-btn {
	height: 50px;
	border: 0;
	border-radius: 14px;
	margin-top: 4px;
	font-size: 15px;
	font-weight: 700;
	color: #fff;
	background: linear-gradient(135deg, #4c5763 0%, #5d6874 100%);
	cursor: pointer;
	transition:
		transform 0.2s ease,
		box-shadow 0.2s ease,
		opacity 0.2s ease;
}

.submit-btn:hover {
	transform: translateY(-1px);
	box-shadow: 0 14px 28px rgba(58, 72, 90, 0.24);
}

.submit-btn:disabled {
	opacity: 0.75;
	cursor: not-allowed;
	transform: none;
	box-shadow: none;
}

.auth-footer {
	margin-top: 14px;
	display: flex;
	align-items: center;
	justify-content: space-between;
	gap: 12px;
	flex-wrap: wrap;
	padding-top: 6px;
}

.auth-hint,
.auth-note {
	margin: 0;
	font-size: 12px;
	color: #6e7f93;
}

.page-footer {
	position: relative;
	z-index: 2;
	margin-top: 10px;
	text-align: center;
	font-size: 12px;
	color: #5f7086;
}

@media (max-width: 1080px) {
	.login-page {
		padding: 14px;
	}

	.login-shell {
		max-width: 100%;
		min-height: auto;
		grid-template-columns: 1fr;
	}

	.brand-panel {
		padding: 28px 24px 24px;
	}

	.brand-copy h1 {
		max-width: none;
	}

	.auth-panel {
		padding: 22px 24px 28px;
	}

	.auth-card {
		max-width: 100%;
	}
}

@media (max-width: 560px) {
	.auth-card {
		padding: 24px 18px;
	}

	.auth-header h2 {
		font-size: 26px;
	}

	.brand-panel {
		padding: 20px 18px;
	}

	.brand-copy h1 {
		font-size: 2.1rem;
	}
}
</style>
