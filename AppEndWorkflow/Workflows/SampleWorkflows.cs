using Elsa.Extensions;
using Elsa.Workflows;

namespace AppEndWorkflow.Workflows
{
    /// <summary>
    /// Sample workflow definitions for Phase 4 implementation.
    /// All workflows are defined in JSON format and loaded from workspace/workflows/ directory.
    /// 
    /// 4 Sample Workflows:
    /// 1. hello-world.json — Basic verification workflow
    /// 2. scheduled-db-check.json — Timer-triggered database health check
    /// 3. order-approval.json — Multi-step approval workflow with human tasks
    /// 4. data-pipeline.json — ETL-style batch data processing with error handling
    /// 
    /// All workflows are triggered exclusively via RPC bridge through WorkflowServices.
    /// Workflows are auto-discovered by WorkflowDefinitionProvider on application startup.
    /// </summary>
    public static class SampleWorkflows
    {
        public const string HelloWorldId = "hello-world";
        public const string ScheduledDbCheckId = "scheduled-db-check";
        public const string OrderApprovalId = "order-approval";
        public const string DataPipelineId = "data-pipeline";

        /// <summary>
        /// Gets a summary of all sample workflows.
        /// </summary>
        public static string[] GetAllWorkflowIds()
        {
            return new[]
            {
                HelloWorldId,
                ScheduledDbCheckId,
                OrderApprovalId,
                DataPipelineId
            };
        }

        /// <summary>
        /// Gets workflow descriptions for documentation purposes.
        /// </summary>
        public static Dictionary<string, string> GetWorkflowDescriptions()
        {
            return new Dictionary<string, string>
            {
                [HelloWorldId] = "Basic hello world workflow demonstrating simple variable setting, JavaScript execution, and output generation",
                [ScheduledDbCheckId] = "Timer-triggered workflow (every 5 minutes) that checks database connectivity, counts active users, and logs health status",
                [OrderApprovalId] = "Multi-step approval workflow with conditional branching based on order amount, human task creation for manager approval, and database updates",
                [DataPipelineId] = "ETL-style data pipeline with ForEach iteration, Try/Catch error handling, data transformation, validation, loading, and summary reporting"
            };
        }

        /// <summary>
        /// Notes for workflow testing via RPC:
        /// 
        /// 1. HelloWorld:
        ///    rpcAEP("ExecuteWorkflow", { WorkflowId: "hello-world", InputParams: {} }, callback)
        /// 
        /// 2. ScheduledDbCheck:
        ///    - Auto-triggers every 5 minutes when published
        ///    - Can also be manually triggered: rpcAEP("ExecuteWorkflow", { WorkflowId: "scheduled-db-check", InputParams: {} }, callback)
        /// 
        /// 3. OrderApproval:
        ///    rpcAEP("ExecuteWorkflow", { 
        ///        WorkflowId: "order-approval", 
        ///        InputParams: { orderId: "ORD-1042", orderAmount: 2500000, requestedBy: "user1" } 
        ///    }, callback)
        /// 
        /// 4. DataPipeline:
        ///    rpcAEP("ExecuteWorkflow", { 
        ///        WorkflowId: "data-pipeline", 
        ///        InputParams: { sourceTable: "TempOrders", targetTable: "Orders", batchSize: 100 } 
        ///    }, callback)
        /// </summary>
    }
}
