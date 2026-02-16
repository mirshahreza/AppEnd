<template>
    <div class="card h-100 bg-transparent rounded-0 border-0 d-flex flex-column">
        <!-- Header Toolbar -->
        <div class="card-header p-2 bg-body-subtle rounded-0 border-0 border-bottom">
            <div class="d-flex align-items-center justify-content-between">
                <!-- Left: Title -->
                <div>
                    <h6 class="m-0">
                        <i class="fa-solid fa-diagram-project me-2"></i>Workflow Designer
                    </h6>
                </div>

                <!-- Center: Status Info -->
                <div class="text-center small text-muted">
                    <span><i class="fa-solid fa-cube me-1"></i>{{ workflowStats.nodes }} nodes</span>
                    <span class="mx-2">|</span>
                    <span><i class="fa-solid fa-link me-1"></i>{{ workflowStats.connections }} connections</span>
                </div>

                <!-- Right: Toolbar -->
                <div class="d-flex gap-2 align-items-center">
                    <!-- Undo/Redo -->
                    <div class="btn-group btn-group-sm" role="group">
                        <button class="btn btn-outline-secondary" @click="undo" :disabled="!canUndo" title="Undo (Ctrl+Z)">
                            <i class="fa-solid fa-undo"></i>
                        </button>
                        <button class="btn btn-outline-secondary" @click="redo" :disabled="!canRedo" title="Redo (Ctrl+Y)">
                            <i class="fa-solid fa-redo"></i>
                        </button>
                    </div>

                    <!-- Zoom -->
                    <div class="btn-group btn-group-sm" role="group">
                        <button class="btn btn-outline-secondary" @click="zoomOut" title="Zoom Out">
                            <i class="fa-solid fa-minus"></i>
                        </button>
                        <span class="btn btn-outline-secondary" style="pointer-events: none; cursor: default; border: 1px solid #dee2e6; min-width: 50px;">
                            {{ Math.round(zoom * 100) }}%
                        </span>
                        <button class="btn btn-outline-secondary" @click="zoomIn" title="Zoom In">
                            <i class="fa-solid fa-plus"></i>
                        </button>
                        <button class="btn btn-outline-secondary" @click="zoomReset" title="Fit to View">
                            <i class="fa-solid fa-compress"></i>
                        </button>
                    </div>

                    <!-- Actions -->
                    <button class="btn btn-sm btn-success" @click="saveWorkflow" title="Save">
                        <i class="fa-solid fa-save"></i> Save
                    </button>
                    <button class="btn btn-sm btn-secondary" @click="cancel" title="Close">
                        <i class="fa-solid fa-times"></i> Close
                    </button>
                </div>
            </div>
        </div>

        <!-- Main Content -->
        <div class="flex-grow-1 d-flex" style="min-height: 0; overflow: hidden;">
            <!-- Left Sidebar: Node Palette -->
            <div class="bg-light border-end workflow-palette">
                <!-- Header -->
                <div class="p-2 border-bottom">
                    <h6 class="m-0 mb-2 small fw-bold">
                        <i class="fa-solid fa-shapes me-2"></i>Components
                    </h6>
                    <input type="text" class="form-control form-control-sm" placeholder="Search..." v-model="searchQuery">
                </div>

                <!-- Categories -->
                <div class="workflow-palette-content">
                    <div v-for="category in categories" :key="category.id" class="workflow-category">
                        <!-- Category Header -->
                        <div class="workflow-category-header" 
                            @click="toggleCategory(category.id)"
                            :class="{ 'expanded': expandedCategory === category.id }">
                            <i :class="category.icon + ' me-2'" style="font-size: 0.85rem;"></i>
                            <span class="small fw-bold flex-grow-1">{{ category.name }}</span>
                            <span class="badge bg-secondary" style="font-size: 0.7rem;">{{ category.nodes.length }}</span>
                            <i class="fa-solid fa-chevron-down ms-2" style="font-size: 0.7rem; transition: transform 0.3s;"></i>
                        </div>

                        <!-- Category Content -->
                        <div v-show="expandedCategory === category.id" class="workflow-category-content">
                            <div v-for="nodeId in category.nodes" :key="nodeId"
                                v-if="!searchQuery || matchesSearch(getNodeType(nodeId))"
                                class="workflow-node-item"
                                @dragstart="startDragNode($event, getNodeType(nodeId))"
                                draggable="true"
                                :title="getNodeType(nodeId).description"
                                :style="{ borderLeftColor: getNodeType(nodeId).color }">
                                <i :class="getNodeType(nodeId).icon + ' me-1'" style="font-size: 0.9rem;"></i>
                                <span class="fw-bold" style="font-size: 0.85rem;">{{ getNodeType(nodeId).label }}</span>
                            </div>
                        </div>
                    </div>
                </div>
            </div>

            <!-- Center: Canvas -->
            <div class="flex-grow-1 position-relative" style="overflow: hidden;">
                <designer-canvas
                    ref="canvas"
                    :workflow="workflow"
                    :zoom="zoom"
                    :selected-node="selectedNode"
                    @node-selected="selectNode"
                    @node-dropped="onNodeDropped"
                    @connection-created="onConnectionCreated"
                    @node-updated="onNodeUpdated"
                    @connection-deleted="onConnectionDeleted"
                    @node-deleted="onNodeDeleted"
                />
            </div>

            <!-- Right Sidebar: Properties -->
            <div class="bg-light border-start workflow-properties">
                <div v-if="selectedNode" class="p-3">
                    <h6 class="small fw-bold mb-3">
                        <i class="fa-solid fa-sliders me-2"></i>Properties
                    </h6>

                    <!-- Node Info -->
                    <div class="p-2 bg-white rounded border-start mb-3"
                        :style="{ borderLeftColor: getNodeType(selectedNode.type).color, borderLeftWidth: '3px' }">
                        <small class="text-muted d-block">Type</small>
                        <small class="fw-bold">{{ getNodeType(selectedNode.type).label }}</small>
                    </div>

                    <!-- Label -->
                    <div class="mb-3">
                        <label class="form-label small fw-bold">Label</label>
                        <input type="text" class="form-control form-control-sm" v-model="selectedNode.label" @change="onNodeUpdated(selectedNode)">
                    </div>

                    <!-- Description -->
                    <div class="mb-3">
                        <label class="form-label small fw-bold">Description</label>
                        <textarea class="form-control form-control-sm" rows="3" v-model="selectedNode.description" @change="onNodeUpdated(selectedNode)"></textarea>
                    </div>

                    <!-- Delete Button -->
                    <button class="btn btn-sm btn-danger w-100" @click="deleteNode(selectedNode)">
                        <i class="fa-solid fa-trash me-1"></i>Delete
                    </button>
                </div>
                <div v-else class="p-3 text-center text-muted">
                    <small>
                        <i class="fa-solid fa-mouse me-1"></i>Select a node to edit properties
                    </small>
                </div>
            </div>
        </div>
    </div>
</template>

<script>
    shared.setAppTitle("$auto$");
    shared.setAppSubTitle("Workflow Designer");

    let _this = {
        cid: "",
        c: null,
        workflow: {
            id: 'wf_' + Date.now(),
            name: 'New Workflow',
            nodes: [],
            connections: [],
            metadata: {
                description: '',
                createdAt: new Date().toISOString(),
                updatedAt: new Date().toISOString()
            }
        },
        builder: null,
        selectedNode: null,
        zoom: 1,
        canUndo: false,
        canRedo: false,
        searchQuery: '',
        categories: [],
        nodeTypes: [],
        expandedCategory: 'control'
    };

    export default {
        components: {
            DesignerCanvas: () => import('./components/DesignerCanvas.vue')
        },
        methods: {
            startDragNode(event, nodeType) {
                event.dataTransfer.effectAllowed = "copy";
                event.dataTransfer.setData("nodeType", JSON.stringify(nodeType));
            },

            matchesSearch(nodeType) {
                const query = _this.searchQuery.toLowerCase();
                return nodeType.label.toLowerCase().includes(query) || 
                       nodeType.description.toLowerCase().includes(query);
            },

            toggleCategory(categoryId) {
                _this.expandedCategory = _this.expandedCategory === categoryId ? null : categoryId;
                this.expandedCategory = _this.expandedCategory;
            },

            onNodeDropped(position, nodeType) {
                const node = _this.builder.addNode(nodeType, position);
                this.selectNode(node);
                this.updateWorkflowData();
            },

            onConnectionCreated(fromNodeId, toNodeId) {
                _this.builder.addConnection(fromNodeId, toNodeId);
                this.updateWorkflowData();
            },

            onConnectionDeleted(connectionId) {
                _this.builder.deleteConnection(connectionId);
                this.updateWorkflowData();
            },

            onNodeUpdated(node) {
                _this.builder.updateNode(node);
                this.updateWorkflowData();
            },

            onNodeDeleted(nodeId) {
                _this.builder.deleteNode(nodeId);
                if (_this.selectedNode?.id === nodeId) {
                    _this.selectedNode = null;
                }
                this.updateWorkflowData();
            },

            deleteNode(node) {
                shared.showConfirm({
                    title: "Delete Node",
                    message1: "Are you sure you want to delete this node?",
                    message2: node.label,
                    callback: () => {
                        _this.builder.deleteNode(node.id);
                        _this.selectedNode = null;
                        this.selectedNode = null;
                        this.updateWorkflowData();
                    }
                });
            },

            selectNode(node) {
                _this.selectedNode = node;
                this.selectedNode = node;
            },

            zoomIn() {
                _this.zoom = Math.min(_this.zoom + 0.1, 2);
                this.zoom = _this.zoom;
            },

            zoomOut() {
                _this.zoom = Math.max(_this.zoom - 0.1, 0.5);
                this.zoom = _this.zoom;
            },

            zoomReset() {
                _this.zoom = 1;
                this.zoom = _this.zoom;
            },

            undo() {
                _this.builder.undo();
                this.updateWorkflowData();
                this.updateUndoRedoState();
            },

            redo() {
                _this.builder.redo();
                this.updateWorkflowData();
                this.updateUndoRedoState();
            },

            updateUndoRedoState() {
                _this.canUndo = _this.builder.canUndo();
                _this.canRedo = _this.builder.canRedo();
                this.canUndo = _this.canUndo;
                this.canRedo = _this.canRedo;
            },

            updateWorkflowData() {
                _this.workflow = _this.builder.getWorkflow();
                this.workflow = _this.workflow;
                this.$forceUpdate();
            },

            validateWorkflow() {
                const validation = _this.builder.validateWorkflow();
                if (!validation.valid) {
                    const errorMsg = validation.errors.join('\n');
                    shared.showError({ title: 'Validation Error', message1: errorMsg });
                    return false;
                }
                return true;
            },

            saveWorkflow() {
                if (!this.validateWorkflow()) {
                    return;
                }

                const workflowData = _this.builder.getWorkflow();
                workflowData.name = this.workflow?.name || 'New Workflow';
                workflowData.metadata = {
                    description: this.workflow?.metadata?.description || '',
                    createdAt: workflowData.metadata?.createdAt || new Date().toISOString(),
                    updatedAt: new Date().toISOString()
                };

                const params = {
                    WorkflowId: workflowData.id,
                    WorkflowName: workflowData.name,
                    Definition: JSON.stringify(workflowData)
                };

                rpcAEP('SaveWorkflowDefinition', params, (data) => {
                    let payload = Array.isArray(data) ? (data[0] || {}) : (data || {});
                    payload = payload.Result || payload.result || payload;
                    
                    if (payload.Success || payload.success) {
                        shared.showSuccess({ title: 'Success', message1: 'Workflow saved successfully' });
                        closeComponent(_this.cid, { success: true, workflow: workflowData });
                    } else {
                        shared.showError({ title: 'Error', message1: payload.ErrorMessage || payload.errorMessage || 'Unknown error' });
                    }
                }, (error) => {
                    shared.showError({ title: 'Error', message1: 'Error saving workflow: ' + error });
                });
            },

            cancel() {
                closeComponent(_this.cid, { success: false });
            },

            getNodeType(typeId) {
                return window.getNodeType ? window.getNodeType(typeId) : { label: typeId, icon: 'fa-solid fa-square', color: '#007bff' };
            }
        },
        computed: {
            workflowStats() {
                if (!_this.workflow) return { nodes: 0, connections: 0, isValid: false };
                const validation = _this.builder ? _this.builder.validateWorkflow() : { valid: false };
                return {
                    nodes: _this.workflow.nodes?.length || 0,
                    connections: _this.workflow.connections?.length || 0,
                    isValid: validation.valid
                };
            }
        },
        setup(props) {
            _this.cid = props['cid'];
            
            const params = shared["params_" + _this.cid] || {};
            
            if (params.workflow) {
                try {
                    const definition = typeof params.workflow.Definition === 'string' 
                        ? JSON.parse(params.workflow.Definition) 
                        : params.workflow.Definition;
                    _this.workflow = definition;
                } catch (e) {
                    _this.workflow = { 
                        id: params.workflow.Id, 
                        name: params.workflow.Name, 
                        nodes: [], 
                        connections: [] 
                    };
                }
            }
        },
        data() {
            return {
                workflow: _this.workflow || { id: '', name: 'New Workflow', nodes: [], connections: [], metadata: {} },
                selectedNode: _this.selectedNode || null,
                zoom: _this.zoom || 1,
                canUndo: _this.canUndo || false,
                canRedo: _this.canRedo || false,
                nodeTypes: _this.nodeTypes || [],
                categories: _this.categories || [],
                searchQuery: _this.searchQuery || '',
                expandedCategory: _this.expandedCategory || 'control'
            };
        },
        created() {
            _this.c = this;
        },
        mounted() {
            // Initialize inline WorkflowBuilder
            if (!window.WorkflowBuilder) {
                window.WorkflowBuilder = class WorkflowBuilder {
                    constructor(workflowData = null) {
                        this.workflow = workflowData || { 
                            id: 'wf_' + Date.now(), 
                            name: 'New Workflow', 
                            nodes: [], 
                            connections: [],
                            metadata: {}
                        };
                        this.history = [JSON.parse(JSON.stringify(this.workflow))];
                        this.historyIndex = 0;
                    }
                    addNode(nodeType, position) {
                        const node = {
                            id: 'node_' + Date.now() + Math.random(),
                            type: nodeType.type || nodeType.id,
                            label: nodeType.label,
                            description: '',
                            position: { x: Math.max(position.x || 0, 0), y: Math.max(position.y || 0, 0) }
                        };
                        this.workflow.nodes.push(node);
                        this.saveHistory();
                        return node;
                    }
                    deleteNode(nodeId) {
                        this.workflow.nodes = this.workflow.nodes.filter(n => n.id !== nodeId);
                        this.workflow.connections = this.workflow.connections.filter(c => c.from !== nodeId && c.to !== nodeId);
                        this.saveHistory();
                    }
                    updateNode(node) {
                        const existing = this.workflow.nodes.find(n => n.id === node.id);
                        if (existing) Object.assign(existing, node);
                        this.saveHistory();
                    }
                    addConnection(fromNodeId, toNodeId) {
                        const connection = { 
                            id: 'conn_' + Date.now(), 
                            from: fromNodeId, 
                            to: toNodeId, 
                            label: '' 
                        };
                        this.workflow.connections.push(connection);
                        this.saveHistory();
                        return connection;
                    }
                    deleteConnection(connectionId) {
                        this.workflow.connections = this.workflow.connections.filter(c => c.id !== connectionId);
                        this.saveHistory();
                    }
                    saveHistory() {
                        this.history = this.history.slice(0, this.historyIndex + 1);
                        this.history.push(JSON.parse(JSON.stringify(this.workflow)));
                        this.historyIndex = this.history.length - 1;
                    }
                    undo() {
                        if (this.historyIndex > 0) {
                            this.historyIndex--;
                            this.workflow = JSON.parse(JSON.stringify(this.history[this.historyIndex]));
                        }
                    }
                    redo() {
                        if (this.historyIndex < this.history.length - 1) {
                            this.historyIndex++;
                            this.workflow = JSON.parse(JSON.stringify(this.history[this.historyIndex]));
                        }
                    }
                    canUndo() { return this.historyIndex > 0; }
                    canRedo() { return this.historyIndex < this.history.length - 1; }
                    validateWorkflow() {
                        const errors = [];
                        if (!this.workflow.nodes.some(n => n.type === 'start')) errors.push('Must have Start node');
                        if (!this.workflow.nodes.some(n => n.type === 'end')) errors.push('Must have End node');
                        return { valid: errors.length === 0, errors };
                    }
                    getWorkflow() { 
                        return JSON.parse(JSON.stringify(this.workflow)); 
                    }
                };
            }

            // Initialize inline NodeTypes
            if (!window.NodeTypes) {
                window.NodeTypes = {
                    START: { id: 'start', type: 'start', label: 'Start', category: 'control', icon: 'fa-solid fa-play-circle', color: '#28a745', description: 'Start workflow', shape: 'circle' },
                    END: { id: 'end', type: 'end', label: 'End', category: 'control', icon: 'fa-solid fa-stop-circle', color: '#dc3545', description: 'End workflow', shape: 'circle' },
                    TRY_CATCH: { id: 'try_catch', type: 'try_catch', label: 'Try/Catch', category: 'control', icon: 'fa-solid fa-shield-alt', color: '#e74c3c', description: 'Error handling', shape: 'rectangle' },
                    TASK: { id: 'task', type: 'task', label: 'Task', category: 'primitives', icon: 'fa-solid fa-square', color: '#007bff', description: 'Execute task', shape: 'rectangle' },
                    DECISION: { id: 'decision', type: 'decision', label: 'Decision', category: 'branching', icon: 'fa-solid fa-diamond', color: '#ffc107', description: 'Make decision', shape: 'diamond' },
                    IF_ELSE: { id: 'if_else', type: 'if_else', label: 'If/Else', category: 'branching', icon: 'fa-solid fa-code-branch', color: '#17a2b8', description: 'Conditional branch', shape: 'rectangle' },
                    SWITCH: { id: 'switch', type: 'switch', label: 'Switch', category: 'branching', icon: 'fa-solid fa-exchange-alt', color: '#17a2b8', description: 'Switch case', shape: 'rectangle' },
                    FOR_LOOP: { id: 'for_loop', type: 'for_loop', label: 'For', category: 'looping', icon: 'fa-solid fa-repeat', color: '#6f42c1', description: 'For loop', shape: 'rectangle' },
                    FOREACH_LOOP: { id: 'foreach_loop', type: 'foreach_loop', label: 'For Each', category: 'looping', icon: 'fa-solid fa-list-ol', color: '#6f42c1', description: 'For each', shape: 'rectangle' },
                    WHILE_LOOP: { id: 'while_loop', type: 'while_loop', label: 'While', category: 'looping', icon: 'fa-solid fa-sync-alt', color: '#6f42c1', description: 'While loop', shape: 'rectangle' },
                    HTTP_REQUEST: { id: 'http_request', type: 'http_request', label: 'HTTP Request', category: 'http', icon: 'fa-solid fa-globe', color: '#e83e8c', description: 'HTTP call', shape: 'rectangle' },
                    EMAIL: { id: 'email', type: 'email', label: 'Send Email', category: 'email', icon: 'fa-solid fa-envelope', color: '#fd7e14', description: 'Send email', shape: 'rectangle' },
                    DATABASE_QUERY: { id: 'database_query', type: 'database_query', label: 'Database', category: 'storage', icon: 'fa-solid fa-database', color: '#0dcaf0', description: 'DB query', shape: 'rectangle' },
                    SCRIPT: { id: 'script', type: 'script', label: 'Script', category: 'scripting', icon: 'fa-solid fa-code', color: '#9b59b6', description: 'Execute script', shape: 'rectangle' },
                    DELAY: { id: 'delay', type: 'delay', label: 'Delay', category: 'scheduling', icon: 'fa-solid fa-hourglass-end', color: '#6c757d', description: 'Add delay', shape: 'rectangle' }
                };

                window.getNodeType = (typeId) => {
                    for (const key in window.NodeTypes) {
                        if (window.NodeTypes[key].id === typeId || window.NodeTypes[key].type === typeId) {
                            return window.NodeTypes[key];
                        }
                    }
                    return window.NodeTypes.TASK;
                };

                window.getAllCategories = () => {
                    return [
                        { id: 'control', name: 'Control Flow', icon: 'fa-solid fa-arrows-alt', nodes: ['start', 'end', 'try_catch'] },
                        { id: 'primitives', name: 'Primitives', icon: 'fa-solid fa-cube', nodes: ['task'] },
                        { id: 'branching', name: 'Branching', icon: 'fa-solid fa-code-branch', nodes: ['decision', 'if_else', 'switch'] },
                        { id: 'looping', name: 'Looping', icon: 'fa-solid fa-repeat', nodes: ['for_loop', 'foreach_loop', 'while_loop'] },
                        { id: 'http', name: 'HTTP', icon: 'fa-solid fa-globe', nodes: ['http_request'] },
                        { id: 'email', name: 'Email', icon: 'fa-solid fa-envelope', nodes: ['email'] },
                        { id: 'storage', name: 'Storage', icon: 'fa-solid fa-database', nodes: ['database_query'] },
                        { id: 'scripting', name: 'Scripting', icon: 'fa-solid fa-code', nodes: ['script'] },
                        { id: 'scheduling', name: 'Scheduling', icon: 'fa-solid fa-clock', nodes: ['delay'] }
                    ];
                };
            }

            // Initialize builder with loaded workflow
            _this.builder = new window.WorkflowBuilder(_this.workflow);
            _this.nodeTypes = Object.values(window.NodeTypes);
            _this.categories = window.getAllCategories();

            // Update component
            this.workflow = _this.builder.getWorkflow();
            this.nodeTypes = _this.nodeTypes;
            this.categories = _this.categories;

            // Keyboard shortcuts
            window.addEventListener('keydown', (e) => {
                if (e.ctrlKey && e.key === 'z') {
                    e.preventDefault();
                    this.undo();
                } else if ((e.ctrlKey && e.key === 'y') || (e.ctrlKey && e.shiftKey && e.key === 'z')) {
                    e.preventDefault();
                    this.redo();
                }
            });
        },
        props: { cid: String }
    }
</script>
