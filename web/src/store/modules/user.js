const getStoredUserInfo = () => {
	try {
		const raw = window.localStorage.getItem('userinfo')
		return raw ? JSON.parse(raw) : { avatar: '' }
	} catch {
		return { avatar: '' }
	}
}

const getStoredToken = () =>
	window.localStorage.getItem('luffy_jwt_token') || null

export default {
	namespaced: true,
	state() {
		const token = getStoredToken()
		return {
			islog: !!token,
			islogin: true,
			isclose: false,
			userinfo: getStoredUserInfo(),
			token,
			unread: 0,
		}
	},
	getters: {
		isAuthenticated(state) {
			return !!state.token
		},
	},
	mutations: {
		SET_USERINFO(state, userinfo) {
			state.userinfo = userinfo || { avatar: '' }
			window.localStorage.setItem(
				'userinfo',
				JSON.stringify(state.userinfo),
			)
		},
		CHANGE_ISLOG(state, value) {
			if (typeof value === 'boolean') {
				state.islog = value
				return
			}
			state.islog = !state.islog
		},
		DELETE_USERINFO(state) {
			state.userinfo = { avatar: '' }
			state.token = null
			state.islog = false
			state.unread = 0
			window.localStorage.removeItem('userinfo')
			window.localStorage.removeItem('luffy_jwt_token')
		},
		SET_TOKEN(state, data) {
			state.token = data || null
			state.islog = !!state.token
			if (state.token) {
				window.localStorage.setItem('luffy_jwt_token', state.token)
			} else {
				window.localStorage.removeItem('luffy_jwt_token')
			}
		},
		JOIN(state, value) {
			if (typeof value === 'boolean') {
				state.islogin = value
				return
			}
			state.islogin = !state.islogin
		},
		CLOSE(state, value) {
			if (typeof value === 'boolean') {
				state.isclose = value
				return
			}
			state.isclose = !state.isclose
		},
		SET_UNREAD(state, num) {
			state.unread = Number.isFinite(num) ? num : 0
		},
	},
	actions: {
		setUserInfo({ commit }, userinfo) {
			commit('SET_USERINFO', userinfo)
		},
		changeislog({ commit }, value) {
			commit('CHANGE_ISLOG', value)
		},
		join({ commit }, value) {
			commit('JOIN', value)
		},
		close({ commit }, value) {
			commit('CLOSE', value)
		},
		deleteuserinfo({ commit }) {
			commit('DELETE_USERINFO')
		},
		setToken({ commit }, data) {
			commit('SET_TOKEN', data)
		},
		setunread({ commit }, data) {
			commit('SET_UNREAD', data)
		},
	},
}
