// Node Types and Activity Categories for Workflow Designer
window.NodeTypes = {
    // Control Flow Nodes
    START: {
        id: 'start',
        type: 'start',
        label: 'Start',
        category: 'control',
        icon: 'fa-solid fa-play-circle',
        color: '#28a745',
        description: 'Start of the workflow',
        shape: 'circle'
    },
    END: {
        id: 'end',
        type: 'end',
        label: 'End',
        category: 'control',
        icon: 'fa-solid fa-stop-circle',
        color: '#dc3545',
        description: 'End of the workflow',
        shape: 'circle'
    },

    // Basic Nodes
    TASK: {
        id: 'task',
        type: 'task',
        label: 'Task',
        category: 'primitives',
        icon: 'fa-solid fa-square',
        color: '#007bff',
        description: 'Execute a task or action',
        shape: 'rectangle'
    },
    DECISION: {
        id: 'decision',
        type: 'decision',
        label: 'Decision',
        category: 'branching',
        icon: 'fa-solid fa-diamond',
        color: '#ffc107',
        description: 'Conditional branching',
        shape: 'diamond'
    },

    // Branching Activities
    IF_ELSE: {
        id: 'if_else',
        type: 'if_else',
        label: 'If/Else',
        category: 'branching',
        icon: 'fa-solid fa-code-branch',
        color: '#17a2b8',
        description: 'Conditional branching',
        shape: 'rectangle'
    },
    SWITCH: {
        id: 'switch',
        type: 'switch',
        label: 'Switch',
        category: 'branching',
        icon: 'fa-solid fa-exchange-alt',
        color: '#17a2b8',
        description: 'Switch/case branching',
        shape: 'rectangle'
    },

    // Looping Activities
    FOR_LOOP: {
        id: 'for_loop',
        type: 'for_loop',
        label: 'For',
        category: 'looping',
        icon: 'fa-solid fa-repeat',
        color: '#6f42c1',
        description: 'For loop iteration',
        shape: 'rectangle'
    },
    FOREACH_LOOP: {
        id: 'foreach_loop',
        type: 'foreach_loop',
        label: 'For Each',
        category: 'looping',
        icon: 'fa-solid fa-list-ol',
        color: '#6f42c1',
        description: 'For each collection item',
        shape: 'rectangle'
    },
    WHILE_LOOP: {
        id: 'while_loop',
        type: 'while_loop',
        label: 'While',
        category: 'looping',
        icon: 'fa-solid fa-sync-alt',
        color: '#6f42c1',
        description: 'While loop',
        shape: 'rectangle'
    },
    PARALLEL_LOOP: {
        id: 'parallel_loop',
        type: 'parallel_loop',
        label: 'Parallel For Each',
        category: 'looping',
        icon: 'fa-solid fa-stream',
        color: '#6f42c1',
        description: 'Parallel iteration',
        shape: 'rectangle'
    },
    BREAK: {
        id: 'break',
        type: 'break',
        label: 'Break',
        category: 'looping',
        icon: 'fa-solid fa-stop',
        color: '#fd7e14',
        description: 'Break from loop',
        shape: 'circle'
    },

    // Composition Activities
    WORKFLOW_INVOKE: {
        id: 'workflow_invoke',
        type: 'workflow_invoke',
        label: 'Invoke Workflow',
        category: 'composition',
        icon: 'fa-solid fa-project-diagram',
        color: '#20c997',
        description: 'Invoke another workflow',
        shape: 'rectangle'
    },

    // HTTP/API Activities
    HTTP_REQUEST: {
        id: 'http_request',
        type: 'http_request',
        label: 'HTTP Request',
        category: 'http',
        icon: 'fa-solid fa-globe',
        color: '#e83e8c',
        description: 'Make an HTTP request',
        shape: 'rectangle'
    },

    // Communication Activities
    EMAIL: {
        id: 'email',
        type: 'email',
        label: 'Send Email',
        category: 'email',
        icon: 'fa-solid fa-envelope',
        color: '#fd7e14',
        description: 'Send email notification',
        shape: 'rectangle'
    },

    // Console/Logging Activities
    CONSOLE_LOG: {
        id: 'console_log',
        type: 'console_log',
        label: 'Log',
        category: 'console',
        icon: 'fa-solid fa-terminal',
        color: '#495057',
        description: 'Write to console',
        shape: 'rectangle'
    },

    // Storage Activities
    DATABASE_QUERY: {
        id: 'database_query',
        type: 'database_query',
        label: 'Database Query',
        category: 'storage',
        icon: 'fa-solid fa-database',
        color: '#0dcaf0',
        description: 'Execute database query',
        shape: 'rectangle'
    },

    // Scheduling Activities
    DELAY: {
        id: 'delay',
        type: 'delay',
        label: 'Delay',
        category: 'scheduling',
        icon: 'fa-solid fa-hourglass-end',
        color: '#6c757d',
        description: 'Add delay/wait',
        shape: 'rectangle'
    },
    TIMER: {
        id: 'timer',
        type: 'timer',
        label: 'Timer',
        category: 'scheduling',
        icon: 'fa-solid fa-clock',
        color: '#6c757d',
        description: 'Schedule a timer',
        shape: 'rectangle'
    },

    // Scripting Activities
    SCRIPT: {
        id: 'script',
        type: 'script',
        label: 'Script',
        category: 'scripting',
        icon: 'fa-solid fa-code',
        color: '#9b59b6',
        description: 'Execute custom script',
        shape: 'rectangle'
    },

    // Variable/Primitive Activities
    ASSIGN_VARIABLE: {
        id: 'assign_variable',
        type: 'assign_variable',
        label: 'Assign Variable',
        category: 'primitives',
        icon: 'fa-solid fa-equals',
        color: '#8e44ad',
        description: 'Assign value to variable',
        shape: 'rectangle'
    },
    VARIABLE_COUNTER: {
        id: 'variable_counter',
        type: 'variable_counter',
        label: 'Counter',
        category: 'primitives',
        icon: 'fa-solid fa-plus-circle',
        color: '#8e44ad',
        description: 'Increment counter',
        shape: 'rectangle'
    },

    // Flow Activities
    FLOWCHART_DECISION: {
        id: 'flowchart_decision',
        type: 'flowchart_decision',
        label: 'Flowchart Decision',
        category: 'flow',
        icon: 'fa-solid fa-code-branch',
        color: '#16a34a',
        description: 'Flowchart decision node',
        shape: 'diamond'
    },
    FLOWCHART_ACTION: {
        id: 'flowchart_action',
        type: 'flowchart_action',
        label: 'Flowchart Action',
        category: 'flow',
        icon: 'fa-solid fa-square-check',
        color: '#16a34a',
        description: 'Flowchart action node',
        shape: 'rectangle'
    },

    // Error Handling
    TRY_CATCH: {
        id: 'try_catch',
        type: 'try_catch',
        label: 'Try/Catch',
        category: 'control',
        icon: 'fa-solid fa-shield-alt',
        color: '#e74c3c',
        description: 'Try/catch error handling',
        shape: 'rectangle'
    }
};

// Group by category
window.NodeCategories = {
    control: {
        name: 'Control Flow',
        icon: 'fa-solid fa-arrows-alt',
        nodes: ['start', 'end', 'try_catch']
    },
    branching: {
        name: 'Branching',
        icon: 'fa-solid fa-code-branch',
        nodes: ['decision', 'if_else', 'switch', 'flowchart_decision']
    },
    composition: {
        name: 'Composition',
        icon: 'fa-solid fa-layer-group',
        nodes: ['workflow_invoke']
    },
    console: {
        name: 'Console',
        icon: 'fa-solid fa-terminal',
        nodes: ['console_log']
    },
    email: {
        name: 'Email',
        icon: 'fa-solid fa-envelope',
        nodes: ['email']
    },
    flow: {
        name: 'Flow',
        icon: 'fa-solid fa-project-diagram',
        nodes: ['flowchart_decision', 'flowchart_action']
    },
    http: {
        name: 'HTTP',
        icon: 'fa-solid fa-globe',
        nodes: ['http_request']
    },
    looping: {
        name: 'Looping',
        icon: 'fa-solid fa-repeat',
        nodes: ['for_loop', 'foreach_loop', 'while_loop', 'parallel_loop', 'break']
    },
    primitives: {
        name: 'Primitives',
        icon: 'fa-solid fa-cube',
        nodes: ['task', 'assign_variable', 'variable_counter']
    },
    scheduling: {
        name: 'Scheduling',
        icon: 'fa-solid fa-clock',
        nodes: ['delay', 'timer']
    },
    scripting: {
        name: 'Scripting',
        icon: 'fa-solid fa-code',
        nodes: ['script']
    },
    storage: {
        name: 'Storage',
        icon: 'fa-solid fa-database',
        nodes: ['database_query']
    }
};

// Utility functions
window.getNodeType = (typeId) => {
    for (const key in window.NodeTypes) {
        if (window.NodeTypes[key].id === typeId || window.NodeTypes[key].type === typeId) {
            return window.NodeTypes[key];
        }
    }
    return window.NodeTypes.TASK;
};

window.getNodesByCategory = (category) => {
    const nodeIds = window.NodeCategories[category]?.nodes || [];
    return nodeIds.map(id => window.getNodeType(id));
};

window.getAllCategories = () => {
    return Object.keys(window.NodeCategories).map(key => ({
        id: key,
        ...window.NodeCategories[key]
    }));
};
