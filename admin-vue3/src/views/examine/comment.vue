<template>
  <div class="comment">
    <el-main>
      <div class="top">
        <el-breadcrumb separator="/">
          <el-breadcrumb-item :to="{ path: '/' }">
            Home
          </el-breadcrumb-item>
          <el-breadcrumb-item>Review Center</el-breadcrumb-item>
          <el-breadcrumb-item>Comment Review</el-breadcrumb-item>
        </el-breadcrumb>
      </div>

      <div class="main">
        <div class="search">
          <el-form
            :inline="true"
            :model="pagelistquery"
            class="demo-form-inline"
          >
            <el-form-item>
              <el-input
                v-model="pagelistquery.admin"
                placeholder="Reviewer"
              />
            </el-form-item>
            <el-form-item>
              <el-select
                v-model="pagelistquery.state"
                :teleported="false"
                style="width: 180px"
                placeholder="Status"
              >
                <el-option
                  label="All"
                  :value="''"
                />
                <el-option
                  label="Pending"
                  value="0"
                />
                <el-option
                  label="Approved"
                  value="1"
                />

                <el-option
                  label="Rejected"
                  value="-1"
                />
              </el-select>
            </el-form-item>

            <el-form-item>
              <el-button
                type="primary"
                @click="getcommentlist"
              >
                Search
              </el-button>
            </el-form-item>
          </el-form>
        </div>

        <el-table
          v-loading="loading"
          :data="tableData"
          border
          style="width: 100%; min-height: 500px"
          element-loading-text="Loading..."
        >
          <el-table-column type="expand">
            <template #default="props">
              <el-table
                :data="props.row.child || []"
                style="width: 100%"
              >
                <el-table-column
                  type="index"
                  width="50"
                />
                <el-table-column
                  prop="createtime"
                  label="Created At"
                >
                  <template #default="scope">
                    {{
                      formatDate(scope.row.comment_createtime)
                    }}
                  </template>
                </el-table-column>
                <el-table-column
                  label="Replier"
                  prop="nickname"
                />
                <el-table-column
                  label="Replied User"
                  prop="tousernickname"
                />
                <el-table-column
                  label="Status"
                  prop="comment_ispublic"
                >
                  <template #default="scope">
                    <span
                      v-if="scope.row.ispublic == 1"
                      style="color: #6cbb7a"
                    >Approved</span>
                    <span
                      v-if="scope.row.ispublic == 0"
                      style="color: #409eff"
                    >Pending</span>
                    <span
                      v-if="scope.row.ispublic == -1"
                      style="color: #f60c6c"
                    >Rejected</span>
                  </template>
                </el-table-column>
                <el-table-column
                  label="Reply Content"
                  prop="desc"
                >
                  <template #default="scope">
                    <el-popover
                      placement="right"
                      width="400"
                      trigger="hover"
                    >
                      <span
                        v-html="scope.row.reply_content"
                      />
                      <template #reference>
                        <el-button
                          type="primary"
                          link
                        >
                          View Reply
                        </el-button>
                      </template>
                    </el-popover>
                  </template>
                </el-table-column>
                <el-table-column
                  label="Reviewer"
                  prop="admin"
                />

                <el-table-column
                  fixed="right"
                  label="Actions"
                  width="250"
                >
                  <template #default="scope">
                    <el-button
                      v-if="scope.row.ispublic == 1"
                      type="primary"
                      link
                      size="small"
                      @click="
                        changestate(
                          'reply',
                          -1,
                          scope.row.reply_id,
                        )
                      "
                    >
                      Reject
                    </el-button>
                    <el-button
                      v-if="scope.row.ispublic == 0"
                      type="primary"
                      link
                      size="small"
                      @click="
                        changestate(
                          'reply',
                          1,
                          scope.row.reply_id,
                        )
                      "
                    >
                      Approve
                    </el-button>
                    <el-button
                      v-if="scope.row.ispublic == 0"
                      type="primary"
                      link
                      size="small"
                      @click="
                        changestate(
                          'reply',
                          -1,
                          scope.row.reply_id,
                        )
                      "
                    >
                      Reject
                    </el-button>
                    <el-button
                      v-if="scope.row.ispublic == -1"
                      type="primary"
                      link
                      size="small"
                      @click="
                        changestate(
                          'reply',
                          1,
                          scope.row.reply_id,
                        )
                      "
                    >
                      Approve
                    </el-button>
                    <el-button
                      type="danger"
                      link
                      size="small"
                      :disabled="!isSuperAdmin"
                      @click="
                        del(scope.row.reply_id, 'reply')
                      "
                    >
                      Delete
                    </el-button>
                  </template>
                </el-table-column>
              </el-table>
            </template>
          </el-table-column>
          <el-table-column
            prop="createtime"
            label="Created At"
          >
            <template #default="scope">
              {{
                formatDate(scope.row.comment_createtime)
              }}
            </template>
          </el-table-column>
          <el-table-column
            label="Commenter"
            prop="nickname"
          />
          <el-table-column
            label="Status"
            prop="comment_ispublic"
          >
            <template #default="scope">
              <span
                v-if="scope.row.ispublic == 1"
                style="color: #6cbb7a"
              >Approved</span>
              <span
                v-if="scope.row.ispublic == 0"
                style="color: #409eff"
              >Pending</span>
              <span
                v-if="scope.row.ispublic == -1"
                style="color: #f60c6c"
              >Rejected</span>
            </template>
          </el-table-column>
          <el-table-column
            label="Comment Content"
            prop="desc"
          >
            <template #default="scope">
              <el-popover
                placement="right"
                width="400"
                trigger="hover"
              >
                <span v-html="scope.row.comment_content" />
                <template #reference>
                  <el-button
                    type="primary"
                    link
                  >
                    View Comment
                  </el-button>
                </template>
              </el-popover>
            </template>
          </el-table-column>
          <el-table-column
            label="Reviewer"
            prop="admin"
          />

          <el-table-column
            fixed="right"
            label="Actions"
            width="200"
          >
            <template #default="scope">
              <el-button
                v-if="scope.row.ispublic == 1"
                type="primary"
                link
                size="small"
                @click="
                  changestate(
                    'comment',
                    -1,
                    scope.row.comment_id,
                  )
                "
              >
                Reject
              </el-button>
              <el-button
                v-if="scope.row.ispublic == 0"
                type="primary"
                link
                size="small"
                @click="
                  changestate(
                    'comment',
                    1,
                    scope.row.comment_id,
                  )
                "
              >
                Approve
              </el-button>
              <el-button
                v-if="scope.row.ispublic == 0"
                type="primary"
                link
                size="small"
                @click="
                  changestate(
                    'comment',
                    -1,
                    scope.row.comment_id,
                  )
                "
              >
                Reject
              </el-button>
              <el-button
                v-if="scope.row.ispublic == -1"
                type="primary"
                link
                size="small"
                @click="
                  changestate(
                    'comment',
                    1,
                    scope.row.comment_id,
                  )
                "
              >
                Approve
              </el-button>
              <el-button
                type="danger"
                link
                size="small"
                :disabled="!isSuperAdmin"
                @click="del(scope.row.comment_id, 'comment')"
              >
                Delete
              </el-button>
            </template>
          </el-table-column>
        </el-table>

        <!-- Pagination -->
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
import { ElMessage } from 'element-plus'
import { useUserStore } from '../../store/modules/user'
import axios from '../../utils/axios'
import { formatDate } from '../../utils/dateFormat'
import { toFormData } from '../../utils/form'

const userStore = useUserStore()

const loading = ref(false)
const tableData = ref([])
const pagelistquery = reactive({
	admin: '',
	state: '',
	total: 0,
	page: 1,
	pagesize: 10,
})

const uinfo = computed(() => userStore.uinfo || {})
const isSuperAdmin = computed(() => uinfo.value?.username === 'admin')

const getreply = async () => {
	if (!tableData.value.length) return

	const mergedRows = await Promise.all(
		tableData.value.map(async item => {
			try {
				const res = await axios.post(
					'/admin/getreply',
					toFormData({ comment_id: item.comment_id }),
				)
				const child =
					res.data?.state?.type === 'SUCCESS' ? res.data.data : []
				return { ...item, child }
			} catch {
				return { ...item, child: [] }
			}
		}),
	)

	tableData.value = mergedRows
}

const changestate = async (type, state, id) => {
	try {
		const res = await axios.post(
			'/admin/changecontentstate',
			toFormData({ id, type, state }),
		)

		if (res.data?.state?.type === 'SUCCESS') {
			ElMessage.success('Operation successful')
			await getcommentlist()
			return
		}

		ElMessage.error(res.data?.state?.msg || 'Operation failed')
	} catch {
		ElMessage.error('Operation failed')
	}
}

const handleSizeChange = val => {
	pagelistquery.pagesize = val
	getcommentlist()
}

const handleCurrentChange = val => {
	pagelistquery.page = val
	getcommentlist()
}

const del = async (id, type) => {
	try {
		const res = await axios.post(
			'/admin/admindelete',
			toFormData({ id, type }),
		)
		if (res.data?.state?.type === 'SUCCESS') {
			ElMessage.success('Deleted successfully')
			await getcommentlist()
			return
		}
		ElMessage.error(res.data?.state?.msg || 'Delete failed')
	} catch {
		ElMessage.error('Delete failed')
	}
}

const getcommentlist = async () => {
	loading.value = true
	try {
		const res = await axios.post(
			'/admin/getcomment',
			toFormData(pagelistquery),
		)
		if (res.data?.state?.type === 'SUCCESS') {
			tableData.value = res.data.data || []
			pagelistquery.total = res.data.count || 0
			await getreply()
			return
		}
		ElMessage.error(res.data?.state?.msg || 'Failed to load data')
	} catch {
		ElMessage.error('Failed to load data')
	} finally {
		loading.value = false
	}
}

onMounted(() => {
	getcommentlist()
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
.comment {
	position: relative;
	width: 100%;
}
.main {
	margin-top: 40px;
	padding: 20px;
	background-color: #fff;
}
</style>
