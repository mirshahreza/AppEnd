<template>
    <div class="workflow-dashboard card h-100 rounded-bottom-0 rounded-end-0 border-0">
        <div class="card-body p-0 h-100 position-relative">
            <div class="h-100 w-100" style="overflow-x: hidden;">
                <div class="h-100" style="width:100%;">
                    <div class="card h-100 rounded-0 border-0">
                        <div class="card-body p-2 p-md-4 scrollable">
                            <!-- Header Section -->
                            <div class="row mb-3">
                                <div class="col-12">
                                    <div class="d-flex justify-content-between align-items-center">
                                        <h4 class="mb-0">
                                            <i class="fa-solid fa-diagram-project me-2"></i>Workflow Dashboard
                                        </h4>
                                        <button class="btn btn-sm btn-primary" @click="refreshDashboard">
                                            <i class="fa-solid fa-rotate"></i> Refresh
                                        </button>
                                    </div>
                                </div>
                            </div>

                            <!-- Summary Cards Row -->
                            <div class="row g-3 mb-4">
                                <div class="col-md-3">
                                    <div class="card summary-card border-0 shadow-sm" style="background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);">
                                        <div class="card-body text-white">
                                            <div class="d-flex justify-content-between align-items-start">
                                                <div>
                                                    <small class="text-white-50">Total Workflows</small>
                                                    <h3 class="mb-0">{{ summary?.totalWorkflows || 0 }}</h3>
                                                </div>
                                                <i class="fa-solid fa-file-lines fa-2x opacity-50"></i>
                                            </div>
                                        </div>
                                    </div>
                                </div>

                                <div class="col-md-3">
                                    <div class="card summary-card border-0 shadow-sm" style="background: linear-gradient(135deg, #f093fb 0%, #f5576c 100%);">
                                        <div class="card-body text-white">
                                            <div class="d-flex justify-content-between align-items-start">
                                                <div>
                                                    <small class="text-white-50">Running Instances</small>
                                                    <h3 class="mb-0">{{ summary?.runningInstances || 0 }}</h3>
                                                </div>
                                                <i class="fa-solid fa-play fa-2x opacity-50"></i>
                                            </div>
                                        </div>
                                    </div>
                                </div>

                                <div class="col-md-3">
                                    <div class="card summary-card border-0 shadow-sm" style="background: linear-gradient(135deg, #4facfe 0%, #00f2fe 100%);">
                                        <div class="card-body text-white">
                                            <div class="d-flex justify-content-between align-items-start">
                                                <div>
                                                    <small class="text-white-50">Completed Today</small>
                                                    <h3 class="mb-0">{{ summary?.completedToday || 0 }}</h3>
                                                </div>
                                                <i class="fa-solid fa-check-circle fa-2x opacity-50"></i>
                                            </div>
                                        </div>
                                    </div>
                                </div>

                                <div class="col-md-3">
                                    <div class="card summary-card border-0 shadow-sm" style="background: linear-gradient(135deg, #fa709a 0%, #fee140 100%);">
                                        <div class="card-body text-white">
                                            <div class="d-flex justify-content-between align-items-start">
                                                <div>
                                                    <small class="text-white-50">Success Rate</small>
                                                    <h3 class="mb-0">{{ summary?.successRate?.toFixed(1) || 0 }}%</h3>
                                                </div>
                                                <i class="fa-solid fa-chart-pie fa-2x opacity-50"></i>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>

                            <!-- Charts and Stats Row -->
                            <div class="row g-3 mb-4">
                                <!-- Status Distribution -->
                                <div class="col-lg-6">
                                    <div class="card border-0 shadow-sm">
                                        <div class="card-header bg-light border-bottom">
                                            <h6 class="mb-0">
                                                <i class="fa-solid fa-chart-donut me-2"></i>Status Distribution
                                            </h6>
                                        </div>
                                        <div class="card-body p-3">
                                            <div v-if="statusStats?.length" class="workflow-stats">
                                                <div v-for="stat in statusStats" :key="stat.status" class="stat-item d-flex justify-content-between align-items-center mb-2">
                                                    <div class="d-flex align-items-center flex-grow-1">
                                                        <span class="status-badge me-2" :class="'status-' + stat.status.toLowerCase()">
                                                            {{ stat.status }}
                                                        </span>
                                                    </div>
                                                    <span class="badge bg-secondary">{{ stat.count }}</span>
                                                </div>
                                            </div>
                                            <div v-else class="text-muted text-center py-3">
                                                <small>No data available</small>
                                            </div>
                                        </div>
                                    </div>
                                </div>

                                <!-- Performance Metrics -->
                                <div class="col-lg-6">
                                    <div class="card border-0 shadow-sm">
                                        <div class="card-header bg-light border-bottom">
                                            <h6 class="mb-0">
                                                <i class="fa-solid fa-gauge me-2"></i>Performance Metrics
                                            </h6>
                                        </div>
                                        <div class="card-body p-3">
                                            <div v-if="performance" class="performance-metrics">
                                                <div class="metric-item d-flex justify-content-between mb-3">
                                                    <span>Average Execution Time:</span>
                                                    <strong>{{ performance?.avgExecutionTime?.toFixed(2) || 0 }}s</strong>
                                                </div>
                                                <div class="metric-item d-flex justify-content-between mb-3">
                                                    <span>Max Execution Time:</span>
                                                    <strong>{{ performance?.maxExecutionTime?.toFixed(2) || 0 }}s</strong>
                                                </div>
                                                <div class="metric-item d-flex justify-content-between">
                                                    <span>Avg Wait Time:</span>
                                                    <strong>{{ performance?.avgWaitTime?.toFixed(2) || 0 }}s</strong>
                                                </div>
                                            </div>
                                            <div v-else class="text-muted text-center py-3">
                                                <small>No data available</small>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>

                            <!-- Recent Activities -->
                            <div class="row">
                                <div class="col-12">
                                    <div class="card border-0 shadow-sm">
                                        <div class="card-header bg-light border-bottom">
                                            <h6 class="mb-0">
                                                <i class="fa-solid fa-history me-2"></i>Recent Workflow Instances
                                            </h6>
                                        </div>
                                        <div class="card-body p-0">
                                            <div v-if="recentInstances?.length" class="table-responsive">
                                                <table class="table table-hover mb-0">
                                                    <thead class="table-light">
                                                        <tr>
                                                            <th>Instance ID</th>
                                                            <th>Workflow</th>
                                                            <th>Status</th>
                                                            <th>Started</th>
                                                            <th>Duration</th>
                                                            <th></th>
                                                        </tr>
                                                    </thead>
                                                    <tbody>
                                                        <tr v-for="instance in recentInstances.slice(0, 10)" :key="instance.id">
                                                            <td>
                                                                <code class="text-truncate" style="max-width: 200px;">{{ instance.id }}</code>
                                                            </td>
                                                            <td>{{ instance.workflowName }}</td>
                                                            <td>
                                                                <span class="badge" :class="'bg-' + getStatusBadgeClass(instance.status)">
                                                                    {{ instance.status }}
                                                                </span>
                                                            </td>
                                                            <td><small>{{ formatDate(instance.startedAt) }}</small></td>
                                                            <td><small>{{ formatDuration(instance.duration) }}</small></td>
                                                            <td>
                                                                <button class="btn btn-sm btn-outline-primary" @click="viewInstanceDetails(instance.id)">
                                                                    <i class="fa-solid fa-eye"></i>
                                                                </button>
                                                            </td>
                                                        </tr>
                                                    </tbody>
                                                </table>
                                            </div>
                                            <div v-else class="text-muted text-center py-4">
                                                <small>No workflow instances found</small>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>

<script>
shared.setAppTitle(`<i class="fa-solid fa-fw fa-diagram-project"></i> <span>Workflow Dashboard</span>`);

let _this = {
    cid: "WorkflowDashboard",
    c: null,
    data: {
        summary: null,
        statusStats: [],
        performance: null,
        recentInstances: []
    }
};

_this.onMounted = function() {
    _this.refreshDashboard();
};

_this.refreshDashboard = function() {
    shared.http.get(`/api/workflows/dashboard`).then(r => {
        if (r.data?.success) {
            _this.c.summary = r.data.data?.summary || {};
            _this.c.statusStats = r.data.data?.statusStatistics || [];
            _this.c.performance = r.data.data?.performanceMetrics || {};
            _this.c.recentInstances = r.data.data?.recentInstances || [];
        }
    }).catch(err => {
        console.error("Failed to load dashboard data:", err);
    });
};

_this.formatDate = function(date) {
    if (!date) return "-";
    return new Date(date).toLocaleString();
};

_this.formatDuration = function(ms) {
    if (!ms) return "-";
    const seconds = Math.floor(ms / 1000);
    const minutes = Math.floor(seconds / 60);
    if (minutes > 0) return `${minutes}m ${seconds % 60}s`;
    return `${seconds}s`;
};

_this.getStatusBadgeClass = function(status) {
    const statusMap = {
        "Running": "info",
        "Completed": "success",
        "Failed": "danger",
        "Suspended": "warning",
        "Pending": "secondary"
    };
    return statusMap[status] || "secondary";
};

_this.viewInstanceDetails = function(instanceId) {
    shared.http.get(`/api/workflows/instances/${instanceId}`).then(r => {
        if (r.data?.success) {
            console.log("Instance details:", r.data.data);
            // TODO: Open details modal or navigate to detail view
        }
    });
};

export default {
    name: _this.cid,
    data() {
        return _this.data;
    },
    methods: {
        refreshDashboard: _this.refreshDashboard,
        formatDate: _this.formatDate,
        formatDuration: _this.formatDuration,
        getStatusBadgeClass: _this.getStatusBadgeClass,
        viewInstanceDetails: _this.viewInstanceDetails
    },
    mounted() {
        _this.c = this;
        _this.onMounted();
    }
};
</script>

<style scoped>
.workflow-dashboard {
    background: #f8f9fa;
}

.summary-card {
    transition: transform 0.2s, box-shadow 0.2s;
}

.summary-card:hover {
    transform: translateY(-2px);
    box-shadow: 0 0.5rem 1rem rgba(0, 0, 0, 0.15) !important;
}

.workflow-stats {
    display: flex;
    flex-direction: column;
}

.stat-item {
    padding: 0.75rem 0;
    border-bottom: 1px solid #e9ecef;
}

.stat-item:last-child {
    border-bottom: none;
}

.status-badge {
    padding: 0.35rem 0.75rem;
    border-radius: 20px;
    font-size: 0.85rem;
    font-weight: 500;
}

.status-running {
    background: #e3f2fd;
    color: #1976d2;
}

.status-completed {
    background: #e8f5e9;
    color: #388e3c;
}

.status-failed {
    background: #ffebee;
    color: #d32f2f;
}

.status-suspended {
    background: #fff3e0;
    color: #f57c00;
}

.status-pending {
    background: #f3e5f5;
    color: #7b1fa2;
}

.performance-metrics {
    display: flex;
    flex-direction: column;
}

.metric-item {
    padding: 0.5rem 0;
    font-size: 0.95rem;
}

.scrollable {
    overflow-y: auto;
    overflow-x: hidden;
    height: 100%;
}

.table {
    font-size: 0.9rem;
}

.table tbody tr {
    border-bottom: 1px solid #e9ecef;
}

.table tbody tr:hover {
    background-color: #f8f9fa;
}
</style>
