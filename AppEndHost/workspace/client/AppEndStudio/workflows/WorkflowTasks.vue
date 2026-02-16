<template>
    <div class="card h-100 bg-transparent rounded-0 border-0">
        <div class="card-header p-2 bg-body-subtle rounded-0 border-0">
            <div class="hstack gap-2">
                <div class="fw-bold text-primary">
                    <i class="fa-solid fa-tasks me-1"></i>
                    <span>My Tasks</span>
                </div>
                <div class="ms-auto">
                    <select v-model="filterStatus" class="form-select form-select-sm" style="width: auto;">
                        <option value="">All Status</option>
                        <option value="Pending">Pending</option>
                        <option value="InProgress">In Progress</option>
                        <option value="Completed">Completed</option>
                        <option value="Rejected">Rejected</option>
                    </select>
                </div>
            </div>
        </div>

        <div class="card-body p-2">
            <div class="container-fluid">
                <div v-if="filteredTasks.length === 0" class="alert alert-info mb-0">
                    <i class="fas fa-check-circle me-2"></i>No tasks to show. You're all caught up!
                </div>

                <div v-else class="table-responsive">
                    <table class="table table-hover table-sm mb-0">
                        <thead class="table-light">
                            <tr>
                                <th style="width: 25%">Task</th>
                                <th style="width: 20%">Workflow</th>
                                <th style="width: 15%">Status</th>
                                <th style="width: 15%">Created</th>
                                <th style="width: 15%">Due Date</th>
                                <th style="width: 10%">Actions</th>
                            </tr>
                        </thead>
                        <tbody>
                            <tr v-for="task in filteredTasks" :key="task.TaskId" class="align-middle">
                                <td>
                                    <strong>{{ task.Title }}</strong>
                                    <div class="small text-muted">{{ task.Description }}</div>
                                </td>
                                <td class="small">{{ task.WorkflowDefinitionId }}</td>
                                <td>
                                    <span :class="statusBadgeClass(task.Status)">
                                        {{ task.Status }}
                                    </span>
                                </td>
                                <td class="small text-muted">{{ formatDate(task.CreatedAt) }}</td>
                                <td class="small" :class="isDueSoon(task.DueDate) ? 'text-danger fw-bold' : 'text-muted'">
                                    {{ formatDate(task.DueDate) }}
                                </td>
                                <td>
                                    <div class="btn-group btn-group-sm" role="group">
                                        <button class="btn btn-outline-primary" @click="openTaskForm(task)" title="Open">
                                            <i class="fas fa-arrow-right"></i>
                                        </button>
                                        <button v-if="task.Status === 'Pending'" class="btn btn-outline-success" @click="completeTask(task)" title="Complete">
                                            <i class="fas fa-check"></i>
                                        </button>
                                        <button v-if="task.Status === 'Pending'" class="btn btn-outline-danger" @click="rejectTask(task)" title="Reject">
                                            <i class="fas fa-times"></i>
                                        </button>
                                    </div>
                                </td>
                            </tr>
                        </tbody>
                    </table>
                </div>
            </div>
        </div>
    </div>
</template>

<script>
    shared.setAppTitle("$auto$");
    shared.setAppSubTitle("Task Management");

    let _this = { cid: "", c: null };

    export default {
        setup(props) {
            _this.cid = props["cid"];
        },
        data() {
            return {
                ..._this,
                tasks: [],
                filterStatus: ''
            };
        },
        computed: {
            filteredTasks() {
                if (!this.filterStatus) return this.tasks;
                return this.tasks.filter(t => t.Status === this.filterStatus);
            }
        },
        created() { _this.c = this; },
        mounted() {
            this.loadTasks();
            setInterval(() => this.loadTasks(), 15000);
        },
        methods: {
            loadTasks() {
                rpcAEP('GetMyTasks', { Status: '', Page: 1, PageSize: 50 }, (data) => {
                    const payload = Array.isArray(data) ? (data[0] || {}) : (data || {});
                    const list = payload.Result || payload.result || payload.tasks || payload.Tasks || payload;
                    this.tasks = Array.isArray(list) ? list : [];
                }, (error) => {
                    console.error('Error loading tasks:', error);
                });
            },

            openTaskForm(task) {
                openComponent("/AppEndStudio/workflows/WorkflowTaskForm", {
                    title: task.Title,
                    modalSize: "modal-lg",
                    windowSizeSwitchable: true,
                    params: {
                        task: task
                    },
                    caller: this,
                    callback: function(result) {
                        if (result?.success) {
                            const action = result.action;
                            if (action === 'completed') {
                                showSuccess('Task completed successfully');
                            } else if (action === 'rejected') {
                                showSuccess('Task rejected');
                            }
                            this.loadTasks();
                        }
                    }
                });
            },

            completeTask(task) {
                this.openTaskForm(task);
            },

            rejectTask(task) {
                this.openTaskForm(task);
            },

            statusBadgeClass(status) {
                const baseClass = 'badge';
                switch(status) {
                    case 'Pending': return baseClass + ' bg-warning text-dark';
                    case 'InProgress': return baseClass + ' bg-info';
                    case 'Completed': return baseClass + ' bg-success';
                    case 'Rejected': return baseClass + ' bg-danger';
                    default: return baseClass + ' bg-secondary';
                }
            },

            formatDate(dateStr) {
                if (!dateStr) return '-';
                return new Date(dateStr).toLocaleString();
            },

            isDueSoon(dueDate) {
                if (!dueDate) return false;
                const due = new Date(dueDate);
                const now = new Date();
                const diffMs = due - now;
                const diffDays = Math.ceil(diffMs / (1000 * 60 * 60 * 24));
                return diffDays <= 1 && diffDays > 0;
            }
        },
        props: { cid: String }
    }
</script>

<style>
.btn-group-sm .btn {
    padding: 0.25rem 0.5rem;
    font-size: 0.75rem;
}
</style>
