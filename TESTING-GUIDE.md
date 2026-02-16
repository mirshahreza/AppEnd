# Workflow Tasks - Quick Testing Guide

**مقصد:** تست کردن endpoints بعد از deploy

---

## 🔧 پیش‌نیاز‌ها

1. **Database deployed** - `WorkflowTasks-Schema.sql` اجرا شده
2. **Application running** - AppEnd Host شروع شده
3. **Browser console open** - F12 در مرورگر

---

## 📝 مراحل تست

### مرحله 1: درج تسک نمونه

```sql
-- در SQL Server Management Studio یا Azure Data Studio اجرا کنید:

USE AppEndDB
GO

INSERT INTO [dbo].[WorkflowTasks] 
(
    [WorkflowInstanceId], 
    [WorkflowDefinitionId], 
    [Title], 
    [Description],
    [AssignedTo],
    [Priority],
    [Status],
    [DueDate],
    [CreatedAt],
    [CreatedBy],
    [ContextData]
)
VALUES 
(
    NEWID(), 
    'order-approval', 
    'تایید سفارش #12345',
    'سفارشی به مبلغ ۲٫۵ میلیون تومان منتظر تایید است',
    'admin',
    'High',
    'Pending',
    DATEADD(DAY, 3, GETUTCDATE()),
    GETUTCDATE(),
    'system',
    '{"orderId": 12345, "amount": 2500000, "customerName": "احمد علی‌زاده"}'
)
GO

-- نتیجه را ببینید (TaskId را یادداشت کنید):
SELECT TaskId, Title, Status FROM [dbo].[WorkflowTasks]
```

### مرحله 2: تست GetMyWorkflowTasks

**در browser console:**

```javascript
// ساده‌ترین تست
rpcAEP("GetMyWorkflowTasks", {}, console.log);

// با فیلتر و pagination
rpcAEP("GetMyWorkflowTasks", { 
    Status: "Pending",
    Page: 1,
    PageSize: 25
}, (result) => {
    console.log("✅ Response:", result);
    if (result.success) {
        console.log("📊 Total tasks:", result.totalCount);
        console.log("📋 Tasks:", result.tasks);
    }
});
```

**انتظار:** 
```json
{
  "success": true,
  "tasks": [
    {
      "taskId": "...",
      "workflowInstanceId": "...",
      "workflowDefinitionId": "order-approval",
      "title": "تایید سفارش #12345",
      "status": "Pending",
      "priority": "High",
      "createdAt": "2025-01-16T10:30:00Z",
      "contextData": "{\"orderId\": 12345, ...}"
    }
  ],
  "totalCount": 1,
  "totalPages": 1,
  "page": 1,
  "pageSize": 25
}
```

---

### مرحله 3: تست CompleteWorkflowTask

```javascript
// توجه: TaskId را از مرحله 1 جایگزین کنید
const taskIdToComplete = "00000000-0000-0000-0000-000000000001"; // نمونه

rpcAEP("CompleteWorkflowTask", {
    TaskId: taskIdToComplete,
    Outcome: "Approve",
    OutputParams: { comment: "تایید شد - خوب است" }
}, (result) => {
    console.log("✅ Response:", result);
});
```

**انتظار:**
```json
{
  "success": true,
  "message": "Task completed successfully",
  "taskId": "...",
  "outcome": "Approve",
  "completedAt": "2025-01-16T10:35:00Z",
  "completedBy": "admin",
  "bookmarkResumed": false
}
```

---

### مرحله 4: تایید تکمیل تسک

```sql
-- بررسی کنید که تسک تکمیل شده است:
SELECT 
    TaskId, 
    Title, 
    Status, 
    Outcome, 
    Comment, 
    CompletedBy, 
    CompletedAt 
FROM [dbo].[WorkflowTasks] 
WHERE Status = 'Completed'
```

---

## 🧪 تست Vue Components

### WorkflowInbox (My Tasks)
1. روی صفحه "My Workflow Tasks" برو
2. باید قائمه تسک‌های pending نمایش داده شود
3. روی دکمه "Approve" کلیک کن
4. Modal باز شود
5. comment وارد کن و Approve کلیک کن
6. تسک باید Completed شود

### WorkflowInstances
1. روی صفحه "Workflow Instances" برو
2. باید لیست instances نمایش داده شود
3. filter و search کار کنند
4. pagination کار کند

---

## 🐛 Troubleshooting

### خطا: "sp_GetMyWorkflowTasks not found"
```
✅ حل: WorkflowTasks-Schema.sql را کامل اجرا کنید
```

### خطا: "Invalid GUID format"
```
✅ حل: TaskId باید GUID معتبر باشد (XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX)
```

### خطا: "Task not found or already completed"
```
✅ حل: 
1. TaskId را از آخرین SELECT بگیرید
2. مطمئن شوید Status = 'Pending' است
3. یکبار برای هر تسک تقاضا کنید
```

### بدون response در console
```
✅ حل:
1. Network tab چک کنید (F12 → Network)
2. RPC call را ببینید
3. Response status را چک کنید
4. Server logs را ببینید
```

---

## 📊 Performance Check

```javascript
// زمان گیری API call
console.time("GetMyWorkflowTasks");
rpcAEP("GetMyWorkflowTasks", { 
    Status: "Pending",
    Page: 1,
    PageSize: 100
}, () => {
    console.timeEnd("GetMyWorkflowTasks");
});

// انتظار: < 100ms برای 100 task
```

---

## ✅ چک‌لیست قبل از Production

- [ ] Database migrations قبلاً اجرا شده
- [ ] WorkflowTasks table ساخته شده
- [ ] Stored procedures موجود هستند
- [ ] RPC endpoints پاسخ می‌دهند
- [ ] GetMyWorkflowTasks کار می‌کند
- [ ] CompleteWorkflowTask کار می‌کند
- [ ] Vue components load می‌شوند
- [ ] Error logging کار می‌کند
- [ ] Database transactions صحیح هستند

---

## 📞 مشکلات؟

1. **Server logs** را چک کنید:
   ```
   AppEnd Host → Output window → Build
   ```

2. **Database** را بررسی کنید:
   ```sql
   SELECT * FROM [dbo].[WorkflowTasks]
   SELECT * FROM [dbo].[WorkflowInstances]  -- اگر موجود است
   ```

3. **Network** requests را ببینید:
   ```
   F12 → Network tab → Filter by "rpcAEP" یا "api"
   ```

---

**آخرین آپدیت:** 2025-01-16
