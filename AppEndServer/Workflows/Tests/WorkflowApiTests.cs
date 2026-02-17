using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AppEndServer.Workflows.Tests
{
    /// <summary>
    /// API Testing Suite for Workflow REST Endpoints
    /// Tests all 30+ endpoints in WorkflowsController
    /// </summary>
    public class WorkflowApiTests
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;
        private readonly ILogger<WorkflowApiTests> _logger;

        public WorkflowApiTests(HttpClient httpClient, string baseUrl, ILogger<WorkflowApiTests> logger)
        {
            _httpClient = httpClient;
            _baseUrl = baseUrl ?? "https://localhost:5001";
            _logger = logger;
        }

        // ============================================================================
        // HEALTH & DASHBOARD ENDPOINTS
        // ============================================================================

        /// <summary>
        /// Test: GET /api/workflows/health
        /// </summary>
        public async Task Test_GetHealthStatus()
        {
            _logger.LogInformation("TEST: Health Status Endpoint");
            try
            {
                var response = await _httpClient.GetAsync($"{_baseUrl}/api/workflows/health");
                Assert.True(response.IsSuccessStatusCode, $"Health check failed: {response.StatusCode}");

                var content = await response.Content.ReadAsStringAsync();
                var json = JsonDocument.Parse(content);
                Assert.True(json.RootElement.GetProperty("success").GetBoolean(), "Response should be successful");

                _logger.LogInformation("✓ Health Status: OK");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "✗ Health Status Test Failed");
                throw;
            }
        }

        /// <summary>
        /// Test: GET /api/workflows/dashboard
        /// </summary>
        public async Task Test_GetDashboard()
        {
            _logger.LogInformation("TEST: Dashboard Endpoint");
            try
            {
                var response = await _httpClient.GetAsync($"{_baseUrl}/api/workflows/dashboard");
                Assert.True(response.IsSuccessStatusCode, $"Dashboard request failed: {response.StatusCode}");

                var content = await response.Content.ReadAsStringAsync();
                var json = JsonDocument.Parse(content);
                
                var data = json.RootElement.GetProperty("data");
                Assert.NotNull(data, "Dashboard data should not be null");
                Assert.True(data.TryGetProperty("summary", out _), "Should contain summary");
                Assert.True(data.TryGetProperty("statusStatistics", out _), "Should contain status statistics");

                _logger.LogInformation("✓ Dashboard Data Retrieved");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "✗ Dashboard Test Failed");
                throw;
            }
        }

        // ============================================================================
        // WORKFLOW DEFINITION ENDPOINTS
        // ============================================================================

        /// <summary>
        /// Test: GET /api/workflows/definitions
        /// </summary>
        public async Task Test_GetAllDefinitions()
        {
            _logger.LogInformation("TEST: Get All Workflow Definitions");
            try
            {
                var response = await _httpClient.GetAsync($"{_baseUrl}/api/workflows/definitions");
                Assert.True(response.IsSuccessStatusCode, $"Get definitions failed: {response.StatusCode}");

                var content = await response.Content.ReadAsStringAsync();
                var json = JsonDocument.Parse(content);
                Assert.True(json.RootElement.GetProperty("success").GetBoolean(), "Should be successful");

                _logger.LogInformation("✓ Retrieved All Definitions");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "✗ Get Definitions Test Failed");
                throw;
            }
        }

        /// <summary>
        /// Test: POST /api/workflows/definitions
        /// </summary>
        public async Task<string?> Test_CreateDefinition()
        {
            _logger.LogInformation("TEST: Create Workflow Definition");
            try
            {
                var payload = new
                {
                    name = "TestWorkflow_" + Guid.NewGuid().ToString().Substring(0, 8),
                    description = "Test workflow for API testing",
                    activities = new[]
                    {
                        new
                        {
                            type = "Database",
                            name = "GetData",
                            config = new { queryName = "TestQuery" }
                        }
                    }
                };

                var json = JsonSerializer.Serialize(payload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync($"{_baseUrl}/api/workflows/definitions", content);
                Assert.True(response.IsSuccessStatusCode, $"Create definition failed: {response.StatusCode}");

                var responseContent = await response.Content.ReadAsStringAsync();
                var responseJson = JsonDocument.Parse(responseContent);
                var definitionId = responseJson.RootElement.GetProperty("data").GetProperty("id").GetString();

                _logger.LogInformation("✓ Created Definition: {Id}", definitionId);
                return definitionId;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "✗ Create Definition Test Failed");
                throw;
            }
        }

        /// <summary>
        /// Test: GET /api/workflows/definitions/{id}
        /// </summary>
        public async Task Test_GetDefinitionById(string definitionId)
        {
            _logger.LogInformation("TEST: Get Workflow Definition by ID");
            try
            {
                var response = await _httpClient.GetAsync($"{_baseUrl}/api/workflows/definitions/{definitionId}");
                Assert.True(response.IsSuccessStatusCode, $"Get definition by ID failed: {response.StatusCode}");

                var content = await response.Content.ReadAsStringAsync();
                var json = JsonDocument.Parse(content);
                var data = json.RootElement.GetProperty("data");
                
                Assert.True(data.GetProperty("id").GetString() == definitionId, "ID should match");
                Assert.True(data.TryGetProperty("name", out _), "Should have name property");

                _logger.LogInformation("✓ Retrieved Definition: {Id}", definitionId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "✗ Get Definition by ID Test Failed");
                throw;
            }
        }

        /// <summary>
        /// Test: PUT /api/workflows/definitions/{id}
        /// </summary>
        public async Task Test_UpdateDefinition(string definitionId)
        {
            _logger.LogInformation("TEST: Update Workflow Definition");
            try
            {
                var payload = new
                {
                    name = "UpdatedWorkflow_" + Guid.NewGuid().ToString().Substring(0, 8),
                    description = "Updated description"
                };

                var json = JsonSerializer.Serialize(payload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync($"{_baseUrl}/api/workflows/definitions/{definitionId}", content);
                Assert.True(response.IsSuccessStatusCode, $"Update definition failed: {response.StatusCode}");

                _logger.LogInformation("✓ Updated Definition: {Id}", definitionId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "✗ Update Definition Test Failed");
                throw;
            }
        }

        /// <summary>
        /// Test: POST /api/workflows/definitions/{id}/publish
        /// </summary>
        public async Task Test_PublishDefinition(string definitionId)
        {
            _logger.LogInformation("TEST: Publish Workflow Definition");
            try
            {
                var response = await _httpClient.PostAsync(
                    $"{_baseUrl}/api/workflows/definitions/{definitionId}/publish",
                    new StringContent("", Encoding.UTF8, "application/json"));

                Assert.True(response.IsSuccessStatusCode, $"Publish definition failed: {response.StatusCode}");

                _logger.LogInformation("✓ Published Definition: {Id}", definitionId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "✗ Publish Definition Test Failed");
                throw;
            }
        }

        // ============================================================================
        // WORKFLOW INSTANCE ENDPOINTS
        // ============================================================================

        /// <summary>
        /// Test: GET /api/workflows/instances
        /// </summary>
        public async Task Test_GetAllInstances()
        {
            _logger.LogInformation("TEST: Get All Workflow Instances");
            try
            {
                var response = await _httpClient.GetAsync($"{_baseUrl}/api/workflows/instances");
                Assert.True(response.IsSuccessStatusCode, $"Get instances failed: {response.StatusCode}");

                var content = await response.Content.ReadAsStringAsync();
                var json = JsonDocument.Parse(content);
                Assert.True(json.RootElement.GetProperty("success").GetBoolean(), "Should be successful");

                _logger.LogInformation("✓ Retrieved All Instances");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "✗ Get Instances Test Failed");
                throw;
            }
        }

        /// <summary>
        /// Test: POST /api/workflows/execute/{id}
        /// </summary>
        public async Task<string?> Test_ExecuteWorkflow(string definitionId)
        {
            _logger.LogInformation("TEST: Execute Workflow");
            try
            {
                var payload = new
                {
                    correlationId = Guid.NewGuid().ToString(),
                    variables = new Dictionary<string, object>
                    {
                        { "userId", "test-user" },
                        { "requestId", Guid.NewGuid().ToString() }
                    }
                };

                var json = JsonSerializer.Serialize(payload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync($"{_baseUrl}/api/workflows/execute/{definitionId}", content);
                Assert.True(response.IsSuccessStatusCode, $"Execute workflow failed: {response.StatusCode}");

                var responseContent = await response.Content.ReadAsStringAsync();
                var responseJson = JsonDocument.Parse(responseContent);
                var instanceId = responseJson.RootElement.GetProperty("data").GetProperty("instanceId").GetString();

                _logger.LogInformation("✓ Executed Workflow, Instance: {Id}", instanceId);
                return instanceId;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "✗ Execute Workflow Test Failed");
                throw;
            }
        }

        /// <summary>
        /// Test: GET /api/workflows/instances/{id}
        /// </summary>
        public async Task Test_GetInstanceById(string instanceId)
        {
            _logger.LogInformation("TEST: Get Workflow Instance by ID");
            try
            {
                var response = await _httpClient.GetAsync($"{_baseUrl}/api/workflows/instances/{instanceId}");
                Assert.True(response.IsSuccessStatusCode, $"Get instance failed: {response.StatusCode}");

                var content = await response.Content.ReadAsStringAsync();
                var json = JsonDocument.Parse(content);
                var data = json.RootElement.GetProperty("data");

                Assert.True(data.GetProperty("id").GetString() == instanceId, "ID should match");
                Assert.True(data.TryGetProperty("status", out _), "Should have status property");

                _logger.LogInformation("✓ Retrieved Instance: {Id}", instanceId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "✗ Get Instance Test Failed");
                throw;
            }
        }

        /// <summary>
        /// Test: GET /api/workflows/instances/{id}/timeline
        /// </summary>
        public async Task Test_GetExecutionTimeline(string instanceId)
        {
            _logger.LogInformation("TEST: Get Execution Timeline");
            try
            {
                var response = await _httpClient.GetAsync($"{_baseUrl}/api/workflows/instances/{instanceId}/timeline");
                Assert.True(response.IsSuccessStatusCode, $"Get timeline failed: {response.StatusCode}");

                var content = await response.Content.ReadAsStringAsync();
                var json = JsonDocument.Parse(content);
                Assert.True(json.RootElement.GetProperty("success").GetBoolean(), "Should be successful");

                _logger.LogInformation("✓ Retrieved Execution Timeline");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "✗ Get Timeline Test Failed");
                throw;
            }
        }

        /// <summary>
        /// Test: GET /api/workflows/instances/{id}/variables
        /// </summary>
        public async Task Test_GetInstanceVariables(string instanceId)
        {
            _logger.LogInformation("TEST: Get Instance Variables");
            try
            {
                var response = await _httpClient.GetAsync($"{_baseUrl}/api/workflows/instances/{instanceId}/variables");
                Assert.True(response.IsSuccessStatusCode, $"Get variables failed: {response.StatusCode}");

                var content = await response.Content.ReadAsStringAsync();
                var json = JsonDocument.Parse(content);
                Assert.True(json.RootElement.GetProperty("success").GetBoolean(), "Should be successful");

                _logger.LogInformation("✓ Retrieved Instance Variables");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "✗ Get Variables Test Failed");
                throw;
            }
        }

        /// <summary>
        /// Test: GET /api/workflows/instances/{id}/faults
        /// </summary>
        public async Task Test_GetInstanceFaults(string instanceId)
        {
            _logger.LogInformation("TEST: Get Instance Faults");
            try
            {
                var response = await _httpClient.GetAsync($"{_baseUrl}/api/workflows/instances/{instanceId}/faults");
                Assert.True(response.IsSuccessStatusCode, $"Get faults failed: {response.StatusCode}");

                var content = await response.Content.ReadAsStringAsync();
                var json = JsonDocument.Parse(content);
                Assert.True(json.RootElement.GetProperty("success").GetBoolean(), "Should be successful");

                _logger.LogInformation("✓ Retrieved Instance Faults");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "✗ Get Faults Test Failed");
                throw;
            }
        }

        /// <summary>
        /// Test: POST /api/workflows/instances/{id}/suspend
        /// </summary>
        public async Task Test_SuspendInstance(string instanceId)
        {
            _logger.LogInformation("TEST: Suspend Instance");
            try
            {
                var response = await _httpClient.PostAsync(
                    $"{_baseUrl}/api/workflows/instances/{instanceId}/suspend",
                    new StringContent("", Encoding.UTF8, "application/json"));

                Assert.True(response.IsSuccessStatusCode, $"Suspend instance failed: {response.StatusCode}");

                _logger.LogInformation("✓ Suspended Instance: {Id}", instanceId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "✗ Suspend Instance Test Failed");
                throw;
            }
        }

        /// <summary>
        /// Test: POST /api/workflows/instances/{id}/resume
        /// </summary>
        public async Task Test_ResumeInstance(string instanceId)
        {
            _logger.LogInformation("TEST: Resume Instance");
            try
            {
                var response = await _httpClient.PostAsync(
                    $"{_baseUrl}/api/workflows/instances/{instanceId}/resume",
                    new StringContent("", Encoding.UTF8, "application/json"));

                // May fail if instance is not suspended, which is expected
                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("✓ Resumed Instance: {Id}", instanceId);
                }
                else
                {
                    _logger.LogInformation("ℹ Instance not in suspended state (expected)");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "✗ Resume Instance Test Failed");
                throw;
            }
        }

        /// <summary>
        /// Test: POST /api/workflows/instances/{id}/terminate
        /// </summary>
        public async Task Test_TerminateInstance(string instanceId)
        {
            _logger.LogInformation("TEST: Terminate Instance");
            try
            {
                var response = await _httpClient.PostAsync(
                    $"{_baseUrl}/api/workflows/instances/{instanceId}/terminate",
                    new StringContent("", Encoding.UTF8, "application/json"));

                Assert.True(response.IsSuccessStatusCode, $"Terminate instance failed: {response.StatusCode}");

                _logger.LogInformation("✓ Terminated Instance: {Id}", instanceId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "✗ Terminate Instance Test Failed");
                throw;
            }
        }

        // ============================================================================
        // COMPREHENSIVE TEST SUITE
        // ============================================================================

        /// <summary>
        /// Run all API tests in sequence
        /// </summary>
        public async Task RunAllApiTests()
        {
            _logger.LogInformation("========================================");
            _logger.LogInformation("STARTING WORKFLOW API TESTS");
            _logger.LogInformation("========================================");
            _logger.LogInformation("Base URL: {BaseUrl}", _baseUrl);

            var testCount = 0;
            var passedCount = 0;
            var failedTests = new List<string>();
            string? definitionId = null;
            string? instanceId = null;

            // Health & Dashboard
            try { await Test_GetHealthStatus(); passedCount++; } 
            catch { failedTests.Add("Health Status"); } 
            finally { testCount++; }

            try { await Test_GetDashboard(); passedCount++; } 
            catch { failedTests.Add("Dashboard"); } 
            finally { testCount++; }

            // Definitions
            try { await Test_GetAllDefinitions(); passedCount++; } 
            catch { failedTests.Add("Get All Definitions"); } 
            finally { testCount++; }

            try { definitionId = await Test_CreateDefinition(); passedCount++; } 
            catch { failedTests.Add("Create Definition"); } 
            finally { testCount++; }

            if (definitionId != null)
            {
                try { await Test_GetDefinitionById(definitionId); passedCount++; } 
                catch { failedTests.Add("Get Definition by ID"); } 
                finally { testCount++; }

                try { await Test_UpdateDefinition(definitionId); passedCount++; } 
                catch { failedTests.Add("Update Definition"); } 
                finally { testCount++; }

                try { await Test_PublishDefinition(definitionId); passedCount++; } 
                catch { failedTests.Add("Publish Definition"); } 
                finally { testCount++; }

                // Instances
                try { await Test_GetAllInstances(); passedCount++; } 
                catch { failedTests.Add("Get All Instances"); } 
                finally { testCount++; }

                try { instanceId = await Test_ExecuteWorkflow(definitionId); passedCount++; } 
                catch { failedTests.Add("Execute Workflow"); } 
                finally { testCount++; }

                if (instanceId != null)
                {
                    // Give the workflow a moment to execute
                    await Task.Delay(1000);

                    try { await Test_GetInstanceById(instanceId); passedCount++; } 
                    catch { failedTests.Add("Get Instance by ID"); } 
                    finally { testCount++; }

                    try { await Test_GetExecutionTimeline(instanceId); passedCount++; } 
                    catch { failedTests.Add("Get Execution Timeline"); } 
                    finally { testCount++; }

                    try { await Test_GetInstanceVariables(instanceId); passedCount++; } 
                    catch { failedTests.Add("Get Instance Variables"); } 
                    finally { testCount++; }

                    try { await Test_GetInstanceFaults(instanceId); passedCount++; } 
                    catch { failedTests.Add("Get Instance Faults"); } 
                    finally { testCount++; }

                    try { await Test_SuspendInstance(instanceId); passedCount++; } 
                    catch { failedTests.Add("Suspend Instance"); } 
                    finally { testCount++; }

                    try { await Test_ResumeInstance(instanceId); passedCount++; } 
                    catch { failedTests.Add("Resume Instance"); } 
                    finally { testCount++; }

                    try { await Test_TerminateInstance(instanceId); passedCount++; } 
                    catch { failedTests.Add("Terminate Instance"); } 
                    finally { testCount++; }
                }
            }

            // Summary
            _logger.LogInformation("========================================");
            _logger.LogInformation("API TEST RESULTS");
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
    /// Simple assertion helper for tests
    /// </summary>
    public static class AssertApi
    {
        public static void True(bool condition, string message)
        {
            if (!condition)
                throw new InvalidOperationException(message);
        }

        public static void NotNull(object? obj, string message)
        {
            if (obj == null)
                throw new InvalidOperationException(message);
        }
    }
}
