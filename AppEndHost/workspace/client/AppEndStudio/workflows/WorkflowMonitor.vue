<template>
    <div class="card h-100 bg-transparent rounded-0 border-0">
        <div class="card-header p-2 bg-body-subtle rounded-0 border-0">
            <div class="hstack gap-2">
                <div class="fw-bold text-primary">
                    <i class="fa-solid fa-chart-line me-1"></i>
                    <span>Workflow Instances Monitor</span>
                </div>
                <div class="ms-auto">
                    <button class="btn btn-sm btn-outline-primary" @click="loadInstances">
                        <i class="fas fa-refresh me-1"></i>Refresh
                    </button>
                </div>
            </div>
        </div>

        <div class="card-body p-2">
            <!-- Statistics -->
            <div class="row g-2 mb-3">
                <div class="col-md-3">
                    <div class="card border-0 bg-light">
                        <div class="card-body p-2 text-center">
                            <div class="text-muted small">Total Running</div>
                            <div class="fw-bold fs-5 text-primary">{{ stats.running || 0 }}</div>
                        </div>
                    </div>
                </div>
                <div class="col-md-3">
                    <div class="card border-0 bg-light">
                        <div class="card-body p-2 text-center">
                            <div class="text-muted small">Completed</div>
                            <div class="fw-bold fs-5 text-success">{{ stats.completed || 0 }}</div>
                        </div>
                    </div>
                </div>
                <div class="col-md-3">
                    <div class="card border-0 bg-light">
                        <div class="card-body p-2 text-center">
                            <div class="text-muted small">Failed</div>
                            <div class="fw-bold fs-5 text-danger">{{ stats.failed || 0 }}</div>
                        </div>
                    </div>
                </div>
                <div class="col-md-3">
                    <div class="card border-0 bg-light">
                        <div class="card-body p-2 text-center">
                            <div class="text-muted small">Paused</div>
                            <div class="fw-bold fs-5 text-warning">{{ stats.paused || 0 }}</div>
                        </div>
                    </div>
                </div>
            </div>

            <!-- Instances List -->
            <div class="container-fluid">
                <div v-if="instances.length === 0" class="alert alert-info mb-0">
                    <i class="fas fa-info-circle me-2"></i>No workflow instances found.
                </div>

                <div v-else class="table-responsive">
                    <table class="table table-hover table-sm mb-0">
                        <thead class="table-light">
                            <tr>
                                <th style="width: 20%">Instance ID</th>
                                <th style="width: 20%">Workflow</th>
                                <th style="width: 15%">Status</th>
                                <th style="width: 15%">Started</th>
                                <th style="width: 15%">Duration</th>
                                <th style="width: 15%">Actions</th>
                            </tr>
                        </thead>
                        <tbody>
                            <tr v-for="instance in instances" :key="instance.InstanceId" class="align-middle">
                                <td class="font-monospace small">{{ instance.InstanceId.substring(0, 12) }}...</td>
                                <td>{{ instance.DefinitionName || instance.DefinitionId }}</td>
                                <td>
                                    <span :class="statusBadgeClass(instance.Status)">
                                        {{ instance.Status }}
                                    </span>
                                </td>
                                <td class="small text-muted">{{ formatDate(instance.StartedAt) }}</td>
                                <td class="small text-muted">{{ calculateDuration(instance.StartedAt, instance.FinishedAt) }}</td>
                                <td>
                                    <div class="btn-group btn-group-sm" role="group">
                                        <button class="btn btn-outline-info" @click="viewInstanceDetails(instance)" title="View Details">
                                            <i class="fas fa-eye"></i>
                                        </button>
                                        <button v-if="instance.Status === 'Running'" class="btn btn-outline-danger" @click="cancelInstance(instance)" title="Cancel">
                                            <i class="fas fa-times"></i>
                                        </button>
                                        <button v-if="instance.Status === 'Failed'" class="btn btn-outline-warning" @click="retryInstance(instance)" title="Retry">
                                            <i class="fas fa-redo"></i>
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
    shared.setAppSubTitle("Instances Monitor");

    let _this = { cid: "", c: null };

    export default {
        setup(props) {
            _this.cid = props["cid"];
        },
        data() {
            return {
                ..._this,
                instances: [],
                stats: {
                    running: 0,
                    completed: 0,
                    failed: 0,
                    paused: 0
                }
            };
        },
        created() { _this.c = this; },
        mounted() {
            this.loadInstances();
            setInterval(() => this.loadInstances(), 10000);
        },
        methods: {
            loadInstances() {
                rpcAEP('GetWorkflowInstances', { Status: '', Page: 1, PageSize: 50 }, (data) => {
                    const payload = Array.isArray(data) ? (data[0] || {}) : (data || {});
                    const list = payload.Result || payload.result || payload.instances || payload.Instances || payload;
                    this.instances = Array.isArray(list) ? list : [];
                    this.calculateStats();
                }, (error) => {
                    console.error('Error loading instances:', error);
                });
            },

            calculateStats() {
                this.stats = {
                    running: this.instances.filter(i => i.Status === 'Running').length,
                    completed: this.instances.filter(i => i.Status === 'Completed').length,
                    failed: this.instances.filter(i => i.Status === 'Failed').length,
                    paused: this.instances.filter(i => i.Status === 'Paused').length
                };
            },

            viewInstanceDetails(instance) {
                openComponent("/AppEndStudio/workflows/WorkflowInstanceDetails", {
                    title: "Instance Details",
                    modalSize: "modal-lg",
                    windowSizeSwitchable: true,
                    params: {
                        instance: instance
                    }
                });
            },

            cancelInstance(instance) {
                if (confirm(`Cancel instance ${instance.InstanceId.substring(0, 12)}?`)) {
                    rpcAEP('CancelInstance', { InstanceId: instance.InstanceId }, (data) => {
                        const payload = Array.isArray(data) ? (data[0] || {}) : (data || {});
                        const success = payload.Success || payload.success;
                        if (success) {
                            showSuccess('Instance cancelled');
                            this.loadInstances();
                        } else {
                            showError('Error: ' + (payload.ErrorMessage || 'Unknown error'));
                        }
                    }, (error) => {
                        showError('Error: ' + error);
                    });
                }
            },

            retryInstance(instance) {
                if (confirm(`Retry instance ${instance.InstanceId.substring(0, 12)}?`)) {
                    rpcAEP('RetryInstance', { InstanceId: instance.InstanceId }, (data) => {
                        const payload = Array.isArray(data) ? (data[0] || {}) : (data || {});
                        const success = payload.Success || payload.success;
                        if (success) {
                            showSuccess('Instance retried');
                            this.loadInstances();
                        } else {
                            showError('Error: ' + (payload.ErrorMessage || 'Unknown error'));
                        }
                    }, (error) => {
                        showError('Error: ' + error);
                    });
                }
            },

            statusBadgeClass(status) {
                const baseClass = 'badge';
                switch(status) {
                    case 'Running': return baseClass + ' bg-primary';
                    case 'Completed': return baseClass + ' bg-success';
                    case 'Failed': return baseClass + ' bg-danger';
                    case 'Paused': return baseClass + ' bg-warning text-dark';
                    default: return baseClass + ' bg-secondary';
                }
            },

            formatDate(dateStr) {
                if (!dateStr) return '-';
                return new Date(dateStr).toLocaleString();
            },

            calculateDuration(startDate, endDate) {
                if (!startDate) return '-';
                const start = new Date(startDate);
                const end = endDate ? new Date(endDate) : new Date();
                const diffMs = end - start;
                const diffSecs = Math.floor(diffMs / 1000);
                const hours = Math.floor(diffSecs / 3600);
                const minutes = Math.floor((diffSecs % 3600) / 60);
                const seconds = diffSecs % 60;
                if (hours > 0) return `${hours}h ${minutes}m`;
                if (minutes > 0) return `${minutes}m ${seconds}s`;
                return `${seconds}s`;
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
