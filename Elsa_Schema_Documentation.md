# Elsa 3.0 Database Schema Documentation

## نمای کلی

این سند شرح جامع از تمام جداول دیتابیس Elsa 3.0 را می‌دهد.

---

## 1. ElsaWorkflowDefinitions
**هدف**: ذخیره تعریف‌های Workflow

### ستون‌ها:
- `Id` (PK): شناسه یکتای تعریف
- `Name`: نام workflow (UNIQUE)
- `DisplayName`: نام نمایشی
- `Description`: توضیح
- `Version`: شماره نسخه فعلی
- `PublishedVersion`: آخرین نسخه منتشر شده
- `IsPublished`: وضعیت انتشار
- `IsPaused`: آیا workflow متوقف است؟
- `DefinitionFormat`: فرمت تعریف ('Json', 'CodeFirst')
- `CreatedAt/UpdatedAt`: زمان ایجاد/بروزرسانی
- `CreatedBy/UpdatedBy`: کاربر ایجادکننده/بروزرسانی‌کننده
- `TenantId`: شناسه تأجیرکننده (برای multi-tenancy)
- `IsDeleted`: نشانگر حذف منطقی

### اندیس‌ها:
- Index بر روی `Name`, `TenantId`, `IsPublished`, `CreatedAt`

---

## 2. ElsaWorkflowDefinitionVersions
**هدف**: مدیریت نسخه‌های مختلف یک workflow

### ستون‌ها:
- `Id` (PK): شناسه نسخه
- `WorkflowDefinitionId` (FK): ارجاع به تعریف
- `Version`: شماره نسخه
- `Data`: محتوای JSON تعریف
- `IsPublished`: آیا این نسخه منتشر شده؟
- `PublishedAt`: تاریخ انتشار
- `CreatedAt`: تاریخ ایجاد
- `CreatedBy`: سازنده
- `CustomAttributes`: ویژگی‌های سفارشی
- `Hash`: Hash محتوا (برای تشخیص تغییرات)

### اندیس‌ها:
- Index بر روی `WorkflowDefinitionId`
- Index composite بر روی `(WorkflowDefinitionId, Version)`

---

## 3. ElsaWorkflowInstances
**هدف**: ذخیره نمونه‌های (instances) Workflow در حال اجرا

### ستون‌ها:
- `Id` (PK): شناسه نمونه
- `WorkflowDefinitionId` (FK): ارجاع به تعریف
- `WorkflowDefinitionVersionId` (FK): نسخه استفاده شده
- `CorrelationId`: شناسه همبستگی (برای ردیابی)
- `Status`: وضعیت ('Running', 'Suspended', 'Completed', 'Faulted', 'Cancelled')
- `SubStatus`: وضعیت فرعی
- `CreatedAt/StartedAt/ResumedAt/CompletedAt/FaultedAt/CancelledAt`: مراحل زمانی
- `ExceptionMessage`: پیام خرابی (اگر وجود داشته باشد)
- `Variables`: متغیرهای workflow (JSON)
- `CustomAttributes`: ویژگی‌های سفارشی
- `TenantId`: شناسه تأجیرکننده
- `UserId`: کاربر شروع کننده
- `Input`: ورودی اولیه (JSON)
- `Output`: خروجی نهایی (JSON)
- `Fault`: جزئیات خرابی (JSON)

### اندیس‌ها:
- Index بر روی `WorkflowDefinitionId`, `Status`, `CorrelationId`, `TenantId`, `CreatedAt`

### استفاده در AppEnd:
برای ردیابی workflow‌های در حال اجرا و تاریخچه کامل آن‌ها

---

## 4. ElsaActivityExecutions
**هدف**: ردیابی اجرای هر Activity در workflow

### ستون‌ها:
- `Id` (PK): شناسه
- `WorkflowInstanceId` (FK): ارجاع به نمونه workflow
- `ActivityId`: شناسه Activity
- `ActivityType`: نوع Activity (مثل 'Delay', 'UserTask', 'Decision')
- `DisplayName`: نام نمایشی
- `Status`: وضعیت اجرا ('Pending', 'Running', 'Completed', 'Faulted', 'Suspended')
- `CreatedAt/StartedAt/CompletedAt/FaultedAt`: زمان‌های مختلف
- `Outputs`: خروجی Activity (JSON)
- `ExceptionMessage`: پیام خرابی
- `ExecutionLog`: لاگ اجرا
- `Sequence`: ترتیب اجرا

### اندیس‌ها:
- Index بر روی `WorkflowInstanceId`, `Status`, `CreatedAt`

---

## 5. ElsaBookmarks
**هدف**: نقاط توقف و Resume کردن workflow

### ستون‌ها:
- `Id` (PK): شناسه
- `WorkflowInstanceId` (FK): ارجاع به نمونه
- `ActivityId`: Activity که bookmark دارد
- `ActivityType`: نوع Activity
- `Name`: نام bookmark
- `Hash`: Hash برای یافتن سریع
- `Payload`: داده‌های اضافی (JSON)
- `Metadata`: متادیتا
- `CreatedAt`: زمان ایجاد
- `ResumableFrom`: آیا قابل Resume است؟
- `IsProcessed`: آیا پردازش شده؟
- `ProcessedAt`: زمان پردازش

### اندیس‌ها:
- Index بر روی `WorkflowInstanceId`, `ActivityId`, `Hash`, `IsProcessed`

### مثال استفاده:
برای Activity‌های wait-based یا user approval

---

## 6. ElsaWorkflowExecutionLogs
**هدف**: تمام رویدادهای اجرای workflow

### ستون‌ها:
- `Id` (PK): شناسه
- `WorkflowInstanceId` (FK): ارجاع به نمونه
- `ActivityExecutionId` (FK): ارجاع به اجرای Activity
- `EventName`: نام رویداد ('WorkflowStarted', 'ActivityCompleted', 'WorkflowFaulted')
- `Message`: پیام
- `Level`: سطح ('Information', 'Warning', 'Error')
- `Timestamp`: زمان رویداد
- `Data`: داده‌های JSON
- `UserId`: کاربری که رویداد را ایجاد کرد
- `CorrelationId`: شناسه همبستگی

### اندیس‌ها:
- Index بر روی `WorkflowInstanceId`, `EventName`, `Timestamp`, `Level`

---

## 7. ElsaVariableInstances
**هدف**: متغیرهای استفاده شده در workflow

### ستون‌ها:
- `Id` (PK): شناسه
- `WorkflowInstanceId` (FK): ارجاع به نمونه
- `Name`: نام متغیر
- `Value`: مقدار (JSON)
- `Type`: نوع داده ('int', 'string', 'object', etc.)
- `IsVolatile`: آیا قابل ذخیره‌سازی است؟
- `CreatedAt/UpdatedAt`: زمان‌های مختلف

### اندیس‌ها:
- Index بر روی `WorkflowInstanceId`, `(WorkflowInstanceId, Name)`

---

## 8. ElsaTriggeredWorkflows
**هدف**: Workflow‌های که به‌صورت خودکار trigger می‌شوند

### ستون‌ها:
- `Id` (PK): شناسه
- `WorkflowDefinitionId` (FK): ارجاع به تعریف
- `TriggerType`: نوع trigger ('Timer', 'Event', 'Webhook', 'Signal')
- `TriggerName`: نام trigger
- `IsActive`: آیا فعال است؟
- `Payload`: کانفیگ trigger (JSON)
- `CreatedAt/UpdatedAt`: زمان‌های مختلف

### اندیس‌ها:
- Index بر روی `WorkflowDefinitionId`, `TriggerType`, `IsActive`

### مثال:
- Workflow هر روز صبح‌ اجرا شود (Timer)
- Workflow زمانی شروع شود که یک رویداد مخصوص رخ دهد (Event)

---

## 9. ElsaWorkflowEvents
**هدف**: رویدادهای workflow برای سیستم

### ستون‌ها:
- `Id` (PK): شناسه
- `WorkflowInstanceId` (FK): ارجاع به نمونه
- `EventName`: نام رویداد
- `Source`: منبع (Activity یا System)
- `Payload`: داده‌های رویداد (JSON)
- `CreatedAt`: زمان
- `ProcessedAt`: زمان پردازش
- `IsProcessed`: آیا پردازش شده؟

### اندیس‌ها:
- Index بر روی `WorkflowInstanceId`, `EventName`, `IsProcessed`

---

## 10. ElsaWorkflowTriggers
**هدف**: Trigger‌های Activities در workflow

### ستون‌ها:
- `Id` (PK): شناسه
- `WorkflowDefinitionId` (FK): ارجاع به تعریف
- `ActivityId`: شناسه Activity
- `ActivityType`: نوع Activity
- `Name`: نام trigger
- `Hash`: Hash برای یافتن
- `Payload`: کانفیگ (JSON)
- `CreatedAt`: زمان

### اندیس‌ها:
- Index بر روی `WorkflowDefinitionId`, `ActivityType`

---

## 11. ElsaExecutionContexts
**هدف**: زمینه‌های اجرا برای پیچیدگی‌های بیشتر

### ستون‌ها:
- `Id` (PK): شناسه
- `WorkflowInstanceId` (FK): ارجاع به نمونه
- `ActivityExecutionId` (FK): ارجاع به Activity
- `ContextType`: نوع ('Workflow', 'Activity', 'Decision')
- `Data`: داده‌های JSON
- `CreatedAt`: زمان
- `ExpiresAt`: تاریخ انقضا

### اندیس‌ها:
- Index بر روی `WorkflowInstanceId`

---

## 12. ElsaApprovalInstances
**هدف**: نمونه‌های تایید برای فرآیندهای Human Approval

### ستون‌ها:
- `Id` (PK): شناسه
- `WorkflowInstanceId` (FK): ارجاع به نمونه
- `ActivityExecutionId` (FK): ارجاع به Activity
- `RequestedBy`: کاربری که درخواست داد
- `RequestedFor`: شخص مورد درخواست
- `Title`: عنوان درخواست
- `Description`: توضیح
- `Status`: وضعیت ('Pending', 'Approved', 'Rejected', 'Escalated')
- `ApprovedBy`: تایید کننده
- `ApprovedAt`: زمان تایید
- `RejectionReason`: دلیل رد
- `DueDate`: مهلت
- `ReminderSent`: آیا یادآوری ارسال شد؟
- `CreatedAt/UpdatedAt`: زمان‌های مختلف

### اندیس‌ها:
- Index بر روی `WorkflowInstanceId`, `Status`, `RequestedFor`, `DueDate`

### مثال استفاده در AppEnd:
برای درخواست‌های تایید مدیر یا سایر مراحل تصویب

---

## 13. ElsaWorkflowSuspensions
**هدف**: ثبت تعلیق‌های دستی workflow

### ستون‌ها:
- `Id` (PK): شناسه
- `WorkflowInstanceId` (FK): ارجاع به نمونه
- `Reason`: دلیل تعلیق
- `SuspendedBy`: کاربری که تعلیق داد
- `SuspendedAt`: زمان تعلیق
- `ResumedBy`: کاربری که از سرگیری کرد
- `ResumedAt`: زمان از سرگیری
- `Notes`: یادداشت‌ها

### اندیس‌ها:
- Index بر روی `WorkflowInstanceId`, `SuspendedAt`

---

## 14. ElsaAuditLogs
**هدف**: لاگ Audit برای تتبع تمام تغییرات

### ستون‌ها:
- `Id` (PK): شناسه
- `EntityType`: نوع Entity ('WorkflowDefinition', 'WorkflowInstance', etc.)
- `EntityId`: شناسه Entity
- `Action`: عملیات ('Create', 'Update', 'Delete', 'Execute', 'Publish')
- `Changes`: تغییرات (JSON)
- `ChangedBy`: کاربری که تغییر داد
- `ChangedAt`: زمان
- `IpAddress`: آدرس IP
- `TenantId`: شناسه تأجیرکننده

### اندیس‌ها:
- Index بر روی `EntityType`, `EntityId`, `ChangedAt`

---

## Relationships نمودار

```
ElsaWorkflowDefinitions (1) ──── (N) ElsaWorkflowDefinitionVersions
            │
            ├──── (N) ElsaWorkflowInstances
            │         │
            │         ├──── (N) ElsaActivityExecutions
            │         ├──── (N) ElsaBookmarks
            │         ├──── (N) ElsaVariableInstances
            │         ├──── (N) ElsaWorkflowExecutionLogs
            │         ├──── (N) ElsaApprovalInstances
            │         ├──── (N) ElsaWorkflowSuspensions
            │         └──── (N) ElsaWorkflowEvents
            │
            ├──── (N) ElsaTriggeredWorkflows
            └──── (N) ElsaWorkflowTriggers
```

---

## نکات مهم

1. **Multi-Tenancy**: تمام جداول مهم دارای `TenantId` برای پشتیبانی از چند مستأجر
2. **Soft Delete**: `IsDeleted` flag برای حذف منطقی به جای حذف فیزیکی
3. **Audit Trail**: `ElsaAuditLogs` تمام تغییرات را ثبت می‌کند
4. **Performance**: اندیس‌ها برای تحسین عملکرد query‌ها طراحی شده‌اند
5. **JSON Columns**: فلکسیبیلیتی برای ذخیره داده‌های غیرساختاری

---

## نمای کل Size

- کل جداول: **14**
- Relations (FK): **20+**
- Indexes: **40+**
