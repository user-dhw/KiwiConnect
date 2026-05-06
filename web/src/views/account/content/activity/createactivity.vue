<template>
	<div class="edit-page">
		<div class="page-heading">
			<h1 class="page-title">{{ isEdit ? 'Edit Activity' : 'Create Activity' }}</h1>
			<p class="page-subtitle">Publish event details, announcements, and participant information.</p>
		</div>

		<el-tabs type="border-card" class="admin-tabs">
			<el-tab-pane label="Activity Editor">
				<section class="admin-form-card">
					<el-form :model="form" label-width="140px" class="admin-form">
						<el-form-item label="Title">
							<el-input v-model="form.activity_title" />
						</el-form-item>
						<el-form-item label="Start Time">
							<el-date-picker
								v-model="form.activity_statetime"
								type="datetime"
								value-format="x"
							/>
						</el-form-item>
						<el-form-item label="End Time">
							<el-date-picker
								v-model="form.activity_endtime"
								type="datetime"
								value-format="x"
							/>
						</el-form-item>
						<el-form-item label="Location">
							<el-input v-model="form.activity_locale" />
						</el-form-item>
						<el-form-item label="Enable Capacity Limit">
							<el-switch v-model="form.activity_impose" />
						</el-form-item>
						<el-form-item label="Capacity">
							<el-input v-model="form.activity_num" />
						</el-form-item>
						<el-form-item label="Category">
							<el-radio-group v-model="form.activity_lable">
								<el-radio v-for="item in labels" :key="item" :value="item">
									{{ item }}
								</el-radio>
							</el-radio-group>
						</el-form-item>
						<el-form-item label="Type">
							<el-radio-group v-model="form.activity_type">
								<el-radio value="线上">Online</el-radio>
								<el-radio value="线下">Offline</el-radio>
							</el-radio-group>
						</el-form-item>
						<el-form-item label="Description">
							<el-input v-model="form.activity_content" type="textarea" :rows="10" />
						</el-form-item>
						<el-form-item>
							<div class="inline-actions">
								<el-button type="primary" @click="submit">Save</el-button>
								<el-button @click="router.push('/admin/createactivitylist')">Cancel</el-button>
							</div>
						</el-form-item>
					</el-form>
				</section>
			</el-tab-pane>

			<el-tab-pane v-if="isEdit" label="Announcements">
				<section class="admin-panel">
					<div class="admin-page-head">
						<div></div>
						<el-button type="primary" @click="dialogVisible = true">Create Announcement</el-button>
					</div>

					<el-dialog v-model="dialogVisible" title="Publish Announcement" width="480px">
						<el-form :model="announcement" label-width="90px">
							<el-form-item label="Title">
								<el-input v-model="announcement.announcement_name" />
							</el-form-item>
							<el-form-item label="Content">
								<el-input
									v-model="announcement.announcement_content"
									type="textarea"
									:rows="4"
								/>
							</el-form-item>
						</el-form>
						<template #footer>
							<el-button @click="dialogVisible = false">Cancel</el-button>
							<el-button type="primary" @click="publishAnnouncement">Publish</el-button>
						</template>
					</el-dialog>

					<el-timeline>
						<el-timeline-item
							v-for="item in announcementList"
							:key="item.announcement_id"
							:timestamp="formatDate(item.announcement_createtime)"
						>
							<el-card>
								<h4>{{ item.announcement_name }}</h4>
								<p>{{ item.announcement_content }}</p>
							</el-card>
						</el-timeline-item>
					</el-timeline>
				</section>
			</el-tab-pane>
			<el-tab-pane v-else label="Announcements">
				<section class="admin-panel">Save this activity first, then you can publish announcements.</section>
			</el-tab-pane>

			<el-tab-pane label="Participants" v-if="isEdit">
				<section class="admin-panel admin-table">
					<el-table :data="joinTableData" border style="width: 100%">
						<el-table-column label="Joined At" width="180">
							<template #default="scope">{{ formatDate(scope.row.joins_createtime) }}</template>
						</el-table-column>
						<el-table-column prop="username" label="Username" width="140" />
						<el-table-column prop="nickname" label="Nickname" width="140" />
						<el-table-column prop="phone" label="Phone" min-width="160" />
					</el-table>
				</section>
			</el-tab-pane>
		</el-tabs>
	</div>
</template>

<script setup>
import { computed, onMounted, ref } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { ElMessage } from 'element-plus'
import moment from 'moment'
import {
	createActivity,
	getActivityDetails,
	getAnnouncementList,
	getLabelOptions,
	getWebJoinsList,
	setAnnouncement,
	updateActivity,
} from '@/api/content'

const route = useRoute()
const router = useRouter()
const id = computed(() => route.params.id || '')
const isEdit = computed(() => !!id.value)

const labels = ref([])
const dialogVisible = ref(false)
const announcementList = ref([])
const joinTableData = ref([])

const announcement = ref({
	content_id: '',
	type: 'activity',
	announcement_content: '',
	announcement_name: '',
})

const form = ref({
	activity_title: '',
	activity_impose: false,
	activity_num: '',
	activity_type: '',
	activity_lable: '',
	activity_content: '',
	activity_statetime: '',
	activity_endtime: '',
	activity_locale: '',
})

const formatDate = value => moment(value).format('YYYY-MM-DD HH:mm')

const submit = async () => {
	if (!form.value.activity_title || !form.value.activity_locale || !form.value.activity_type || !form.value.activity_lable) {
		ElMessage.error('Please complete all required fields')
		return
	}
	if (!form.value.activity_statetime || !form.value.activity_endtime) {
		ElMessage.error('Start and end time are required')
		return
	}

	try {
		const payload = {
			...form.value,
			activity_impose: String(!!form.value.activity_impose),
		}
		const res = isEdit.value
			? await updateActivity({ ...payload, id: id.value })
			: await createActivity(payload)
		if (res.state?.type === 'SUCCESS') {
			if (isEdit.value) {
				ElMessage.success('Saved successfully')
			} else {
				ElMessage.success(
					'Event submitted successfully. It will be visible to others after admin review.',
				)
			}
			router.push('/admin/createactivitylist')
			return
		}
		ElMessage.error(res.state?.msg || 'Save failed')
	} catch {
		ElMessage.error('Save failed')
	}
}

const publishAnnouncement = async () => {
	if (!announcement.value.announcement_name || !announcement.value.announcement_content) {
		ElMessage.error('Announcement title and content are required')
		return
	}
	const res = await setAnnouncement({
		...announcement.value,
		content_id: id.value,
		contentname: form.value.activity_title,
	})
	if (res.state?.type === 'SUCCESS') {
		ElMessage.success('Announcement published')
		dialogVisible.value = false
		announcement.value.announcement_name = ''
		announcement.value.announcement_content = ''
		loadAnnouncements()
		return
	}
	ElMessage.error(res.state?.msg || 'Publish failed')
}

const loadLabels = async () => {
	labels.value = await getLabelOptions('activity')
}

const loadDetails = async () => {
	if (!isEdit.value) return
	const res = await getActivityDetails(id.value)
	if (res.state?.type !== 'SUCCESS') return
	const data = res.data || {}
	form.value = {
		...form.value,
		...data,
		activity_impose: String(data.activity_impose) === 'true',
	}
}

const loadJoins = async () => {
	if (!isEdit.value) return
	const res = await getWebJoinsList(id.value)
	if (res.state?.type === 'SUCCESS') {
		joinTableData.value = res.data || []
	}
}

const loadAnnouncements = async () => {
	if (!isEdit.value) return
	const res = await getAnnouncementList(id.value)
	if (res.state?.type === 'SUCCESS') {
		announcementList.value = res.data || []
	}
}

onMounted(() => {
	loadLabels()
	loadDetails()
	loadJoins()
	loadAnnouncements()
})
</script>
