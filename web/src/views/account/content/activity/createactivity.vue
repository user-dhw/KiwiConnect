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
				<section class="admin-panel announcement-panel">
					<div class="announcement-summary">
						<div class="announcement-summary__copy">
							<p class="announcement-summary__eyebrow">Activity Updates</p>
							<h3 class="announcement-summary__title">Manage participant announcements</h3>
							<p class="announcement-summary__text">
								Share schedule updates, reminders, and important notices with everyone who joined this activity.
							</p>
						</div>

						<div class="announcement-summary__meta">
							<div class="announcement-metric">
								<span class="announcement-metric__label">Published</span>
								<strong>{{ announcementList.length }}</strong>
							</div>
							<el-button type="primary" :disabled="!canManageAnnouncements" @click="openCreateDialog">
								Create Announcement
							</el-button>
						</div>
					</div>

					<div v-if="!canManageAnnouncements" class="announcement-empty announcement-empty--notice">
						<h4>Announcements become available after approval</h4>
						<p>
							Only verified users can publish announcements, and the activity must first pass admin review.
						</p>
					</div>

					<div v-else-if="!announcementList.length" class="announcement-empty">
						<h4>No announcements yet</h4>
						<p>Create the first update so participants can see the latest activity information.</p>
					</div>

					<div v-else class="announcement-grid">
						<article
							v-for="item in announcementList"
							:key="item.announcement_id"
							class="announcement-card"
						>
							<div class="announcement-card__header">
								<div>
									<h4>{{ item.announcement_name }}</h4>
									<p>{{ formatDate(item.announcement_createtime) }}</p>
								</div>

								<div class="announcement-card__actions">
									<el-button text type="primary" @click="openEditDialog(item)">Edit</el-button>
									<el-button text type="danger" @click="removeAnnouncement(item)">Delete</el-button>
								</div>
							</div>

							<p class="announcement-card__content">
								{{ item.announcement_content }}
							</p>
						</article>
					</div>

					<el-dialog
						v-model="dialogVisible"
						:title="isEditingAnnouncement ? 'Edit Announcement' : 'Publish Announcement'"
						width="520px"
					>
						<el-form :model="announcement" label-width="90px">
							<el-form-item label="Title">
								<el-input v-model="announcement.announcement_name" maxlength="80" show-word-limit />
							</el-form-item>
							<el-form-item label="Content">
								<el-input
									v-model="announcement.announcement_content"
									type="textarea"
									:rows="5"
									maxlength="500"
									show-word-limit
								/>
							</el-form-item>
						</el-form>
						<template #footer>
							<el-button @click="closeAnnouncementDialog">Cancel</el-button>
							<el-button type="primary" @click="publishAnnouncement">
								{{ isEditingAnnouncement ? 'Save Changes' : 'Publish' }}
							</el-button>
						</template>
					</el-dialog>
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
import { ElMessage, ElMessageBox } from 'element-plus'
import moment from 'moment'
import {
	createActivity,
	deleteAnnouncement,
	getActivityDetails,
	getAnnouncementList,
	getLabelOptions,
	getWebJoinsList,
	setAnnouncement,
	updateAnnouncement,
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
const editingAnnouncementId = ref('')

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

const canManageAnnouncements = computed(
	() => isEdit.value && Number(form.value.ispublic) === 1,
)

const isEditingAnnouncement = computed(() => !!editingAnnouncementId.value)

const formatDate = value => moment(value).format('YYYY-MM-DD HH:mm')

const resetAnnouncementForm = () => {
	editingAnnouncementId.value = ''
	announcement.value = {
		content_id: '',
		type: 'activity',
		announcement_content: '',
		announcement_name: '',
	}
}

const closeAnnouncementDialog = () => {
	dialogVisible.value = false
	resetAnnouncementForm()
}

const openCreateDialog = () => {
	resetAnnouncementForm()
	dialogVisible.value = true
}

const openEditDialog = item => {
	editingAnnouncementId.value = item.announcement_id
	announcement.value = {
		content_id: item.content_id || id.value,
		type: item.announcement_type || 'activity',
		announcement_content: item.announcement_content || '',
		announcement_name: item.announcement_name || '',
	}
	dialogVisible.value = true
}

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
	const res = isEditingAnnouncement.value
		? await updateAnnouncement({
				announcement_id: editingAnnouncementId.value,
				announcement_name: announcement.value.announcement_name,
				announcement_content: announcement.value.announcement_content,
			})
		: await setAnnouncement({
				...announcement.value,
				content_id: id.value,
				contentname: form.value.activity_title,
			})
	if (res.state?.type === 'SUCCESS') {
		ElMessage.success(
			isEditingAnnouncement.value
				? 'Announcement updated'
				: 'Announcement published',
		)
		closeAnnouncementDialog()
		loadAnnouncements()
		return
	}
	ElMessage.error(
		res.state?.msg
			|| (isEditingAnnouncement.value ? 'Update failed' : 'Publish failed'),
	)
}

const removeAnnouncement = async item => {
	try {
		await ElMessageBox.confirm(
			'Delete this announcement? Participants will no longer see it.',
			'Confirm Deletion',
			{
				type: 'warning',
				confirmButtonText: 'Delete',
				cancelButtonText: 'Cancel',
			},
		)
		const res = await deleteAnnouncement(item.announcement_id)
		if (res.state?.type === 'SUCCESS') {
			ElMessage.success('Announcement deleted')
			loadAnnouncements()
			return
		}
		ElMessage.error(res.state?.msg || 'Delete failed')
	} catch {
		// User cancelled
	}
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

<style scoped>
.announcement-panel {
	display: grid;
	gap: 20px;
}

.announcement-summary {
	display: flex;
	align-items: flex-start;
	justify-content: space-between;
	gap: 20px;
	padding: 24px 28px;
	border: 1px solid rgba(38, 99, 235, 0.12);
	border-radius: 24px;
	background: linear-gradient(180deg, #fdfefe 0%, #f6f9ff 100%);
}

.announcement-summary__eyebrow {
	margin: 0 0 8px;
	font-size: 12px;
	font-weight: 800;
	letter-spacing: 0.12em;
	text-transform: uppercase;
	color: #2663eb;
}

.announcement-summary__title {
	margin: 0 0 8px;
	font-size: 1.55rem;
	font-weight: 800;
	color: #1f2a44;
}

.announcement-summary__text {
	margin: 0;
	max-width: 560px;
	line-height: 1.7;
	color: #6b7893;
}

.announcement-summary__meta {
	display: grid;
	justify-items: end;
	gap: 14px;
}

.announcement-metric {
	min-width: 128px;
	padding: 14px 16px;
	border-radius: 20px;
	background: rgba(38, 99, 235, 0.08);
	color: #1f2a44;
	text-align: right;
}

.announcement-metric__label {
	display: block;
	margin-bottom: 6px;
	font-size: 12px;
	font-weight: 700;
	letter-spacing: 0.08em;
	text-transform: uppercase;
	color: #6b7893;
}

.announcement-metric strong {
	font-size: 1.65rem;
	line-height: 1;
}

.announcement-grid {
	display: grid;
	gap: 16px;
}

.announcement-card {
	padding: 22px 24px;
	border: 1px solid rgba(38, 99, 235, 0.12);
	border-radius: 22px;
	background: #fff;
	box-shadow: 0 14px 30px rgba(38, 99, 235, 0.06);
}

.announcement-card__header {
	display: flex;
	align-items: flex-start;
	justify-content: space-between;
	gap: 16px;
	margin-bottom: 14px;
}

.announcement-card__header h4 {
	margin: 0 0 6px;
	font-size: 1.15rem;
	font-weight: 800;
	color: #1f2a44;
}

.announcement-card__header p {
	margin: 0;
	font-size: 0.92rem;
	color: #7b88a5;
}

.announcement-card__actions {
	display: inline-flex;
	align-items: center;
	gap: 4px;
}

.announcement-card__content {
	margin: 0;
	line-height: 1.8;
	color: #43506a;
	white-space: pre-wrap;
	word-break: break-word;
}

.announcement-empty {
	padding: 38px 24px;
	border: 1px dashed rgba(38, 99, 235, 0.22);
	border-radius: 22px;
	background: linear-gradient(180deg, #fbfcff 0%, #f5f8ff 100%);
	text-align: center;
}

.announcement-empty h4 {
	margin: 0 0 8px;
	font-size: 1.15rem;
	font-weight: 800;
	color: #1f2a44;
}

.announcement-empty p {
	margin: 0;
	line-height: 1.7;
	color: #6b7893;
}

.announcement-empty--notice {
	text-align: left;
}

@media (max-width: 768px) {
	.announcement-summary {
		grid-template-columns: 1fr;
		padding: 20px;
	}

	.announcement-summary__meta {
		width: 100%;
		justify-items: stretch;
	}

	.announcement-metric {
		text-align: left;
	}

	.announcement-card {
		padding: 18px;
	}

	.announcement-card__header {
		flex-direction: column;
	}

	.announcement-card__actions {
		width: 100%;
		justify-content: flex-start;
	}
}
</style>
