<template>
    <div class="workflow-designer card h-100 rounded-bottom-0 rounded-end-0 border-0">
        <div class="card-body p-0 h-100 position-relative">
            <div class="h-100 w-100" style="overflow-x: hidden;">
                <!-- Header -->
                <div class="designer-header bg-light border-bottom p-3">
                    <div class="d-flex justify-content-between align-items-center">
                        <div>
                            <h5 class="mb-0">
                                <i class="fa-solid fa-pen-to-square me-2"></i>Workflow Designer
                            </h5>
                            <small class="text-muted">{{ currentWorkflow?.name || 'New Workflow' }}</small>
                        </div>
                        <div class="btn-group" role="group">
                            <button type="button" class="btn btn-sm btn-outline-primary" @click="saveWorkflow">
                                <i class="fa-solid fa-save"></i> Save
                            </button>
                            <button type="button" class="btn btn-sm btn-outline-primary" @click="publishWorkflow">
                                <i class="fa-solid fa-arrow-up-from-bracket"></i> Publish
                            </button>
                            <button type="button" class="btn btn-sm btn-outline-danger" @click="closeDesigner">
                                <i class="fa-solid fa-times"></i> Close
                            </button>
                        </div>
                    </div>
                </div>

                <!-- Split Container with Flex Splitter -->
                <div class="designer-body h-100" :data-flex-splitter-h="true">
                    <!-- Left Panel: Activity Toolbox -->
                    <div class="designer-toolbox" data-flex-splitter-pane="1" style="flex-basis: 250px; min-width: 200px;">
                        <div class="p-3 h-100 d-flex flex-column">
                            <h6 class="mb-3">
                                <i class="fa-solid fa-toolbox me-2"></i>Activities
                            </h6>

                            <div class="activity-search mb-3">
                                <input type="text" 
                                    class="form-control form-control-sm" 
                                    placeholder="Search activities..." 
                                    v-model="activitySearch"
                                    @input="filterActivities">
                            </div>

                            <div class="activity-list flex-grow-1 overflow-auto">
                                <div v-for="activity in filteredActivities" 
                                    :key="activity.id" 
                                    class="activity-item card mb-2"
                                    draggable="true"
                                    @dragstart="startDragActivity($event, activity)">
                                    <div class="card-body p-2">
                                        <small class="d-block fw-bold">{{ activity.name }}</small>
                                        <small class="text-muted text-truncate">{{ activity.description }}</small>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                    <!-- Center Panel: Canvas -->
                    <div class="designer-canvas" data-flex-splitter-pane="2">
                        <div class="canvas-header bg-light border-bottom p-2">
                            <div class="btn-group btn-group-sm me-2" role="group">
                                <button type="button" class="btn btn-outline-secondary" @click="zoomIn" title="Zoom In">
                                    <i class="fa-solid fa-magnifying-glass-plus"></i>
                                </button>
                                <button type="button" class="btn btn-outline-secondary" @click="zoomOut" title="Zoom Out">
                                    <i class="fa-solid fa-magnifying-glass-minus"></i>
                                </button>
                                <button type="button" class="btn btn-outline-secondary" @click="zoomReset" title="Reset Zoom">
                                    <i class="fa-solid fa-expand"></i>
                                </button>
                            </div>
                            <small class="text-muted">Zoom: {{ (zoom * 100).toFixed(0) }}%</small>
                        </div>
                        <div class="canvas-area p-4 h-100 overflow-auto position-relative"
                            @dragover.prevent="canvasDragOver"
                            @drop.prevent="canvasDrop">
                            <div class="canvas-content" :style="{ transform: `scale(${zoom})`, transformOrigin: 'top left' }">
                                <!-- Activities on Canvas -->
                                <div v-for="(instance, idx) in workflowActivities" 
                                    :key="`activity-${idx}`"
                                    class="activity-instance card position-absolute"
                                    :style="{ 
                                        left: instance.x + 'px', 
                                        top: instance.y + 'px',
                                        width: '150px',
                                        cursor: 'move'
                                    }"
                                    draggable="true"
                                    @dragstart="startDragActivityInstance($event, idx)"
                                    @click="selectActivity(idx)">
                                    <div class="card-body p-2">
                                        <small class="d-block fw-bold">{{ instance.type }}</small>
                                        <small class="text-muted">{{ instance.name }}</small>
                                    </div>
                                </div>

                                <!-- Connection Lines -->
                                <svg v-if="workflowActivities.length > 1" class="canvas-svg" width="100%" height="100%">
                                    <defs>
                                        <marker id="arrowhead" markerWidth="10" markerHeight="10" refX="9" refY="3" orient="auto">
                                            <polygon points="0 0, 10 3, 0 6" fill="#666" />
                                        </marker>
                                    </defs>
                                    <line v-for="(conn, idx) in connections" 
                                        :key="`conn-${idx}`"
                                        :x1="conn.x1" :y1="conn.y1" 
                                        :x2="conn.x2" :y2="conn.y2" 
                                        stroke="#666" 
                                        stroke-width="2" 
                                        marker-end="url(#arrowhead)"
                                        style="pointer-events: none;" />
                                </svg>
                            </div>
                        </div>
                    </div>

                    <!-- Right Panel: Properties -->
                    <div class="designer-properties" data-flex-splitter-pane="3" style="flex-basis: 300px; min-width: 250px;">
                        <div class="p-3 h-100 d-flex flex-column">
                            <h6 class="mb-3">
                                <i class="fa-solid fa-sliders me-2"></i>Properties
                            </h6>

                            <div v-if="selectedActivity" class="properties-panel flex-grow-1 overflow-auto">
                                <div class="mb-3">
                                    <label class="form-label form-label-sm">Activity Name</label>
                                    <input type="text" class="form-control form-control-sm" 
                                        v-model="selectedActivity.name">
                                </div>

                                <div class="mb-3">
                                    <label class="form-label form-label-sm">Activity Type</label>
                                    <select class="form-select form-select-sm" v-model="selectedActivity.type">
                                        <option value="Database">Database Query</option>
                                        <option value="DynaCode">Dynamic Code</option>
                                        <option value="Notification">Notification</option>
                                        <option value="Approval">Approval</option>
                                        <option value="Decision">Decision</option>
                                        <option value="Delay">Delay</option>
                                    </select>
                                </div>

                                <div class="mb-3">
                                    <label class="form-label form-label-sm">Description</label>
                                    <textarea class="form-control form-control-sm" rows="2" 
                                        v-model="selectedActivity.description"></textarea>
                                </div>

                                <!-- Activity-specific Properties -->
                                <div v-if="selectedActivity.type === 'Database'" class="mb-3">
                                    <label class="form-label form-label-sm">Query Name</label>
                                    <input type="text" class="form-control form-control-sm" 
                                        v-model="selectedActivity.config.queryName"
                                        placeholder="e.g., GetUserById">
                                </div>

                                <div v-if="selectedActivity.type === 'DynaCode'" class="mb-3">
                                    <label class="form-label form-label-sm">Method Full Name</label>
                                    <input type="text" class="form-control form-control-sm" 
                                        v-model="selectedActivity.config.methodFullName"
                                        placeholder="e.g., Namespace.Class.Method">
                                </div>

                                <div v-if="selectedActivity.type === 'Notification'" class="mb-3">
                                    <label class="form-label form-label-sm">Channel</label>
                                    <select class="form-select form-select-sm" v-model="selectedActivity.config.channel">
                                        <option value="Email">Email</option>
                                        <option value="SMS">SMS</option>
                                        <option value="AppNotification">App Notification</option>
                                        <option value="Webhook">Webhook</option>
                                    </select>
                                </div>

                                <div v-if="selectedActivity.type === 'Approval'" class="mb-3">
                                    <label class="form-label form-label-sm">Approver</label>
                                    <input type="text" class="form-control form-control-sm" 
                                        v-model="selectedActivity.config.approver"
                                        placeholder="User ID or Role">
                                </div>

                                <div class="mb-3">
                                    <button class="btn btn-sm btn-danger w-100" @click="removeActivity">
                                        <i class="fa-solid fa-trash"></i> Remove Activity
                                    </button>
                                </div>
                            </div>

                            <div v-else class="text-muted text-center py-4">
                                <small>Select an activity to edit properties</small>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>

<script>
shared.setAppTitle(`<i class="fa-solid fa-fw fa-pen-to-square"></i> <span>Workflow Designer</span>`);

let _this = {
    cid: "WorkflowDesigner",
    c: null,
    data: {
        currentWorkflow: null,
        workflowActivities: [],
        selectedActivityIdx: null,
        activitySearch: "",
        zoom: 1,
        connections: [],
        activities: [
            { id: 1, name: "Database Query", description: "Execute database query", type: "Database" },
            { id: 2, name: "Dynamic Code", description: "Execute custom code", type: "DynaCode" },
            { id: 3, name: "Send Email", description: "Send email notification", type: "Notification" },
            { id: 4, name: "Approval Task", description: "Request approval", type: "Approval" },
            { id: 5, name: "Decision", description: "Conditional branching", type: "Decision" },
            { id: 6, name: "Delay", description: "Wait for specified time", type: "Delay" }
        ]
    }
};

_this.onMounted = function() {
    _this.loadWorkflow();
};

_this.loadWorkflow = function() {
    // TODO: Load workflow from API or props
    _this.c.currentWorkflow = {
        id: "wf-1",
        name: "Sample Workflow",
        activities: []
    };
};

_this.filterActivities = function() {
    return _this.c.activities.filter(a => 
        a.name.toLowerCase().includes(_this.c.activitySearch.toLowerCase()) ||
        a.description.toLowerCase().includes(_this.c.activitySearch.toLowerCase())
    );
};

_this.startDragActivity = function(e, activity) {
    e.dataTransfer.effectAllowed = "copy";
    e.dataTransfer.setData("activity", JSON.stringify(activity));
};

_this.startDragActivityInstance = function(e, idx) {
    e.dataTransfer.effectAllowed = "move";
    e.dataTransfer.setData("instanceIdx", idx.toString());
};

_this.canvasDragOver = function(e) {
    e.dataTransfer.dropEffect = "copy";
};

_this.canvasDrop = function(e) {
    const canvas = e.currentTarget;
    const rect = canvas.getBoundingClientRect();
    const x = (e.clientX - rect.left) / _this.c.zoom;
    const y = (e.clientY - rect.top) / _this.c.zoom;

    const instanceIdx = e.dataTransfer.getData("instanceIdx");
    if (instanceIdx) {
        // Move existing activity
        _this.c.workflowActivities[parseInt(instanceIdx)].x = x;
        _this.c.workflowActivities[parseInt(instanceIdx)].y = y;
    } else {
        // Add new activity
        const activityData = e.dataTransfer.getData("activity");
        if (activityData) {
            const activity = JSON.parse(activityData);
            _this.c.workflowActivities.push({
                type: activity.type,
                name: activity.name,
                description: activity.description,
                x: x,
                y: y,
                config: {}
            });
        }
    }
};

_this.selectActivity = function(idx) {
    _this.c.selectedActivityIdx = idx;
};

_this.removeActivity = function() {
    if (_this.c.selectedActivityIdx !== null) {
        _this.c.workflowActivities.splice(_this.c.selectedActivityIdx, 1);
        _this.c.selectedActivityIdx = null;
    }
};

_this.zoomIn = function() {
    _this.c.zoom = Math.min(_this.c.zoom + 0.1, 2);
};

_this.zoomOut = function() {
    _this.c.zoom = Math.max(_this.c.zoom - 0.1, 0.5);
};

_this.zoomReset = function() {
    _this.c.zoom = 1;
};

_this.saveWorkflow = function() {
    const payload = {
        name: _this.c.currentWorkflow.name,
        activities: _this.c.workflowActivities
    };
    shared.http.post(`/api/workflows/definitions`, payload).then(r => {
        if (r.data?.success) {
            shared.notify("Workflow saved successfully", "success");
        }
    });
};

_this.publishWorkflow = function() {
    if (!_this.c.currentWorkflow.id) {
        shared.notify("Please save workflow first", "warning");
        return;
    }
    shared.http.post(`/api/workflows/definitions/${_this.c.currentWorkflow.id}/publish`).then(r => {
        if (r.data?.success) {
            shared.notify("Workflow published successfully", "success");
        }
    });
};

_this.closeDesigner = function() {
    // TODO: Navigate back or close designer
    shared.navigate("/");
};

Object.defineProperty(_this.data, 'filteredActivities', {
    get() { return _this.filterActivities(); }
});

Object.defineProperty(_this.data, 'selectedActivity', {
    get() { 
        return _this.c?.selectedActivityIdx !== null ? 
            _this.c?.workflowActivities[_this.c?.selectedActivityIdx] : null;
    }
});

export default {
    name: _this.cid,
    data() {
        return _this.data;
    },
    computed: {
        filteredActivities() {
            return _this.filterActivities();
        },
        selectedActivity() {
            return _this.c?.selectedActivityIdx !== null ? 
                _this.c?.workflowActivities[_this.c?.selectedActivityIdx] : null;
        }
    },
    methods: {
        startDragActivity: _this.startDragActivity,
        startDragActivityInstance: _this.startDragActivityInstance,
        canvasDragOver: _this.canvasDragOver,
        canvasDrop: _this.canvasDrop,
        selectActivity: _this.selectActivity,
        removeActivity: _this.removeActivity,
        filterActivities: _this.filterActivities,
        zoomIn: _this.zoomIn,
        zoomOut: _this.zoomOut,
        zoomReset: _this.zoomReset,
        saveWorkflow: _this.saveWorkflow,
        publishWorkflow: _this.publishWorkflow,
        closeDesigner: _this.closeDesigner
    },
    mounted() {
        _this.c = this;
        _this.onMounted();
    }
};
</script>

<style scoped>
.workflow-designer {
    display: flex;
    flex-direction: column;
}

.designer-body {
    display: flex;
    overflow: hidden;
}

.designer-toolbox,
.designer-canvas,
.designer-properties {
    display: flex;
    flex-direction: column;
    overflow: hidden;
}

.designer-toolbox {
    border-right: 1px solid #dee2e6;
    background: #f8f9fa;
}

.designer-properties {
    border-left: 1px solid #dee2e6;
    background: #f8f9fa;
}

.activity-item {
    cursor: grab;
    transition: box-shadow 0.2s, transform 0.2s;
}

.activity-item:hover {
    box-shadow: 0 0.125rem 0.25rem rgba(0, 0, 0, 0.075);
    transform: translateX(2px);
}

.activity-item:active {
    cursor: grabbing;
}

.canvas-area {
    background: linear-gradient(45deg, #e9ecef 25%, transparent 25%, transparent 75%, #e9ecef 75%, #e9ecef),
                linear-gradient(45deg, #e9ecef 25%, transparent 25%, transparent 75%, #e9ecef 75%, #e9ecef);
    background-size: 20px 20px;
    background-position: 0 0, 10px 10px;
    background-color: #ffffff;
}

.canvas-content {
    position: relative;
    width: 100%;
    height: 100%;
}

.activity-instance {
    border: 2px solid #007bff;
    border-radius: 4px;
    background: white;
    box-shadow: 0 0.125rem 0.25rem rgba(0, 0, 0, 0.075);
}

.activity-instance:hover {
    box-shadow: 0 0.25rem 0.5rem rgba(0, 0, 0, 0.15);
    border-color: #0056b3;
}

.canvas-svg {
    position: absolute;
    top: 0;
    left: 0;
    pointer-events: none;
}

.canvas-header {
    display: flex;
    align-items: center;
    justify-content: space-between;
    border-bottom: 1px solid #dee2e6;
}

.properties-panel {
    display: flex;
    flex-direction: column;
}

.form-label-sm {
    font-size: 0.875rem;
    font-weight: 500;
    margin-bottom: 0.25rem;
}

.form-control-sm,
.form-select-sm {
    font-size: 0.875rem;
}
</style>
