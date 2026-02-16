window.WorkflowBuilder = class WorkflowBuilder {
    constructor(workflowData = null) {
        if (workflowData && workflowData.id) {
            this.workflow = JSON.parse(JSON.stringify(workflowData));
        } else {
            this.workflow = {
                id: 'wf_' + Date.now(),
                name: 'New Workflow',
                nodes: [],
                connections: [],
                metadata: {
                    description: '',
                    createdAt: new Date().toISOString(),
                    updatedAt: new Date().toISOString()
                }
            };
        }

        this.history = [JSON.parse(JSON.stringify(this.workflow))];
        this.historyIndex = 0;
    }

    addNode(nodeType, position) {
        const nodeId = 'node_' + Date.now() + Math.random().toString(36).substr(2, 9);
        const node = {
            id: nodeId,
            type: nodeType.type || nodeType.id,
            category: nodeType.category || 'primitives',
            label: nodeType.label,
            description: '',
            condition: '',
            configuration: {},
            inputs: [],
            outputs: [],
            position: {
                x: Math.max(Math.round((position.x || 50) / 10) * 10, 0),
                y: Math.max(Math.round((position.y || 50) / 10) * 10, 0)
            }
        };

        this.workflow.nodes.push(node);
        this.saveHistory();
        return node;
    }

    deleteNode(nodeId) {
        this.workflow.nodes = this.workflow.nodes.filter(n => n.id !== nodeId);
        this.workflow.connections = this.workflow.connections.filter(
            c => c.from !== nodeId && c.to !== nodeId
        );
        this.saveHistory();
    }

    updateNode(node) {
        const existing = this.workflow.nodes.find(n => n.id === node.id);
        if (existing) {
            Object.assign(existing, node);
            existing.metadata = existing.metadata || {};
            existing.metadata.updatedAt = new Date().toISOString();
            this.saveHistory();
        }
    }

    moveNode(nodeId, position) {
        const node = this.workflow.nodes.find(n => n.id === nodeId);
        if (node) {
            node.position = {
                x: Math.max(Math.round(position.x / 10) * 10, 0),
                y: Math.max(Math.round(position.y / 10) * 10, 0)
            };
            this.saveHistory();
        }
    }

    addConnection(fromNodeId, toNodeId, label = '') {
        if (fromNodeId === toNodeId) return null;

        const fromNode = this.workflow.nodes.find(n => n.id === fromNodeId);
        const toNode = this.workflow.nodes.find(n => n.id === toNodeId);

        if (!fromNode || !toNode) return null;

        // Validate connection rules
        if (toNode.type === 'start') {
            console.warn('Cannot connect to Start node');
            return null;
        }

        if (fromNode.type === 'end') {
            console.warn('Cannot connect from End node');
            return null;
        }

        const connectionExists = this.workflow.connections.some(
            c => c.from === fromNodeId && c.to === toNodeId
        );

        if (connectionExists) return null;

        const connection = {
            id: 'conn_' + Date.now() + Math.random().toString(36).substr(2, 9),
            from: fromNodeId,
            to: toNodeId,
            label: label,
            type: fromNode.type === 'decision' ? 'conditional' : 'flow'
        };

        this.workflow.connections.push(connection);
        this.saveHistory();
        return connection;
    }

    deleteConnection(connectionId) {
        this.workflow.connections = this.workflow.connections.filter(c => c.id !== connectionId);
        this.saveHistory();
    }

    updateConnection(connectionId, updates) {
        const conn = this.workflow.connections.find(c => c.id === connectionId);
        if (conn) {
            Object.assign(conn, updates);
            this.saveHistory();
        }
    }

    validateWorkflow() {
        const errors = [];

        if (this.workflow.nodes.length === 0) {
            errors.push('Workflow must have at least one node');
        }

        const hasStart = this.workflow.nodes.some(n => n.type === 'start');
        const hasEnd = this.workflow.nodes.some(n => n.type === 'end');

        if (!hasStart) errors.push('Workflow must have a Start node');
        if (!hasEnd) errors.push('Workflow must have an End node');

        // Check for disconnected nodes
        const connectedNodes = new Set();
        this.workflow.connections.forEach(c => {
            connectedNodes.add(c.from);
            connectedNodes.add(c.to);
        });

        const nodeIds = new Set(this.workflow.nodes.map(n => n.id));
        nodeIds.forEach(id => {
            if (!connectedNodes.has(id) && this.workflow.nodes.find(n => n.id === id)?.type !== 'start') {
                if (this.workflow.nodes.find(n => n.id === id)?.type !== 'end') {
                    errors.push(`Node is disconnected`);
                }
            }
        });

        return { valid: errors.length === 0, errors };
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

    canUndo() {
        return this.historyIndex > 0;
    }

    canRedo() {
        return this.historyIndex < this.history.length - 1;
    }

    getWorkflow() {
        return JSON.parse(JSON.stringify(this.workflow));
    }

    updateMetadata(metadata) {
        this.workflow.metadata = { ...this.workflow.metadata, ...metadata };
        this.workflow.metadata.updatedAt = new Date().toISOString();
        this.saveHistory();
    }
};
