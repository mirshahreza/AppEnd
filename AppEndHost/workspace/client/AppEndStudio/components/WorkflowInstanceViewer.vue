<template>
    <div class="workflow-instance-viewer card h-100 rounded-bottom-0 rounded-end-0 border-0">
        <div class="card-body p-0 h-100 position-relative">
            <div class="h-100 w-100" style="overflow-x: hidden;">
                <div class="h-100" style="width:100%;">
                    <div class="card h-100 rounded-0 border-0">
                        <!-- Header -->
                        <div class="instance-header bg-light border-bottom p-3">
                            <div class="d-flex justify-content-between align-items-center">
                                <div>
                                    <h5 class="mb-0">
                                        <i class="fa-solid fa-person-running me-2"></i>Workflow Instance: {{ instance?.id }}
                                    </h5>
                                    <small class="text-muted">{{ instance?.workflowName }}</small>
                                </div>
                                <div>
                                    <span class="badge" :class="'bg-' + getStatusBadgeClass(instance?.status)">
                                        {{ instance?.status }}
                                    </span>
                                </div>
                            </div>
                        </div>

                        <div class="card-body p-0 scrollable">
                            <!-- Instance Overview -->
                            <div class="p-3 border-bottom">
                                <div class="row g-3">
                                    <div class="col-md-6">
                                        <div class="overview-item">
                                            <label class="text-muted small">Started At</label>
                                            <div class="fw-bold">{{ formatDate(instance?.startedAt) }}</div>
                                        </div>
                                    </div>
                                    <div class="col-md-6">
                                        <div class="overview-item">
                                            <label class="text-muted small">Completed At</label>
                                            <div class="fw-bold">{{ formatDate(instance?.completedAt) }}</div>
                                        </div>
                                    </div>
                                    <div class="col-md-6">
                                        <div class="overview-item">
                                            <label class="text-muted small">Duration</label>
                                            <div class="fw-bold">{{ formatDuration(instance?.duration) }}</div>
                                        </div>
                                    </div>
                                    <div class="col-md-6">
                                        <div class="overview-item">
                                            <label class="text-muted small">Progress</label>
                                            <div class="progress mt-1" style="height: 1.5rem;">
                                                <div class="progress-bar" 
                                                    :style="{ width: (instance?.progress || 0) + '%' }">
                                                    {{ instance?.progress || 0 }}%
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>

                            <!-- Execution Timeline -->
                            <div class="p-3 border-bottom">
                                <h6 class="mb-3">
                                    <i class="fa-solid fa-timeline me-2"></i>Execution Timeline
                                </h6>
                                <div v-if="executionTimeline?.length" class="timeline">
                                    <div v-for="(event, idx) in executionTimeline" :key="idx" class="timeline-item">
                                        <div class="timeline-marker" :class="'status-' + event.status.toLowerCase()"></div>
                                        <div class="timeline-content">
                                            <div class="fw-bold small">{{ event.activityName }}</div>
                                            <div class="text-muted small">{{ event.status }}</div>
                                            <div class="text-secondary" style="font-size: 0.8rem;">
                                                {{ formatDate(event.timestamp) }}
                                                <span v-if="event.duration"> ({{ event.duration }}ms)</span>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <div v-else class="text-muted text-center py-3">
                                    <small>No execution timeline available</small>
                                </div>
                            </div>

                            <!-- Variables -->
                            <div class="p-3 border-bottom">
                                <h6 class="mb-3">
                                    <i class="fa-solid fa-database me-2"></i>Variables ({{ variables?.length || 0 }})
                                </h6>
                                <div v-if="variables?.length" class="variables-list">
                                    <div v-for="(variable, idx) in variables" :key="idx" class="variable-item card mb-2">
                                        <div class="card-body p-2">
                                            <div class="d-flex justify-content-between align-items-start">
                                                <div>
                                                    <small class="d-block fw-bold">{{ variable.name }}</small>
                                                    <small class="text-muted">{{ variable.type }}</small>
                                                </div>
                                                <button class="btn btn-sm btn-outline-secondary" 
                                                    @click="toggleVariableValue(idx)">
                                                    <i class="fa-solid" :class="expandedVariables[idx] ? 'fa-chevron-up' : 'fa-chevron-down'"></i>
                                                </button>
                                            </div>
                                            <div v-if="expandedVariables[idx]" class="mt-2">
                                                <pre class="bg-light p-2 rounded" style="font-size: 0.8rem; overflow-x: auto;">{{ JSON.stringify(variable.value, null, 2) }}</pre>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <div v-else class="text-muted text-center py-3">
                                    <small>No variables available</small>
                                </div>
                            </div>

                            <!-- Errors/Faults -->
                            <div v-if="faults?.length" class="p-3 border-bottom">
                                <h6 class="mb-3">
                                    <i class="fa-solid fa-triangle-exclamation me-2"></i>Errors ({{ faults?.length }})
                                </h6>
                                <div v-for="(fault, idx) in faults" :key="idx" class="fault-item alert alert-danger mb-2">
                                    <div class="d-flex justify-content-between align-items-start">
                                        <div>
                                            <strong>{{ fault.activity }}</strong>
                                            <div class="text-muted small">{{ fault.errorMessage }}</div>
                                            <div class="text-secondary" style="font-size: 0.8rem;">{{ formatDate(fault.timestamp) }}</div>
                                        </div>
                                        <button class="btn btn-sm btn-outline-danger" 
                                            @click="toggleFaultDetails(idx)">
                                            <i class="fa-solid" :class="expandedFaults[idx] ? 'fa-chevron-up' : 'fa-chevron-down'"></i>
                                        </button>
                                    </div>
                                    <div v-if="expandedFaults[idx]" class="mt-2">
                                        <pre class="bg-light p-2 rounded" style="font-size: 0.8rem; overflow-x: auto;">{{ fault.stackTrace }}</pre>
                                    </div>
                                </div>
                            </div>

                            <!-- Instance Actions -->
                            <div class="p-3">
                                <h6 class="mb-3">Actions</h6>
                                <div class="btn-group d-flex flex-wrap gap-2" role="group">
                                    <button v-if="instance?.status === 'Running'" 
                                        type="button" class="btn btn-sm btn-warning"
                                        @click="suspendInstance">
                                        <i class="fa-solid fa-pause"></i> Suspend
                                    </button>
                                    <button v-if="instance?.status === 'Suspended'" 
                                        type="button" class="btn btn-sm btn-info"
                                        @click="resumeInstance">
                                        <i class="fa-solid fa-play"></i> Resume
                                    </button>
                                    <button v-if="['Running', 'Suspended'].includes(instance?.status)" 
                                        type="button" class="btn btn-sm btn-danger"
                                        @click="terminateInstance">
                                        <i class="fa-solid fa-stop"></i> Terminate
                                    </button>
                                    <button type="button" class="btn btn-sm btn-secondary"
                                        @click="refreshInstance">
                                        <i class="fa-solid fa-rotate"></i> Refresh
                                    </button>
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
shared.setAppTitle(`<i class="fa-solid fa-fw fa-person-running"></i> <span>Workflow Instance</span>`);

let _this = {
    cid: "WorkflowInstanceViewer",
    c: null,
    instanceId: null,
    data: {
        instance: null,
        executionTimeline: [],
        variables: [],
        faults: [],
        expandedVariables: {},
        expandedFaults: {}
    }
};

_this.onMounted = function() {
    const params = new URLSearchParams(window.location.search);
    _this.instanceId = params.get("id");
    if (_this.instanceId) {
        _this.refreshInstance();
    }
};

_this.refreshInstance = function() {
    if (!_this.instanceId) return;
    
    shared.http.get(`/api/workflows/instances/${_this.instanceId}`).then(r => {
        if (r.data?.success) {
            _this.c.instance = r.data.data?.instance || {};
            _this.c.executionTimeline = r.data.data?.timeline || [];
            _this.c.variables = r.data.data?.variables || [];
            _this.c.faults = r.data.data?.faults || [];
        }
    }).catch(err => {
        console.error("Failed to load instance data:", err);
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

_this.toggleVariableValue = function(idx) {
    _this.c.$set(_this.c.expandedVariables, idx, !_this.c.expandedVariables[idx]);
};

_this.toggleFaultDetails = function(idx) {
    _this.c.$set(_this.c.expandedFaults, idx, !_this.c.expandedFaults[idx]);
};

_this.suspendInstance = function() {
    shared.http.post(`/api/workflows/instances/${_this.instanceId}/suspend`).then(r => {
        if (r.data?.success) {
            shared.notify("Instance suspended", "success");
            _this.refreshInstance();
        }
    });
};

_this.resumeInstance = function() {
    shared.http.post(`/api/workflows/instances/${_this.instanceId}/resume`).then(r => {
        if (r.data?.success) {
            shared.notify("Instance resumed", "success");
            _this.refreshInstance();
        }
    });
};

_this.terminateInstance = function() {
    if (!confirm("Are you sure you want to terminate this instance?")) return;
    
    shared.http.post(`/api/workflows/instances/${_this.instanceId}/terminate`).then(r => {
        if (r.data?.success) {
            shared.notify("Instance terminated", "success");
            _this.refreshInstance();
        }
    });
};

export default {
    name: _this.cid,
    data() {
        return _this.data;
    },
    methods: {
        refreshInstance: _this.refreshInstance,
        formatDate: _this.formatDate,
        formatDuration: _this.formatDuration,
        getStatusBadgeClass: _this.getStatusBadgeClass,
        toggleVariableValue: _this.toggleVariableValue,
        toggleFaultDetails: _this.toggleFaultDetails,
        suspendInstance: _this.suspendInstance,
        resumeInstance: _this.resumeInstance,
        terminateInstance: _this.terminateInstance
    },
    mounted() {
        _this.c = this;
        _this.onMounted();
    }
};
</script>

<style scoped>
.workflow-instance-viewer {
    background: #f8f9fa;
}

.scrollable {
    overflow-y: auto;
    overflow-x: hidden;
}

.overview-item {
    display: flex;
    flex-direction: column;
}

.timeline {
    position: relative;
    padding-left: 2rem;
}

.timeline::before {
    content: '';
    position: absolute;
    left: 0.4rem;
    top: 0;
    bottom: 0;
    width: 2px;
    background: #dee2e6;
}

.timeline-item {
    position: relative;
    display: flex;
    margin-bottom: 1rem;
}

.timeline-marker {
    position: absolute;
    left: -1.15rem;
    top: 0.25rem;
    width: 1rem;
    height: 1rem;
    border-radius: 50%;
    border: 3px solid white;
    background: #dee2e6;
}

.timeline-marker.status-running {
    background: #17a2b8;
    animation: pulse 1s infinite;
}

.timeline-marker.status-completed {
    background: #28a745;
}

.timeline-marker.status-failed {
    background: #dc3545;
}

.timeline-marker.status-skipped {
    background: #6c757d;
}

@keyframes pulse {
    0%, 100% { opacity: 1; }
    50% { opacity: 0.5; }
}

.timeline-content {
    flex-grow: 1;
    padding: 0.5rem;
    background: white;
    border-radius: 4px;
    border-left: 3px solid #dee2e6;
}

.variables-list {
    display: flex;
    flex-direction: column;
}

.variable-item {
    border: 1px solid #dee2e6;
}

.fault-item {
    margin-bottom: 0.5rem;
}

.fault-item pre {
    margin: 0;
    max-height: 300px;
    overflow-y: auto;
}

.btn-group {
    align-items: flex-start;
}

@media (max-width: 768px) {
    .btn-group {
        flex-direction: column;
    }
    
    .btn-group > .btn {
        width: 100%;
    }
}
</style>
