<template>
    <div class="card h-100 bg-transparent rounded-0 border-0">
        <div class="card-header p-2 bg-body-subtle rounded-0 border-0">
            <div class="hstack gap-2">
                <div class="fw-bold text-warning">
                    <i class="fa-solid fa-inbox me-1"></i>
                    <span>My Workflow Tasks</span>
                    <span v-if="pendingCount > 0" class="badge bg-danger ms-2">{{ pendingCount }}</span>
                </div>
                <div class="ms-auto hstack gap-1">
                    <select class="form-select form-select-sm" v-model="statusFilter" @change="filterTasks" style="width: auto;">
                        <option value="Pending">⏳ Pending</option>
                        <option value="Completed">✅ Completed</option>
                        <option value="">All</option>
                    </select>
                    <button 
                        class="btn btn-sm btn-outline-secondary" 
                        @click="refreshTasks" 
                        title="Refresh" 
                        :disabled="loading"
                    >
                        <i class="fa-solid fa-arrows-rotate" :class="{'fa-spin': loading}"></i>
                    </button>
                    <div class="form-check form-switch mb-0">
                        <input 
                            class="form-check-input" 
                            type="checkbox" 
                            id="autoRefreshTasks" 
                            v-model="autoRefresh"
                        >
                        <label class="form-check-label small" for="autoRefreshTasks">
                            Auto-refresh (15s)
                        </label>
                    </div>
                </div>
            </div>
        </div>

        <div class="card-body p-2 overflow-auto">
            <!-- Loading State -->
            <div v-if="loading && tasks.length === 0" class="text-center py-5">
                <i class="fa-solid fa-spinner fa-spin fa-2x text-muted"></i>
                <p class="text-muted mt-2">Loading tasks...</p>
            </div>

            <!-- Empty State -->
            <div v-else-if="!loading && filteredTasks.length === 0 && statusFilter === 'Pending'" class="text-center py-5">
                <i class="fa-solid fa-check-double fa-3x text-success mb-3"></i>
                <p class="text-muted">No pending tasks</p>
                <small class="text-muted">All caught up! 🎉</small>
            </div>

            <div v-else-if="!loading && filteredTasks.length === 0" class="text-center py-5">
                <i class="fa-solid fa-inbox fa-3x text-muted mb-3"></i>
                <p class="text-muted">No tasks found</p>
            </div>

            <!-- Tasks Table -->
            <table v-else class="table table-sm table-hover mb-0">
                <thead class="table-light sticky-top">
                    <tr>
                        <th style="width: 40px;">#</th>
                        <th>Workflow</th>
                        <th>Task</th>
                        <th style="width: 150px;">Assigned On</th>
                        <th style="width: 150px;">Due Date</th>
                        <th style="width: 100px;">Status</th>
                        <th style="width: 180px;" class="text-end">Actions</th>
                    </tr>
                </thead>
                <tbody>
                    <tr v-for="(task, index) in paginatedTasks" :key="task.TaskId" :class="{'table-warning': task.IsOverdue}">
                        <td class="text-muted">{{ (currentPage - 1) * pageSize + index + 1 }}</td>
                        <td>
                            <strong>{{ task.WorkflowName }}</strong>
                            <br>
                            <small class="text-muted">{{ task.WorkflowId }}</small>
                        </td>
                        <td>
                            <div>{{ task.Title }}</div>
                            <small class="text-muted">{{ task.Description }}</small>
                        </td>
                        <td>
                            <small class="text-muted">{{ formatDate(task.AssignedOn) }}</small>
                        </td>
                        <td>
                            <small :class="task.IsOverdue ? 'text-danger fw-bold' : 'text-muted'">
                                {{ formatDate(task.DueDate) }}
                                <i v-if="task.IsOverdue" class="fa-solid fa-exclamation-triangle ms-1"></i>
                            </small>
                        </td>
                        <td>
                            <span class="badge" :class="task.Status === 'Pending' ? 'bg-warning' : 'bg-success'">
                                {{ task.Status }}
                            </span>
                        </td>
                        <td class="text-end">
                            <div v-if="task.Status === 'Pending'" class="btn-group btn-group-sm">
                                <button 
                                    class="btn btn-success" 
                                    @click="approveTask(task)"
                                    title="Approve"
                                >
                                    <i class="fa-solid fa-check me-1"></i>
                                    Approve
                                </button>
                                <button 
                                    class="btn btn-danger" 
                                    @click="rejectTask(task)"
                                    title="Reject"
                                >
                                    <i class="fa-solid fa-times me-1"></i>
                                    Reject
                                </button>
                                <button 
                                    class="btn btn-outline-info" 
                                    @click="viewTaskDetails(task)"
                                    title="View details"
                                >
                                    <i class="fa-solid fa-eye"></i>
                                </button>
                            </div>
                            <div v-else>
                                <button 
                                    class="btn btn-sm btn-outline-secondary" 
                                    @click="viewTaskDetails(task)"
                                    title="View details"
                                >
                                    <i class="fa-solid fa-eye me-1"></i>
                                    View
                                </button>
                            </div>
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>

        <!-- Pagination Footer -->
        <div class="card-footer p-2 bg-body-subtle border-0" v-if="filteredTasks.length > 0">
            <div class="hstack gap-2">
                <div class="text-muted small">
                    Showing {{ (currentPage - 1) * pageSize + 1 }} to {{ Math.min(currentPage * pageSize, filteredTasks.length) }} of {{ filteredTasks.length }} tasks
                    <span v-if="pendingCount > 0" class="text-warning fw-bold ms-2">
                        ({{ pendingCount }} pending)
                    </span>
                </div>
                <div class="ms-auto hstack gap-1">
                    <select class="form-select form-select-sm" v-model.number="pageSize" style="width: auto;" @change="currentPage = 1">
                        <option :value="10">10 per page</option>
                        <option :value="25">25 per page</option>
                        <option :value="50">50 per page</option>
                    </select>
                    <button 
                        class="btn btn-sm btn-outline-secondary" 
                        @click="currentPage--" 
                        :disabled="currentPage === 1"
                    >
                        <i class="fa-solid fa-chevron-left"></i>
                    </button>
                    <span class="badge bg-secondary">{{ currentPage }} / {{ totalPages }}</span>
                    <button 
                        class="btn btn-sm btn-outline-secondary" 
                        @click="currentPage++" 
                        :disabled="currentPage === totalPages"
                    >
                        <i class="fa-solid fa-chevron-right"></i>
                    </button>
                </div>
            </div>
        </div>

        <!-- Task Details Modal -->
        <div class="modal fade" ref="detailsModal" tabindex="-1">
            <div class="modal-dialog modal-lg">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title">
                            <i class="fa-solid fa-clipboard-list text-info me-2"></i>
                            Task Details
                        </h5>
                        <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
                    </div>
                    <div class="modal-body" v-if="selectedTask">
                        <div class="row g-3 mb-3">
                            <div class="col-md-6">
                                <label class="form-label fw-bold">Workflow</label>
                                <div>{{ selectedTask.WorkflowName }}</div>
                            </div>
                            <div class="col-md-6">
                                <label class="form-label fw-bold">Status</label>
                                <div>
                                    <span class="badge" :class="selectedTask.Status === 'Pending' ? 'bg-warning' : 'bg-success'">
                                        {{ selectedTask.Status }}
                                    </span>
                                </div>
                            </div>
                        </div>

                        <div class="mb-3">
                            <label class="form-label fw-bold">Task</label>
                            <div>{{ selectedTask.Title }}</div>
                            <small class="text-muted">{{ selectedTask.Description }}</small>
                        </div>

                        <div class="row g-3 mb-3">
                            <div class="col-md-6">
                                <label class="form-label fw-bold">Assigned On</label>
                                <div>{{ formatDate(selectedTask.AssignedOn) }}</div>
                            </div>
                            <div class="col-md-6">
                                <label class="form-label fw-bold">Due Date</label>
                                <div :class="selectedTask.IsOverdue ? 'text-danger fw-bold' : ''">
                                    {{ formatDate(selectedTask.DueDate) }}
                                    <i v-if="selectedTask.IsOverdue" class="fa-solid fa-exclamation-triangle ms-1"></i>
                                </div>
                            </div>
                        </div>

                        <!-- Context Data -->
                        <div v-if="selectedTask.Context" class="mb-3">
                            <label class="form-label fw-bold">Task Context</label>
                            <pre class="bg-light p-3 rounded border small">{{ JSON.stringify(selectedTask.Context, null, 2) }}</pre>
                        </div>

                        <!-- Comment Input (if pending) -->
                        <div v-if="selectedTask.Status === 'Pending'" class="mb-3">
                            <label class="form-label fw-bold">Comment</label>
                            <textarea 
                                class="form-control" 
                                v-model="taskComment" 
                                rows="3"
                                placeholder="Enter your comments..."
                            ></textarea>
                        </div>

                        <!-- Completion Info (if completed) -->
                        <div v-if="selectedTask.Status === 'Completed'" class="alert alert-success">
                            <div class="fw-bold">Completed by {{ selectedTask.CompletedBy }} on {{ formatDate(selectedTask.CompletedAt) }}</div>
                            <div v-if="selectedTask.Comment" class="mt-2">
                                <strong>Comment:</strong> {{ selectedTask.Comment }}
                            </div>
                            <div class="mt-2">
                                <strong>Outcome:</strong> 
                                <span class="badge" :class="selectedTask.Outcome === 'Approved' ? 'bg-success' : 'bg-danger'">
                                    {{ selectedTask.Outcome }}
                                </span>
                            </div>
                        </div>
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Close</button>
                        <div v-if="selectedTask && selectedTask.Status === 'Pending'">
                            <button type="button" class="btn btn-danger me-2" @click="confirmReject" :disabled="processing">
                                <i class="fa-solid fa-times me-1"></i>
                                Reject
                            </button>
                            <button type="button" class="btn btn-success" @click="confirmApprove" :disabled="processing">
                                <i class="fa-solid fa-check me-1" :class="{'fa-spin fa-spinner': processing}"></i>
                                Approve
                            </button>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>

<script>
shared.setAppTitle("$auto$");

let _this = { cid: "", c: null };

export default {
    setup(props) {
        _this.cid = props["cid"];
    },
    data() {
        return {
            ..._this,
            tasks: [],
            filteredTasks: [],
            statusFilter: 'Pending',
            loading: false,
            processing: false,
            autoRefresh: false,
            refreshInterval: null,
            pageSize: 25,
            currentPage: 1,
            totalCount: 0,
            selectedTask: null,
            taskComment: '',
            detailsModalInstance: null,
            currentUser: null
        };
    },
    computed: {
        paginatedTasks() {
            const start = (this.currentPage - 1) * this.pageSize;
            const end = start + this.pageSize;
            return this.filteredTasks.slice(start, end);
        },
        totalPages() {
            return Math.ceil(this.filteredTasks.length / this.pageSize);
        },
        pendingCount() {
            return this.tasks.filter(t => t.Status === 'Pending').length;
        }
    },
    watch: {
        autoRefresh(enabled) {
            if (enabled) {
                this.startAutoRefresh();
            } else {
                this.stopAutoRefresh();
            }
        }
    },
    created() {
        _this.c = this;
    },
    mounted() {
        this.detailsModalInstance = new bootstrap.Modal(this.$refs.detailsModal);
        this.currentUser = shared.getLogedInUserContext();
        this.refreshTasks();
    },
    beforeUnmount() {
        this.stopAutoRefresh();
    },
    methods: {
        async refreshTasks() {
            this.loading = true;
            try {
                // Call the real GetMyWorkflowTasks RPC endpoint
                rpcAEP("GetMyWorkflowTasks", { 
                    Status: this.statusFilter || null,
                    Page: this.currentPage,
                    PageSize: this.pageSize
                }, (result) => {
                    if (result?.success) {
                        // Map API response to component format
                        this.tasks = (result.tasks || []).map(task => ({
                            TaskId: task.TaskId,
                            WorkflowId: task.WorkflowDefinitionId,
                            WorkflowName: task.WorkflowDefinitionId,  // Could be enhanced with workflow name lookup
                            Title: task.Title,
                            Description: task.Description,
                            Status: task.Status,
                            Priority: task.Priority,
                            AssignedOn: task.CreatedAt,
                            DueDate: task.DueDate,
                            IsOverdue: task.DueDate && new Date(task.DueDate) < new Date() && task.Status === 'Pending',
                            Context: task.ContextData ? JSON.parse(task.ContextData) : null,
                            Comment: task.Comment,
                            Outcome: task.Outcome,
                            CompletedAt: task.CompletedAt,
                            CompletedBy: task.CompletedBy
                        }));
                        this.totalCount = result.totalCount || 0;
                        this.filterTasks();
                    } else {
                        shared.showNotification('Failed to load workflow tasks', 'error');
                        this.tasks = [];
                    }
                    this.loading = false;
                });
            } catch (error) {
                console.error('Failed to refresh tasks:', error);
                shared.showNotification('Failed to load workflow tasks: ' + error.message, 'error');
                this.loading = false;
            }
        },

        filterTasks() {
            if (this.statusFilter) {
                this.filteredTasks = this.tasks.filter(t => t.Status === this.statusFilter);
            } else {
                this.filteredTasks = [...this.tasks];
            }
            this.currentPage = 1;
        },

        startAutoRefresh() {
            this.refreshInterval = setInterval(() => {
                this.refreshTasks();
            }, 15000); // 15 seconds
        },

        stopAutoRefresh() {
            if (this.refreshInterval) {
                clearInterval(this.refreshInterval);
                this.refreshInterval = null;
            }
        },

        approveTask(task) {
            this.selectedTask = task;
            this.taskComment = '';
            this.detailsModalInstance.show();
        },

        rejectTask(task) {
            this.selectedTask = task;
            this.taskComment = '';
            this.detailsModalInstance.show();
        },

        viewTaskDetails(task) {
            this.selectedTask = task;
            this.taskComment = task.Comment || '';
            this.detailsModalInstance.show();
        },

        async confirmApprove() {
            await this.completeTask('Approved');
        },

        async confirmReject() {
            await this.completeTask('Rejected');
        },

        async completeTask(outcome) {
            this.processing = true;
            try {
                await new Promise((resolve, reject) => {
                    rpcAEP('CompleteWorkflowTask', {
                        TaskId: this.selectedTask.TaskId,
                        Outcome: outcome,
                        OutputParams: { comment: this.taskComment }
                    }, (data) => {
                        shared.toast(`Task ${outcome.toLowerCase()} successfully`, 'success');
                        this.detailsModalInstance.hide();
                        this.refreshTasks();
                        resolve();
                    }, (error) => {
                        shared.toast(`Failed to complete task: ${error}`, 'danger');
                        reject(error);
                    });
                });
            } catch (e) {
                console.error('Complete task error:', e);
            } finally {
                this.processing = false;
            }
        },

        formatDate(date) {
            if (!date) return '—';
            try {
                return new Date(date).toLocaleString();
            } catch {
                return '—';
            }
        }
    },
    props: { cid: String }
}
</script>

<style scoped>
.table {
    font-size: 0.9rem;
}

.table td {
    vertical-align: middle;
}

.table-warning {
    --bs-table-bg: #fff3cd;
}

.btn-group-sm > .btn {
    padding: 0.25rem 0.5rem;
}
</style>
