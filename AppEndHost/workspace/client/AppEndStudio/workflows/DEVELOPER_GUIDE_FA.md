# 🛠️ Workflow Designer - Developer's Guide

## 📚 فهرست مطالب
1. [معماری](#معماری)
2. [شروع کار](#شروع-کار)
3. [API Reference](#api-reference)
4. [Extending](#extending)
5. [Troubleshooting](#troubleshooting)

---

## معماری

### Layer Structure

```
┌─────────────────────────────────────┐
│        Presentation Layer           │
│  (Vue Components - WorkflowEditor)  │
└────────────────┬────────────────────┘
                 │
┌────────────────▼────────────────────┐
│      Business Logic Layer           │
│  (workflowBuilder.js - State Mgmt)  │
└────────────────┬────────────────────┘
                 │
┌────────────────▼────────────────────┐
│      Data Definition Layer          │
│    (nodeTypes.js - Metadata)        │
└─────────────────────────────────────┘
```

### Data Flow

```
User Action (UI)
       ↓
WorkflowEditor.vue (Event Handler)
       ↓
workflowBuilder.js (Business Logic)
       ↓
State Update (nodes, connections, history)
       ↓
DesignerCanvas.vue (Reactive Rendering)
       ↓
SVG Output (Visual Feedback)
```

---

## شروع کار

### Prerequisites
- Node.js/npm
- Vue 3
- Bootstrap 5
- FontAwesome Icons
- .NET 10 backend

### Installation

#### 1. Load NodeTypes
```javascript
// در mounted hook
const nodeTypesScript = document.createElement('script');
nodeTypesScript.src = '/AppEndStudio/workflows/lib/nodeTypes.js';
nodeTypesScript.onload = () => {
    // Nodes available as window.NodeTypes
};
document.head.appendChild(nodeTypesScript);
```

#### 2. Load WorkflowBuilder
```javascript
const builderScript = document.createElement('script');
builderScript.src = '/AppEndStudio/workflows/lib/workflowBuilder.js';
builderScript.onload = () => {
    // WorkflowBuilder available as window.WorkflowBuilder
};
document.head.appendChild(builderScript);
```

#### 3. Initialize Component
```javascript
const builder = new WorkflowBuilder(existingWorkflow);
```

---

## API Reference

### WorkflowBuilder Class

#### Constructor
```javascript
new WorkflowBuilder(workflowData = null)
```
- `workflowData`: Optional existing workflow object
- Returns: WorkflowBuilder instance

#### Node Operations

##### addNode(nodeType, position)
```javascript
const node = builder.addNode(window.NodeTypes.TASK, { x: 100, y: 100 });
// Returns: Node object with id, type, label, position, etc.
```

**Properties:**
- `nodeType`: Node type object (from NodeTypes)
- `position`: { x: number, y: number }

**Returns:**
```javascript
{
  id: 'node_1234567890',
  type: 'task',
  category: 'primitives',
  label: 'Task',
  description: '',
  condition: '',
  configuration: {},
  inputs: [],
  outputs: [],
  position: { x: 100, y: 100 }
}
```

##### updateNode(node)
```javascript
node.label = 'New Label';
node.description = 'Updated description';
builder.updateNode(node);
```

##### deleteNode(nodeId)
```javascript
builder.deleteNode('node_1234567890');
// Also removes all connections to/from this node
```

##### moveNode(nodeId, position)
```javascript
builder.moveNode('node_1234567890', { x: 150, y: 200 });
// Snaps to 10px grid
```

#### Connection Operations

##### addConnection(fromNodeId, toNodeId, label)
```javascript
const conn = builder.addConnection('node_001', 'node_002', 'Success');
// Returns: Connection object or null if invalid
```

**Validation Rules:**
- Cannot connect to Start nodes
- Cannot connect from End nodes
- Cannot create duplicate connections
- Cannot self-connect

##### updateConnection(connectionId, updates)
```javascript
builder.updateConnection('conn_001', {
  label: 'On Approved',
  type: 'conditional'
});
```

##### deleteConnection(connectionId)
```javascript
builder.deleteConnection('conn_001');
```

#### History Management

##### undo()
```javascript
builder.undo(); // Reverts to previous state
```

##### redo()
```javascript
builder.redo(); // Applies next state
```

##### canUndo()
```javascript
if (builder.canUndo()) {
  // Show undo button
}
```

##### canRedo()
```javascript
if (builder.canRedo()) {
  // Show redo button
}
```

#### Validation

##### validateWorkflow()
```javascript
const result = builder.validateWorkflow();
// Returns: { valid: boolean, errors: string[] }
```

**Validation Rules:**
- Must have at least one node
- Must have Start node
- Must have End node
- No orphaned nodes (optional check)

#### Data Access

##### getWorkflow()
```javascript
const workflowData = builder.getWorkflow();
// Returns: Deep copy of current workflow
```

##### updateMetadata(metadata)
```javascript
builder.updateMetadata({
  description: 'New description',
  author: 'John Doe'
});
```

---

## Extending

### Adding New Node Type

#### Step 1: Define in nodeTypes.js
```javascript
window.NodeTypes.MY_CUSTOM_TYPE = {
    id: 'my_custom',
    type: 'my_custom',
    label: 'My Custom Node',
    category: 'custom',
    icon: 'fa-solid fa-star',
    color: '#ff00ff',
    description: 'My custom workflow node',
    shape: 'rectangle'  // circle, rectangle, diamond
};
```

#### Step 2: Add to Category
```javascript
window.NodeCategories.custom = {
    name: 'Custom Nodes',
    icon: 'fa-solid fa-plus',
    nodes: ['my_custom']
};
```

#### Step 3: Handle in DesignerCanvas (if special rendering)
```javascript
// In getNodeColor method
if (node.type === 'my_custom') {
    return '#ff00ff';
}

// In getNodeShape method
if (node.type === 'my_custom') {
    return 'rectangle';
}
```

#### Step 4: Add Properties to WorkflowEditor
```vue
<!-- In Properties Panel -->
<div v-if="selectedNode.type === 'my_custom'" class="mb-3">
  <label class="form-label small fw-bold">Custom Property</label>
  <input type="text" class="form-control form-control-sm"
    v-model="selectedNode.customProperty"
    @change="onNodeUpdated(selectedNode)" />
</div>
```

### Custom Connection Validation

```javascript
// Override in WorkflowBuilder
const originalAddConnection = builder.addConnection;
builder.addConnection = function(fromNodeId, toNodeId, label) {
    // Custom validation
    if (shouldPreventConnection(fromNodeId, toNodeId)) {
        return null;
    }
    return originalAddConnection.call(this, fromNodeId, toNodeId, label);
};
```

### Custom Rendering

```javascript
// In DesignerCanvas.vue methods
getNodeColor(node) {
    // Custom logic
    if (node.type === 'my_custom' && node.active) {
        return '#ff0000';
    }
    return super.getNodeColor(node);
}
```

---

## Troubleshooting

### Issue: Nodes not appearing
**Solution:** Check if nodeTypes.js is loaded
```javascript
if (window.NodeTypes === undefined) {
    console.error('NodeTypes not loaded');
}
```

### Issue: Connection not creating
**Possible Causes:**
1. Try to connect from End node
2. Try to connect to Start node
3. Duplicate connection
4. Invalid node IDs

**Debug:**
```javascript
console.log('From Node:', builder.getWorkflow().nodes.find(n => n.id === fromId));
console.log('To Node:', builder.getWorkflow().nodes.find(n => n.id === toId));
```

### Issue: Undo/Redo not working
**Solution:** Check history state
```javascript
console.log('History Index:', builder.historyIndex);
console.log('History Length:', builder.history.length);
```

### Issue: Validation always fails
**Debug:**
```javascript
const result = builder.validateWorkflow();
console.log('Errors:', result.errors);
result.errors.forEach(err => console.log('- ' + err));
```

### Issue: Performance slow with many nodes
**Solutions:**
1. Use `$forceUpdate()` sparingly
2. Debounce node movement
3. Optimize SVG rendering
4. Consider canvas-based rendering for 1000+ nodes

### Issue: CSS not applying
**Solution:** Import CSS in component
```javascript
import './assets/workflow-designer.css';
// OR
// Reference in index.html
<link rel="stylesheet" href="/AppEndStudio/assets/workflow-designer.css">
```

---

## Best Practices

### 1. State Management
```javascript
// ❌ Don't mutate directly
node.label = 'New Label';

// ✅ Do use updateNode
const updated = { ...node, label: 'New Label' };
builder.updateNode(updated);
```

### 2. History Awareness
```javascript
// ❌ Don't make changes without history
workflow.nodes.push(newNode);

// ✅ Do use builder methods
builder.addNode(nodeType, position);
```

### 3. Validation Before Save
```javascript
// ❌ Don't save invalid workflows
builder.getWorkflow();

// ✅ Do validate first
if (!builder.validateWorkflow().valid) {
    showError('Workflow has errors');
    return;
}
const data = builder.getWorkflow();
```

### 4. Error Handling
```javascript
try {
    const nodeType = JSON.parse(nodeTypeStr);
    const node = builder.addNode(nodeType, position);
} catch (e) {
    console.error('Failed to add node:', e);
    showError('Invalid node type');
}
```

### 5. Performance
```javascript
// ❌ Avoid unnecessary renders
methods: {
    onNodeUpdated(node) {
        builder.updateNode(node);
        this.$forceUpdate(); // Every time
    }
}

// ✅ Use watchers and computed properties
computed: {
    canUndo() {
        return builder.canUndo();
    }
}
```

---

## Testing

### Unit Testing Example
```javascript
describe('WorkflowBuilder', () => {
    let builder;

    beforeEach(() => {
        builder = new WorkflowBuilder();
    });

    test('should add node', () => {
        const node = builder.addNode(NodeTypes.TASK, { x: 0, y: 0 });
        expect(node).toBeDefined();
        expect(builder.getWorkflow().nodes).toHaveLength(1);
    });

    test('should validate empty workflow', () => {
        const result = builder.validateWorkflow();
        expect(result.valid).toBe(false);
        expect(result.errors).toHaveLength(3);
    });

    test('should undo/redo', () => {
        const node = builder.addNode(NodeTypes.TASK, { x: 0, y: 0 });
        expect(builder.getWorkflow().nodes).toHaveLength(1);
        
        builder.undo();
        expect(builder.getWorkflow().nodes).toHaveLength(0);
        
        builder.redo();
        expect(builder.getWorkflow().nodes).toHaveLength(1);
    });
});
```

---

## Debugging Tips

### 1. Console Logging
```javascript
console.log('Workflow:', builder.getWorkflow());
console.log('History:', builder.history);
console.log('Nodes:', builder.workflow.nodes);
console.log('Connections:', builder.workflow.connections);
```

### 2. Vue DevTools
- Inspect component hierarchy
- Monitor data changes
- Track events

### 3. Network Tab
- Check API calls
- Monitor payload size
- Check response times

### 4. Performance Profiling
```javascript
performance.mark('workflow-save-start');
// ... save logic
performance.mark('workflow-save-end');
performance.measure('workflow-save', 'workflow-save-start', 'workflow-save-end');
console.log(performance.getEntriesByName('workflow-save'));
```

---

## Resources

- 📚 [Vue 3 Documentation](https://vuejs.org)
- 🎨 [Bootstrap 5 Docs](https://getbootstrap.com)
- 🎯 [SVG Specs](https://www.w3.org/TR/SVG2)
- 📖 Internal: README_FA.md

---

**Last Updated:** Today
**Version:** 1.0.0
**Maintainer:** Development Team
