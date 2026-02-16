<template>
    <div class="card h-100 bg-transparent rounded-0 border-0">
        <div class="card-header p-2 bg-body-subtle rounded-0 border-0">
            <div class="hstack gap-2">
                <div class="fw-bold text-primary">
                    <i class="fa-solid fa-diagram-project me-1"></i>
                    <span>Workflow Definitions</span>
                </div>
                <div class="ms-auto hstack gap-1">
                    <input 
                        type="text" 
                        class="form-control form-control-sm" 
                        v-model="searchQuery" 
                        placeholder="Search workflows..."
                        @input="filterWorkflows"
                        style="width: 200px;"
                    >
                    <button class="btn btn-sm btn-outline-secondary" @click="refreshWorkflows" title="Refresh" :disabled="loading">
                        <i class="fa-solid fa-arrows-rotate" :class="{'fa-spin': loading}"></i>
                    </button>
                    <button class="btn btn-sm btn-primary" @click="showCreateModal" title="Create new workflow">
                        <i class="fa-solid fa-plus me-1"></i>
                        New Workflow
                    </button>
                </div>
            </div>
        </div>

        <div class="card-body p-2 overflow-auto">
            <!-- Loading State -->
            <div v-if="loading && workflows.length === 0" class="text-center py-5">
                <i class="fa-solid fa-spinner fa-spin fa-2x text-muted"></i>
                <p class="text-muted mt-2">Loading workflows...</p>
            </div>

            <!-- Empty State -->
            <div v-else-if="!loading && filteredWorkflows.length === 0 && searchQuery === ''" class="text-center py-5">
                <i class="fa-solid fa-diagram-project fa-3x text-muted mb-3"></i>
                <p class="text-muted">No workflows found</p>
                <button class="btn btn-sm btn-primary" @click="showCreateModal">
                    <i class="fa-solid fa-plus me-1"></i>
                    Create your first workflow
                </button>
            </div>

            <!-- No Search Results -->
            <div v-else-if="!loading && filteredWorkflows.length === 0 && searchQuery !== ''" class="text-center py-5">
                <i class="fa-solid fa-search fa-3x text-muted mb-3"></i>
                <p class="text-muted">No workflows match "{{ searchQuery }}"</p>
            </div>

            <!-- Workflows Table -->
            <table v-else class="table table-sm table-hover mb-0">
                <thead class="table-light sticky-top">
                    <tr>
                        <th style="width: 40px;">#</th>
                        <th>Name</th>
                        <th>Description</th>
                        <th style="width: 100px;">Status</th>
                        <th style="width: 80px;">Version</th>
                        <th style="width: 150px;">Last Modified</th>
                        <th style="width: 200px;" class="text-end">Actions</th>
                    </tr>
                </thead>
                <tbody>
                    <tr v-for="(workflow, index) in paginatedWorkflows" :key="workflow.Id">
                        <td class="text-muted">{{ (currentPage - 1) * pageSize + index + 1 }}</td>
                        <td>
                            <strong>{{ workflow.Name }}</strong>
                            <br>
                            <small class="text-muted">{{ workflow.Id }}</small>
                        </td>
                        <td>
                            <span class="text-muted">{{ workflow.Description || '—' }}</span>
                        </td>
                        <td>
                            <span class="badge" :class="workflow.IsPublished ? 'bg-success' : 'bg-warning'">
                                <i class="fa-solid" :class="workflow.IsPublished ? 'fa-check-circle' : 'fa-clock'"></i>
                                {{ workflow.IsPublished ? 'Published' : 'Draft' }}
                            </span>
                        </td>
                        <td class="text-center">
                            <span class="badge bg-secondary">v{{ workflow.Version }}</span>
                        </td>
                        <td>
                            <small class="text-muted">{{ formatDate(workflow.LoadedAt) }}</small>
                        </td>
                        <td class="text-end">
                            <div class="btn-group btn-group-sm">
                                <button 
                                    class="btn btn-outline-success" 
                                    @click="executeWorkflow(workflow)" 
                                    :disabled="!workflow.IsPublished"
                                    title="Execute workflow"
                                >
                                    <i class="fa-solid fa-play"></i>
                                </button>
                                <button 
                                    class="btn btn-outline-primary" 
                                    @click="editWorkflow(workflow)"
                                    title="Edit workflow"
                                >
                                    <i class="fa-solid fa-pen"></i>
                                </button>
                                <button 
                                    class="btn btn-outline-warning" 
                                    @click="togglePublish(workflow)"
                                    :title="workflow.IsPublished ? 'Retract workflow' : 'Publish workflow'"
                                >
                                    <i class="fa-solid" :class="workflow.IsPublished ? 'fa-arrow-down' : 'fa-arrow-up'"></i>
                                </button>
                                <button 
                                    class="btn btn-outline-info" 
                                    @click="reloadWorkflow(workflow)"
                                    title="Reload from disk"
                                >
                                    <i class="fa-solid fa-sync"></i>
                                </button>
                                <button 
                                    class="btn btn-outline-danger" 
                                    @click="deleteWorkflow(workflow)"
                                    title="Delete workflow"
                                >
                                    <i class="fa-solid fa-trash"></i>
                                </button>
                            </div>
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>

        <!-- Pagination Footer -->
        <div class="card-footer p-2 bg-body-subtle border-0" v-if="filteredWorkflows.length > 0">
            <div class="hstack gap-2">
                <div class="text-muted small">
                    Showing {{ (currentPage - 1) * pageSize + 1 }} to {{ Math.min(currentPage * pageSize, filteredWorkflows.length) }} of {{ filteredWorkflows.length }} workflows
                </div>
                <div class="ms-auto hstack gap-1">
                    <select class="form-select form-select-sm" v-model.number="pageSize" style="width: auto;" @change="currentPage = 1">
                        <option :value="10">10 per page</option>
                        <option :value="25">25 per page</option>
                        <option :value="50">50 per page</option>
                        <option :value="100">100 per page</option>
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

        <!-- Execute Workflow Modal -->
        <div class="modal fade" ref="executeModal" tabindex="-1">
            <div class="modal-dialog">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title">
                            <i class="fa-solid fa-play-circle text-success me-2"></i>
                            Execute Workflow
                        </h5>
                        <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
                    </div>
                    <div class="modal-body" v-if="selectedWorkflow">
                        <div class="mb-3">
                            <strong>{{ selectedWorkflow.Name }}</strong>
                            <p class="text-muted small mb-0">{{ selectedWorkflow.Description }}</p>
                        </div>
                        <div class="mb-3">
                            <label class="form-label">Input Parameters (JSON)</label>
                            <textarea 
                                class="form-control font-monospace small" 
                                v-model="executionParams" 
                                rows="8"
                                placeholder='{"key": "value"}'
                            ></textarea>
                            <div class="form-text">Enter input parameters as JSON object</div>
                        </div>
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Cancel</button>
                        <button type="button" class="btn btn-success" @click="confirmExecute" :disabled="executing">
                            <i class="fa-solid fa-play me-1" :class="{'fa-spin fa-spinner': executing}"></i>
                            Execute
                        </button>
                    </div>
                </div>
            </div>
        </div>

        <!-- Edit Workflow Modal -->
        <div class="modal fade" ref="editModal" tabindex="-1">
            <div class="modal-dialog modal-lg">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title">
                            <i class="fa-solid fa-pen text-primary me-2"></i>
                            {{ editMode === 'create' ? 'Create Workflow' : 'Edit Workflow' }}
                        </h5>
                        <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
                    </div>
                    <div class="modal-body" v-if="editingWorkflow">
                        <div class="mb-3">
                            <label class="form-label">Workflow ID</label>
                            <input 
                                type="text" 
                                class="form-control" 
                                v-model="editingWorkflow.Id" 
                                :disabled="editMode === 'edit'"
                                placeholder="e.g., my-workflow"
                            >
                        </div>
                        <div class="mb-3">
                            <label class="form-label">Name</label>
                            <input 
                                type="text" 
                                class="form-control" 
                                v-model="editingWorkflow.Name"
                                placeholder="e.g., My Workflow"
                            >
                        </div>
                        <div class="mb-3">
                            <label class="form-label">Description</label>
                            <textarea 
                                class="form-control" 
                                v-model="editingWorkflow.Description" 
                                rows="3"
                                placeholder="Describe what this workflow does..."
                            ></textarea>
                        </div>
                        <div class="alert alert-info">
                            <i class="fa-solid fa-info-circle me-1"></i>
                            Workflow activities must be edited in the JSON file located at <code>workspace/workflows/{{ editingWorkflow.Id }}.json</code>
                        </div>
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Cancel</button>
                        <button type="button" class="btn btn-primary" @click="confirmEdit" :disabled="saving">
                            <i class="fa-solid fa-save me-1" :class="{'fa-spin fa-spinner': saving}"></i>
                            {{ editMode === 'create' ? 'Create' : 'Save Changes' }}
                        </button>
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
            workflows: [],
            filteredWorkflows: [],
            searchQuery: '',
            loading: false,
            executing: false,
            saving: false,
            pageSize: 25,
            currentPage: 1,
            selectedWorkflow: null,
            editingWorkflow: null,
            editMode: 'edit', // 'edit' or 'create'
            executionParams: '{}',
            executeModalInstance: null,
            editModalInstance: null
        };
    },
    computed: {
        paginatedWorkflows() {
            const start = (this.currentPage - 1) * this.pageSize;
            const end = start + this.pageSize;
            return this.filteredWorkflows.slice(start, end);
        },
        totalPages() {
            return Math.ceil(this.filteredWorkflows.length / this.pageSize);
        }
    },
    created() {
        _this.c = this;
    },
    mounted() {
        this.executeModalInstance = new bootstrap.Modal(this.$refs.executeModal);
        this.editModalInstance = new bootstrap.Modal(this.$refs.editModal);
        this.refreshWorkflows();
    },
    methods: {
        async refreshWorkflows() {
            this.loading = true;
            try {
                await new Promise((resolve, reject) => {
                    rpcAEP('GetWorkflowDefinitions', {}, (data) => {
                        this.workflows = data || [];
                        this.filterWorkflows();
                        resolve();
                    }, (error) => {
                        console.error('Failed to load workflows:', error);
                        shared.toast('Failed to load workflows', 'danger');
                        reject(error);
                    });
                });
            } finally {
                this.loading = false;
            }
        },

        filterWorkflows() {
            if (!this.searchQuery) {
                this.filteredWorkflows = [...this.workflows];
            } else {
                const query = this.searchQuery.toLowerCase();
                this.filteredWorkflows = this.workflows.filter(w => 
                    w.Name.toLowerCase().includes(query) ||
                    w.Id.toLowerCase().includes(query) ||
                    (w.Description && w.Description.toLowerCase().includes(query))
                );
            }
            this.currentPage = 1;
        },

        executeWorkflow(workflow) {
            this.selectedWorkflow = workflow;
            this.executionParams = '{}';
            this.executeModalInstance.show();
        },

        async confirmExecute() {
            this.executing = true;
            try {
                let params = {};
                try {
                    params = JSON.parse(this.executionParams);
                } catch (e) {
                    shared.toast('Invalid JSON in input parameters', 'danger');
                    return;
                }

                await new Promise((resolve, reject) => {
                    rpcAEP('ExecuteWorkflow', {
                        WorkflowId: this.selectedWorkflow.Id,
                        InputParams: params
                    }, (data) => {
                        const result = Array.isArray(data) ? data[0] : data;
                        if (result && result.Success) {
                            shared.toast(`Workflow "${this.selectedWorkflow.Name}" executed successfully`, 'success');
                            console.log('Execution result:', result);
                        } else {
                            shared.toast(`Execution failed: ${result?.ErrorMessage || 'Unknown error'}`, 'danger');
                        }
                        this.executeModalInstance.hide();
                        resolve();
                    }, (error) => {
                        shared.toast(`Execution failed: ${error}`, 'danger');
                        reject(error);
                    });
                });
            } finally {
                this.executing = false;
            }
        },

        editWorkflow(workflow) {
            this.editMode = 'edit';
            this.editingWorkflow = { ...workflow };
            this.editModalInstance.show();
        },

        showCreateModal() {
            this.editMode = 'create';
            this.editingWorkflow = {
                Id: '',
                Name: '',
                Description: '',
                Version: 1,
                IsPublished: false
            };
            this.editModalInstance.show();
        },

        async confirmEdit() {
            // Note: This is a placeholder. Actual implementation would need
            // server-side support for creating/updating workflow JSON files
            shared.toast('Edit/Create functionality requires file system access - not implemented in this demo', 'warning');
            this.editModalInstance.hide();
        },

        async togglePublish(workflow) {
            // Note: This is a placeholder. Actual implementation would need
            // server-side support for publishing/retracting workflows
            const action = workflow.IsPublished ? 'retract' : 'publish';
            shared.toast(`Publish/Retract functionality not yet implemented`, 'info');
        },

        async reloadWorkflow(workflow) {
            try {
                await new Promise((resolve, reject) => {
                    rpcAEP('ReloadWorkflow', { WorkflowId: workflow.Id }, (data) => {
                        shared.toast(`Workflow "${workflow.Name}" reloaded successfully`, 'success');
                        this.refreshWorkflows();
                        resolve();
                    }, (error) => {
                        shared.toast(`Reload failed: ${error}`, 'danger');
                        reject(error);
                    });
                });
            } catch (e) {
                console.error('Reload error:', e);
            }
        },

        async deleteWorkflow(workflow) {
            if (!confirm(`Are you sure you want to delete workflow "${workflow.Name}"?`)) {
                return;
            }
            // Note: This is a placeholder. Actual implementation would need
            // server-side support for deleting workflow files
            shared.toast('Delete functionality requires file system access - not implemented in this demo', 'warning');
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

.btn-group-sm > .btn {
    padding: 0.25rem 0.5rem;
}
</style>
