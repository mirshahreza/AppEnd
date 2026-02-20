import React, { useCallback, useEffect, useRef, useState } from 'react';
import ReactFlow, {
    Controls,
    Background,
    useNodesState,
    useEdgesState,
    addEdge,
    MarkerType
} from 'reactflow';
import 'reactflow/dist/style.css';
import './App.css';

export default function App() {
    const [workflowId, setWorkflowId] = useState(null);
    const [nodes, setNodes, onNodesChange] = useNodesState([]);
    const [edges, setEdges, onEdgesChange] = useEdgesState([]);
    const [isReady, setIsReady] = useState(false);
    const messageHandlerRef = useRef(null);

    const requestSave = useCallback(() => {
        if (window.parent !== window) {
            window.parent.postMessage({
                type: 'REQUEST_SAVE',
                timestamp: Date.now()
            }, window.location.origin);
        }
    }, []);

    const getNodeType = useCallback((activityType) => {
        const key = (activityType || '').toLowerCase();
        if (key.includes('start') || key.includes('trigger')) return 'input';
        if (key.includes('end') || key.includes('finish')) return 'output';
        return 'default';
    }, []);

    const buildGraphFromDefinition = useCallback((definition) => {
        const activities = definition?.activities || definition?.root?.activities || [];
        const connections = definition?.connections || [];

        if (!Array.isArray(activities) || activities.length === 0) {
            return { nodes: [], edges: [] };
        }

        const builtNodes = activities.map((activity, index) => {
            const label = activity.displayName || activity.name || activity.type || 'Activity';
            const subtitle = activity.type || '';
            const x = Number.isFinite(activity.x) ? activity.x : 250;
            const y = Number.isFinite(activity.y) ? activity.y : index * 80;
            const width = Number.isFinite(activity.width) ? activity.width : 160;
            const height = Number.isFinite(activity.height) ? activity.height : 40;

            return {
                id: activity.id || `activity-${index}`,
                type: getNodeType(activity.type),
                data: { label: `${label}${subtitle ? ` (${subtitle})` : ''}` },
                position: { x, y },
                style: { width, height }
            };
        });

        const nodeIds = new Set(builtNodes.map(node => node.id));
        let builtEdges = Array.isArray(connections)
            ? connections
                .filter(conn => nodeIds.has(conn.sourceActivityId) && nodeIds.has(conn.targetActivityId))
                .map(conn => ({
                    id: conn.id || `edge-${conn.sourceActivityId}-${conn.targetActivityId}`,
                    source: conn.sourceActivityId,
                    target: conn.targetActivityId,
                    label: conn.label || conn.sourceOutcome || undefined,
                    type: 'smoothstep',
                    markerEnd: { type: MarkerType.ArrowClosed }
                }))
            : [];

        if (builtEdges.length === 0 && builtNodes.length > 1) {
            builtEdges = builtNodes.slice(1).map((node, index) => ({
                id: `edge-${builtNodes[index].id}-${node.id}`,
                source: builtNodes[index].id,
                target: node.id,
                type: 'smoothstep',
                markerEnd: { type: MarkerType.ArrowClosed }
            }));
        }

        return { nodes: builtNodes, edges: builtEdges };
    }, [getNodeType]);

    const loadDefaultWorkflow = useCallback((id) => {
        const defaultNodes = [
            {
                id: 'start',
                type: 'input',
                data: { label: 'Start' },
                position: { x: 250, y: 0 },
                style: { width: 160, height: 40 }
            },
            {
                id: 'action1',
                type: 'default',
                data: { label: 'Action 1' },
                position: { x: 250, y: 80 },
                style: { width: 160, height: 40 }
            },
            {
                id: 'end',
                type: 'output',
                data: { label: 'End' },
                position: { x: 250, y: 160 },
                style: { width: 160, height: 40 }
            }
        ];

        const defaultEdges = [
            { id: 'edge-start-action1', source: 'start', target: 'action1', type: 'smoothstep', markerEnd: { type: MarkerType.ArrowClosed } },
            { id: 'edge-action1-end', source: 'action1', target: 'end', type: 'smoothstep', markerEnd: { type: MarkerType.ArrowClosed } }
        ];

        setNodes(defaultNodes);
        setEdges(defaultEdges);
        setIsReady(true);

        console.log('[ReactApp] Sending WORKFLOW_LOADED message to parent');
        if (window.parent !== window) {
            window.parent.postMessage({
                type: 'WORKFLOW_LOADED',
                data: {
                    nodes: defaultNodes,
                    edges: defaultEdges,
                    id: id
                },
                timestamp: Date.now()
            }, window.location.origin);
        }
    }, [setNodes, setEdges]);

    // Get workflow ID from URL parameters
    useEffect(() => {
        const params = new URLSearchParams(window.location.search);
        const id = params.get('workflowId');
        console.log('[ReactApp] Workflow ID from URL:', id);
        setWorkflowId(id);
        loadDefaultWorkflow(id);
    }, [loadDefaultWorkflow]);

    // Listen for messages from parent Vue component
    useEffect(() => {
        const handleMessage = (event) => {
            // Security: Only accept messages from same origin
            if (event.origin !== window.location.origin) {
                console.warn('[ReactApp] Message from untrusted origin:', event.origin);
                return;
            }

            const { type, data } = event.data;
            console.log('[ReactApp] Received message from parent:', type);

            switch (type) {
                case 'LOAD_WORKFLOW': {
                    const workflowDefinitionJson = data?.workflowDefinitionJson;
                    if (workflowDefinitionJson) {
                        try {
                            const workflowDefinition = JSON.parse(workflowDefinitionJson);
                            console.log('[ReactApp] Loading workflow definition');
                            const graph = buildGraphFromDefinition(workflowDefinition);
                            setNodes(graph.nodes);
                            setEdges(graph.edges);
                            setIsReady(true);
                        } catch (error) {
                            console.error('[ReactApp] Failed to parse workflow definition:', error);
                        }
                    }
                    break;
                }
                case 'GET_WORKFLOW':
                    console.log('[ReactApp] Sending workflow data to parent');
                    if (window.parent !== window) {
                        window.parent.postMessage({
                            type: 'WORKFLOW_CHANGED',
                            data: {
                                nodes: nodes,
                                edges: edges,
                                id: workflowId
                            },
                            timestamp: Date.now()
                        }, window.location.origin);
                    }
                    break;
                default:
                    console.log('[ReactApp] Unknown message:', type);
            }
        };

        messageHandlerRef.current = handleMessage;
        window.addEventListener('message', handleMessage);
        
        return () => {
            window.removeEventListener('message', handleMessage);
        };
    }, [workflowId, nodes, edges, buildGraphFromDefinition, setNodes, setEdges]);

    const onConnect = useCallback(
        (connection) => {
            const newEdges = addEdge(connection, edges);
            setEdges(newEdges);
            
            // Notify parent of change
            if (window.parent !== window) {
                window.parent.postMessage({
                    type: 'WORKFLOW_CHANGED',
                    data: {
                        nodes: nodes,
                        edges: newEdges
                    },
                    timestamp: Date.now()
                }, window.location.origin);
            }
        },
        [setEdges, nodes, edges]
    );

    if (!isReady) {
        return (
            <div style={{
                width: '100%',
                height: '100%',
                display: 'flex',
                alignItems: 'center',
                justifyContent: 'center'
            }}>
                <p>Loading...</p>
            </div>
        );
    }

    return (
        <div className="rf-layout">
            <aside className="rf-panel rf-panel-left">
                <div className="rf-panel-header">Toolbar</div>
            </aside>

            <div className="rf-canvas">
                <div className="rf-toolbar">
                    <button className="rf-save" onClick={requestSave} title="Save">
                        Save
                    </button>
                </div>
                <ReactFlow
                    nodes={nodes}
                    edges={edges}
                    onNodesChange={onNodesChange}
                    onEdgesChange={onEdgesChange}
                    onConnect={onConnect}
                    fitView
                >
                    <Background variant="dots" gap={18} size={1.2} color="#d7dbe2" />
                    <Controls />
                </ReactFlow>
            </div>

            <aside className="rf-panel rf-panel-right">
                <div className="rf-panel-header">Properties</div>
            </aside>
        </div>
    );
}
