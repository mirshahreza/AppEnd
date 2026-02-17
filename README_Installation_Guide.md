# Elsa 3.0 - AppEnd Integration - Installation Guide

## فاز 1: Foundation - بخش Database

این راهنما برای نصب و راه‌اندازی دیتابیس Elsa 3.0 است.

---

## مرحله 1: پیش‌نیازها

### سیستم‌های مورد نیاز:
- **SQL Server**: 2019 یا بالاتر
- **.NET**: 10.0
- **AppEnd**: آخرین نسخه develop

### اطلاعات دیتابیس:
- **Server**: SQL Server شما
- **Database**: دیتابیسی که AppEnd استفاده می‌کند
- **Schema**: `dbo` (به‌طور پیش‌فرض)

---

## مرحله 2: اسکریپت‌های SQL

سه اسکریپت SQL برای شما آماده‌شده است:

### 1️⃣ `01_Elsa_Schema_Foundation.sql`
**هدف**: ایجاد تمام جداول

**مسیر**: `Database/Scripts/01_Elsa_Schema_Foundation.sql`

**محتوا**:
- 14 جدول
- Foreign Keys
- Indexes
- Constraints

**نحوه اجرا**:
1. SQL Server Management Studio را باز کنید
2. به دیتابیس AppEnd متصل شوید
3. فایل را باز کنید و اجرا کنید

```sql
-- یا از خط فرمان:
sqlcmd -S <SERVER> -d <DATABASE> -i "01_Elsa_Schema_Foundation.sql"
```

---

### 2️⃣ `02_Elsa_Package_Install.sql`
**هدف**: نصب Elsa بهعنوان Package AppEnd

**مسیر**: `Database/Scripts/02_Elsa_Package_Install.sql`

**توضیح**:
- این اسکریپت همان جداول را از طریق `sp_executesql` ایجاد می‌کند
- برای استفاده در Package Management AppEnd

---

### 3️⃣ `03_Elsa_Package_Uninstall.sql`
**هدف**: حذف Elsa از دیتابیس

**مسیر**: `Database/Scripts/03_Elsa_Package_Uninstall.sql`

**توجه**:
- ابتدا جداول وابسته حذف می‌شوند
- سپس جداول اصلی

---

## مرحله 3: نصب جداول

### گزینه 1: دستی (Manual) - برای تست

```bash
# SQL Server Management Studio
# 1. Database را انتخاب کنید
# 2. فایل 01_Elsa_Schema_Foundation.sql را باز کنید
# 3. F5 یا Ctrl+Shift+E را بزنید
```

### گزینه 2: خودکار (Script) - در Production

```bash
# PowerShell
$server = "YOUR_SQL_SERVER"
$database = "YOUR_DATABASE"
$scriptPath = "Database/Scripts/01_Elsa_Schema_Foundation.sql"

sqlcmd -S $server -d $database -i $scriptPath
```

---

## مرحله 4: تأیید نصب

بعد از اجرای اسکریپت، تأیید کنید که جداول ایجاد شده‌اند:

```sql
-- SELECT تمام جداول Elsa
SELECT TABLE_NAME 
FROM INFORMATION_SCHEMA.TABLES 
WHERE TABLE_NAME LIKE 'Elsa%'
ORDER BY TABLE_NAME;
```

**نتیجه مورد انتظار**: 14 جدول
```
ElsaActivityExecutions
ElsaApprovalInstances
ElsaAuditLogs
ElsaBookmarks
ElsaExecutionContexts
ElsaTriggeredWorkflows
ElsaVariableInstances
ElsaWorkflowDefinitions
ElsaWorkflowDefinitionVersions
ElsaWorkflowEvents
ElsaWorkflowExecutionLogs
ElsaWorkflowInstances
ElsaWorkflowSuspensions
ElsaWorkflowTriggers
```

---

## مرحله 5: بررسی Indexes

```sql
-- بررسی indexes
SELECT 
    OBJECT_NAME(i.object_id) AS TableName,
    i.name AS IndexName,
    i.type_desc AS IndexType
FROM sys.indexes i
WHERE OBJECT_NAME(i.object_id) LIKE 'Elsa%'
ORDER BY OBJECT_NAME(i.object_id), i.name;
```

---

## مرحله 6: بررسی Foreign Keys

```sql
-- بررسی FK‌ها
SELECT 
    OBJECT_NAME(fk.parent_object_id) AS TableName,
    fk.name AS ForeignKeyName,
    OBJECT_NAME(fk.referenced_object_id) AS ReferencedTable
FROM sys.foreign_keys fk
WHERE OBJECT_NAME(fk.parent_object_id) LIKE 'Elsa%'
ORDER BY OBJECT_NAME(fk.parent_object_id);
```

---

## مرحله 7: تست Database

یک رکورد تست وارد کنید:

```sql
-- تست: ایجاد یک Workflow Definition
INSERT INTO [dbo].[ElsaWorkflowDefinitions] 
    (Id, Name, DisplayName, Version, IsPublished, CreatedBy, TenantId)
VALUES 
    ('test-workflow-001', 'TestWorkflow', 'Test Workflow', 1, 0, 'admin', '1');

-- تأیید
SELECT * FROM [dbo].[ElsaWorkflowDefinitions];
```

بعد از تأیید، این رکورد را حذف کنید:

```sql
DELETE FROM [dbo].[ElsaWorkflowDefinitions] WHERE Id = 'test-workflow-001';
```

---

## مرحله 8: Backup و Documentation

### Backup کنید:
```bash
# SQL Server Backup
BACKUP DATABASE [YOUR_DATABASE] 
TO DISK = 'C:\Backups\AppEnd_Elsa_Schema.bak'
WITH INIT, COMP, STATS=10;
```

### Documentation را ذخیره کنید:
- `Database/Scripts/Elsa_Schema_Documentation.md` را برای مرجع نگاه دارید

---

## حل مشکلات

### خطا: "Table already exists"
```sql
-- جداول قدیمی را حذف کنید
EXEC sp_executesql N'DROP TABLE [dbo].[ElsaWorkflowDefinitions]'
```

### خطا: "Cannot create foreign key"
- اطمینان حاصل کنید که جدول parent قبل‌تر ایجاد شده است
- ترتیب اجرا مهم است

### خطا: "Permission denied"
- اطمینان حاصل کنید که کاربر `db_owner` دارد

---

## مرحله بعد

بعد از نصب جداول:

1. ✅ **Database Schema** - تمام (Phase 1.1)
2. ⏳ **Elsa NuGet Packages** - بعدی
3. ⏳ **Service Registration** - سپس
4. ⏳ **IWorkflowService Interface** - سپس
5. ⏳ **سرویس‌های اضافی** - بعد‌تر

---

## نکات مهم

| نکته | توضیح |
|------|-------|
| **Multi-Tenancy** | تمام جداول `TenantId` دارند |
| **Soft Delete** | از `IsDeleted` استفاده می‌کند |
| **Audit** | `ElsaAuditLogs` تمام تغییرات را نگاه می‌دارد |
| **Indexes** | برای Performance بهینه‌شده‌اند |
| **JSON Columns** | برای فلکسیبیلیتی |

---

## مرجع‌ها

- **Schema Documentation**: `Database/Scripts/Elsa_Schema_Documentation.md`
- **Plan Overview**: `Elsa-AppEnd-Plan.md`
- **Phase 1 Details**: `Elsa-AppEnd-Phase1-Foundation.md`

---

## راه‌های تماس

اگر سؤال دارید:
- بخش Support را بررسی کنید
- Issue در GitHub ایجاد کنید
- مستندات را مجدداً بخوانید

---

## نسخه

- **Elsa**: 3.0
- **SQL Server**: 2019+
- **.NET**: 10.0
- **AppEnd**: develop

**آخرین بروزرسانی**: [تاریخ]
