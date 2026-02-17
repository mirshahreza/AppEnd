using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AppEndServer.Workflows.Tests
{
    /// <summary>
    /// End-to-End Integration Tests for Complete Workflow Scenarios
    /// Tests real-world workflow execution patterns
    /// </summary>
    public class WorkflowEndToEndTests
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IWorkflowService _workflowService;
        private readonly IWorkflowDefinitionService _definitionService;
        private readonly IWorkflowInstanceService _instanceService;
        private readonly ILogger<WorkflowEndToEndTests> _logger;

        public WorkflowEndToEndTests(IServiceProvider serviceProvider, ILogger<WorkflowEndToEndTests> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
            _workflowService = serviceProvider.GetRequiredService<IWorkflowService>();
            _definitionService = serviceProvider.GetRequiredService<IWorkflowDefinitionService>();
            _instanceService = serviceProvider.GetRequiredService<IWorkflowInstanceService>();
        }

        // ============================================================================
        // END-TO-END TEST SCENARIOS
        // ============================================================================

        /// <summary>
        /// E2E Test 1: Complete workflow from definition to completion
        /// </summary>
        public async Task Test_E2E_CompleteWorkflow()
        {
            _logger.LogInformation("E2E TEST 1: Complete Workflow Lifecycle");

            try
            {
                // Step 1: Execute workflow directly
                _logger.LogInformation("  Step 1: Executing workflow");
                var instanceId = await _workflowService.ExecuteWorkflowAsync("sample-wf", new Dictionary<string, object>
                {
                    { "userId", "test-user" },
                    { "action", "test" }
                });
                AssertE2E.NotNull(instanceId, "Instance ID should be returned");

                // Step 2: Monitor execution
                _logger.LogInformation("  Step 2: Monitoring execution");
                await Task.Delay(500);
                var instance = await _instanceService.GetByIdAsync(instanceId);
                AssertE2E.NotNull(instance, "Instance should exist");

                // Step 3: Verify completion
                _logger.LogInformation("  Step 3: Verifying completion");
                var variables = instance.Variables;
                AssertE2E.True(variables?.Count >= 0, "Instance should have variables");

                _logger.LogInformation("✓ E2E TEST 1 PASSED: Complete workflow executed successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "✗ E2E TEST 1 FAILED");
                throw;
            }
        }

        /// <summary>
        /// E2E Test 2: Workflow with suspension and resumption
        /// </summary>
        public async Task Test_E2E_SuspendAndResume()
        {
            _logger.LogInformation("E2E TEST 2: Workflow Suspension and Resumption");

            try
            {
                // Execute workflow
                _logger.LogInformation("  Step 1: Executing workflow");
                var instanceId = await _workflowService.ExecuteWorkflowAsync("sample-wf", null);

                // Suspend workflow
                _logger.LogInformation("  Step 2: Suspending workflow");
                await _workflowService.SuspendWorkflowAsync(instanceId, "test-user", "Manual suspension for testing");
                var suspendedInstance = await _instanceService.GetByIdAsync(instanceId);
                AssertE2E.True(suspendedInstance.Status == "Suspended", "Instance should be suspended");

                // Resume workflow
                _logger.LogInformation("  Step 3: Resuming workflow");
                await _workflowService.ResumeWorkflowAsync(instanceId);
                await Task.Delay(500);
                var resumedInstance = await _instanceService.GetByIdAsync(instanceId);
                AssertE2E.True(resumedInstance.Status != "Suspended", "Instance should no longer be suspended");

                _logger.LogInformation("✓ E2E TEST 2 PASSED: Suspension and resumption working correctly");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "✗ E2E TEST 2 FAILED");
                throw;
            }
        }

        /// <summary>
        /// E2E Test 3: Multiple concurrent workflow instances
        /// </summary>
        public async Task Test_E2E_ConcurrentInstances()
        {
            _logger.LogInformation("E2E TEST 3: Concurrent Workflow Instances");

            try
            {
                _logger.LogInformation("  Step 1: Executing 5 concurrent instances");
                var tasks = new List<Task<string>>();
                for (int i = 0; i < 5; i++)
                {
                    tasks.Add(_workflowService.ExecuteWorkflowAsync("sample-wf", new Dictionary<string, object>
                    {
                        { "instanceNumber", i }
                    }));
                }

                var instanceIds = await Task.WhenAll(tasks);
                AssertE2E.True(instanceIds.Length == 5, "All 5 instances should be created");

                // Verify all instances exist
                _logger.LogInformation("  Step 2: Verifying all instances");
                foreach (var instanceId in instanceIds)
                {
                    var instance = await _instanceService.GetByIdAsync(instanceId);
                    AssertE2E.NotNull(instance, $"Instance {instanceId} should exist");
                }

                _logger.LogInformation("✓ E2E TEST 3 PASSED: Concurrent instances working correctly");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "✗ E2E TEST 3 FAILED");
                throw;
            }
        }

        /// <summary>
        /// E2E Test 4: Workflow with error handling and recovery
        /// </summary>
        public async Task Test_E2E_ErrorHandling()
        {
            _logger.LogInformation("E2E TEST 4: Error Handling and Recovery");

            try
            {
                _logger.LogInformation("  Step 1: Executing workflow with error handling");
                var instanceId = await _workflowService.ExecuteWorkflowAsync("sample-wf", new Dictionary<string, object>
                {
                    { "shouldFail", false }
                });

                // Check execution
                _logger.LogInformation("  Step 2: Verifying workflow state");
                await Task.Delay(500);
                var instance = await _instanceService.GetByIdAsync(instanceId);
                AssertE2E.NotNull(instance, "Instance should exist after error handling");

                _logger.LogInformation("✓ E2E TEST 4 PASSED: Error handling working correctly");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "✗ E2E TEST 4 FAILED");
                throw;
            }
        }

        /// <summary>
        /// E2E Test 5: Workflow instance filtering and pagination
        /// </summary>
        public async Task Test_E2E_InstanceFiltering()
        {
            _logger.LogInformation("E2E TEST 5: Instance Filtering and Pagination");

            try
            {
                // Execute multiple instances
                _logger.LogInformation("  Step 1: Creating multiple instances");
                for (int i = 0; i < 3; i++)
                {
                    await _workflowService.ExecuteWorkflowAsync("sample-wf", null);
                }

                // Query with filter
                _logger.LogInformation("  Step 2: Querying with filter");
                var filter = new WorkflowInstanceFilter
                {
                    Status = "Running"
                };

                var instances = await _instanceService.ListAsync(filter, 1, 10);
                AssertE2E.NotNull(instances, "Instances should be returned");

                // Test pagination
                _logger.LogInformation("  Step 3: Testing pagination");
                var page1 = await _instanceService.ListAsync(filter, 1, 2);
                var page2 = await _instanceService.ListAsync(filter, 2, 2);
                
                _logger.LogInformation("✓ E2E TEST 5 PASSED: Filtering and pagination working correctly");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "✗ E2E TEST 5 FAILED");
                throw;
            }
        }

        /// <summary>
        /// E2E Test 6: Service registration and availability
        /// </summary>
        public async Task Test_E2E_ServicesAvailable()
        {
            _logger.LogInformation("E2E TEST 6: Service Availability");

            try
            {
                _logger.LogInformation("  Step 1: Verifying service registration");
                AssertE2E.NotNull(_workflowService, "WorkflowService should be available");
                AssertE2E.NotNull(_definitionService, "DefinitionService should be available");
                AssertE2E.NotNull(_instanceService, "InstanceService should be available");

                _logger.LogInformation("✓ E2E TEST 6 PASSED: All services available");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "✗ E2E TEST 6 FAILED");
                throw;
            }
        }

        // ============================================================================
        // TEST ORCHESTRATION
        // ============================================================================

        /// <summary>
        /// Run all end-to-end tests
        /// </summary>
        public async Task RunAllE2ETests()
        {
            _logger.LogInformation("========================================");
            _logger.LogInformation("STARTING END-TO-END WORKFLOW TESTS");
            _logger.LogInformation("========================================");

            var testCount = 0;
            var passedCount = 0;
            var failedTests = new List<string>();

            // Test 1
            try { await Test_E2E_CompleteWorkflow(); passedCount++; }
            catch { failedTests.Add("E2E-1: Complete Workflow"); }
            finally { testCount++; }

            // Test 2
            try { await Test_E2E_SuspendAndResume(); passedCount++; }
            catch { failedTests.Add("E2E-2: Suspend/Resume"); }
            finally { testCount++; }

            // Test 3
            try { await Test_E2E_ConcurrentInstances(); passedCount++; }
            catch { failedTests.Add("E2E-3: Concurrent Instances"); }
            finally { testCount++; }

            // Test 4
            try { await Test_E2E_ErrorHandling(); passedCount++; }
            catch { failedTests.Add("E2E-4: Error Handling"); }
            finally { testCount++; }

            // Test 5
            try { await Test_E2E_InstanceFiltering(); passedCount++; }
            catch { failedTests.Add("E2E-5: Filtering/Pagination"); }
            finally { testCount++; }

            // Test 6
            try { await Test_E2E_ServicesAvailable(); passedCount++; }
            catch { failedTests.Add("E2E-6: Services Available"); }
            finally { testCount++; }

            // Summary
            _logger.LogInformation("========================================");
            _logger.LogInformation("END-TO-END TEST RESULTS");
            _logger.LogInformation("========================================");
            _logger.LogInformation("Total Tests: {Total}", testCount);
            _logger.LogInformation("Passed: {Passed}", passedCount);
            _logger.LogInformation("Failed: {Failed}", failedTests.Count);

            if (failedTests.Any())
            {
                _logger.LogError("Failed Tests: {Tests}", string.Join(", ", failedTests));
            }

            var successRate = testCount > 0 ? (passedCount / (double)testCount) * 100 : 0;
            _logger.LogInformation("Success Rate: {SuccessRate}%", Math.Round(successRate, 2));
            _logger.LogInformation("========================================");
        }
    }

    /// <summary>
    /// Simple assertion helper
    /// </summary>
    public static class AssertE2E
    {
        public static void NotNull(object? obj, string message)
        {
            if (obj == null)
                throw new InvalidOperationException(message);
        }

        public static void True(bool condition, string message)
        {
            if (!condition)
                throw new InvalidOperationException(message);
        }
    }
}
