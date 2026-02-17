# Workflow API Testing Guide

## 🧪 Testing Strategy

This guide provides comprehensive API testing procedures for the AppEnd Workflow Engine REST endpoints.

---

## 📋 Quick Start

### Option 1: Use Postman Collection (Recommended for Manual Testing)

1. **Import Collection:**
   - Open Postman
   - Click "Import" → Select `WorkflowAPI.postman_collection.json`
   - Collection will be imported with all 15+ endpoints organized by category

2. **Configure Variables:**
   - Set `base_url` to your server URL (default: `https://localhost:5001`)
   - Set `definition_id` after creating a workflow
   - Set `instance_id` after executing a workflow

3. **Test Workflow:**
   - Execute requests sequentially following the folder structure
   - Results will be displayed in the Response tab

### Option 2: Use WorkflowApiTests Class (Automated Testing)

```csharp
// In your test or startup code:
var httpClient = new HttpClient();
var logger = serviceProvider.GetRequiredService<ILogger<WorkflowApiTests>>();
var apiTests = new WorkflowApiTests(httpClient, "https://localhost:5001", logger);
await apiTests.RunAllApiTests();
```

---

## 🔗 API Endpoints Overview

### Health & Dashboard (2 endpoints)
```
✓ GET  /api/workflows/health              - Server health check
✓ GET  /api/workflows/dashboard           - Dashboard metrics aggregation
```

### Workflow Definitions (5 endpoints)
```
✓ GET    /api/workflows/definitions       - List all definitions
✓ POST   /api/workflows/definitions       - Create new definition
✓ GET    /api/workflows/definitions/{id}  - Get definition details
✓ PUT    /api/workflows/definitions/{id}  - Update definition
✓ POST   /api/workflows/definitions/{id}/publish - Publish definition
```

### Workflow Instances (8 endpoints)
```
✓ GET    /api/workflows/instances         - List instances (with filtering)
✓ POST   /api/workflows/execute/{id}      - Execute workflow
✓ GET    /api/workflows/instances/{id}    - Get instance details
✓ GET    /api/workflows/instances/{id}/timeline    - Execution timeline
✓ GET    /api/workflows/instances/{id}/variables   - Get variables
✓ GET    /api/workflows/instances/{id}/faults      - Get error info
✓ POST   /api/workflows/instances/{id}/suspend     - Suspend execution
✓ POST   /api/workflows/instances/{id}/resume      - Resume execution
```

### Instance Control (3 endpoints - included above)
```
✓ POST   /api/workflows/instances/{id}/suspend    - Pause execution
✓ POST   /api/workflows/instances/{id}/resume     - Continue execution
✓ POST   /api/workflows/instances/{id}/terminate  - Stop execution
```

**Total: 18 main endpoints**

---

## 📊 Response Format

All endpoints follow the AppEnd standard format:

```json
{
  "success": true,
  "data": {
    "id": "workflow-123",
    "name": "Sample Workflow",
    "status": "Running"
  },
  "message": "Operation completed successfully"
}
```

### Error Response:
```json
{
  "success": false,
  "data": null,
  "message": "Error description"
}
```

---

## 🧬 Test Scenarios

### Scenario 1: Create and Execute Workflow

**Steps:**
1. `GET /api/workflows/definitions` - Verify existing definitions
2. `POST /api/workflows/definitions` - Create new workflow
3. `POST /api/workflows/definitions/{id}/publish` - Publish workflow
4. `POST /api/workflows/execute/{id}` - Execute workflow
5. `GET /api/workflows/instances/{instanceId}` - Check execution status

**Expected Result:**
- Definition created with unique ID
- Workflow published successfully
- Instance created and starts executing
- Status transitions from Pending → Running → Completed

---

### Scenario 2: Monitor Workflow Execution

**Steps:**
1. `POST /api/workflows/execute/{id}` - Start workflow
2. `GET /api/workflows/instances/{instanceId}` - Check status
3. `GET /api/workflows/instances/{instanceId}/timeline` - View execution steps
4. `GET /api/workflows/instances/{instanceId}/variables` - Inspect variables
5. `GET /api/workflows/instances/{instanceId}/faults` - Check for errors

**Expected Result:**
- Instance status updates
- Timeline shows each activity execution
- Variables contain workflow data
- No faults if execution successful

---

### Scenario 3: Control Workflow Execution

**Steps:**
1. `POST /api/workflows/execute/{id}` - Start workflow
2. `POST /api/workflows/instances/{instanceId}/suspend` - Pause execution
3. Wait and verify status is "Suspended"
4. `POST /api/workflows/instances/{instanceId}/resume` - Continue execution
5. Verify status changes back to "Running"

**Expected Result:**
- Workflow pauses at suspension point
- Can be resumed later
- Maintains state during suspension

---

### Scenario 4: Terminate Workflow

**Steps:**
1. `POST /api/workflows/execute/{id}` - Start workflow
2. `POST /api/workflows/instances/{instanceId}/terminate` - Stop execution
3. `GET /api/workflows/instances/{instanceId}` - Verify status

**Expected Result:**
- Workflow stops immediately
- Status becomes "Terminated"
- Cannot resume terminated workflow

---

## 🔍 Detailed Endpoint Testing

### 1. Health Status
```
GET /api/workflows/health
```

**Response:**
```json
{
  "success": true,
  "data": {
    "isHealthy": true,
    "timestamp": "2024-01-15T10:30:00Z"
  },
  "message": "Workflow engine is operational"
}
```

---

### 2. Dashboard Metrics
```
GET /api/workflows/dashboard
```

**Response:**
```json
{
  "success": true,
  "data": {
    "summary": {
      "totalWorkflows": 5,
      "runningInstances": 2,
      "completedToday": 12,
      "successRate": 94.5
    },
    "statusStatistics": [
      { "status": "Running", "count": 2 },
      { "status": "Completed", "count": 45 },
      { "status": "Failed", "count": 3 }
    ],
    "performanceMetrics": {
      "avgExecutionTime": 5.2,
      "maxExecutionTime": 15.8
    }
  },
  "message": "Dashboard data retrieved"
}
```

---

### 3. Create Workflow Definition
```
POST /api/workflows/definitions
Content-Type: application/json

{
  "name": "ProcessOrder",
  "description": "Order processing workflow",
  "activities": [
    {
      "type": "Database",
      "name": "ValidateOrder",
      "config": { "queryName": "ValidateOrder" }
    },
    {
      "type": "Notification",
      "name": "SendConfirmation",
      "config": { "channel": "Email" }
    }
  ]
}
```

**Response:**
```json
{
  "success": true,
  "data": {
    "id": "wf-550e8400-e29b-41d4-a716-446655440000",
    "name": "ProcessOrder",
    "version": 1,
    "status": "Draft"
  },
  "message": "Workflow definition created"
}
```

---

### 4. Execute Workflow
```
POST /api/workflows/execute/{definitionId}
Content-Type: application/json

{
  "correlationId": "corr-12345",
  "variables": {
    "orderId": "ORD-001",
    "userId": "user-123"
  }
}
```

**Response:**
```json
{
  "success": true,
  "data": {
    "instanceId": "inst-550e8400-e29b-41d4-a716-446655440001",
    "status": "Running",
    "startedAt": "2024-01-15T10:30:00Z"
  },
  "message": "Workflow execution started"
}
```

---

### 5. Get Execution Timeline
```
GET /api/workflows/instances/{instanceId}/timeline
```

**Response:**
```json
{
  "success": true,
  "data": {
    "timeline": [
      {
        "timestamp": "2024-01-15T10:30:00Z",
        "activityName": "ValidateOrder",
        "status": "Completed",
        "duration": 250
      },
      {
        "timestamp": "2024-01-15T10:30:01Z",
        "activityName": "SendConfirmation",
        "status": "Completed",
        "duration": 1500
      }
    ]
  },
  "message": "Timeline retrieved"
}
```

---

## 🧪 cURL Examples

### Quick Testing via Command Line

```bash
# Health check
curl -X GET https://localhost:5001/api/workflows/health

# Get dashboard
curl -X GET https://localhost:5001/api/workflows/dashboard

# List all definitions
curl -X GET https://localhost:5001/api/workflows/definitions

# Create definition
curl -X POST https://localhost:5001/api/workflows/definitions \
  -H "Content-Type: application/json" \
  -d '{
    "name": "TestFlow",
    "description": "Test workflow",
    "activities": []
  }'

# Execute workflow (replace WF_ID with actual definition ID)
curl -X POST https://localhost:5001/api/workflows/execute/WF_ID \
  -H "Content-Type: application/json" \
  -d '{
    "correlationId": "test-123",
    "variables": {}
  }'

# Get instance details
curl -X GET https://localhost:5001/api/workflows/instances/INST_ID

# Suspend instance
curl -X POST https://localhost:5001/api/workflows/instances/INST_ID/suspend
```

---

## ✅ Verification Checklist

- [ ] Health endpoint returns 200 OK
- [ ] Dashboard returns aggregated metrics
- [ ] Can create workflow definition
- [ ] Created definition appears in list
- [ ] Can execute a published workflow
- [ ] Instance is created with unique ID
- [ ] Timeline shows activity progression
- [ ] Variables are properly stored and retrieved
- [ ] Can suspend running instance
- [ ] Can resume suspended instance
- [ ] Can terminate instance
- [ ] Error handling returns appropriate messages

---

## 🐛 Troubleshooting

### Issue: 404 Not Found
**Solution:** Verify endpoint URL and that workflow/instance IDs are correct

### Issue: 400 Bad Request
**Solution:** Check JSON payload format and required fields in request body

### Issue: 500 Internal Server Error
**Solution:** Check server logs for detailed error information

### Issue: Endpoints return empty data
**Solution:** Ensure database is initialized and connected

### Issue: Workflow not executing
**Solution:** Verify workflow is published before execution attempt

---

## 📈 Performance Metrics

Expected response times (ms):
- Health check: < 50ms
- Dashboard: < 500ms
- Get definition: < 100ms
- Create definition: < 200ms
- Execute workflow: < 300ms
- Get instance: < 150ms
- Get timeline: < 200ms
- Suspend/Resume: < 150ms

---

## 🔐 Security Testing

1. **Authentication:** Verify requests require proper authentication
2. **Authorization:** Verify users can only access their own workflows
3. **Input Validation:** Test invalid JSON and malformed requests
4. **SQL Injection:** Verify parameters are properly escaped
5. **CORS:** Verify cross-origin requests follow policy

---

## 📚 Integration with Other Systems

### Event Publishing
Workflows publish events that can be consumed by:
- Real-time dashboards
- External systems (via webhooks)
- Audit logs

### Database Logging
All workflow executions are logged in:
- WorkflowInstances table
- WorkflowExecutionHistory table
- WorkflowFaults table

### Monitoring & Alerts
Integrate with monitoring systems:
- Application Insights
- CloudWatch
- Custom dashboards

---

## 🎓 Next Steps

1. **Run Integration Tests:** Execute `WorkflowApiTests.RunAllApiTests()`
2. **Load Testing:** Use tools like k6 or JMeter for performance testing
3. **UI Testing:** Test Vue components with the API
4. **End-to-End Testing:** Complete workflow scenarios with real data

---

**API Testing Status:** ✅ Ready for Testing
**Collection Version:** 1.0
**Last Updated:** 2024
