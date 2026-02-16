<template>
    <div class="card h-100 bg-transparent rounded-0 border-0">
        <div class="card-body p-3">
            <div v-if="task" class="container-fluid">
                <div class="row mb-3">
                    <div class="col-md-6">
                        <strong>Task ID:</strong>
                        <div class="font-monospace small text-muted">{{ task.TaskId }}</div>
                    </div>
                    <div class="col-md-6">
                        <strong>Status:</strong>
                        <div>
                            <span :class="statusBadgeClass(task.Status)">
                                {{ task.Status }}
                            </span>
                        </div>
                    </div>
                </div>

                <div class="row mb-3">
                    <div class="col-md-6">
                        <strong>Assigned To:</strong>
                        <div>{{ task.AssignedTo || task.AssignedRole || 'N/A' }}</div>
                    </div>
                    <div class="col-md-6">
                        <strong>Priority:</strong>
                        <span :class="getPriorityBadgeClass(task.Priority)">
                            {{ task.Priority }}
                        </span>
                    </div>
                </div>

                <div class="row mb-3">
                    <div class="col-md-6">
                        <strong>Created:</strong>
                        <div class="small text-muted">{{ formatDate(task.CreatedAt) }}</div>
                    </div>
                    <div class="col-md-6">
                        <strong>Due Date:</strong>
                        <div class="small text-muted">{{ formatDate(task.DueDate) }}</div>
                    </div>
                </div>

                <div class="row mb-3">
                    <div class="col-12">
                        <strong>Description:</strong>
                        <p class="text-muted mt-2" style="white-space: pre-wrap;">{{ task.Description || 'N/A' }}</p>
                    </div>
                </div>

                <div v-if="task.ContextData" class="row mb-3">
                    <div class="col-12">
                        <strong>Context Data:</strong>
                        <pre class="bg-light p-2 rounded mt-2" style="max-height: 200px; overflow-y: auto; font-size: 0.85rem;">{{ task.ContextData }}</pre>
                    </div>
                </div>

                <hr>

                <div v-if="task.Status === 'Pending'" class="row">
                    <div class="col-12">
                        <label class="form-label fw-bold">Your Response</label>
                        <textarea v-model="taskResponse" class="form-control" rows="4" placeholder="Enter your response here..."></textarea>
                    </div>
                </div>
            </div>

            <div class="d-flex gap-2 justify-content-end mt-3">
                <button type="button" class="btn btn-secondary" @click="close">Close</button>
                <button v-if="task?.Status === 'Pending'" type="button" class="btn btn-danger" @click="submitReject">
                    Reject
                </button>
                <button v-if="task?.Status === 'Pending'" type="button" class="btn btn-success" @click="submitComplete">
                    Complete
                </button>
            </div>
        </div>
    </div>
</template>

<script>
    shared.setAppTitle("$auto$");
    shared.setAppSubTitle("Task Form");

    let _this = { cid: "", c: null };

    export default {
        setup(props) {
            _this.cid = props["cid"];
            _this.inputs = shared["params_" + _this.cid];
        },
        data() {
            return {
                ..._this,
                task: null,
                taskResponse: ''
            };
        },
        created() { _this.c = this; },
        mounted() {
            this.task = this.inputs?.task;
        },
        methods: {
            submitComplete() {
                if (!this.taskResponse.trim()) {
                    showError('Please provide a response');
                    return;
                }

                rpcAEP('CompleteTask', {
                    TaskId: this.task.TaskId,
                    Output: this.taskResponse
                }, (data) => {
                    const payload = Array.isArray(data) ? (data[0] || {}) : (data || {});
                    const success = payload.Success || payload.success;
                    if (success) {
                        closeComponent(this.cid, { success: true, action: 'completed' });
                    } else {
                        showError('Error: ' + (payload.ErrorMessage || 'Unknown error'));
                    }
                }, (error) => {
                    showError('Error: ' + error);
                });
            },

            submitReject() {
                if (!this.taskResponse.trim()) {
                    showError('Please provide a reason for rejection');
                    return;
                }

                rpcAEP('RejectTask', {
                    TaskId: this.task.TaskId,
                    Reason: this.taskResponse
                }, (data) => {
                    const payload = Array.isArray(data) ? (data[0] || {}) : (data || {});
                    const success = payload.Success || payload.success;
                    if (success) {
                        closeComponent(this.cid, { success: true, action: 'rejected' });
                    } else {
                        showError('Error: ' + (payload.ErrorMessage || 'Unknown error'));
                    }
                }, (error) => {
                    showError('Error: ' + error);
                });
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

            getPriorityBadgeClass(priority) {
                const baseClass = 'badge';
                switch(priority) {
                    case 'High': return baseClass + ' bg-danger';
                    case 'Medium': return baseClass + ' bg-warning text-dark';
                    case 'Low': return baseClass + ' bg-info';
                    default: return baseClass + ' bg-secondary';
                }
            },

            formatDate(dateStr) {
                if (!dateStr) return '-';
                return new Date(dateStr).toLocaleString();
            },

            close() {
                closeComponent(this.cid);
            }
        },
        props: { cid: String }
    }
</script>
