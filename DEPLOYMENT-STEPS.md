# 🛠️ استقرار و تست - مرحله به مرحله

**هدف:** استقرار اسکیمای SQL و تست آن  
**زمان:** ۳۰ تا ۴۵ دقیقه  
**سطح سختی:** آسان ⭐⭐

---

## مرحله ۱: اتصال به SQL Server

### گزینه A: SQL Server Management Studio (SSMS)
```
1. SSMS را باز کنید
2. به AppEndDB وصل شوید
3. New Query باز کنید (Ctrl+N)
```

### گزینه B: Azure Data Studio
```
1. Azure Data Studio را باز کنید
2. AppEndDB را انتخاب کنید
3. New Query بسازید
```

### گزینه C: خط فرمان
```powershell
sqlcmd -S <ServerName> -d AppEndDB -E
```

---

## مرحله ۲: استقرار Schema

```sql
-- ✅ برای اطمینان: اول بکاپ بگیرید
BACKUP DATABASE [AppEndDB] 
TO DISK = 'C:\Backups\AppEndDB_Before_WorkflowTasks.bak'
GO

-- ✅ حالا اسکیما را اجرا کنید
-- کل محتوای WorkflowTasks-Schema.sql را کپی و اینجا جای‌گذاری کنید:

USE [AppEndDB]
GO

-- حذف جدول اگر وجود داشت (برای نصب تمیز)
IF OBJECT_ID(N'[dbo].[WorkflowTasks]', N'U') IS NOT NULL
    DROP TABLE [dbo].[WorkflowTasks];
GO

-- ... (بقیه SQL را کپی کنید)
```

---

## مرحله ۳: بررسی صحت

```sql
-- ✅ جدول ساخته شده؟
SELECT * FROM INFORMATION_SCHEMA.TABLES 
WHERE TABLE_NAME = 'WorkflowTasks'
GO

-- ✅ ستون‌ها درست هستند؟
SELECT COLUMN_NAME, DATA_TYPE 
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'WorkflowTasks'
GO

-- ✅ ایندکس‌ها ساخته شده‌اند؟
SELECT name FROM sys.indexes 
WHERE object_id = OBJECT_ID('WorkflowTasks')
GO

-- ✅ رویه‌های ذخیره‌شده وجود دارند؟
SELECT name FROM sys.procedures 
WHERE name LIKE 'sp_Get%' OR name LIKE 'sp_Complete%'
GO

-- ✅ ویوها ساخته شده‌اند؟
SELECT name FROM sys.views 
WHERE name LIKE 'vw_%'
GO
```

---

## مرحله ۴: وارد کردن داده تست

```sql
-- داده تست:
DECLARE @TaskId1 UNIQUEIDENTIFIER = NEWID()
DECLARE @TaskId2 UNIQUEIDENTIFIER = NEWID()
DECLARE @InstanceId UNIQUEIDENTIFIER = NEWID()

INSERT INTO [dbo].[WorkflowTasks] 
(
    [TaskId],
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
    @TaskId1,
    @InstanceId, 
    'order-approval', 
    'سفارش #12345 را تایید کنید',
    'سفارش ₹2,500,000 منتظر تایید است',
    'admin',
    'High',
    'Pending',
    DATEADD(DAY, 3, GETUTCDATE()),
    GETUTCDATE(),
    'system',
    '{"orderId": 12345, "amount": 2500000, "customer": "علی"}'
),
(
    @TaskId2,
    @InstanceId, 
    'order-approval', 
    'سفارش #12346 را تایید کنید',
    'سفارش ₹1,500,000 منتظر تایید است',
    'admin',
    'Normal',
    'Pending',
    DATEADD(DAY, 2, GETUTCDATE()),
    GETUTCDATE(),
    'system',
    '{"orderId": 12346, "amount": 1500000, "customer": "احمد"}'
)
GO

-- ✅ بررسی
SELECT COUNT(*) AS PendingCount FROM [dbo].[WorkflowTasks]
GO
```

---

## مرحله ۵: تست رویه‌های ذخیره‌شده

### تست ۱: GetMyWorkflowTasks
```sql
-- همه کارهای Pending
EXEC [dbo].[sp_GetMyWorkflowTasks] 
    @UserId = 'admin',
    @Status = 'Pending',
    @Page = 1,
    @PageSize = 25
GO

-- انتظار: ۲ ردیف + ۱ ردیف تعداد کل
```

### تست ۲: CompleteWorkflowTask
```sql
-- اول TaskId را بگیرید:
DECLARE @TaskIdToComplete UNIQUEIDENTIFIER
SELECT TOP 1 @TaskIdToComplete = TaskId 
FROM [dbo].[WorkflowTasks] 
WHERE Status = 'Pending'

-- حالا تکمیل کنید:
EXEC [dbo].[sp_CompleteWorkflowTask]
    @TaskId = @TaskIdToComplete,
    @UserId = 'admin',
    @Outcome = 'Approved',
    @Comment = 'خوب است. تایید شد'
GO

-- ✅ بررسی
SELECT TaskId, Status, Outcome, Comment, CompletedBy 
FROM [dbo].[WorkflowTasks] 
WHERE Status = 'Completed'
GO
```

---

## مرحله ۶: Build پروژه

```bash
cd C:\Workspace\Projects\AppEnd

# Build
 dotnet build AppEnd.sln

# اگر خطا آمد:
# 1. بررسی کنید خطای نحوی نباشد
# 2. بسته‌ها را بازیابی کنید:
 dotnet restore AppEnd.sln

# دوباره build
 dotnet build AppEnd.sln
```

---

## مرحله ۷: تست API در مرورگر

```javascript
// این را در Console مرورگر paste کنید (F12)

// تست ۱: GetMyWorkflowTasks
console.log("🔍 Testing GetMyWorkflowTasks...");
rpcAEP("GetMyWorkflowTasks", { 
    Status: "Pending",
    Page: 1,
    PageSize: 25
}, (result) => {
    console.log("✅ Response:", result);
    if (result.success) {
        console.log(`📊 Total: ${result.totalCount} tasks`);
        console.log("📋 Tasks:", result.tasks);
    }
});

// تست ۲: CompleteWorkflowTask (اگر TaskId دارید)
setTimeout(() => {
    console.log("\n🔍 Testing CompleteWorkflowTask...");
    rpcAEP("CompleteWorkflowTask", {
        TaskId: "TASK-GUID-HERE",  // ↑ TaskId خودتان را بگذارید
        Outcome: "Approved",
        OutputParams: { comment: "بسیار خوب. تایید شد" }
    }, (result) => {
        console.log("✅ Response:", result);
    });
}, 2000);
```

---

## مرحله ۸: چک‌لیست صحت

```
Database:
☑️  جدول WorkflowTasks وجود دارد
☑️  ستون‌ها درست هستند
☑️  ایندکس‌ها ساخته شده‌اند
☑️  ویوها ساخته شده‌اند
☑️  رویه‌های ذخیره‌شده موجودند

Test Data:
☑️  2 وظیفه تست وارد شده
☑️  وضعیت Pending دارند
☑️  ContextData JSON موجود است

Stored Procedures:
☑️  sp_GetMyWorkflowTasks کار می‌کند
☑️  sp_CompleteWorkflowTask کار می‌کند
☑️  تعداد کل درست است

API:
☑️  GetMyWorkflowTasks پاسخ می‌دهد
☑️  CompleteWorkflowTask پاسخ می‌دهد
☑️  Error handling کار می‌کند

Build:
☑️  dotnet build موفق شد
☑️  خطا ندارد
☑️  هشدار ندارد
```

---

## 🐛 رفع اشکال

### مشکل: "Invalid object name 'WorkflowTasks'"
```
راه‌حل: اسکریپت SQL را کامل اجرا کنید، نه فقط بخشی از آن
```

### مشکل: "Stored procedure not found"
```
راه‌حل: بعد از هر GO، اسکریپت را ادامه دهید
     دستور CREATE PROCEDURE باید کامل اجرا شود
```

### مشکل: "RAISERROR in stored procedure"
```
راه‌حل: وضعیت Task باید Pending باشد
     یا قبلاً Completed نشده باشد
```

### مشکل: "RPC not responding"
```
راه‌حل:
1. اپلیکیشن را ری‌استارت کنید
2. کش مرورگر را پاک کنید (Ctrl+Shift+Delete)
3. خطای Console را ببینید (F12)
```

---

## ✅ موفقیت یعنی:

- ✅ اشیای SQL ساخته شدند
- ✅ داده تست وارد شد
- ✅ رویه‌های ذخیره‌شده کار می‌کنند
- ✅ Build موفق است
- ✅ API پاسخ می‌دهد
- ✅ کوئری‌ها درست کار می‌کنند

---

**مرحله بعدی:** تنظیمات (Configuration Setup)  
**زمان:** ۳۰ دقیقه

---

وقتی این مراحل کامل شد خبر بدهید. ✅
