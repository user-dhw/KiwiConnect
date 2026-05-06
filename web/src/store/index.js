import { createStore } from 'vuex'
import user from './modules/user'

const getBaseUrl = () => {
	if (typeof window === 'undefined') {
		return ''
	}
	return window.location.href.split('#')[0]
}

const createRootState = () => ({
	commentnum: 0,
	contentid: '',
	contentname: '',
	contentuserid: '',
	url: getBaseUrl(),
})

export default createStore({
	state() {
		return createRootState()
	},
	getters: {
		contentInfo(state) {
			return {
				contentid: state.contentid,
				contentname: state.contentname,
				contentuserid: state.contentuserid,
			}
		},
		hasSelectedContent(state) {
			return !!state.contentid
		},
	},
	mutations: {
		SET_CONTENTID(state, id) {
			state.contentid = id ?? ''
		},
		SET_COMMENTNUM(state, num) {
			const parsed = Number(num)
			state.commentnum =
				Number.isFinite(parsed) && parsed >= 0 ? parsed : 0
		},
		SET_CONTENTINFO(state, data) {
			state.contentname = data?.contentname ?? ''
			state.contentuserid = data?.contentuserid ?? ''
		},
		RESET_CONTENTSTATE(state) {
			state.commentnum = 0
			state.contentid = ''
			state.contentname = ''
			state.contentuserid = ''
			state.url = getBaseUrl()
		},
	},
	actions: {
		setcontentid({ commit }, data) {
			commit('SET_CONTENTID', data)
		},
		setcommentnum({ commit }, data) {
			commit('SET_COMMENTNUM', data)
		},
		setcontentinfo({ commit }, data) {
			commit('SET_CONTENTINFO', data)
		},
		resetcontentstate({ commit }) {
			commit('RESET_CONTENTSTATE')
		},
	},
	modules: {
		user,
	},
})
