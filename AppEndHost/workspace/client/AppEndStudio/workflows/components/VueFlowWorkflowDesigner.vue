<template>
    <div class="workflow-designer-container">
        <div class="designer-header p-2 bg-body-subtle border-bottom">
            <div class="hstack gap-2">
                <div class="workflow-info">
                    <h6 class="mb-0 fw-bold">{{ workflowName }}</h6>
                    <small class="text-muted">{{ workflowId }}</small>
                </div>

                <div class="vr"></div>

                <button class="btn btn-sm btn-outline-primary" @click="addActivity" title="Add Activity">
                    <i class="fa-solid fa-plus"></i> Add Activity
                </button>
                
                <button class="btn btn-sm btn-outline-secondary" @click="autoLayout" title="Auto Layout">
                    <i class="fa-solid fa-diagram-project"></i> Auto Layout
                </button>

                <button class="btn btn-sm btn-outline-info" @click="zoomToFit" title="Zoom to Fit">
                    <i class="fa-solid fa-expand"></i> Fit
                </button>

                <div class="vr"></div>

                <div class="btn-group btn-group-sm" role="group">
                    <button class="btn btn-outline-secondary" @click="handleZoomOut" title="Zoom Out">
                        <i class="fa-solid fa-minus"></i>
                    </button>
                    <button class="btn btn-outline-secondary" @click="resetZoom" title="Reset Zoom">
                        {{ currentZoom }}%
                    </button>
                    <button class="btn btn-outline-secondary" @click="handleZoomIn" title="Zoom In">
                        <i class="fa-solid fa-plus"></i>
                    </button>
                </div>

                <div class="p-0 ms-auto"></div>

                <button class="btn btn-sm btn-outline-secondary" @click="close(false)" title="Close">
                    <i class="fa-solid fa-times"></i> Close
                </button>
                <button class="btn btn-sm btn-success" @click="saveWorkflow" title="Save">
                    <i class="fa-solid fa-save"></i> Save
                </button>
            </div>
        </div>

        <div class="designer-body position-relative">
            <VueFlow 
                v-model:nodes="nodes" 
                v-model:edges="edges"
                :default-viewport="{ zoom: 1, x: 0, y: 0 }"
                :min-zoom="0.2"
                :max-zoom="4"
                fit-view-on-init
                @node-click="onNodeClick"
                @edge-click="onEdgeClick"
                @pane-click="onPaneClick"
                @connect="onConnect"
                class="vue-flow-designer"
            >
                <Background pattern-color="#aaa" :gap="16" />
                <Controls />
                <MiniMap />

                <template #node-activity="nodeProps">
                    <div class="custom-node activity-node" :class="{ 'selected': nodeProps.data.selected }">
                        <div class="node-header" :style="{ backgroundColor: nodeProps.data.color || '#4CAF50' }">
                            <i :class="nodeProps.data.icon || 'fa-solid fa-cog'"></i>
                            <span class="node-title">{{ nodeProps.data.label }}</span>
                        </div>
                        <div class="node-body">
                            <div class="node-type">{{ nodeProps.data.type }}</div>
                            <div v-if="nodeProps.data.description" class="node-description">{{ nodeProps.data.description }}</div>
                        </div>
                        <Handle type="target" :position="Position.Top" class="node-handle" />
                        <Handle type="source" :position="Position.Bottom" class="node-handle" />
                    </div>
                </template>

                <template #node-start="nodeProps">
                    <div class="custom-node start-node">
                        <div class="node-content">
                            <i class="fa-solid fa-play"></i>
                            <span>{{ nodeProps.data.label }}</span>
                        </div>
                        <Handle type="source" :position="Position.Bottom" class="node-handle" />
                    </div>
                </template>

                <template #node-end="nodeProps">
                    <div class="custom-node end-node">
                        <div class="node-content">
                            <i class="fa-solid fa-flag-checkered"></i>
                            <span>{{ nodeProps.data.label }}</span>
                        </div>
                        <Handle type="target" :position="Position.Top" class="node-handle" />
                    </div>
                </template>
            </VueFlow>

            <div class="activity-palette bg-white border-end shadow-sm">
                <div class="palette-header p-2 bg-light border-bottom">
                    <h6 class="mb-0 fw-bold">Activities</h6>
                </div>
                <div class="palette-body p-2">
                    <div class="activity-category mb-3" v-for="category in activityCategories" :key="category.name">
                        <div class="category-title fw-bold mb-2">{{ category.name }}</div>
                        <div 
                            v-for="activity in category.activities" 
                            :key="activity.type"
                            class="activity-item"
                            @click="addActivityNode(activity)"
                        >
                            <i :class="activity.icon" :style="{ color: activity.color }"></i>
                            <span>{{ activity.label }}</span>
                        </div>
                    </div>
                </div>
            </div>

            <div v-if="selectedNode" class="properties-panel bg-white border-start shadow-sm">
                <div class="panel-header p-2 bg-light border-bottom d-flex align-items-center">
                    <h6 class="mb-0 fw-bold flex-grow-1">Properties</h6>
                    <button class="btn btn-sm btn-outline-danger" @click="deleteSelectedNode" title="Delete">
                        <i class="fa-solid fa-trash"></i>
                    </button>
                </div>
                <div class="panel-body p-3">
                    <div class="mb-3">
                        <label class="form-label fw-bold">Activity Name</label>
                        <input type="text" class="form-control form-control-sm" v-model="selectedNode.data.label" />
                    </div>
                    <div class="mb-3">
                        <label class="form-label fw-bold">Activity Type</label>
                        <input type="text" class="form-control form-control-sm" v-model="selectedNode.data.type" readonly />
                    </div>
                    <div class="mb-3">
                        <label class="form-label fw-bold">Description</label>
                        <textarea class="form-control form-control-sm" rows="3" v-model="selectedNode.data.description"></textarea>
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>

<script>
import { ref, computed, onMounted } from 'vue';
import { VueFlow, useVueFlow, Position, Handle } from '@vue-flow/core';
import { Background } from '@vue-flow/background';
import { Controls } from '@vue-flow/controls';
import { MiniMap } from '@vue-flow/minimap';

let _this = { cid: "", c: null };

export default {
    components: {
        VueFlow,
        Background,
        Controls,
        MiniMap,
        Handle
    },
    setup(props) {
        _this.cid = props['cid'];
        
        console.log('🚀 Vue Flow Designer initializing...');
        
        const nodes = ref([]);
        const edges = ref([]);
        const selectedNode = ref(null);
        const workflowId = ref('');
        const workflowName = ref('Workflow Designer');
        
        let zoomIn, zoomOut, setViewport, fitView, getViewport;
        
        try {
            const vueFlowInstance = useVueFlow();
            zoomIn = vueFlowInstance.zoomIn;
            zoomOut = vueFlowInstance.zoomOut;
            setViewport = vueFlowInstance.setViewport;
            fitView = vueFlowInstance.fitView;
            getViewport = vueFlowInstance.getViewport;
            console.log('✅ Vue Flow instance created successfully');
        } catch (error) {
            console.error('❌ Error creating Vue Flow instance:', error);
        }
        
        const currentZoom = computed(() => {
            try {
                if (getViewport) {
                    const viewport = getViewport();
                    return Math.round((viewport?.zoom || 1) * 100);
                }
                return 100;
            } catch {
                return 100;
            }
        });

        const activityCategories = ref([
            {
                name: 'Control Flow',
                activities: [
                    { type: 'If', label: 'If/Else', icon: 'fa-solid fa-code-branch', color: '#2196F3' },
                    { type: 'While', label: 'While Loop', icon: 'fa-solid fa-rotate', color: '#2196F3' },
                    { type: 'ForEach', label: 'For Each', icon: 'fa-solid fa-list', color: '#2196F3' },
                    { type: 'Switch', label: 'Switch', icon: 'fa-solid fa-sitemap', color: '#2196F3' }
                ]
            },
            {
                name: 'HTTP',
                activities: [
                    { type: 'HttpEndpoint', label: 'HTTP Endpoint', icon: 'fa-solid fa-globe', color: '#FF9800' },
                    { type: 'SendHttpRequest', label: 'HTTP Request', icon: 'fa-solid fa-paper-plane', color: '#FF9800' },
                    { type: 'WriteHttpResponse', label: 'HTTP Response', icon: 'fa-solid fa-reply', color: '#FF9800' }
                ]
            },
            {
                name: 'Primitives',
                activities: [
                    { type: 'SetVariable', label: 'Set Variable', icon: 'fa-solid fa-database', color: '#4CAF50' },
                    { type: 'WriteLine', label: 'Write Line', icon: 'fa-solid fa-file-lines', color: '#4CAF50' },
                    { type: 'Delay', label: 'Delay', icon: 'fa-solid fa-clock', color: '#4CAF50' }
                ]
            }
        ]);

        const loadWorkflow = () => {
            const params = shared["params_" + _this.cid] || {};
            workflowId.value = params.workflowId || '';

            if (workflowId.value) {
                rpcAEP('GetWorkflowDefinition', { WorkflowId: workflowId.value }, (data) => {
                    const payload = Array.isArray(data) ? (data[0] || {}) : (data || {});
                    const workflow = payload.Result || payload.result || payload;
                    
                    if (workflow) {
                        workflowName.value = workflow.Name || 'Workflow';
                        convertElsaToVueFlow(workflow);
                    }
                }, (error) => {
                    console.error('Error loading workflow:', error);
                    showError('Failed to load workflow');
                    initializeEmptyWorkflow();
                });
            } else {
                initializeEmptyWorkflow();
            }
        };

        const initializeEmptyWorkflow = () => {
            nodes.value = [
                {
                    id: 'start-1',
                    type: 'start',
                    position: { x: 250, y: 50 },
                    data: { label: 'Start' }
                },
                {
                    id: 'end-1',
                    type: 'end',
                    position: { x: 250, y: 400 },
                    data: { label: 'End' }
                }
            ];
            edges.value = [];
        };

        const convertElsaToVueFlow = (workflow) => {
            try {
                const activities = workflow.Activities || [];
                const connections = workflow.Connections || [];

                const vueNodes = [
                    {
                        id: 'start-1',
                        type: 'start',
                        position: { x: 250, y: 50 },
                        data: { label: 'Start' }
                    }
                ];

                let yPosition = 150;
                activities.forEach((activity, index) => {
                    const activityInfo = findActivityInfo(activity.Type);
                    vueNodes.push({
                        id: activity.Id || `activity-${index}`,
                        type: 'activity',
                        position: { x: 250, y: yPosition },
                        data: {
                            label: activity.Name || activity.Type,
                            type: activity.Type,
                            description: activity.Description || '',
                            icon: activityInfo?.icon || 'fa-solid fa-cog',
                            color: activityInfo?.color || '#4CAF50',
                            properties: activity.Properties || {},
                            selected: false
                        }
                    });
                    yPosition += 120;
                });

                vueNodes.push({
                    id: 'end-1',
                    type: 'end',
                    position: { x: 250, y: yPosition + 50 },
                    data: { label: 'End' }
                });

                nodes.value = vueNodes;

                const vueEdges = [];
                connections.forEach((conn, index) => {
                    vueEdges.push({
                        id: `edge-${index}`,
                        source: conn.SourceActivityId || 'start-1',
                        target: conn.TargetActivityId || 'end-1',
                        label: conn.OutcomeName || '',
                        animated: true
                    });
                });

                edges.value = vueEdges;
            } catch (error) {
                console.error('Error converting Elsa workflow:', error);
                initializeEmptyWorkflow();
            }
        };

        const findActivityInfo = (activityType) => {
            for (const category of activityCategories.value) {
                const activity = category.activities.find(a => a.type === activityType);
                if (activity) return activity;
            }
            return null;
        };

        const addActivityNode = (activity) => {
            const newNode = {
                id: `node-${Date.now()}`,
                type: 'activity',
                position: { x: Math.random() * 400 + 100, y: Math.random() * 300 + 100 },
                data: {
                    label: activity.label,
                    type: activity.type,
                    description: '',
                    icon: activity.icon,
                    color: activity.color,
                    properties: {},
                    selected: false
                }
            };
            nodes.value.push(newNode);
        };

        const onNodeClick = (event) => {
            nodes.value.forEach(n => {
                if (n.data) n.data.selected = false;
            });
            
            const node = nodes.value.find(n => n.id === event.node.id);
            if (node && node.data) {
                node.data.selected = true;
                selectedNode.value = node;
            }
        };

        const onPaneClick = () => {
            nodes.value.forEach(n => {
                if (n.data) n.data.selected = false;
            });
            selectedNode.value = null;
        };

        const onEdgeClick = (event) => {
            console.log('Edge clicked:', event.edge);
        };

        const onConnect = (params) => {
            edges.value.push({
                id: `edge-${Date.now()}`,
                source: params.source,
                target: params.target,
                animated: true
            });
        };

        const deleteSelectedNode = () => {
            if (selectedNode.value) {
                nodes.value = nodes.value.filter(n => n.id !== selectedNode.value.id);
                edges.value = edges.value.filter(e => 
                    e.source !== selectedNode.value.id && e.target !== selectedNode.value.id
                );
                selectedNode.value = null;
            }
        };

        const handleZoomIn = () => {
            zoomIn();
        };

        const handleZoomOut = () => {
            zoomOut();
        };

        const resetZoom = () => {
            setViewport({ x: 0, y: 0, zoom: 1 });
        };

        const zoomToFit = () => {
            fitView();
        };

        const addActivity = () => {
            const defaultActivity = activityCategories.value[0].activities[0];
            addActivityNode(defaultActivity);
        };

        const autoLayout = () => {
            let yPos = 50;
            nodes.value.forEach((node) => {
                node.position.x = 250;
                node.position.y = yPos;
                yPos += 120;
            });
        };

        const saveWorkflow = () => {
            const workflowData = convertVueFlowToElsa();
            
            rpcAEP('UpdateWorkflowDefinition', {
                WorkflowId: workflowId.value,
                Workflow: workflowData
            }, (data) => {
                const payload = Array.isArray(data) ? (data[0] || {}) : (data || {});
                const success = payload.Success || payload.success;
                
                if (success) {
                    showSuccess('Workflow saved successfully');
                    close(true);
                } else {
                    showError('Failed to save workflow');
                }
            }, (error) => {
                console.error('Error saving workflow:', error);
                showError('Failed to save workflow');
            });
        };

        const convertVueFlowToElsa = () => {
            const activities = nodes.value
                .filter(n => n.type === 'activity')
                .map(n => ({
                    Id: n.id,
                    Type: n.data.type,
                    Name: n.data.label,
                    Description: n.data.description || '',
                    Properties: n.data.properties || {}
                }));

            const connections = edges.value.map(e => ({
                SourceActivityId: e.source,
                TargetActivityId: e.target,
                OutcomeName: e.label || 'Done'
            }));

            return {
                Id: workflowId.value,
                Activities: activities,
                Connections: connections
            };
        };

        const close = (success) => {
            closeComponent(_this.cid, { success: success });
        };

        onMounted(() => {
            loadWorkflow();
        });

        return {
            Position,
            nodes,
            edges,
            selectedNode,
            workflowId,
            workflowName,
            currentZoom,
            activityCategories,
            addActivityNode,
            onNodeClick,
            onEdgeClick,
            onPaneClick,
            onConnect,
            deleteSelectedNode,
            handleZoomIn,
            handleZoomOut,
            resetZoom,
            zoomToFit,
            addActivity,
            autoLayout,
            saveWorkflow,
            close
        };
    },
    created() { 
        _this.c = this; 
    },
    props: { 
        cid: String 
    }
}
</script>

<style>
.workflow-designer-container {
    display: flex;
    flex-direction: column;
    height: 100vh;
    background: #f5f5f5;
}

.designer-header {
    height: 60px;
    flex-shrink: 0;
}

.workflow-info h6 {
    font-size: 14px;
}

.workflow-info small {
    font-size: 11px;
}

.designer-body {
    flex: 1;
    position: relative;
    overflow: hidden;
}

.vue-flow-designer {
    width: 100%;
    height: 100%;
}

/* Vue Flow Base Styles */
:deep(.vue-flow) {
    background: #fff;
}

:deep(.vue-flow__edge-path) {
    stroke: #2196F3;
    stroke-width: 2;
}

:deep(.vue-flow__edge.animated path) {
    stroke-dasharray: 5;
    animation: dashdraw 0.5s linear infinite;
}

:deep(.vue-flow__edge.selected .vue-flow__edge-path) {
    stroke: #FF9800;
}

@keyframes dashdraw {
    from {
        stroke-dashoffset: 10;
    }
}

:deep(.vue-flow__handle) {
    width: 12px;
    height: 12px;
    background: #2196F3;
    border: 2px solid white;
}

:deep(.vue-flow__controls) {
    display: flex;
    flex-direction: column;
    gap: 4px;
}

:deep(.vue-flow__controls-button) {
    width: 32px;
    height: 32px;
    background: white;
    border: 1px solid #ddd;
    border-radius: 4px;
    cursor: pointer;
    display: flex;
    align-items: center;
    justify-content: center;
}

:deep(.vue-flow__controls-button:hover) {
    background: #f5f5f5;
}

:deep(.vue-flow__minimap) {
    background: white;
    border: 1px solid #ddd;
}

:deep(.vue-flow__minimap-node) {
    fill: #e0e0e0;
    stroke: #999;
}

.activity-palette {
    position: absolute;
    left: 0;
    top: 0;
    bottom: 0;
    width: 250px;
    z-index: 10;
    overflow-y: auto;
}

.palette-header {
    height: 40px;
}

.palette-body {
    max-height: calc(100% - 40px);
    overflow-y: auto;
}

.activity-category {
    margin-bottom: 1rem;
}

.category-title {
    font-size: 12px;
    text-transform: uppercase;
    color: #666;
}

.activity-item {
    padding: 8px 10px;
    margin: 4px 0;
    background: #f8f9fa;
    border: 1px solid #dee2e6;
    border-radius: 4px;
    cursor: pointer;
    display: flex;
    align-items: center;
    gap: 8px;
    font-size: 13px;
    transition: all 0.2s;
}

.activity-item:hover {
    background: #e9ecef;
    border-color: #adb5bd;
    transform: translateX(4px);
}

.activity-item i {
    width: 20px;
    text-align: center;
}

.properties-panel {
    position: absolute;
    right: 0;
    top: 0;
    bottom: 0;
    width: 300px;
    z-index: 10;
    overflow-y: auto;
}

.panel-header {
    height: 40px;
}

.panel-body {
    max-height: calc(100% - 40px);
    overflow-y: auto;
}

.custom-node {
    background: white;
    border: 2px solid #ddd;
    border-radius: 8px;
    min-width: 200px;
    box-shadow: 0 2px 8px rgba(0,0,0,0.1);
    transition: all 0.2s;
}

.custom-node.selected {
    border-color: #2196F3;
    box-shadow: 0 4px 16px rgba(33, 150, 243, 0.3);
}

.custom-node:hover {
    box-shadow: 0 4px 12px rgba(0,0,0,0.15);
}

.activity-node .node-header {
    padding: 8px 12px;
    color: white;
    border-radius: 6px 6px 0 0;
    display: flex;
    align-items: center;
    gap: 8px;
    font-weight: bold;
    font-size: 13px;
}

.activity-node .node-body {
    padding: 10px 12px;
}

.activity-node .node-type {
    font-size: 11px;
    color: #666;
    text-transform: uppercase;
    margin-bottom: 4px;
}

.activity-node .node-description {
    font-size: 12px;
    color: #999;
    margin-top: 4px;
}

.start-node, .end-node {
    padding: 12px 20px;
    border-radius: 25px;
    font-weight: bold;
}

.start-node {
    background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
    color: white;
    border-color: #667eea;
}

.end-node {
    background: linear-gradient(135deg, #f093fb 0%, #f5576c 100%);
    color: white;
    border-color: #f093fb;
}

.start-node .node-content, 
.end-node .node-content {
    display: flex;
    align-items: center;
    gap: 8px;
}

.node-handle {
    width: 12px;
    height: 12px;
    background: #2196F3;
    border: 2px solid white;
}
</style>
