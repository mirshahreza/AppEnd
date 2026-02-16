# Phase 7 - Custom Workflow Activities (اختیاری - 4-6 ساعت)

**وضعیت:** آماده برای پیادهسازی  
**سطح دشکلی:** ⭐⭐⭐ (متوسط-سخت)

---

## 🎯 Custom Activities مورد نیاز

هنگام استفاده از موتور Elsa برای کار‌های پیچیده، ممکن است به فعالیت‌های سفارشی نیاز داشته باشید:

### 1. CreateTaskActivity ✅ (آماده)
**هدف:** ایجاد وظیفه جدید در جدول WorkflowTasks

```csharp
namespace AppEndWorkflow.Activities
{
    [Activity(
        Category = "Workflow Tasks",
        DisplayName = "Create Task",
        Description = "Create a new workflow task"
    )]
    public class CreateTaskActivity : Activity
    {
        [Input(DisplayName = "Title")]
        public Input<string> Title { get; set; } = new();

        [Input(DisplayName = "Description")]
        public Input<string> Description { get; set; } = new();

        [Input(DisplayName = "Assigned To")]
        public Input<string> AssignedTo { get; set; } = new();

        [Input(DisplayName = "Priority")]
        public Input<string> Priority { get; set; } = new();

        [Output(DisplayName = "Task ID")]
        public Output<string> TaskId { get; set; } = new();

        protected override async ValueTask ExecuteAsync(ActivityExecutionContext context)
        {
            try
            {
                using var dbIO = AppEndDbIO.DbIO.Instance();
                
                var taskId = Guid.NewGuid();
                var sql = @"
                    INSERT INTO [dbo].[WorkflowTasks] 
                    (TaskId, WorkflowInstanceId, Title, Description, AssignedTo, Priority, Status, CreatedAt, CreatedBy)
                    VALUES (@TaskId, @InstanceId, @Title, @Desc, @AssignedTo, @Priority, 'Pending', GETUTCDATE(), @CreatedBy)
                ";

                var parameters = new List<System.Data.Common.DbParameter>
                {
                    dbIO.CreateParameter("@TaskId", "UNIQUEIDENTIFIER", null, taskId),
                    dbIO.CreateParameter("@InstanceId", "UNIQUEIDENTIFIER", null, context.WorkflowInstanceId),
                    dbIO.CreateParameter("@Title", "NVARCHAR", 255, Title.Get(context) ?? ""),
                    dbIO.CreateParameter("@Desc", "NVARCHAR", -1, Description.Get(context)),
                    dbIO.CreateParameter("@AssignedTo", "NVARCHAR", 100, AssignedTo.Get(context) ?? ""),
                    dbIO.CreateParameter("@Priority", "NVARCHAR", 50, Priority.Get(context) ?? "Normal"),
                    dbIO.CreateParameter("@CreatedBy", "NVARCHAR", 100, "system")
                };

                dbIO.Execute(sql, parameters);
                TaskId.Set(context, taskId.ToString());

                LogMan.LogConsole($"Task created: {taskId}");
            }
            catch (Exception ex)
            {
                LogMan.LogError($"CreateTaskActivity failed: {ex.Message}");
                throw;
            }
        }
    }
}
```

### 2. AssignToUserActivity ✅
**هدف:** تخصیص وظیفه به کاربر خاص

```csharp
[Activity(
    Category = "Workflow Tasks",
    DisplayName = "Assign Task to User",
    Description = "Assign task to a specific user"
)]
public class AssignToUserActivity : Activity
{
    [Input(DisplayName = "Task ID")]
    public Input<string> TaskId { get; set; } = new();

    [Input(DisplayName = "User ID")]
    public Input<string> UserId { get; set; } = new();

    [Output(DisplayName = "Success")]
    public Output<bool> Success { get; set; } = new();

    protected override async ValueTask ExecuteAsync(ActivityExecutionContext context)
    {
        try
        {
            using var dbIO = AppEndDbIO.DbIO.Instance();
            
            var sql = @"
                UPDATE [dbo].[WorkflowTasks]
                SET AssignedTo = @UserId, UpdatedAt = GETUTCDATE()
                WHERE TaskId = @TaskId
            ";

            var parameters = new List<System.Data.Common.DbParameter>
            {
                dbIO.CreateParameter("@TaskId", "UNIQUEIDENTIFIER", null, new Guid(TaskId.Get(context))),
                dbIO.CreateParameter("@UserId", "NVARCHAR", 100, UserId.Get(context) ?? "")
            };

            dbIO.Execute(sql, parameters);
            Success.Set(context, true);

            LogMan.LogConsole($"Task assigned to user: {UserId.Get(context)}");
        }
        catch (Exception ex)
        {
            LogMan.LogError($"AssignToUserActivity failed: {ex.Message}");
            Success.Set(context, false);
        }
    }
}
```

### 3. WaitForApprovalActivity ⏳
**هدف:** انتظار تایید از کاربر (Bookmark)

```csharp
[Activity(
    Category = "Workflow Tasks",
    DisplayName = "Wait for Approval",
    Description = "Wait for user approval with timeout"
)]
public class WaitForApprovalActivity : Activity
{
    [Input(DisplayName = "Task ID")]
    public Input<string> TaskId { get; set; } = new();

    [Input(DisplayName = "Timeout (hours)")]
    public Input<int> TimeoutHours { get; set; } = new() { Value = 24 };

    [Output(DisplayName = "Outcome")]
    public Output<string> Outcome { get; set; } = new();

    protected override async ValueTask ExecuteAsync(ActivityExecutionContext context)
    {
        var taskId = TaskId.Get(context);
        var bookmarkName = $"approval-{taskId}";

        context.CreateBookmark(bookmarkName, async (activityExecutionContext, input) =>
        {
            var outcome = input as string ?? "Reject";
            Outcome.Set(context, outcome);
            LogMan.LogConsole($"Approval received: {outcome}");
        });
    }
}
```

### 4. SendEmailActivity 📧
**هدف:** ارسال ایمیل برای اطلاع رسانی

```csharp
[Activity(
    Category = "Workflow Tasks",
    DisplayName = "Send Email",
    Description = "Send email notification"
)]
public class SendEmailActivity : Activity
{
    [Input(DisplayName = "Email To")]
    public Input<string> EmailTo { get; set; } = new();

    [Input(DisplayName = "Subject")]
    public Input<string> Subject { get; set; } = new();

    [Input(DisplayName = "Body")]
    public Input<string> Body { get; set; } = new();

    [Output(DisplayName = "Success")]
    public Output<bool> Success { get; set; } = new();

    protected override async ValueTask ExecuteAsync(ActivityExecutionContext context)
    {
        try
        {
            var email = EmailTo.Get(context) ?? "";
            var subject = Subject.Get(context) ?? "";
            var body = Body.Get(context) ?? "";

            // TODO: Send email using your email service
            // Example with SMTP:
            // using var client = new SmtpClient("smtp.gmail.com", 587);
            // client.EnableSsl = true;
            // client.Send(new MailMessage("noreply@domain.com", email) { Subject = subject, Body = body });

            Success.Set(context, true);
            LogMan.LogConsole($"Email sent to: {email}");
        }
        catch (Exception ex)
        {
            LogMan.LogError($"SendEmailActivity failed: {ex.Message}");
            Success.Set(context, false);
        }
    }
}
```

### 5. RunSqlQueryActivity 🗄️
**هدف:** اجرای Query SQL دلخواه

```csharp
[Activity(
    Category = "Database",
    DisplayName = "Run SQL Query",
    Description = "Execute SQL query and return results"
)]
public class RunSqlQueryActivity : Activity
{
    [Input(DisplayName = "SQL Query")]
    public Input<string> SqlQuery { get; set; } = new();

    [Output(DisplayName = "Result")]
    public Output<object?> Result { get; set; } = new();

    protected override async ValueTask ExecuteAsync(ActivityExecutionContext context)
    {
        try
        {
            var sql = SqlQuery.Get(context) ?? "";

            using var dbIO = AppEndDbIO.DbIO.Instance();
            var result = dbIO.ToDataSet(sql);

            Result.Set(context, result);
            LogMan.LogConsole($"SQL query executed");
        }
        catch (Exception ex)
        {
            LogMan.LogError($"RunSqlQueryActivity failed: {ex.Message}");
            Result.Set(context, null);
        }
    }
}
```

---

## 📦 نحوه استفاده

### مثال 1: Workflow with CreateTaskActivity

```json
{
  "id": "order-approval-workflow",
  "name": "Order Approval Workflow",
  "activities": [
    {
      "id": "create-task",
      "type": "CreateTask",
      "properties": {
        "title": {
          "expression": {
            "text": "تایید سفارش #${orderId}"
          }
        },
        "description": {
          "expression": {
            "text": "سفارشی به مبلغ ${amount} تومان"
          }
        },
        "assignedTo": "admin"
      }
    },
    {
      "id": "wait-approval",
      "type": "WaitForApproval",
      "properties": {
        "taskId": {
          "expression": {
            "text": "${activities.CreateTask.taskId}"
          }
        },
        "timeoutHours": 24
      }
    }
  ]
}
```

---

## 🔧 نصب Custom Activities

### Step 1: Create Activity Class
```csharp
// File: AppEndWorkflow/Activities/CreateTaskActivity.cs
[Activity(...)]
public class CreateTaskActivity : Activity { ... }
```

### Step 2: Register in ElsaSetup
```csharp
services.AddElsa(elsa =>
{
    // ... existing config ...
    elsa.AddActivitiesFrom(typeof(CreateTaskActivity).Assembly);
});
```

### Step 3: استفاده در Workflow Definition
```json
{
  "activities": [
    {
      "type": "CreateTask",
      "properties": { ... }
    }
  ]
}
```

---

## 📊 Workflow Example

```
┌─────────────────────────┐
│   Start (HTTP Trigger)  │
└────────────┬────────────┘
             │
             ▼
    ┌────────────────┐
    │  Create Task   │
    └────────┬───────┘
             │
             ▼
    ┌────────────────┐
    │ Assign to User │
    └────────┬───────┘
             │
             ▼
    ┌────────────────┐
    │ Wait Approval  │ ◄── Bookmark
    └────────┬───────┘
             │
      ┌──────┴──────┐
      │ Outcome     │
      ├─────┬───────┤
    Done  Reject  Wait
```

---

## 🎯 خلاصه

### Activities به ترتیب اولویت:
1. ⭐⭐⭐ **CreateTaskActivity** - اساسی
2. ⭐⭐⭐ **WaitForApprovalActivity** - اساسی (Bookmark)
3. ⭐⭐ **AssignToUserActivity** - کمکی
4. ⭐ **SendEmailActivity** - اختیاری
5. ⭐ **RunSqlQueryActivity** - اختیاری

### زمان پیادهسازی:
- CreateTaskActivity: 30 دقیقه
- WaitForApprovalActivity: 45 دقیقه (Bookmark logic)
- AssignToUserActivity: 20 دقیقه
- SendEmailActivity: 30 دقیقه
- RunSqlQueryActivity: 20 دقیقه
- **کل: 2-3 ساعت**

---

**توجه:** این مرحله اختیاری است. بعد از تست موفق مرحله 1-3، می‌توانید این فعالیت‌ها را پیادهسازی کنید.
