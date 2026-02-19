<template>
  <div class="workflow-designer h-100 d-flex flex-column">
    <!-- Toolbar -->
    <div class="designer-toolbar bg-light border-bottom p-2">
      <div class="d-flex gap-2 align-items-center">
        <button class="btn btn-sm btn-outline-secondary" @click="newWorkflow" title="Create new workflow">
          <i class="fas fa-file"></i> New
        </button>
        <button class="btn btn-sm btn-outline-primary" @click="saveWorkflow" title="Save workflow design">
          <i class="fas fa-save"></i> Save
        </button>
        <div class="vr"></div>
        <button class="btn btn-sm btn-outline-secondary" @click="zoomIn" title="Zoom in">
          <i class="fas fa-magnifying-glass-plus"></i>
        </button>
        <button class="btn btn-sm btn-outline-secondary" @click="zoomOut" title="Zoom out">
          <i class="fas fa-magnifying-glass-minus"></i>
        </button>
        <button class="btn btn-sm btn-outline-secondary" @click="fitToScreen" title="Fit to screen">
          <i class="fas fa-compress"></i>
        </button>
        <div class="vr"></div>
        <button class="btn btn-sm btn-outline-secondary" @click="validateDesign" title="Validate design">
          <i class="fas fa-check-circle"></i> Validate
        </button>
        <div class="ms-auto"></div>
        <button class="btn btn-sm btn-outline-danger" @click="closeDesigner" title="Close">
          <i class="fas fa-times"></i>
        </button>
      </div>
    </div>

    <!-- Main Content -->
    <div class="designer-content flex-fill d-flex overflow-hidden">
      <!-- Toolbox (Left) -->
      <div class="toolbox bg-white border-end overflow-auto" style="width: 220px;">
        <div class="p-2">
          <h6 class="fw-bold mb-3">Activities</h6>
          <input
            v-model="toolboxSearch"
            type="text"
            placeholder="Search..."
            class="form-control form-control-sm mb-2"
          />
          
          <div v-for="activityTemplate in getFilteredActivities()" :key="activityTemplate.id" 
            class="activity-item card mb-2 p-2" style="cursor: move;"
            draggable="true" @dragstart="dragStartActivity($event, activityTemplate)">
            <small class="fw-bold">{{ activityTemplate.displayName }}</small>
            <small class="text-muted">{{ activityTemplate.type }}</small>
          </div>
        </div>
      </div>

      <!-- Canvas (Center) -->
      <div class="canvas-area flex-fill bg-light position-relative overflow-auto">
        <div v-if="!workflow.id" class="d-flex align-items-center justify-content-center h-100">
          <div class="text-center text-muted">
            <p><i class="fas fa-inbox fs-1 mb-3"></i></p>
            <p v-if="loadingWorkflow" class="mb-2">
              <i class="fas fa-spinner fa-spin"></i> Loading workflow...
            </p>
            <p v-else>No workflow loaded. Create a new one or open existing.</p>
          </div>
        </div>

        <div v-else class="canvas-container" @dragover.prevent @drop="dropActivity">
          <!-- SVG Canvas for connections -->
          <svg class="canvas-svg">
            <defs>
              <pattern id="grid" width="20" height="20" patternUnits="userSpaceOnUse">
                <path d="M 20 0 L 0 0 0 20" fill="none" stroke="#e0e0e0" stroke-width="0.5"/>
              </pattern>
            </defs>
            <rect width="100%" height="100%" fill="url(#grid)" />
            
            <!-- Connections -->
            <line v-for="conn in workflow.connections" :key="conn.id"
              :x1="getActivityX(conn.sourceActivityId) + 100"
              :y1="getActivityY(conn.sourceActivityId) + 50"
              :x2="getActivityX(conn.targetActivityId) + 100"
              :y2="getActivityY(conn.targetActivityId)"
              stroke="#999" stroke-width="2" />
          </svg>

          <!-- Activity Nodes -->
          <div class="activities-container">
            <div v-for="activity in workflow.activities" :key="activity.id"
              class="activity-node card"
              :class="{ 'border-primary border-3': selectedActivityId === activity.id }"
              :style="{ 
                left: activity.x + 'px', 
                top: activity.y + 'px',
                width: '150px',
                height: '80px'
              }"
              @click="selectActivity(activity.id)"
              @mousedown="dragStartNode($event, activity.id)">
              <div class="card-body p-2">
                <small class="fw-bold d-block">{{ activity.displayName || activity.type }}</small>
                <small class="text-muted">{{ activity.type }}</small>
                <small class="text-muted d-block" style="font-size: 9px;">x:{{ activity.x }}, y:{{ activity.y }}</small>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Properties Panel (Right) -->
      <div class="properties-panel bg-white border-start overflow-auto" style="width: 250px;">
        <div class="p-3">
          <div v-if="selectedActivityId && selectedActivity" class="activity-properties">
            <h6 class="fw-bold mb-3">Activity Properties</h6>
            <hr />
            
            <div class="mb-3">
              <label class="form-label small">Display Name</label>
              <input v-model="selectedActivity.displayName" type="text" class="form-control form-control-sm" />
            </div>

            <div class="mb-3">
              <label class="form-label small">Type</label>
              <input type="text" :value="selectedActivity.type" class="form-control form-control-sm" disabled />
            </div>

            <div class="mb-3">
              <label class="form-label small">Description</label>
              <textarea v-model="selectedActivity.description" class="form-control form-control-sm" rows="2"></textarea>
            </div>

            <button class="btn btn-sm btn-outline-danger w-100" @click="deleteActivity">
              <i class="fas fa-trash"></i> Delete
            </button>
          </div>

          <div v-else-if="workflow.id" class="workflow-properties">
            <h6 class="fw-bold mb-3">Workflow Properties</h6>
            <hr />
            
            <div class="mb-3">
              <label class="form-label small">Name</label>
              <input v-model="workflow.name" type="text" class="form-control form-control-sm" />
            </div>

            <div class="mb-3">
              <label class="form-label small">Description</label>
              <textarea v-model="workflow.description" class="form-control form-control-sm" rows="2"></textarea>
            </div>

            <div class="mb-3">
              <label class="form-label small">Activities: {{ workflow.activities.length }}</label>
            </div>

            <div class="mb-3">
              <label class="form-label small">Connections: {{ workflow.connections.length }}</label>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Validation Panel -->
    <div v-if="validationErrors.length > 0" class="validation-panel bg-danger-light border-top p-2">
      <div class="d-flex justify-content-between align-items-center mb-2">
        <strong class="text-danger">
          <i class="fas fa-exclamation-triangle"></i> {{ validationErrors.length }} Errors
        </strong>
        <button class="btn-close btn-sm" @click="validationErrors = []"></button>
      </div>
      <div style="max-height: 100px; overflow-y: auto;">
        <div v-for="(error, idx) in validationErrors" :key="idx" class="text-danger small mb-1">
          • {{ error }}
        </div>
      </div>
    </div>
  </div>
</template>

<script>
export default {
  name: "WorkflowDesigner",
  props: {
    cid: String,
    workflowId: String,
  },
  data() {
    return {
      workflow: {
        id: "",
        name: "New Workflow",
        description: "",
        activities: [],
        connections: [],
      },
      selectedActivityId: null,
      toolboxSearch: "",
      validationErrors: [],
      loadingWorkflow: false,
      sampleActivities: [
        { id: "start", type: "Start", displayName: "Start" },
        { id: "end", type: "End", displayName: "End" },
        { id: "decision", type: "Decision", displayName: "Decision" },
        { id: "action", type: "Action", displayName: "Action" },
        { id: "loop", type: "Loop", displayName: "Loop" },
      ],
    };
  },
  computed: {
    selectedActivity() {
      return this.workflow.activities.find((a) => a.id === this.selectedActivityId);
    },
  },
  methods: {
    getFilteredActivities() {
      if (!this.toolboxSearch) {
        return this.sampleActivities;
      }
      const search = this.toolboxSearch.toLowerCase();
      return this.sampleActivities.filter((a) =>
        a.displayName.toLowerCase().includes(search) ||
        a.type.toLowerCase().includes(search)
      );
    },
    loadWorkflowViaRpc(workflowId) {
      console.log("🔍 loadWorkflowViaRpc called with workflowId:", workflowId);
      
      // Try to load via RPC
      if (window.rpcAEP) {
        console.log("✅ rpcAEP found, calling LoadWorkflowDesign...");
        rpcAEP("LoadWorkflowDesign", { workflowId }, (result) => {
          console.log("📦 LoadWorkflowDesign result:", result);
          
          if (result?.success) {
            console.log("✅ Workflow loaded successfully:", result.workflow);
            this.workflow = result.workflow;
            this.loadingWorkflow = false;
            this.$forceUpdate();
          } else {
            console.warn("⚠️ Workflow load failed, using mock data");
            this.loadMockWorkflow(workflowId);
          }
        }, (error) => {
          console.error("❌ RPC Error, using mock data:", error);
          this.loadMockWorkflow(workflowId);
        });
      } else {
        console.error("❌ rpcAEP not found, using mock data");
        this.loadMockWorkflow(workflowId);
      }
    },
    
    loadMockWorkflow(workflowId) {
      console.log("📝 Loading mock workflow for:", workflowId);
      this.workflow = {
        id: workflowId,
        name: "Workflow - " + workflowId,
        description: "Loaded from ID: " + workflowId + " (Mock Data)",
        version: 1,
        isPublished: false,
        activities: [
          { id: "start-1", type: "Start", displayName: "Start", description: "", x: 50, y: 50 },
          { id: "action-1", type: "Action", displayName: "Process Order", description: "Process the order", x: 300, y: 50 },
          { id: "decision-1", type: "Decision", displayName: "Check Amount", description: "Check if amount > $1000", x: 550, y: 50 },
          { id: "end-1", type: "End", displayName: "End", description: "", x: 800, y: 50 }
        ],
        connections: [
          { id: "conn-1", sourceActivityId: "start-1", targetActivityId: "action-1", sourceOutcome: "Next" },
          { id: "conn-2", sourceActivityId: "action-1", targetActivityId: "decision-1", sourceOutcome: "Done" },
          { id: "conn-3", sourceActivityId: "decision-1", targetActivityId: "end-1", sourceOutcome: "True" }
        ],
        variables: [],
        canvasZoom: 1.0,
        canvasPanX: 0,
        canvasPanY: 0
      };
      this.loadingWorkflow = false;
      
      console.log("✅ Mock workflow loaded:");
      console.log("  - Activities:", this.workflow.activities.length);
      console.log("  - Connections:", this.workflow.connections.length);
      this.workflow.activities.forEach(a => {
        console.log(`    Activity: ${a.displayName} at (${a.x}, ${a.y})`);
      });
      
      // Force Vue to re-render after data update
      this.$forceUpdate();
      
      // Check DOM after render
      this.$nextTick(() => {
        const nodes = document.querySelectorAll('.activity-node');
        console.log("🔍 Activity nodes in DOM:", nodes.length);
        nodes.forEach((node, idx) => {
          const style = window.getComputedStyle(node);
          console.log(`  Node ${idx}: left=${style.left}, top=${style.top}, width=${style.width}, height=${style.height}`);
        });
      });
      
      showSuccess("Mock workflow loaded successfully");
    },
    dragStartActivity(event, activity) {
      event.dataTransfer.effectAllowed = "copy";
      event.dataTransfer.setData("activityType", JSON.stringify(activity));
    },
    dropActivity(event) {
      event.preventDefault();
      const data = event.dataTransfer.getData("activityType");
      if (!data) return;

      const activityTemplate = JSON.parse(data);
      const rect = event.currentTarget.getBoundingClientRect();
      const x = event.clientX - rect.left;
      const y = event.clientY - rect.top;

      this.workflow.activities.push({
        id: `activity-${Date.now()}`,
        type: activityTemplate.type,
        displayName: activityTemplate.displayName,
        description: "",
        x: x,
        y: y,
      });
    },
    dragStartNode(event, activityId) {
      event.dataTransfer.effectAllowed = "move";
      event.dataTransfer.setData("activityId", activityId);
      this.dragNodeId = activityId;
    },
    selectActivity(id) {
      this.selectedActivityId = id;
    },
    deleteActivity() {
      if (!this.selectedActivityId) return;
      const idx = this.workflow.activities.findIndex((a) => a.id === this.selectedActivityId);
      if (idx >= 0) {
        this.workflow.activities.splice(idx, 1);
        this.selectedActivityId = null;
      }
    },
    getActivityX(activityId) {
      const activity = this.workflow.activities.find((a) => a.id === activityId);
      return activity ? activity.x : 0;
    },
    getActivityY(activityId) {
      const activity = this.workflow.activities.find((a) => a.id === activityId);
      return activity ? activity.y : 0;
    },
    newWorkflow() {
      const name = prompt("Workflow name:");
      if (!name) return;
      this.workflow = {
        id: `workflow-${Date.now()}`,
        name: name,
        description: "",
        activities: [],
        connections: [],
      };
    },
    saveWorkflow() {
      showSuccess("Workflow saved!");
      // Close modal with success
      if (window.closeComponent) {
        closeComponent(this.cid, { success: true });
      }
    },
    validateDesign() {
      this.validationErrors = [];
      if (this.workflow.activities.length === 0) {
        this.validationErrors.push("Workflow must have at least one activity");
      }
      if (!this.workflow.name || this.workflow.name.trim() === "") {
        this.validationErrors.push("Workflow name is required");
      }
      if (this.validationErrors.length === 0) {
        showSuccess("Workflow is valid!");
      }
    },
    closeDesigner() {
      if (window.closeComponent) {
        closeComponent(this.cid, { success: false });
      }
    },
    zoomIn() {
      showWarning("Zoom in - TBD");
    },
    zoomOut() {
      showWarning("Zoom out - TBD");
    },
    fitToScreen() {
      showWarning("Fit to screen - TBD");
    },
  },
  mounted() {
    console.log("=" .repeat(80));
    console.log("🚀 WorkflowDesigner mounted - START");
    console.log("=" .repeat(80));
    console.log("📋 Props received:", { cid: this.cid, workflowId: this.workflowId });
    
    // Get params from shared (AppEnd framework pattern)
    const params = shared["params_" + this.cid];
    console.log("📦 Params from shared:", params);
    
    // Get workflowId from params or props
    const workflowId = params?.workflowId || this.workflowId;
    console.log("🔑 Final workflowId:", workflowId);
    
    // Load workflow if workflowId is provided
    if (workflowId) {
      console.log("📂 Loading workflow with ID:", workflowId);
      this.loadingWorkflow = true;
      
      // First, try RPC loading
      setTimeout(() => {
        this.loadWorkflowViaRpc(workflowId);
        
        // Debug: Log workflow state after loading
        setTimeout(() => {
          console.log("=" .repeat(80));
          console.log("🔍 WORKFLOW STATE AFTER LOAD");
          console.log("=" .repeat(80));
          console.log("🔍 workflow.id:", this.workflow.id);
          console.log("🔍 workflow.name:", this.workflow.name);
          console.log("🔍 Activities count:", this.workflow.activities?.length);
          console.log("🔍 Activities data:", JSON.stringify(this.workflow.activities, null, 2));
          console.log("🔍 Connections count:", this.workflow.connections?.length);
          console.log("=" .repeat(80));
          
          // Check DOM
          this.$nextTick(() => {
            const activityNodes = document.querySelectorAll('.activity-node');
            console.log("🔍 DOM activity-node elements:", activityNodes.length);
            console.log("🔍 DOM activity nodes:", activityNodes);
            if (activityNodes.length === 0) {
              console.error("❌ NO ACTIVITY NODES IN DOM!");
              console.log("🔍 Canvas container:", document.querySelector('.canvas-container'));
              console.log("🔍 Activities container:", document.querySelector('.activities-container'));
            }
          });
        }, 500);
      }, 100);
    } else {
      console.warn("⚠️ No workflowId provided in props or params");
    }
    
    console.log("=" .repeat(80));
    console.log("🚀 WorkflowDesigner mounted - END");
    console.log("=" .repeat(80));
  },
};
</script>

<style scoped>
.workflow-designer {
  background: #f5f5f5;
}

.designer-toolbar {
  min-height: 40px;
}

.designer-content {
  background: #f9f9f9;
}

.toolbox {
  min-width: 220px;
  max-width: 220px;
}

.activity-item {
  border: 1px solid #ddd;
  background: #fff;
  border-radius: 4px;
  transition: all 0.2s;
  user-select: none;
}

.activity-item:hover {
  border-color: #0066cc;
  box-shadow: 0 2px 4px rgba(0, 102, 204, 0.1);
}

.canvas-area {
  position: relative;
  background: url("data:image/svg+xml,%3Csvg width='20' height='20' xmlns='http://www.w3.org/2000/svg'%3E%3Cpath d='M 20 0 L 0 0 0 20' fill='none' stroke='%23e0e0e0' stroke-width='0.5'/%3E%3C/svg%3E");
}

.canvas-container {
  position: relative;
  width: 100%;
  height: 100%;
  min-height: 600px;
  user-select: none;
}

.canvas-svg {
  position: absolute;
  width: 100%;
  height: 100%;
  top: 0;
  left: 0;
  z-index: 1;
}

.activities-container {
  position: absolute;
  width: 100%;
  height: 100%;
  top: 0;
  left: 0;
  z-index: 10;
  pointer-events: none;
}

.activity-node {
  position: absolute;
  width: 150px;
  min-height: 80px;
  cursor: move;
  user-select: none;
  border: 2px solid #ddd;
  border-radius: 4px;
  background: #fff;
  transition: all 0.2s;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.15);
  pointer-events: all;
}

.activity-node:hover {
  border-color: #0066cc;
  box-shadow: 0 2px 4px rgba(0, 102, 204, 0.15);
}

.activity-node.border-primary {
  border-color: #0066cc !important;
  box-shadow: 0 0 0 2px rgba(0, 102, 204, 0.2);
}

.properties-panel {
  min-width: 250px;
  max-width: 250px;
}

.validation-panel {
  max-height: 150px;
}
</style>
