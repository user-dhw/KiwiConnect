<template>
	<div class="status-panel">
		<el-alert
			v-if="!restricted"
			title="This account is currently active"
			type="success"
			:closable="false"
		/>

		<template v-else>
			<el-alert
				title="This account is restricted"
				type="warning"
				:description="`Restricted until ${unlockTime}`"
				:closable="false"
			/>

			<el-skeleton :loading="loading" animated style="margin-top: 20px">
				<template #default>
					<el-descriptions :column="1" border>
						<el-descriptions-item label="Report Details">
							{{ report?.jubao_content || '-' }}
						</el-descriptions-item>
						<el-descriptions-item label="Related URL">
							<a
								v-if="report?.jubao_url"
								:href="report.jubao_url"
								target="_blank"
								rel="noopener noreferrer"
							>
								{{ report.jubao_url }}
							</a>
							<span v-else>-</span>
						</el-descriptions-item>
						<el-descriptions-item label="Review Result">
							{{ report?.result || '-' }}
						</el-descriptions-item>
						<el-descriptions-item label="Screenshots">
							<div
								v-if="
									Array.isArray(report?.jubao_img) &&
									report.jubao_img.length > 0
								"
								class="image-list"
							>
								<img
									v-for="(img, idx) in report.jubao_img"
									:key="img.id || img.url || idx"
									:src="img.url || img"
									alt="Report screenshot"
								/>
							</div>
							<span v-else>-</span>
						</el-descriptions-item>
					</el-descriptions>
				</template>
			</el-skeleton>
		</template>
	</div>
</template>

<script setup>
defineProps({
	restricted: {
		type: Boolean,
		default: false,
	},
	unlockTime: {
		type: String,
		default: '-',
	},
	report: {
		type: Object,
		default: null,
	},
	loading: {
		type: Boolean,
		default: false,
	},
})
</script>

<style scoped>
.status-panel {
	margin-top: 12px;
}

.image-list {
	display: flex;
	flex-wrap: wrap;
	gap: 10px;
}

.image-list img {
	width: 220px;
	max-width: 100%;
	border-radius: 8px;
	border: 1px solid #e5e7eb;
}
</style>
