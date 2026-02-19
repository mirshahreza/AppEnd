<template>
  <div class="workflow-designer-test p-3">
    <!-- Header -->
    <div class="d-flex justify-content-between align-items-center mb-3">
      <h5>Workflow Designer - Test Version</h5>
      <button class="btn btn-sm btn-outline-danger" @click="closeDesigner">
        <i class="fas fa-times"></i> Close
      </button>
    </div>

    <!-- Workflow Info Card -->
    <div class="card mb-3">
      <div class="card-header bg-primary text-white">
        <strong>{{ workflow.name }}</strong>
      </div>
      <div class="card-body">
        <p class="mb-1"><strong>ID:</strong> {{ workflow.id }}</p>
        <p class="mb-1"><strong>Description:</strong> {{ workflow.description }}</p>
        <p class="mb-0"><strong>Activities:</strong> {{ workflow.activities.length }}</p>
      </div>
    </div>

    <!-- Activities Display -->
    <div class="card">
      <div class="card-header">
        <strong>Activities Flow</strong>
      </div>
      <div class="card-body" style="background: #f9f9f9;">
        <!-- Simple horizontal layout -->
        <div class="activities-flow">
          <div 
            v-for="(activity, index) in workflow.activities" 
            :key="activity.id"
            class="activity-box"
            :class="'activity-' + activity.type.toLowerCase()"
            @click="selectActivity(activity.id)">
            
            <div class="activity-header">
              <i :class="getActivityIcon(activity.type)"></i>
              <strong>{{ activity.displayName || activity.type }}</strong>
            </div>
            
            <div class="activity-body">
              <small class="text-muted">{{ activity.type }}</small>
              <div class="mt-1">
                <span class="badge bg-secondary">ID: {{ activity.id }}</span>
              </div>
            </div>

            <!-- Arrow to next -->
            <div v-if="index < workflow.activities.length - 1" class="activity-arrow">
              ➜
            </div>
          </div>
        </div>

        <!-- Connections Info -->
        <div class="mt-4 p-3 bg-white border rounded">
          <h6>Connections ({{ workflow.connections.length }})</h6>
          <div v-for="conn in workflow.connections" :key="conn.id" class="connection-item">
            <span class="badge bg-info">{{ getActivityName(conn.sourceActivityId) }}</span>
            <span class="mx-2">→ [{{ conn.sourceOutcome }}] →</span>
            <span class="badge bg-success">{{ getActivityName(conn.targetActivityId) }}</span>
          </div>
        </div>
      </div>
    </div>

    <!-- Debug Panel -->
    <div class="card mt-3">
      <div class="card-header bg-dark text-white">
        <small>Debug Info</small>
      </div>
      <div class="card-body bg-light">
        <pre style="font-size: 11px; max-height: 200px; overflow: auto;">{{ debugInfo }}</pre>
      </div>
    </div>

    <!-- Raw Workflow Data Panel -->
    <div class="card mt-3">
      <div class="card-header bg-warning text-dark">
        <small><strong>Raw Workflow Data (from backend)</strong></small>
      </div>
      <div class="card-body bg-light">
        <pre style="font-size: 10px; max-height: 300px; overflow: auto; white-space: pre-wrap;">{{ rawWorkflowData }}</pre>
      </div>
    </div>

    <!-- Save Button -->
    <div class="mt-3">
      <button class="btn btn-primary" @click="saveWorkflow">
        <i class="fas fa-save"></i> Save & Close
      </button>
      <button class="btn btn-secondary ms-2" @click="closeDesigner">
        <i class="fas fa-times"></i> Cancel
      </button>
    </div>
  </div>
</template>

<script>
export default {
  name: "WorkflowDesignerTest",
  props: {
    cid: String,
  },
  data() {
    return {
      workflow: {
        id: "",
        name: "",
        description: "",
        activities: [],
        connections: [],
      },
      selectedActivityId: null,
      rawWorkflowData: null, // Store raw backend data for debugging
    };
  },
  computed: {
    debugInfo() {
      const params = shared["params_" + this.cid];
      return {
        cid: this.cid,
        workflowId: this.workflow.id,
        workflowName: this.workflow.name,
        activitiesCount: this.workflow.activities.length,
        connectionsCount: this.workflow.connections.length,
        params: params,
        
        // Show full workflow structure
        fullWorkflow: this.workflow,
        
        // Show raw activities data
        activitiesData: this.workflow.activities,
      };
    },
  },
  methods: {
    loadWorkflow() {
      console.log("=".repeat(60));
      console.log("🚀 WorkflowDesignerTest - Loading");
      console.log("=".repeat(60));

      try {
        const params = shared["params_" + this.cid];
        const workflowId = params?.workflowId;
        
        console.log("📦 Params:", params);
        console.log("🔑 Workflow ID:", workflowId);

        // Try to load real workflow data from backend
        if (workflowId) {
          console.log("📡 Attempting to load workflow from backend...");
          
          rpcAEP('GetWorkflowDefinitions', {}, (data) => {
            try {
              console.log("📦 GetWorkflowDefinitions response:", data);
              
              const payload = Array.isArray(data) ? (data[0] || {}) : (data || {});
              const list = payload.Result || payload.result || payload.workflows || payload.Workflows || payload;
              const workflows = Array.isArray(list) ? list : [];
              
              console.log("📋 All workflows:", workflows.length);
              
              // Find the workflow by ID
              const targetWorkflow = workflows.find(w => w.Id === workflowId || w.id === workflowId);
              
              if (targetWorkflow) {
                console.log("✅ Found workflow:", targetWorkflow);
                this.parseWorkflowDefinition(targetWorkflow);
              } else {
                console.warn("⚠️ Workflow not found in list, using mock data");
                this.loadMockWorkflow(workflowId);
              }
            } catch (error) {
              console.error("❌ Error processing response:", error);
              this.loadMockWorkflow(workflowId);
            }
          }, (error) => {
            console.error("❌ RPC Error loading workflows:", error);
            this.loadMockWorkflow(workflowId);
          });
        } else {
          console.warn("⚠️ No workflowId provided, using mock data");
          this.loadMockWorkflow("unknown");
        }
      } catch (error) {
        console.error("❌ Critical error in loadWorkflow:", error);
        this.loadMockWorkflow("error");
      }
    },
    
    parseWorkflowDefinition(workflowData) {
      console.log("🔍 Parsing workflow definition...");
      console.log("🔍 Workflow data:", workflowData);
      
      try {
        // Validate input
        if (!workflowData) {
          console.error("❌ workflowData is null or undefined");
          this.loadMockWorkflow("invalid");
          return;
        }
        
        // Store raw data for debugging
        this.rawWorkflowData = JSON.parse(JSON.stringify(workflowData));
        
        // Extract RawJson safely
        let rawJson = workflowData.RawJson || workflowData.rawJson;
        console.log("🔍 RawJson type:", typeof rawJson);
        console.log("🔍 RawJson content:", rawJson);
        
        let definition = {};
        if (rawJson) {
          if (typeof rawJson === 'string') {
            try {
              definition = JSON.parse(rawJson);
            } catch (parseError) {
              console.error("❌ Failed to parse RawJson:", parseError);
              definition = {};
            }
          } else {
            definition = rawJson || {};
          }
        }
        
        console.log("🔍 Parsed definition:", definition);
        console.log("🔍 Definition keys:", Object.keys(definition));
        
        // Extract activities from multiple possible locations
        let activities = [];
        
        // Try different paths
        if (definition.root && definition.root.activities) {
          console.log("✅ Found activities in root.activities");
          activities = definition.root.activities;
        } else if (definition.root && definition.root.Activities) {
          console.log("✅ Found activities in root.Activities");
          activities = definition.root.Activities;
        } else if (definition.activities) {
          console.log("✅ Found activities in activities");
          activities = definition.activities;
        } else if (definition.Activities) {
          console.log("✅ Found activities in Activities");
          activities = definition.Activities;
        } else {
          console.warn("⚠️ No activities found in definition");
          console.log("Available definition keys:", Object.keys(definition));
          if (definition.root) {
            console.log("Root keys:", Object.keys(definition.root));
          }
        }
        
        console.log("🔍 Found activities:", activities);
        console.log("🔍 Activities count:", Array.isArray(activities) ? activities.length : 'not an array');
        
        if (!Array.isArray(activities)) {
          console.warn("⚠️ Activities is not an array, converting to empty array");
          activities = [];
        }
        
        // Map Elsa activities to our format
        const mappedActivities = activities.map((act, index) => {
          console.log(`🔍 Processing activity ${index}:`, act);
          
          const actType = act?.type || act?.Type || 'Unknown';
          const actName = act?.name || act?.Name || actType;
          const actId = act?.id || act?.Id || `activity-${index}`;
          
          return {
            id: actId,
            type: this.getActivityType(actType),
            displayName: actName,
            description: act?.description || act?.Description || '',
            x: 50 + (index * 250),
            y: 50,
            ...act
          };
        });
        
        console.log("✅ Mapped activities:", mappedActivities);
        
        // Extract connections
        let connections = [];
        activities.forEach((act, index) => {
          if (!act) return;
          
          const actId = act.id || act.Id || `activity-${index}`;
          
          // Check for outcomes/transitions
          if (act.transitions || act.Transitions) {
            const trans = act.transitions || act.Transitions;
            Object.keys(trans).forEach(outcome => {
              const targets = trans[outcome];
              if (Array.isArray(targets)) {
                targets.forEach((targetId, tIndex) => {
                  connections.push({
                    id: `conn-${actId}-${outcome}-${tIndex}`,
                    sourceActivityId: actId,
                    targetActivityId: targetId,
                    sourceOutcome: outcome
                  });
                });
              }
            });
          }
        });
        
        console.log("✅ Extracted connections:", connections);
        
        this.workflow = {
          id: workflowData.Id || workflowData.id || 'unknown',
          name: workflowData.Name || workflowData.name || definition.name || 'Unnamed Workflow',
          description: workflowData.Description || workflowData.description || definition.description || '',
          activities: mappedActivities,
          connections: connections,
        };
        
        console.log("✅ Final workflow:", this.workflow);
        
        if (mappedActivities.length > 0) {
          showSuccess(`Workflow loaded: ${mappedActivities.length} activities found`);
        } else {
          showWarning("Workflow loaded but contains no activities");
        }
        
      } catch (error) {
        console.error("❌ Error parsing workflow:", error);
        console.error("Error stack:", error.stack);
        showError("Failed to parse workflow: " + error.message);
        
        // Fallback to mock data
        const fallbackId = workflowData?.Id || workflowData?.id || 'error';
        this.loadMockWorkflow(fallbackId);
      }
    },
    
    loadMockWorkflow(workflowId) {
      console.warn("Loading mock workflow data...");
      
      // Reset workflow data
      this.workflow = {
        id: "mock-" + workflowId,
        name: "Mock Workflow " + workflowId,
        description: "This is a mock workflow for testing purposes.",
        activities: [],
        connections: [],
      };
      
      this.$emit('workflowLoaded', this.workflow);
    },
    
    closeDesigner() {
      this.$emit('closeDesigner');
    },
    
    saveWorkflow() {
      console.log("🚀 Saving workflow...", this.workflow);
      
      const params = shared["params_" + this.cid];
      
      // Basic validation
      if (!this.workflow.name) {
        return alert("Please provide a name for the workflow");
      }
      
      // Prepare payload
      const payload = {
        id: this.workflow.id,
        name: this.workflow.name,
        description: this.workflow.description,
        activities: this.workflow.activities.map(act => ({
          id: act.id,
          type: act.type,
          name: act.displayName,
          description: act.description,
          transitions: this.getActivityTransitions(act),
        })),
        connections: this.workflow.connections,
      };
      
      console.log("📦 Save payload:", payload);
      
      // For debugging, disable actual saving
      return alert("Debug mode is active, not saving real data.");

      rpcAEP('SaveWorkflowDefinition', payload, (response) => {
        console.log("✅ Save response:", response);
        alert("Workflow saved successfully!");
        this.closeDesigner();
      }, (error) => {
        console.error("❌ Error saving workflow:", error);
        alert("Failed to save workflow: " + (error.message || "Unknown error"));
      });
    },
    
    getActivityTransitions(activity) {
      const trans = {};
      
      // Extract transition info from activity
      this.workflow.connections.forEach(conn => {
        if (conn.sourceActivityId === activity.id) {
          trans[conn.sourceOutcome] = trans[conn.sourceOutcome] || [];
          trans[conn.sourceOutcome].push(conn.targetActivityId);
        }
      });
      
      console.log("🔄 Transitions for activity", activity.id, ":", trans);
      
      return trans;
    },
    
    getActivityIcon(type) {
      const icons = {
        Start: 'fas fa-play-circle',
        End: 'fas fa-stop-circle',
        Task: 'fas fa-tasks',
        Gateway: 'fas fa-exchange-alt',
        Timer: 'fas fa-clock',
        Signal: 'fas fa-bell',
        Webhook: 'fas fa-code-branch',
        // Add more mappings as needed
      };
      
      return icons[type] || 'fas fa-cogs'; // Default to 'cogs' icon
    },
    
    getActivityName(activityId) {
      const activity = this.workflow.activities.find(act => act.id === activityId);
      return activity ? activity.displayName || activityId : activityId;
    },
    
    getActivityType(elementType) {
      const typeMapping = {
        'HttpRequest': 'Request',
        'HttpResponse': 'Response',
        'Timer': 'Delay',
        'Signal': 'Event',
        'Webhook': 'API Call',
        'Assign': 'Script',
        'Log': 'Logger',
        // Add more mappings as needed
      };
      
      return typeMapping[elementType] || elementType;
    },
    
    selectActivity(activityId) {
      this.selectedActivityId = activityId;
      
      // Emit event for the selected activity
      this.$emit('activitySelected', activityId);
    },
    
    closeDesigner() {
      // Confirm dialog
      if (confirm("Are you sure you want to close the designer? All unsaved changes will be lost.")) {
        this.$emit('closeDesigner');
      }
    },
    
    async saveWorkflow() {
      console.log("🚀 Saving workflow...", this.workflow);
      
      const params = shared["params_" + this.cid];
      
      // Basic validations
      if (!this.workflow.name) {
        return alert("Please provide a name for the workflow");
      }
      
      if (this.workflow.activities.length === 0) {
        return alert("No activities to save. Please add at least one activity to the workflow.");
      }
      
      // Check for duplicate activity IDs
      const activityIds = this.workflow.activities.map(act => act.id);
      const hasDuplicates = activityIds.length !== new Set(activityIds).size;
      if (hasDuplicates) {
        return alert("Duplicate activity IDs found. Please ensure all activities have unique IDs.");
      }
      
      // Prepare payload
      const payload = {
        id: this.workflow.id,
        name: this.workflow.name,
        description: this.workflow.description,
        activities: this.workflow.activities.map(act => ({
          id: act.id,
          type: act.type,
          name: act.displayName,
          description: act.description,
          transitions: this.getActivityTransitions(act),
        })),
        connections: this.workflow.connections,
      };
      
      console.log("📦 Save payload:", payload);
      
      try {
        // For debugging, disable actual saving
        return alert("Debug mode is active, not saving real data.");

        const response = await rpcAEP('SaveWorkflowDefinition', payload);
        console.log("✅ Save response:", response);
        alert("Workflow saved successfully!");
        this.closeDesigner();
      } catch (error) {
        console.error("❌ Error saving workflow:", error);
        alert("Failed to save workflow: " + (error.message || "Unknown error"));
      }
    },
  },
  
  mounted() {
    this.loadWorkflow();
  },
};
</script>

<style scoped>
.workflow-designer-test {
  max-width: 1200px;
  margin: 0 auto;
}

.activities-flow {
  display: flex;
  flex-wrap: nowrap;
  overflow-x: auto;
  padding: 10px 0;
}

.activity-box {
  background: #fff;
  border: 1px solid #ddd;
  border-radius: 4px;
  padding: 10px;
  margin-right: 10px;
  min-width: 150px;
  position: relative;
  cursor: pointer;
  transition: transform 0.2s;
}

.activity-box:hover {
  transform: translateY(-2px);
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.15);
}

.activity-header {
  font-weight: bold;
  margin-bottom: 5px;
}

.activity-body {
  font-size: 14px;
}

.activity-arrow {
  position: absolute;
  top: 10px;
  right: -20px;
  font-size: 18px;
  color: #007bff;
}

.card {
  border: 1px solid #ddd;
  border-radius: 4px;
  margin-bottom: 20px;
}

.card-header {
  font-weight: bold;
  background: #f7f7f7;
  border-bottom: 1px solid #ddd;
}

.card-body {
  padding: 15px;
}

.badge {
  font-size: 12px;
}

.btn-primary {
  background: #007bff;
  border-color: #007bff;
}

.btn-primary:hover {
  background: #0056b3;
  border-color: #004085;
}

.btn-secondary {
  background: #6c757d;
  border-color: #6c757d;
}

.btn-secondary:hover {
  background: #5a6268;
  border-color: #545b62;
}

.bg-warning {
  background-color: #fff3cd !important;
}

.bg-info {
  background-color: #d1e7dd !important;
}

.bg-light {
  background-color: #f8f9fa !important;
}

.text-dark {
  color: #23272b !important;
}

.text-muted {
  color: #868e96 !important;
}
</style>
