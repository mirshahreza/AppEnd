# Workflow Designer - Architecture & Implementation Plan

## 🏗️ معماری سطح بالا

```
┌─────────────────────────────────────────────────────────────┐
│                    Workflow Designer UI                     │
│  (Vue.js Component - آپاند استودیو)                      │
└────────────┬────────────────────────────────────┬───────────┘
             │                                    │
             ▼                                    ▼
┌────────────────────────────────┐   ┌──────────────────────┐
│  Canvas Manager                 │   │  Properties Panel    │
│ - Drag & Drop                   │   │ - Activity Props     │
│ - Zoom & Pan                    │   │ - Variable Editor    │
│ - Connection Drawing            │   │ - Expression Editor  │
└────────────┬────────────────────┘   └──────────────────────┘
             │
             ▼
┌─────────────────────────────────────────────────────────────┐
│              Workflow Model (JSON Structure)                │
│  {                                                          │
│    "activities": [...],                                    │
│    "connections": [...],                                   │
│    "variables": [...]                                      │
│  }                                                          │
└────────────┬────────────────────────────────────────────────┘
             │
             ▼ (RPC: SaveWorkflow, ExecuteWorkflow)
┌─────────────────────────────────────────────────────────────┐
│           Backend Services (C# - AppEndWorkflow)            │
│  - WorkflowEngine                                           │
│  - ActivityRegistry                                         │
│  - WorkflowValidator                                        │
│  - ExecutionContext Manager                                │
└────────────┬────────────────────────────────────────────────┘
             │
             ▼
┌─────────────────────────────────────────────────────────────┐
│        SQL Server Persistence (EF Core)                     │
│  - WorkflowDefinition                                       │
│  - WorkflowInstance                                         │
│  - WorkflowTask                                             │
│  - ExecutionLog                                             │
└─────────────────────────────────────────────────────────────┘
```

---

## 📦 Project Structure

```
AppEndWorkflow/
├── Designer/                          # NEW: Designer Logic
│   ├── Models/
│   │   ├── WorkflowDefinitionModel.cs
│   │   ├── ActivityModel.cs
│   │   ├── ConnectionModel.cs
│   │   ├── VariableModel.cs
│   │   └── WorkflowCanvasModel.cs
│   ├── Services/
│   │   ├── WorkflowDesignerService.cs
│   │   ├── ActivityRegistryService.cs
│   │   └── WorkflowValidationService.cs
│   └── Validators/
│       ├── ConnectionValidator.cs
│       └── ExpressionValidator.cs
├── Engine/                            # NEW: Execution Engine
│   ├── WorkflowExecutor.cs
│   ├── ActivityExecutor.cs
│   ├── ExecutionContext.cs
│   └── ExecutionLog.cs
├── Database/                          # NEW: Database Models
│   ├── WorkflowDefinitionDbModel.cs
│   ├── WorkflowInstanceDbModel.cs
│   ├── WorkflowTaskDbModel.cs
│   └── WorkflowDbContext.cs
├── Activities/                        # EXISTING: Activity Implementations
│   ├── BaseActivity.cs                # NEW: Base class
│   ├── FlowControl/
│   │   ├── StartActivity.cs           # NEW
│   │   ├── EndActivity.cs             # NEW
│   │   ├── DecisionActivity.cs        # NEW
│   │   ├── SwitchActivity.cs          # UPDATE
│   │   ├── WhileActivity.cs           # UPDATE
│   │   └── ForEachActivity.cs         # UPDATE
│   └── ...
└── WorkflowServices.cs                # EXISTING: RPC Bridge
```

---

## 📐 Core Data Models

### 1. WorkflowDefinitionModel

```csharp
public class WorkflowDefinitionModel
{
    public string Id { get; set; }              // "order-approval-v1"
    public string Name { get; set; }            // "Order Approval"
    public string Description { get; set; }
    public int Version { get; set; } = 1;
    public bool IsPublished { get; set; }
    
    // Canvas State
    public double? CanvasZoom { get; set; } = 1.0;
    public double? CanvasPanX { get; set; } = 0;
    public double? CanvasPanY { get; set; } = 0;
    
    // Workflow Composition
    public List<ActivityModel> Activities { get; set; } = [];
    public List<ConnectionModel> Connections { get; set; } = [];
    public List<VariableModel> Variables { get; set; } = [];
    
    // Metadata
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }
    
    // Tags & Categories
    public List<string> Tags { get; set; } = [];
}
```

### 2. ActivityModel

```csharp
public class ActivityModel
{
    public string Id { get; set; }                  // "activity-1"
    public string Type { get; set; }                // "Decision", "Switch", "HttpCall"
    public string? DisplayName { get; set; }        // "Check Order Amount"
    
    // Visual Position on Canvas
    public double X { get; set; }                   // Canvas coordinates
    public double Y { get; set; }
    public double Width { get; set; } = 200;
    public double Height { get; set; } = 100;
    
    // Activity Configuration
    public Dictionary<string, object?> Properties { get; set; } = [];
    // Example: 
    // {
    //   "condition": "Variables.orderAmount > 1000",
    //   "timeout": "00:05:00"
    // }
    
    // Input/Output Mapping
    public Dictionary<string, string> InputMapping { get; set; } = [];  // "paramName" -> "variablePath"
    public Dictionary<string, string> OutputMapping { get; set; } = []; // "variablePath" -> "propertyName"
    
    // Variable Assignments
    public List<VariableAssignment> VariableAssignments { get; set; } = [];
    // Example: "orderStatus = orderData.status"
    
    // Error Handling
    public string? ErrorOutcome { get; set; } = "Error";
    public ActivityModel? ErrorHandler { get; set; }  // Reference to error handling activity
    
    // Metadata
    public bool IsDisabled { get; set; }
    public string? Description { get; set; }
    public Dictionary<string, string>? Metadata { get; set; }
}

public class VariableAssignment
{
    public string VariableName { get; set; }        // "orderStatus"
    public string Expression { get; set; }          // "Variables.data.status"
    public string ExpressionLanguage { get; set; } = "JavaScript"; // JavaScript, Liquid
}
```

### 3. ConnectionModel

```csharp
public class ConnectionModel
{
    public string Id { get; set; }                  // Unique identifier
    
    // Source & Target Activities
    public string SourceActivityId { get; set; }
    public string? SourceOutcome { get; set; }      // "True", "False", "Done", "Error"
    
    public string TargetActivityId { get; set; }
    
    // Visual representation
    public bool IsHighlighted { get; set; }
    public string? Label { get; set; }              // Optional label on connection
    
    // Validation
    public bool IsValid { get; set; } = true;
    public string? ValidationMessage { get; set; }
}
```

### 4. VariableModel

```csharp
public class VariableModel
{
    public string Name { get; set; }                // "orderAmount"
    public string Type { get; set; }                // "number", "string", "object", "array"
    public string Scope { get; set; } = "Global";   // Global, Local
    
    public object? DefaultValue { get; set; }
    public string? Description { get; set; }
    
    public bool IsReadOnly { get; set; }
    public bool IsRequired { get; set; }
}
```

### 5. VariableAssignmentModel

```csharp
public class VariableAssignmentModel
{
    public string VariableName { get; set; }        // "customerApproved"
    public string Expression { get; set; }          // "approvalTask.result == 'approved'"
    public string Language { get; set; } = "JavaScript";
    
    public bool IsConditional { get; set; }
    public string? Condition { get; set; }          // Only assign if true
}
```

---

## 🔌 Activity Base Classes

### BaseActivity

```csharp
public abstract class BaseActivity
{
    public string Id { get; set; }
    public string DisplayName { get; set; }
    
    // Outcomes this activity can produce
    public virtual string[] SupportedOutcomes => new[] { "Done" };
    
    // Metadata
    public virtual string ActivityType => this.GetType().Name;
    public virtual string ActivityCategory => "General";
    
    // Input/Output
    public Dictionary<string, object?> Inputs { get; set; } = [];
    public Dictionary<string, object?> Outputs { get; set; } = [];
    
    // Execution
    public abstract ExecutionOutcome Execute(ExecutionContext context);
    
    // Error Handling
    public virtual bool HasErrorHandler { get; set; }
    public virtual ExecutionOutcome OnError(ExecutionContext context, Exception ex)
    {
        return new ExecutionOutcome { OutcomeName = "Error", Data = ex.Message };
    }
    
    // Validation
    public virtual ValidationResult Validate()
    {
        return ValidationResult.Valid();
    }
}

public class ExecutionOutcome
{
    public string OutcomeName { get; set; }        // "Done", "True", "False", "Error"
    public object? Data { get; set; }              // Output data
    public TimeSpan Duration { get; set; }
}
```

### DecisionActivity

```csharp
public class DecisionActivity : BaseActivity
{
    public override string[] SupportedOutcomes => new[] { "True", "False" };
    public override string ActivityCategory => "FlowControl";
    
    public string Condition { get; set; }          // "Variables.amount > 1000"
    public string ConditionLanguage { get; set; } = "JavaScript";
    
    public override ExecutionOutcome Execute(ExecutionContext context)
    {
        bool result = EvaluateCondition(context);
        return new ExecutionOutcome
        {
            OutcomeName = result ? "True" : "False",
            Duration = context.StopWatch.Elapsed
        };
    }
    
    private bool EvaluateCondition(ExecutionContext context)
    {
        // JavaScript engine evaluation
        // Access: Variables, InputParameters, etc.
    }
}
```

### SwitchActivity

```csharp
public class SwitchActivity : BaseActivity
{
    public string Expression { get; set; }         // "Variables.status"
    public Dictionary<string, string> Cases { get; set; } = []; // "pending" -> "ProcessPending", "approved" -> "ProcessApproved"
    public string DefaultCase { get; set; } = "Default";
    
    public override string[] SupportedOutcomes => 
        Cases.Values.Concat(new[] { DefaultCase }).ToArray();
    
    public override ExecutionOutcome Execute(ExecutionContext context)
    {
        var value = EvaluateExpression(context);
        var outcome = Cases.ContainsKey(value?.ToString() ?? "") 
            ? Cases[value.ToString()]
            : DefaultCase;
        
        return new ExecutionOutcome { OutcomeName = outcome };
    }
}
```

### ParallelActivity / ForkJoin

```csharp
public class ParallelActivity : BaseActivity
{
    public List<string> ParallelBranchIds { get; set; } = [];
    public string JoinMode { get; set; } = "WaitAll"; // WaitAll, WaitAny
    
    public override string[] SupportedOutcomes => new[] { "Done" };
    
    public override ExecutionOutcome Execute(ExecutionContext context)
    {
        // Fork branches to parallel execution
        // Track completion based on JoinMode
        // Join results
        return new ExecutionOutcome { OutcomeName = "Done" };
    }
}
```

---

## 🎨 Designer Canvas Component (Vue.js)

### File: `AppEndStudio/components/WorkflowDesigner.vue`

```vue
<template>
  <div class="workflow-designer">
    <!-- Toolbar -->
    <div class="designer-toolbar">
      <button @click="newWorkflow">New</button>
      <button @click="openWorkflow">Open</button>
      <button @click="saveWorkflow">Save</button>
      <button @click="executeWorkflow">Execute</button>
      <div class="vr"></div>
      <button @click="zoomIn">Zoom In</button>
      <button @click="zoomOut">Zoom Out</button>
      <button @click="fitToScreen">Fit</button>
      <button @click="exportJson">Export</button>
      <button @click="importJson">Import</button>
    </div>

    <!-- Main Content Area -->
    <div class="designer-content">
      <!-- Activities Toolbox (Left Sidebar) -->
      <div class="toolbox">
        <div class="toolbox-section" v-for="category in categories" :key="category.name">
          <h4>{{ category.name }}</h4>
          <div class="activity-item" 
               v-for="activity in category.activities" 
               :key="activity.type"
               draggable="true"
               @dragstart="dragStartActivity($event, activity)">
            <i :class="activity.icon"></i>
            <span>{{ activity.displayName }}</span>
          </div>
        </div>
      </div>

      <!-- Canvas Area -->
      <div class="canvas-container"
           ref="canvasContainer"
           @drop="dragDropActivity"
           @dragover.prevent="dragOverCanvas"
           @dragend="dragEndActivity"
           @mousewheel="handleZoom"
           @contextmenu="showContextMenu">
        
        <!-- Canvas Background -->
        <svg class="canvas" :style="canvasStyle">
          <!-- Grid Background -->
          <defs>
            <pattern id="grid" :width="gridSize" :height="gridSize" patternUnits="userSpaceOnUse">
              <path :d="`M ${gridSize} 0 L 0 0 0 ${gridSize}`" fill="none" stroke="#e0e0e0" stroke-width="0.5"/>
            </pattern>
          </defs>
          <rect width="100%" height="100%" fill="url(#grid)" />
          
          <!-- Connections (Lines) -->
          <g class="connections">
            <line v-for="conn in workflow.connections" 
                  :key="conn.id"
                  :x1="getActivityPosition(conn.sourceActivityId).x + 100"
                  :y1="getActivityPosition(conn.sourceActivityId).y + 50"
                  :x2="getActivityPosition(conn.targetActivityId).x"
                  :y2="getActivityPosition(conn.targetActivityId).y + 50"
                  :class="['connection', { 'connection-invalid': !conn.isValid }]"
                  @click="selectConnection(conn.id)"/>
          </g>
        </svg>

        <!-- Activities (Nodes) -->
        <div class="activities-container" :style="canvasTransform">
          <div v-for="activity in workflow.activities" 
               :key="activity.id"
               class="activity-node"
               :class="[activity.type.toLowerCase(), { 'selected': selectedActivityId === activity.id, 'disabled': activity.isDisabled }]"
               :style="{ left: activity.x + 'px', top: activity.y + 'px', width: activity.width + 'px', height: activity.height + 'px' }"
               @click="selectActivity(activity.id)"
               @mousedown="dragStartNode($event, activity.id)">
            
            <!-- Activity Header -->
            <div class="activity-header">
              <i :class="getActivityIcon(activity.type)"></i>
              <span class="activity-name">{{ activity.displayName || activity.type }}</span>
              <button class="activity-menu" @click.stop="showActivityMenu(activity.id)">⋮</button>
            </div>

            <!-- Activity Body (Properties Preview) -->
            <div class="activity-body">
              <small>{{ activity.type }}</small>
            </div>

            <!-- Outcome Ports -->
            <div class="outcome-ports">
              <div v-for="outcome in getActivityOutcomes(activity.type)" 
                   :key="outcome"
                   class="outcome-port"
                   :title="outcome"
                   @mousedown="dragStartConnection($event, activity.id, outcome)">
                <span class="outcome-label">{{ outcome }}</span>
              </div>
            </div>

            <!-- Input Port -->
            <div class="input-port" 
                 @mousedown="dragStartConnection($event, null, null, activity.id)">
              ▼
            </div>

            <!-- Error Indicator -->
            <div v-if="activity.hasErrors" class="error-indicator">⚠️</div>
          </div>
        </div>

        <!-- Temporary Connection Line (During Drawing) -->
        <svg class="temp-connection-canvas" v-if="isDrawingConnection">
          <line :x1="connectionStartPos.x"
                :y1="connectionStartPos.y"
                :x2="connectionCurrentPos.x"
                :y2="connectionCurrentPos.y"
                class="temp-connection"/>
        </svg>
      </div>

      <!-- Properties Panel (Right Sidebar) -->
      <div class="properties-panel">
        <div v-if="selectedActivityId" class="activity-properties">
          <h3>{{ selectedActivity?.displayName || 'Activity' }}</h3>
          
          <!-- General Properties -->
          <div class="prop-group">
            <label>Display Name:</label>
            <input v-model="selectedActivity.displayName" type="text"/>
          </div>
          
          <div class="prop-group">
            <label>Description:</label>
            <textarea v-model="selectedActivity.description"></textarea>
          </div>

          <!-- Activity-Specific Properties -->
          <component v-if="selectedActivity" 
                     :is="getActivityPropertiesComponent(selectedActivity.type)"
                     :activity="selectedActivity"
                     :variables="workflow.variables"
                     @updateProperty="updateActivityProperty"/>

          <!-- Variable Assignments -->
          <div class="prop-group">
            <label>Variable Assignments:</label>
            <div v-for="(assignment, idx) in selectedActivity.variableAssignments" 
                 :key="idx"
                 class="assignment-row">
              <input v-model="assignment.variableName" placeholder="Variable Name" class="assignment-var"/>
              <span>=</span>
              <input v-model="assignment.expression" placeholder="Expression" class="assignment-expr"/>
              <button @click="removeAssignment(idx)">×</button>
            </div>
            <button @click="addAssignment" class="btn-sm btn-outline-secondary">+ Add Assignment</button>
          </div>

          <!-- Connections Info -->
          <div class="prop-group">
            <label>Connections:</label>
            <div class="connections-info">
              <div v-for="conn in getActivityConnections(selectedActivityId)" :key="conn.id" class="connection-info">
                <span>{{ conn.sourceOutcome || 'input' }} → {{ getActivityName(conn.targetActivityId) }}</span>
                <button @click="deleteConnection(conn.id)" class="btn-sm btn-outline-danger">Delete</button>
              </div>
            </div>
          </div>
        </div>

        <div v-else-if="selectedConnectionId" class="connection-properties">
          <h3>Connection</h3>
          <p>{{ selectedConnection?.sourceOutcome }} → {{ getActivityName(selectedConnection?.targetActivityId) }}</p>
          <button @click="deleteConnection(selectedConnectionId)" class="btn btn-danger">Delete</button>
        </div>

        <div v-else class="workflow-properties">
          <h3>Workflow Properties</h3>
          
          <div class="prop-group">
            <label>Name:</label>
            <input v-model="workflow.name" type="text"/>
          </div>

          <div class="prop-group">
            <label>Description:</label>
            <textarea v-model="workflow.description"></textarea>
          </div>

          <div class="prop-group">
            <label>Version:</label>
            <input v-model="workflow.version" type="number" disabled/>
          </div>

          <!-- Variables Management -->
          <div class="prop-group">
            <label>Variables:</label>
            <div v-for="(variable, idx) in workflow.variables" :key="idx" class="variable-row">
              <input v-model="variable.name" placeholder="Name" class="var-name"/>
              <select v-model="variable.type" class="var-type">
                <option value="string">String</option>
                <option value="number">Number</option>
                <option value="boolean">Boolean</option>
                <option value="object">Object</option>
                <option value="array">Array</option>
              </select>
              <button @click="removeVariable(idx)" class="btn-sm btn-outline-danger">×</button>
            </div>
            <button @click="addVariable" class="btn-sm btn-outline-secondary">+ Add Variable</button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
export default {
  name: 'WorkflowDesigner',
  data() {
    return {
      workflow: {
        id: '',
        name: '',
        description: '',
        version: 1,
        activities: [],
        connections: [],
        variables: [],
        canvasZoom: 1.0,
        canvasPanX: 0,
        canvasPanY: 0
      },
      selectedActivityId: null,
      selectedConnectionId: null,
      isDrawingConnection: false,
      connectionStartPos: { x: 0, y: 0 },
      connectionCurrentPos: { x: 0, y: 0 },
      draggedActivity: null,
      canvasZoom: 1.0,
      gridSize: 20,
      categories: []
    };
  },
  computed: {
    selectedActivity() {
      return this.workflow.activities.find(a => a.id === this.selectedActivityId);
    },
    selectedConnection() {
      return this.workflow.connections.find(c => c.id === this.selectedConnectionId);
    },
    canvasStyle() {
      return {
        transform: `scale(${this.canvasZoom}) translate(${this.canvasPanX}px, ${this.canvasPanY}px)`,
        transformOrigin: '0 0'
      };
    },
    canvasTransform() {
      return {
        transform: `translate(${this.canvasPanX}px, ${this.canvasPanY}px) scale(${this.canvasZoom})`,
        transformOrigin: '0 0'
      };
    }
  },
  methods: {
    selectActivity(id) {
      this.selectedActivityId = id;
      this.selectedConnectionId = null;
    },
    selectConnection(id) {
      this.selectedConnectionId = id;
      this.selectedActivityId = null;
    },
    dragStartActivity(event, activity) {
      this.draggedActivity = activity;
      event.dataTransfer.effectAllowed = 'copy';
    },
    dragOverCanvas(event) {
      event.dataTransfer.dropEffect = 'copy';
    },
    dragDropActivity(event) {
      if (!this.draggedActivity) return;
      
      const rect = this.$refs.canvasContainer.getBoundingClientRect();
      const x = (event.clientX - rect.left - this.canvasPanX) / this.canvasZoom;
      const y = (event.clientY - rect.top - this.canvasPanY) / this.canvasZoom;
      
      const newActivity = {
        id: `activity-${Date.now()}`,
        type: this.draggedActivity.type,
        displayName: this.draggedActivity.displayName,
        x, y,
        width: 200,
        height: 100,
        properties: {},
        inputMapping: {},
        outputMapping: {},
        variableAssignments: []
      };
      
      this.workflow.activities.push(newActivity);
      this.draggedActivity = null;
    },
    dragStartConnection(event, sourceId, outcome, targetId) {
      this.isDrawingConnection = true;
      this.connectionStartPos = { x: event.clientX, y: event.clientY };
      // ... connection drawing logic
    },
    dragStartNode(event, activityId) {
      // Node dragging logic
    },
    saveWorkflow() {
      rpcAEP("SaveWorkflow", { workflow: this.workflow }, (result) => {
        if (result.success) {
          alert('Workflow saved!');
        }
      });
    },
    executeWorkflow() {
      rpcAEP("ExecuteWorkflow", { workflowId: this.workflow.id }, (result) => {
        if (result.success) {
          alert('Workflow execution started: ' + result.instanceId);
        }
      });
    },
    zoomIn() {
      this.canvasZoom = Math.min(this.canvasZoom + 0.1, 3);
    },
    zoomOut() {
      this.canvasZoom = Math.max(this.canvasZoom - 0.1, 0.5);
    },
    fitToScreen() {
      // Calculate bounds and auto-zoom
      this.canvasZoom = 1.0;
    }
    // ... more methods
  },
  mounted() {
    // Load activity registry
    rpcAEP("GetActivityRegistry", {}, (result) => {
      this.categories = result.categories;
    });
  }
};
</script>

<style scoped>
/* Canvas and Layout */
.workflow-designer {
  display: flex;
  flex-direction: column;
  height: 100vh;
  background: #f5f5f5;
}

.designer-toolbar {
  padding: 10px;
  background: #fff;
  border-bottom: 1px solid #ddd;
  display: flex;
  gap: 10px;
}

.designer-content {
  display: flex;
  flex: 1;
  overflow: hidden;
}

/* Toolbox */
.toolbox {
  width: 200px;
  background: #fff;
  border-right: 1px solid #ddd;
  overflow-y: auto;
  padding: 10px;
}

.toolbox-section h4 {
  margin: 10px 0 5px 0;
  font-size: 12px;
  font-weight: bold;
  color: #666;
}

.activity-item {
  padding: 8px;
  margin: 5px 0;
  background: #f9f9f9;
  border: 1px solid #ddd;
  border-radius: 3px;
  cursor: move;
  font-size: 12px;
  display: flex;
  align-items: center;
  gap: 5px;
}

.activity-item:hover {
  background: #e8f4f8;
  border-color: #0066cc;
}

/* Canvas */
.canvas-container {
  flex: 1;
  overflow: hidden;
  background: #fafafa;
  position: relative;
}

.canvas {
  width: 100%;
  height: 100%;
  background: #fff;
}

.activities-container {
  position: absolute;
  width: 100%;
  height: 100%;
}

/* Activity Node */
.activity-node {
  position: absolute;
  background: #fff;
  border: 2px solid #ddd;
  border-radius: 4px;
  cursor: move;
  display: flex;
  flex-direction: column;
  box-shadow: 0 2px 4px rgba(0,0,0,0.1);
  transition: all 0.2s;
}

.activity-node:hover {
  border-color: #0066cc;
  box-shadow: 0 2px 8px rgba(0,102,204,0.2);
}

.activity-node.selected {
  border-color: #0066cc;
  border-width: 2px;
  box-shadow: 0 0 8px rgba(0,102,204,0.4);
}

.activity-node.disabled {
  opacity: 0.5;
  background: #f5f5f5;
}

/* Activity Node Content */
.activity-header {
  padding: 8px;
  background: #f0f8ff;
  border-bottom: 1px solid #ddd;
  font-size: 12px;
  font-weight: bold;
  display: flex;
  align-items: center;
  gap: 5px;
}

.activity-body {
  flex: 1;
  padding: 8px;
  font-size: 11px;
  color: #666;
}

/* Outcome Ports */
.outcome-ports {
  display: flex;
  gap: 5px;
  padding: 5px;
  border-top: 1px solid #eee;
  flex-wrap: wrap;
}

.outcome-port {
  background: #e8f4f8;
  border: 1px solid #0066cc;
  border-radius: 3px;
  padding: 3px 6px;
  font-size: 10px;
  cursor: crosshair;
  position: relative;
}

.outcome-port::after {
  content: '';
  position: absolute;
  width: 8px;
  height: 8px;
  background: #0066cc;
  border-radius: 50%;
  right: -6px;
  top: 50%;
  transform: translateY(-50%);
}

/* Connection Line */
.connection {
  stroke: #999;
  stroke-width: 2;
  fill: none;
  cursor: pointer;
  pointer-events: stroke;
}

.connection:hover {
  stroke: #0066cc;
  stroke-width: 3;
}

.connection-invalid {
  stroke: #ff3333;
  stroke-dasharray: 5,5;
}

/* Properties Panel */
.properties-panel {
  width: 300px;
  background: #fff;
  border-left: 1px solid #ddd;
  overflow-y: auto;
  padding: 15px;
}

.prop-group {
  margin-bottom: 15px;
}

.prop-group label {
  display: block;
  font-size: 12px;
  font-weight: bold;
  margin-bottom: 5px;
  color: #333;
}

.prop-group input,
.prop-group select,
.prop-group textarea {
  width: 100%;
  padding: 6px;
  border: 1px solid #ddd;
  border-radius: 3px;
  font-size: 12px;
}

.prop-group textarea {
  resize: vertical;
  height: 60px;
}
</style>
```

---

## 🗄️ Database Schema (EF Core Models)

```csharp
// DbContext
public class WorkflowDbContext : DbContext
{
    public DbSet<WorkflowDefinitionEntity> WorkflowDefinitions { get; set; }
    public DbSet<WorkflowInstanceEntity> WorkflowInstances { get; set; }
    public DbSet<WorkflowTaskEntity> WorkflowTasks { get; set; }
    public DbSet<ExecutionLogEntity> ExecutionLogs { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<WorkflowDefinitionEntity>()
            .HasKey(w => w.Id);
        
        modelBuilder.Entity<WorkflowInstanceEntity>()
            .HasOne(i => i.Definition)
            .WithMany()
            .HasForeignKey(i => i.DefinitionId);
        
        // ... other configurations
    }
}

// Entities
[Table("WorkflowDefinitions")]
public class WorkflowDefinitionEntity
{
    [Key]
    public string Id { get; set; }
    
    public string Name { get; set; }
    public string Description { get; set; }
    public int Version { get; set; }
    public bool IsPublished { get; set; }
    
    [Column(TypeName = "nvarchar(max)")]
    public string ActivitiesJson { get; set; }     // Serialized List<ActivityModel>
    
    [Column(TypeName = "nvarchar(max)")]
    public string ConnectionsJson { get; set; }    // Serialized List<ConnectionModel>
    
    [Column(TypeName = "nvarchar(max)")]
    public string VariablesJson { get; set; }      // Serialized List<VariableModel>
    
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string CreatedBy { get; set; }
    public string UpdatedBy { get; set; }
}

[Table("WorkflowInstances")]
public class WorkflowInstanceEntity
{
    [Key]
    public string Id { get; set; }
    
    public string DefinitionId { get; set; }
    public int DefinitionVersion { get; set; }
    
    public string Status { get; set; }             // Running, Completed, Failed, Suspended
    public string CurrentActivityId { get; set; }
    
    [Column(TypeName = "nvarchar(max)")]
    public string VariablesJson { get; set; }     // Current variable state
    
    public DateTime StartedAt { get; set; }
    public DateTime? FinishedAt { get; set; }
    public DateTime? LastActivityAt { get; set; }
    
    public string CorrelationId { get; set; }
    
    public WorkflowDefinitionEntity Definition { get; set; }
}

[Table("WorkflowTasks")]
public class WorkflowTaskEntity
{
    [Key]
    public string Id { get; set; }
    
    public string InstanceId { get; set; }
    public string ActivityId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    
    public string Status { get; set; }             // Pending, Completed, Rejected
    public string AssignedTo { get; set; }
    public string AssignedRole { get; set; }
    public string Priority { get; set; }
    
    public DateTime? DueDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    
    [Column(TypeName = "nvarchar(max)")]
    public string ContextDataJson { get; set; }   // Task-specific data
    
    public string Outcome { get; set; }            // User decision/result
}

[Table("ExecutionLogs")]
public class ExecutionLogEntity
{
    [Key]
    public int Id { get; set; }
    
    public string InstanceId { get; set; }
    public string ActivityId { get; set; }
    public string ActivityName { get; set; }
    
    public string Outcome { get; set; }
    public int DurationMs { get; set; }
    
    [Column(TypeName = "nvarchar(max)")]
    public string InputJson { get; set; }         // Activity inputs
    
    [Column(TypeName = "nvarchar(max)")]
    public string OutputJson { get; set; }        // Activity outputs
    
    [Column(TypeName = "nvarchar(max)")]
    public string ErrorMessage { get; set; }      // If failed
    
    public DateTime ExecutedAt { get; set; }
}
```

---

## 🚀 RPC Methods in WorkflowServices

```csharp
public static class WorkflowServices
{
    // Designer Methods
    public static WorkflowDefinitionModel GetWorkflowDefinition(string workflowId)
    public static void SaveWorkflowDefinition(WorkflowDefinitionModel workflow)
    public static void PublishWorkflow(string workflowId, int version)
    public static List<WorkflowDefinitionModel> GetAllWorkflows(int page = 1, int pageSize = 25)
    public static void DeleteWorkflow(string workflowId)
    
    // Execution Methods
    public static ExecutionResult ExecuteWorkflow(string workflowId, object inputParams)
    public static ExecutionResult ResumeWorkflow(string instanceId, string outcome, object resultData)
    
    // Monitoring Methods
    public static InstanceSummary GetWorkflowInstance(string instanceId)
    public static List<InstanceSummary> GetWorkflowInstances(string status, int page = 1)
    public static List<ExecutionLogEntity> GetExecutionLog(string instanceId)
    
    // Task Management
    public static List<TaskSummary> GetUserTasks(string userId, string status = "Pending")
    public static void CompleteTask(string taskId, string outcome, object resultData)
    
    // Validation
    public static ValidationResult ValidateWorkflow(WorkflowDefinitionModel workflow)
    
    // Activity Registry
    public static ActivityRegistryDto GetActivityRegistry()
}
```

---

## 📝 Implementation Phases

### Phase 1: Core Infrastructure
- [ ] Create Activity Base Classes
- [ ] Create Workflow Models (Definition, Activity, Connection, Variable)
- [ ] Create Database Entities & DbContext
- [ ] Setup EF Core Migrations

### Phase 2: Designer UI
- [ ] Create Canvas Component
- [ ] Implement Drag & Drop
- [ ] Activity Rendering
- [ ] Connection Drawing
- [ ] Properties Panel

### Phase 3: Execution Engine
- [ ] Activity Executor
- [ ] Execution Context
- [ ] Variable Management
- [ ] Expression Evaluation (JavaScript)

### Phase 4: Flow Control Activities
- [ ] Decision Activity
- [ ] Switch Activity
- [ ] While/For/ForEach Loops
- [ ] Parallel Fork/Join

### Phase 5: Advanced Features
- [ ] Error Handling
- [ ] Wait/Resume Mechanism
- [ ] Task Assignment
- [ ] Monitoring & Logging

---

## 🔗 Integration Points

1. **RPC Bridge**: WorkflowServices → Vue.js Components
2. **Database**: EF Core DbContext → SQL Server
3. **Expression Engine**: JavaScript evaluation
4. **Activity Registry**: Dynamic activity loading
5. **WebSocket**: Real-time execution updates (future)
