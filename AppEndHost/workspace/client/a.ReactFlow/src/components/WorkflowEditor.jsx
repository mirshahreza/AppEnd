import React, { useState } from 'react';

export default function WorkflowEditor({ initialNodes = [], initialEdges = [] }) {
    const [nodes, setNodes] = useState(initialNodes);
    const [edges, setEdges] = useState(initialEdges);

    const addNode = (type = 'action') => {
        const newNode = {
            id: `node-${Date.now()}`,
            data: { label: `New ${type}` },
            position: {
                x: Math.random() * 400,
                y: Math.random() * 400
            },
            style: {
                background: type === 'start' ? '#90EE90' : type === 'end' ? '#FFB6C6' : '#87CEEB',
                border: '2px solid #333'
            }
        };

        setNodes([...nodes, newNode]);
    };

    const deleteNode = (nodeId) => {
        setNodes(nodes.filter(node => node.id !== nodeId));
        setEdges(edges.filter(edge => edge.source !== nodeId && edge.target !== nodeId));
    };

    return (
        <div style={{ padding: '10px', borderBottom: '1px solid #ddd' }}>
            <button onClick={() => addNode('start')} style={{ marginRight: '5px' }}>
                + Start
            </button>
            <button onClick={() => addNode('action')} style={{ marginRight: '5px' }}>
                + Action
            </button>
            <button onClick={() => addNode('end')}>
                + End
            </button>
        </div>
    );
}
