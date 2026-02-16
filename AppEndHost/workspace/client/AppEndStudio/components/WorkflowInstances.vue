<template>
    <div class="card h-100 bg-transparent rounded-0 border-0">
        <div class="card-header p-2 bg-body-subtle rounded-0 border-0">
            <div class="hstack gap-2">
                <div class="fw-bold text-info">
                    <i class="fa-solid fa-list-check me-1"></i>
                    <span>Workflow Instances</span>
                </div>
                <div class="ms-auto hstack gap-1">
                    <select class="form-select form-select-sm" v-model="statusFilter" @change="filterInstances" style="width: auto;">
                        <option value="">All Statuses</option>
                        <option value="Running">🔄 Running</option>
                        <option value="Completed">✅ Completed</option>
                        <option value="Faulted">❌ Faulted</option>
                        <option value="Cancelled">🚫 Cancelled</option>
                        <option value="Suspended">⏸️ Suspended</option>
                    </select>
                    <input 
                        type="text" 
                        class="form-control form-control-sm" 
                        v-model="searchQuery" 
                        placeholder="Search by workflow name..."
                        @input="filterInstances"
                        style="width: 200px;"
                    >
                    <button 
                        class="btn btn-sm btn-outline-secondary" 
                        @click="refreshInstances" 
                        title="Refresh" 
                        :disabled="loading"
                    >
                        <i class="fa-solid fa-arrows-rotate" :class="{'fa-spin': loading}"></i>
                    </button>
                    <div class="form-check form-switch mb-0">
                        <input 
                            class="form-check-input" 
                            type="checkbox" 
                            id="autoRefresh" 
                            v-model="autoRefresh"
                        >
                        <label class="form-check-label small" for="autoRefresh">
                            Auto-refresh (10s)
                        </label>
                    </div>
                </div>
            </div>
        </div>

        <div class="card-body p-2 overflow-auto">
            <!-- Loading State -->
            <div v-if="loading && instances.length === 0" class="text-center py-5">
                <i class="fa-solid fa-spinner fa-spin fa-2x text-muted"></i>
                <p class="text-muted mt-2">Loading instances...</p>
            </div>

            <!-- Empty State -->
            <div v-else-if="!loading && filteredInstances.length === 0" class="text-center py-5">
                <i class="fa-solid fa-list-check fa-3x text-muted mb-3"></i>
                <p class="text-muted">No workflow instances found</p>
                <small class="text-muted">Execute workflows from the Workflow Definitions page</small>
            </div>

            <!-- Instances Table -->
            <table v-else class="table table-sm table-hover mb-0">
                <thead class="table-light sticky-top">
                    <tr>
                        <th style="width: 40px;">#</th>
                        <th>Workflow</th>
                        <th>Instance ID</th>
                        <th style="width: 120px;">Status</th>
                        <th style="width: 150px;">Started</th>
                        <th style="width: 150px;">Completed</th>
                        <th style="width: 100px;">Duration</th>
                        <th style="width: 120px;" class="text-end">Actions</th>
                    </tr>
                </thead>
                <tbody>
                    <tr v-for="(instance, index) in paginatedInstances" :key="instance.InstanceId">
                        <td class="text-muted">{{ (currentPage - 1) * pageSize + index + 1 }}</td>
                        <td>
                            <strong>{{ instance.WorkflowName }}</strong>
                            <br>
                            <small class="text-muted">{{ instance.DefinitionId }}</small>
                        </td>
                        <td>
                            <code class="small">{{ truncateId(instance.InstanceId) }}</code>
                        </td>
                        <td>
                            <span class="badge" :class="getStatusBadgeClass(instance.Status)">
                                <i class="fa-solid" :class="getStatusIcon(instance.Status)"></i>
                                {{ instance.Status }}
                            </span>
                        </td>
                        <td>
                            <small class="text-muted">{{ formatDate(instance.StartedAt) }}</small>
                        </td>
                        <td>
                            <small class="text-muted">{{ formatDate(instance.CompletedAt) }}</small>
                        </td>
                        <td>
                            <small class="text-muted">{{ formatDuration(instance.StartedAt, instance.CompletedAt) }}</small>
                        </td>
                        <td class="text-end">
                            <div class="btn-group btn-group-sm">
                                <button 
                                    class="btn btn-outline-primary" 
                                    @click="viewDetails(instance)"
                                    title="View details"
                                >
                                    <i class="fa-solid fa-eye"></i>
                                </button>
                                <button 
                                    v-if="instance.Status === 'Running' || instance.Status === 'Suspended'"
                                    class="btn btn-outline-danger" 
                                    @click="cancelInstance(instance)"
                                    title="Cancel instance"
                                >
                                    <i class="fa-solid fa-stop"></i>
                                </button>
                            </div>
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>

        <!-- Pagination Footer -->
        <div class="card-footer p-2 bg-body-subtle border-0" v-if="filteredInstances.length > 0">
            <div class="hstack gap-2">
                <div class="text-muted small">
                    Showing {{ (currentPage - 1) * pageSize + 1 }} to {{ Math.min(currentPage * pageSize, filteredInstances.length) }} of {{ filteredInstances.length }} instances
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

        <!-- Instance Details Modal -->
        <div class="modal fade" ref="detailsModal" tabindex="-1">
            <div class="modal-dialog modal-xl">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title">
                            <i class="fa-solid fa-info-circle text-info me-2"></i>
                            Workflow Instance Details
                        </h5>
                        <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
                    </div>
                    <div class="modal-body" v-if="selectedInstance">
                        <!-- Summary Cards -->
                        <div class="row g-3 mb-4">
                            <div class="col-md-3">
                                <div class="card border-start border-5 border-primary">
                                    <div class="card-body">
                                        <div class="small text-muted">Workflow</div>
                                        <div class="fw-bold">{{ selectedInstance.WorkflowName }}</div>
                                    </div>
                                </div>
                            </div>
                            <div class="col-md-3">
                                <div class="card border-start border-5" :class="'border-' + getStatusColor(selectedInstance.Status)">
                                    <div class="card-body">
                                        <div class="small text-muted">Status</div>
                                        <div class="fw-bold">{{ selectedInstance.Status }}</div>
                                    </div>
                                </div>
                            </div>
                            <div class="col-md-3">
                                <div class="card border-start border-5 border-info">
                                    <div class="card-body">
                                        <div class="small text-muted">Started</div>
                                        <div class="fw-bold small">{{ formatDate(selectedInstance.StartedAt) }}</div>
                                    </div>
                                </div>
                            </div>
                            <div class="col-md-3">
                                <div class="card border-start border-5 border-success">
                                    <div class="card-body">
                                        <div class="small text-muted">Duration</div>
                                        <div class="fw-bold">{{ formatDuration(selectedInstance.StartedAt, selectedInstance.CompletedAt) }}</div>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <!-- Instance ID -->
                        <div class="mb-3">
                            <label class="form-label fw-bold">Instance ID</label>
                            <code class="d-block p-2 bg-light rounded">{{ selectedInstance.InstanceId }}</code>
                        </div>

                        <!-- Fault Message (if any) -->
                        <div v-if="selectedInstance.Status === 'Faulted'" class="alert alert-danger">
                            <i class="fa-solid fa-exclamation-triangle me-1"></i>
                            <strong>Error:</strong> {{ selectedInstance.FaultMessage || 'Unknown error' }}
                        </div>

                        <!-- Execution Log Placeholder -->
                        <div class="mb-3">
                            <label class="form-label fw-bold">Execution Log</label>
                            <div class="alert alert-info">
                                <i class="fa-solid fa-info-circle me-1"></i>
                                Detailed execution log requires additional RPC endpoint: <code>GetWorkflowExecutionLog(instanceId)</code>
                            </div>
                        </div>

                        <!-- Output (if available) -->
                        <div v-if="selectedInstance.Output" class="mb-3">
                            <label class="form-label fw-bold">Output</label>
                            <pre class="bg-light p-3 rounded border small">{{ JSON.stringify(selectedInstance.Output, null, 2) }}</pre>
                        </div>
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Close</button>
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
            instances: [],
            filteredInstances: [],
            searchQuery: '',
            statusFilter: '',
            loading: false,
            autoRefresh: false,
            refreshInterval: null,
            pageSize: 25,
            currentPage: 1,
            totalCount: 0,
            totalPages: 0,
            selectedInstance: null,
            detailsModalInstance: null
        };
    },
    computed: {
        paginatedInstances() {
            const start = (this.currentPage - 1) * this.pageSize;
            const end = start + this.pageSize;
            return this.filteredInstances.slice(start, end);
        },
        totalPages() {
            return this.totalPages || 0;
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
        this.refreshInstances();
    },
    beforeUnmount() {
        this.stopAutoRefresh();
    },
    methods: {
        async refreshInstances() {
            this.loading = true;
            try {
                // Call the real GetWorkflowInstances RPC endpoint
                rpcAEP("GetWorkflowInstances", { 
                    Status: this.statusFilter || null,
                    Page: this.currentPage,
                    PageSize: this.pageSize
                }, (result) => {
                    if (result?.success) {
                        this.instances = result.instances || [];
                        this.totalCount = result.totalCount || 0;
                        this.totalPages = result.totalPages || 0;
                        this.filterInstances();
                    } else {
                        shared.showNotification('Failed to load workflow instances', 'error');
                        this.instances = [];
                    }
                    this.loading = false;
                });
            } catch (error) {
                console.error('Failed to refresh instances:', error);
                shared.showNotification('Failed to load workflow instances: ' + error.message, 'error');
                this.loading = false;
            }
        },

        filterInstances() {
            let filtered = [...this.instances];
            
            // Filter by status
            if (this.statusFilter) {
                filtered = filtered.filter(i => i.Status === this.statusFilter);
            }
            
            // Filter by search query
            if (this.searchQuery) {
                const query = this.searchQuery.toLowerCase();
                filtered = filtered.filter(i => 
                    i.WorkflowName.toLowerCase().includes(query) ||
                    i.DefinitionId.toLowerCase().includes(query) ||
                    i.InstanceId.toLowerCase().includes(query)
                );
            }
            
            this.filteredInstances = filtered;
            this.currentPage = 1;
        },

        startAutoRefresh() {
            this.refreshInterval = setInterval(() => {
                this.refreshInstances();
            }, 10000); // 10 seconds
        },

        stopAutoRefresh() {
            if (this.refreshInterval) {
                clearInterval(this.refreshInterval);
                this.refreshInterval = null;
            }
        },

        viewDetails(instance) {
            this.selectedInstance = instance;
            this.detailsModalInstance.show();
        },

        async cancelInstance(instance) {
            if (!confirm(`Are you sure you want to cancel instance "${instance.InstanceId}"?`)) {
                return;
            }
            // Note: Requires CancelWorkflowInstance RPC endpoint
            shared.toast('Cancel functionality not yet implemented', 'info');
        },

        getStatusBadgeClass(status) {
            const classes = {
                'Running': 'bg-info',
                'Completed': 'bg-success',
                'Faulted': 'bg-danger',
                'Cancelled': 'bg-secondary',
                'Suspended': 'bg-warning'
            };
            return classes[status] || 'bg-secondary';
        },

        getStatusIcon(status) {
            const icons = {
                'Running': 'fa-spinner fa-spin',
                'Completed': 'fa-check-circle',
                'Faulted': 'fa-exclamation-triangle',
                'Cancelled': 'fa-ban',
                'Suspended': 'fa-pause-circle'
            };
            return icons[status] || 'fa-question-circle';
        },

        getStatusColor(status) {
            const colors = {
                'Running': 'info',
                'Completed': 'success',
                'Faulted': 'danger',
                'Cancelled': 'secondary',
                'Suspended': 'warning'
            };
            return colors[status] || 'secondary';
        },

        truncateId(id) {
            if (!id) return '—';
            return id.length > 16 ? id.substring(0, 16) + '...' : id;
        },

        formatDate(date) {
            if (!date) return '—';
            try {
                return new Date(date).toLocaleString();
            } catch {
                return '—';
            }
        },

        formatDuration(start, end) {
            if (!start) return '—';
            if (!end) return 'In progress...';
            try {
                const duration = new Date(end) - new Date(start);
                const seconds = Math.floor(duration / 1000);
                const minutes = Math.floor(seconds / 60);
                const hours = Math.floor(minutes / 60);
                
                if (hours > 0) {
                    return `${hours}h ${minutes % 60}m`;
                } else if (minutes > 0) {
                    return `${minutes}m ${seconds % 60}s`;
                } else {
                    return `${seconds}s`;
                }
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

code {
    font-size: 0.85em;
}
</style>
