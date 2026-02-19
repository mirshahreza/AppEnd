/**
 * Workflow Designer RPC Methods - JavaScript Mock
 * Provides temporary RPC handling for WorkflowDesigner
 * TODO: Replace with actual backend RPC methods
 */

// Global RPC method registry
window.rpcMethods = window.rpcMethods || {};

// Mock RPC methods for Workflow Designer
window.rpcMethods.LoadWorkflowDesign = function(params) {
    console.log("LoadWorkflowDesign called with:", params);
    return {
        success: true,
        workflow: {
            id: params.workflowId,
            name: "Workflow - " + params.workflowId,
            description: "Loaded from ID: " + params.workflowId,
            version: 1,
            isPublished: false,
            activities: [
                { id: "start-1", type: "Start", displayName: "Start", description: "", x: 50, y: 50 },
                { id: "action-1", type: "Action", displayName: "Process", description: "", x: 250, y: 50 },
                { id: "end-1", type: "End", displayName: "End", description: "", x: 450, y: 50 }
            ],
            connections: [
                { id: "conn-1", sourceActivityId: "start-1", targetActivityId: "action-1" },
                { id: "conn-2", sourceActivityId: "action-1", targetActivityId: "end-1" }
            ],
            variables: [],
            canvasZoom: 1.0,
            canvasPanX: 0,
            canvasPanY: 0
        }
    };
};

window.rpcMethods.SaveWorkflowDesign = function(params) {
    console.log("SaveWorkflowDesign called with:", params);
    return {
        success: true,
        message: "Workflow saved successfully",
        workflowId: params.workflow?.id || Date.now().toString()
    };
};

window.rpcMethods.CreateNewWorkflowDesign = function(params) {
    console.log("CreateNewWorkflowDesign called with:", params);
    return {
        success: true,
        workflow: {
            id: params.id || "workflow-" + Date.now(),
            name: params.name || "New Workflow",
            description: "",
            version: 1,
            isPublished: false,
            activities: [],
            connections: [],
            variables: [],
            canvasZoom: 1.0,
            canvasPanX: 0,
            canvasPanY: 0
        }
    };
};

window.rpcMethods.ValidateWorkflowDesign = function(params) {
    console.log("ValidateWorkflowDesign called with:", params);
    return {
        success: true,
        isValid: true,
        errors: [],
        warnings: []
    };
};

window.rpcMethods.GetActivityRegistry = function(params) {
    console.log("GetActivityRegistry called");
    return {
        success: true,
        categories: [
            {
                name: "Core",
                activities: [
                    { id: "start", type: "Start", displayName: "Start", icon: "fas fa-play", color: "#28a745", description: "Workflow entry point" },
                    { id: "end", type: "End", displayName: "End", icon: "fas fa-stop", color: "#dc3545", description: "Workflow exit point" }
                ]
            },
            {
                name: "FlowControl",
                activities: [
                    { id: "decision", type: "Decision", displayName: "Decision", icon: "fas fa-code-branch", color: "#ffc107", description: "If/Then branching" },
                    { id: "loop", type: "Loop", displayName: "Loop", icon: "fas fa-redo", color: "#17a2b8", description: "Repeat actions" },
                    { id: "action", type: "Action", displayName: "Action", icon: "fas fa-cog", color: "#6c757d", description: "Execute action" }
                ]
            }
        ]
    };
};

window.rpcMethods.GetActivityInfo = function(params) {
    console.log("GetActivityInfo called with:", params);
    return {
        success: true,
        activity: {
            type: params.activityType,
            displayName: params.activityType,
            properties: {},
            inputMapping: {},
            outputMapping: {}
        }
    };
};

window.rpcMethods.ExportWorkflowDesign = function(params) {
    console.log("ExportWorkflowDesign called");
    return {
        success: true,
        json: JSON.stringify(params.workflow),
        filename: "workflow-" + Date.now() + ".json"
    };
};

window.rpcMethods.DeleteWorkflowDesign = function(params) {
    console.log("DeleteWorkflowDesign called with:", params);
    return {
        success: true,
        message: "Workflow deleted successfully"
    };
};

window.rpcMethods.GetAllWorkflowDesigns = function(params) {
    console.log("GetAllWorkflowDesigns called");
    return {
        success: true,
        workflows: [
            { id: "wf-1", name: "Order Processing", description: "Sample workflow", version: 1, isPublished: true },
            { id: "wf-2", name: "Approval Workflow", description: "Approval process", version: 2, isPublished: false }
        ],
        total: 2
    };
};

console.log("✓ Workflow Designer RPC Mock Methods Loaded");
