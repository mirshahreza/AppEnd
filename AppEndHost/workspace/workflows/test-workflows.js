// Workflow System Test Script
// Use this in browser console to test workflow RPC calls
// Location: AppEndHost/workspace/workflows/test-workflows.js

console.log("=== Workflow System Test Script ===");
console.log("Run these commands one by one to test workflows:");

// Test 1: Get all workflow definitions
console.log("\n--- Test 1: Get All Workflow Definitions ---");
console.log(`
rpcAEP("GetWorkflowDefinitions", {}, function(res) {
    console.log("Loaded workflows:", res);
    if (res && res.length === 4) {
        console.log("✅ SUCCESS: All 4 workflows loaded");
    } else {
        console.log("❌ FAILED: Expected 4 workflows, got", res?.length || 0);
    }
});
`);

// Test 2: Get single workflow definition
console.log("\n--- Test 2: Get Single Workflow Definition ---");
console.log(`
rpcAEP("GetWorkflowDefinition", { WorkflowId: "hello-world" }, function(res) {
    console.log("Workflow details:", res);
    if (res && res.Id === "hello-world") {
        console.log("✅ SUCCESS: Workflow loaded");
    } else {
        console.log("❌ FAILED: Could not load workflow");
    }
});
`);

// Test 3: Execute Hello World workflow
console.log("\n--- Test 3: Execute Hello World Workflow ---");
console.log(`
rpcAEP("ExecuteWorkflow", { 
    WorkflowId: "hello-world", 
    InputParams: {} 
}, function(res) {
    console.log("Execution result:", res);
    if (res && res.Success) {
        console.log("✅ SUCCESS: Workflow executed");
        console.log("   Output:", res.Output);
        console.log("   Status:", res.Status);
        console.log("   Instance ID:", res.InstanceId);
    } else {
        console.log("❌ FAILED: Workflow execution failed");
        console.log("   Error:", res?.ErrorMessage);
    }
});
`);

// Test 4: Execute Scheduled DB Check workflow
console.log("\n--- Test 4: Execute Scheduled DB Check Workflow ---");
console.log(`
rpcAEP("ExecuteWorkflow", { 
    WorkflowId: "scheduled-db-check", 
    InputParams: {} 
}, function(res) {
    console.log("Execution result:", res);
    if (res && res.Success) {
        console.log("✅ SUCCESS: Workflow executed");
        console.log("   Status:", res.Output?.status);
        console.log("   Record Count:", res.Output?.recordCount);
    } else {
        console.log("❌ FAILED: Workflow execution failed");
    }
});
`);

// Test 5: Execute Order Approval workflow (auto-approve path)
console.log("\n--- Test 5: Execute Order Approval (Auto-Approve) ---");
console.log(`
rpcAEP("ExecuteWorkflow", { 
    WorkflowId: "order-approval", 
    InputParams: {
        orderId: "ORD-TEST-001",
        orderAmount: 500000,  // Below 1M threshold = auto-approve
        requestedBy: "test-user"
    }
}, function(res) {
    console.log("Execution result:", res);
    if (res && res.Success) {
        console.log("✅ SUCCESS: Workflow executed");
        console.log("   Approved:", res.Output?.approved);
        console.log("   Status:", res.Output?.approvalStatus);
        console.log("   Approved By:", res.Output?.approvedBy);
    } else {
        console.log("❌ FAILED: Workflow execution failed");
    }
});
`);

// Test 6: Execute Order Approval workflow (manager approval path)
console.log("\n--- Test 6: Execute Order Approval (Manager Required) ---");
console.log(`
rpcAEP("ExecuteWorkflow", { 
    WorkflowId: "order-approval", 
    InputParams: {
        orderId: "ORD-TEST-002",
        orderAmount: 2500000,  // Above 1M threshold = requires manager
        requestedBy: "test-user"
    }
}, function(res) {
    console.log("Execution result:", res);
    if (res && res.Status === "Suspended") {
        console.log("✅ SUCCESS: Workflow suspended (waiting for manager approval)");
        console.log("   Instance ID:", res.InstanceId);
        console.log("   Task should be created in kartabl");
    } else if (res && res.Success) {
        console.log("⚠️ WARNING: Workflow completed instead of suspending");
    } else {
        console.log("❌ FAILED: Workflow execution failed");
    }
});
`);

// Test 7: Execute Data Pipeline workflow
console.log("\n--- Test 7: Execute Data Pipeline Workflow ---");
console.log(`
rpcAEP("ExecuteWorkflow", { 
    WorkflowId: "data-pipeline", 
    InputParams: {
        sourceTable: "BaseUsers",     // Use existing table
        targetTable: "TempUsers",     // Test target table
        batchSize: 10
    }
}, function(res) {
    console.log("Execution result:", res);
    if (res && res.Success) {
        console.log("✅ SUCCESS: Pipeline executed");
        console.log("   Extracted:", res.Output?.totalExtracted);
        console.log("   Loaded:", res.Output?.totalLoaded);
        console.log("   Errors:", res.Output?.totalErrors);
        console.log("   Duration:", res.Output?.executionDuration);
    } else {
        console.log("❌ FAILED: Pipeline execution failed");
    }
});
`);

// Test 8: Reload all workflows
console.log("\n--- Test 8: Reload All Workflows ---");
console.log(`
rpcAEP("ReloadAllWorkflows", {}, function(res) {
    console.log("Reload result:", res);
    if (res && res.success) {
        console.log("✅ SUCCESS: Workflows reloaded");
        console.log("   Count:", res.count);
        console.log("   Message:", res.message);
    } else {
        console.log("❌ FAILED: Reload failed");
    }
});
`);

console.log("\n=== Copy and paste commands above to test ===");
console.log("Note: Some tests require Elsa database schema to be created first");
