<template>
    <div class="designer-canvas" @drop="handleDrop" @dragover.prevent @dragenter.prevent 
        @mousedown="onCanvasMouseDown" @mousemove="onCanvasMouseMove" @mouseup="onCanvasMouseUp">
        <svg ref="svgCanvas" class="w-100 h-100" style="background-color: #f8f9fa; background-image: url('data:image/svg+xml;utf8,<svg xmlns=%22http://www.w3.org/2000/svg%22 width=%22100%22 height=%22100%22><defs><pattern id=%22grid%22 width=%22100%22 height=%22100%22 patternUnits=%22userSpaceOnUse%22><path d=%22M 100 0 L 0 0 0 100%22 fill=%22none%22 stroke=%22%23eee%22 stroke-width=%220.5%22/></pattern></defs><rect width=%22100%22 height=%22100%22 fill=%22url(%23grid)%22/></svg>'); background-size: 50px 50px;">
            
            <!-- Connections (edges) -->
            <g class="connections-layer">
                <template v-if="workflow && workflow.connections">
                    <line v-for="conn in workflow.connections" :key="'conn-' + conn.id"
                        :x1="getNodeCenterX(getNode(conn.from))"
                        :y1="getNodeCenterY(getNode(conn.from))"
                        :x2="getNodeCenterX(getNode(conn.to))"
                        :y2="getNodeCenterY(getNode(conn.to))"
                        class="connection-line"
                        @click.stop="selectConnection(conn)"
                        :class="{ 'connection-selected': selectedConnectionId === conn.id }" />
                </template>

                <!-- Delete button for selected connection -->
                <g v-if="selectedConnectionId && selectedConnection" class="connection-delete-btn">
                    <circle :cx="getMidpointX(getNode(selectedConnection.from), getNode(selectedConnection.to))"
                        :cy="getMidpointY(getNode(selectedConnection.from), getNode(selectedConnection.to))"
                        r="12" fill="white" stroke="#dc3545" stroke-width="2" />
                    <text :x="getMidpointX(getNode(selectedConnection.from), getNode(selectedConnection.to))"
                        :y="getMidpointY(getNode(selectedConnection.from), getNode(selectedConnection.to))"
                        text-anchor="middle" dy="0.3em" font-size="16" fill="#dc3545" 
                        @click.stop="deleteConnection(selectedConnection)"
                        style="cursor: pointer;">×</text>
                </g>

                <!-- Temporary connection line while dragging -->
                <line v-if="draggingConnection" class="connection-temp"
                    :x1="draggingConnection.x1"
                    :y1="draggingConnection.y1"
                    :x2="draggingConnection.x2"
                    :y2="draggingConnection.y2" />
            </g>

            <!-- Nodes -->
            <g class="nodes-layer">
                <template v-if="workflow && workflow.nodes">
                    <g v-for="node in workflow.nodes" :key="'node-' + node.id"
                        :class="['workflow-node', node.type, { 'node-selected': selectedNode?.id === node.id }]"
                        @mousedown.stop="selectNode(node, $event)"
                        @click.stop="selectNode(node)">
                        
                        <!-- Dynamic Node Rendering based on type -->
                        <g v-if="getNodeShape(node.type) === 'circle'">
                            <circle :cx="node.position.x + 40" :cy="node.position.y + 30" r="30"
                                class="node-shape"
                                :fill="getNodeColor(node)" :stroke="getNodeStroke(node)" stroke-width="2" />
                        </g>
                        
                        <g v-else-if="getNodeShape(node.type) === 'diamond'">
                            <polygon :points="getPolygonPoints(node)"
                                class="node-shape"
                                :fill="getNodeColor(node)" :stroke="getNodeStroke(node)" stroke-width="2" />
                        </g>
                        
                        <g v-else>
                            <!-- Default rectangle for all other nodes -->
                            <rect :x="node.position.x" :y="node.position.y" width="80" height="60" rx="4"
                                class="node-shape"
                                :fill="getNodeColor(node)" :stroke="getNodeStroke(node)" stroke-width="2" />
                        </g>
                        
                        <!-- Node label text -->
                        <text :x="node.position.x + 40" :y="node.position.y + 30"
                            text-anchor="middle" dy="0.3em"
                            :fill="getNodeTextColor(node)"
                            font-weight="bold" font-size="11"
                            pointer-events="none"
                            class="node-label">
                            {{ node.label }}
                        </text>

                        <!-- Node icon -->
                        <i v-if="getNodeIcon(node.type)" 
                            :class="getNodeIcon(node.type) + ' node-icon'"
                            :style="{ left: (node.position.x + 40 - 6) + 'px', top: (node.position.y + 45) + 'px' }"></i>
                        
                        <!-- Output port (for connecting to next node) -->
                        <circle v-if="!isEndNode(node.type)" class="port port-output" 
                            :cx="node.position.x + 80"
                            :cy="node.position.y + 30"
                            r="5" @mousedown.stop="startConnection(node, $event)" 
                            title="Connect to next node" />
                        
                        <!-- Input port (for connecting from previous node) -->
                        <circle v-if="!isStartNode(node.type)" class="port port-input"
                            :cx="node.position.x"
                            :cy="node.position.y + 30"
                            r="5" @mousedown.stop="startConnection(node, $event)"
                            title="Connect from previous node" />
                    </g>
                </template>
            </g>
        </svg>
    </div>
</template>

<script>
    export default {
        props: {
            workflow: Object,
            zoom: Number,
            selectedNode: Object
        },
        emits: ['node-selected', 'node-dropped', 'connection-created', 'node-updated', 'connection-deleted', 'node-deleted'],
        data() {
            return {
                selectedConnectionId: null,
                selectedConnection: null,
                draggingNode: null,
                draggingConnection: null,
                dragOffset: { x: 0, y: 0 }
            };
        },
        methods: {
            getNode(nodeId) {
                if (!this.workflow || !this.workflow.nodes) return null;
                return this.workflow.nodes.find(n => n.id === nodeId);
            },

            getNodeType(typeId) {
                return window.getNodeType ? window.getNodeType(typeId) : { label: typeId, icon: 'fa-solid fa-square', color: '#007bff', shape: 'rectangle' };
            },

            getNodeShape(nodeType) {
                const nt = this.getNodeType(nodeType);
                return nt.shape || 'rectangle';
            },

            getNodeColor(node) {
                const nt = this.getNodeType(node.type);
                return nt.color || '#007bff';
            },

            getNodeStroke(node) {
                const color = this.getNodeColor(node);
                // Darken color for stroke
                return this.adjustColor(color, -0.2);
            },

            getNodeTextColor(node) {
                const nodeType = node.type;
                if (nodeType === 'decision' || nodeType === 'flowchart_decision') {
                    return '#000000';
                }
                return '#ffffff';
            },

            getNodeIcon(nodeType) {
                const nt = this.getNodeType(nodeType);
                return nt.icon || 'fa-solid fa-square';
            },

            isStartNode(nodeType) {
                return nodeType === 'start';
            },

            isEndNode(nodeType) {
                return nodeType === 'end';
            },

            adjustColor(color, percent) {
                const num = parseInt(color.replace("#",""), 16);
                const amt = Math.round(2.55 * percent);
                const R = (num >> 16) + amt;
                const G = (num >> 8 & 0x00FF) + amt;
                const B = (num & 0x0000FF) + amt;
                return "#" + (0x1000000 + (R<255?R<1?0:R:255)*0x10000 +
                    (G<255?G<1?0:G:255)*0x100 +
                    (B<255?B<1?0:B:255))
                    .toString(16).slice(1);
            },

            getNodeCenterX(node) {
                return node ? node.position.x + 40 : 0;
            },

            getNodeCenterY(node) {
                return node ? node.position.y + 30 : 0;
            },

            getMidpointX(node1, node2) {
                return (this.getNodeCenterX(node1) + this.getNodeCenterX(node2)) / 2;
            },

            getMidpointY(node1, node2) {
                return (this.getNodeCenterY(node1) + this.getNodeCenterY(node2)) / 2;
            },

            getPolygonPoints(node) {
                const x = node.position.x + 40;
                const y = node.position.y + 30;
                return `${x},${y - 35} ${x + 45},${y} ${x},${y + 35} ${x - 45},${y}`;
            },

            selectNode(node, event) {
                if (event && event.button === 0) {
                    this.draggingNode = node;
                    this.dragOffset = {
                        x: event.clientX - node.position.x,
                        y: event.clientY - node.position.y
                    };
                }
                this.$emit('node-selected', node);
            },

            selectConnection(conn) {
                this.selectedConnectionId = conn.id;
                this.selectedConnection = conn;
            },

            startConnection(node, event) {
                this.draggingConnection = {
                    x1: this.getNodeCenterX(node),
                    y1: this.getNodeCenterY(node),
                    x2: event.clientX,
                    y2: event.clientY,
                    fromNode: node
                };
            },

            onCanvasMouseDown(event) {
                if (event.target.tagName === 'svg' || event.target.classList.contains('designer-canvas')) {
                    this.selectedConnectionId = null;
                    this.selectedConnection = null;
                }
            },

            onCanvasMouseMove(event) {
                if (this.draggingNode) {
                    this.draggingNode.position.x = event.clientX - this.dragOffset.x;
                    this.draggingNode.position.y = event.clientY - this.dragOffset.y;
                    this.$emit('node-updated', this.draggingNode);
                    this.$forceUpdate();
                }

                if (this.draggingConnection) {
                    this.draggingConnection.x2 = event.clientX;
                    this.draggingConnection.y2 = event.clientY;
                    this.$forceUpdate();
                }
            },

            onCanvasMouseUp(event) {
                if (this.draggingConnection) {
                    const connEndNode = this.getNodeAtPosition(event.clientX, event.clientY);
                    if (connEndNode && connEndNode.id !== this.draggingConnection.fromNode.id) {
                        this.$emit('connection-created', this.draggingConnection.fromNode.id, connEndNode.id);
                    }
                    this.draggingConnection = null;
                    this.$forceUpdate();
                }
                this.draggingNode = null;
            },

            getNodeAtPosition(x, y) {
                if (!this.workflow || !this.workflow.nodes) return null;
                for (const node of this.workflow.nodes) {
                    const nx = node.position.x;
                    const ny = node.position.y;
                    if (x >= nx && x <= nx + 80 && y >= ny && y <= ny + 60) {
                        return node;
                    }
                }
                return null;
            },

            handleDrop(event) {
                event.preventDefault();
                const nodeTypeStr = event.dataTransfer.getData("nodeType");
                if (!nodeTypeStr) return;

                try {
                    const nodeType = JSON.parse(nodeTypeStr);
                    const rect = this.$refs.svgCanvas.getBoundingClientRect();
                    const position = {
                        x: event.clientX - rect.left - 40,
                        y: event.clientY - rect.top - 30
                    };
                    this.$emit('node-dropped', position, nodeType);
                } catch (e) {
                    console.error('Error parsing node type:', e);
                }
            },

            deleteConnection(conn) {
                shared.showConfirm({
                    title: "Delete Connection",
                    message1: "Are you sure you want to delete this connection?",
                    callback: () => {
                        this.$emit('connection-deleted', conn.id);
                        this.selectedConnectionId = null;
                        this.selectedConnection = null;
                        this.$forceUpdate();
                    }
                });
            }
        }
    }
</script>

<style scoped>
    .designer-canvas {
        width: 100%;
        height: 100%;
        position: relative;
        overflow: auto;
        user-select: none;
        cursor: default;
    }

    svg {
        display: block;
        width: 100%;
        height: 100%;
        background-color: #f8f9fa;
    }

    .workflow-node {
        cursor: grab;
    }

    .workflow-node:active {
        cursor: grabbing;
    }

    .node-selected .node-shape {
        filter: drop-shadow(0 0 5px rgba(0, 123, 255, 0.7));
    }

    .port {
        fill: white;
        stroke: #007bff;
        stroke-width: 2;
        cursor: crosshair;
    }

    .port:hover {
        fill: #007bff;
    }

    .connection-line {
        stroke: #666;
        stroke-width: 2;
        fill: none;
        cursor: pointer;
    }

    .connection-line:hover {
        stroke: #ff6b6b;
        stroke-width: 3;
    }

    .connection-selected {
        stroke: #ff6b6b;
        stroke-width: 3;
    }

    .connection-temp {
        stroke: #007bff;
        stroke-width: 2;
        stroke-dasharray: 5, 5;
        fill: none;
    }

    .node-shape {
        cursor: grab;
    }

    .node-shape:active {
        cursor: grabbing;
    }

    .node-icon {
        position: absolute;
        font-size: 12px;
        color: white;
        pointer-events: none;
        opacity: 0.7;
    }

    .node-label {
        word-wrap: break-word;
        max-width: 60px;
    }
</style>
