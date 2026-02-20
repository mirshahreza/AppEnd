# استانداردهای Elsa Workflow Engine

## 📋 فهرست محتویات
1. [ساختارهای کنترلی و جریان (Flow Control)](#1-ساختارهای-کنترلی-و-جریان)
2. [قابلیت‌های پیشرفته](#2-قابلیت‌های-پیشرفته)
3. [انواع ارتباطات (Connectivity)](#3-انواع-ارتباطات)
4. [ویژگی‌های محیط بصری (UI/UX)](#4-ویژگی‌های-محیط-بصری)
5. [ساختار داده برای Storage](#5-ساختار-داده-برای-storage)
6. [معماری و الگوهای طراحی](#6-معماری-و-الگوهای-طراحی)

---

## 1. ساختارهای کنترلی و جریان (Flow Control)

### الف) منشعب شدن (Branching)
- **توضیح**: یک خروجی (Outcome) می‌تواند به چندین فعالیت متصل شود
- **کاربرد**: ایجاد شاخه‌های موازی (Parallel Execution)
- **قواعد**:
  - هر فعالیت می‌تواند چندین خروجی داشته باشد
  - هر خروجی می‌تواند به چندین فعالیت متصل شود
  - خروجی‌ها نام‌گذاری شده باشند (مثل: "Done", "Error", "Next")

### ب) تصمیم‌گیری شرطی (Decision / If)
- **توضیح**: بر اساس یک شرط Boolean، جریان را به دو یا چند مسیر هدایت می‌کند
- **خروجی‌های پیش‌فرض**: `True`, `False`
- **قابلیت‌ها**:
  - پشتیبانی از عبارات JavaScript
  - دسترسی به متغیرهای جریان (Workflow Variables)
  - مقایسه‌های پیچیده (>=, <=, ==, !=, &&, ||)
- **مثال**:
  ```javascript
  Variables.orderAmount > 1000
  Variables.status == "pending" && Variables.isUrgent == true
  ```

### ج) انتخاب چندگانه (Switch)
- **توضیح**: بر اساس یک متغیر، خروجی‌های متعددی (Cases) تعریف می‌کند
- **خروجی‌های پیش‌فرض**: 
  - هر Case یک مقدار مختلف
  - یک `Default` برای مقادیر نامطابق
- **قابلیت‌ها**:
  - مقایسه string, number, enum
  - Case-insensitive matching
  - Wildcard patterns (اختیاری)

### د) حلقه‌ها (Loops)

#### While Loop
- **شرط**: بر اساس یک عبارات Boolean
- **خروجی‌ها**: 
  - `Iterate` (هنگام اجرای حلقه)
  - `Done` (پایان حلقه)
- **متغیرهای دسترسی**:
  - `Variables.LoopIndex` یا `Variables.Iteration`

#### For Loop
- **پارامترها**: Start, End, Step
- **خروجی‌ها**: `Iterate`, `Done`
- **دسترسی به شاخص‌**: `Variables.LoopIndex`

#### ForEach Loop
- **پارامترها**: Collection/Array
- **خروجی‌ها**: `Iterate`, `Done`
- **دسترسی به عناصر**:
  - `Variables.CurrentItem` (عنصر فعلی)
  - `Variables.LoopIndex` (شاخص)

### ه) انتظار و سیگنال (Wait/Signal)
- **توضیح**: توقف اجرای وورک‌فلو تا دریافت سیگنال خارجی
- **حالت‌های Suspension**:
  - `Suspended` (منتظر سیگنال)
  - `Resumed` (از سر گرفته شده)
- **کاربردها**:
  - انتظار تایید کاربر
  - انتظار webhook
  - انتظار رویداد خارجی

---

## 2. قابلیت‌های پیشرفته

### الف) اجرای موازی (Parallel Execution)
- **Fork / Join Pattern**:
  - `Fork`: انشعاب جریان اصلی به چندین شاخه
  - `Join`: دوباره یکپارچه کردن شاخه‌ها
- **دو حالت پیوند**:
  - **Wait All**: اتمام تمام شاخه‌ها قبل از ادامه
  - **Wait Any**: اتمام اولین شاخه برای ادامه

### ب) مدیریت خطا (Error/Fault Handling)
- **Exception Handlers**:
  - هر فعالیت می‌تواند شاخه‌های Error داشته باشد
  - خروجی پیش‌فرض: `Error`
- **Error Context**:
  - `Variables.ExceptionMessage`
  - `Variables.ExceptionType`
  - `Variables.ExceptionStackTrace`
- **استراتژی‌های Recovery**:
  - Retry (دوباره سعی)
  - Fallback (راه جایگزین)
  - Terminate (توقف با خطا)

### ج) متغیرها (Variables)
- **سطوح Variable**:
  - **Global**: در سطح Workflow Definition
  - **Local**: در سطح Composite Activity
  - **Input/Output**: Parameters یک Activity
- **نوع‌های داده**:
  - String, Number, Boolean
  - Object, Array
  - DateTime, Guid
- **اسکوپ و Lifetime**:
  - زمان‌های زندگی: Instance, Execution, Activity
  - دسترسی: محدود به محدوده تعریف

### د) عبارات پویا (Dynamic Expressions)
- **زبان‌های پشتیبانی شده**:
  - **JavaScript**: `Variables.amount > 100 && Variables.status == "pending"`
  - **Liquid**: `{% if order.total > 1000 %}approved{% endif %}`
  - **Expressions**: `${variable1} + ${variable2}`
- **Context دسترسی**:
  - تمام Workflow Variables
  - Input Parameters
  - Custom Functions
- **Caching**:
  - عبارات compiled-cached برای کارایی بیشتر

---

## 3. انواع ارتباطات (Connectivity)

### الف) Outcomes (خروجی‌ها)
- **تعریف**: هر فعالیت خروجی‌های نام‌گذاری شده‌ای دارد
- **مثال‌های استاندارد**:
  - `Done` (تکمیل موفق)
  - `Error` (خطا)
  - `True` / `False` (Decision)
  - `Success` / `Failure`
- **Custom Outcomes**:
  - کسی‌مایه توسط Custom Activities
- **Validation**:
  - خروجی‌های استفاده نشده هشدار می‌دهند

### ب) Triggers
- **تعریف**: نقاط شروع وورک‌فلو
- **انواع**:
  - **HTTP Trigger**: دریافت HTTP Request
  - **Event Trigger**: رویداد دیتابیسی، سیستمی
  - **Scheduled Trigger**: Cron یا DateTime مشخص
  - **Manual Trigger**: شروع دستی
  - **Webhook Trigger**: Payload خارجی
- **Correlation**:
  - هر Trigger می‌تواند Correlation Token داشته باشد برای شناسایی

### ج) Blocking Activities
- **تعریف**: فعالیتی که وورک‌فلو را تا سیگنال خارجی متوقف می‌کند
- **مثال‌ها**:
  - `WaitForApproval`: منتظر تایید از کاربر
  - `WaitForSignal`: سیگنال دستی
  - `WaitForWebhook`: بازگشت Webhook
- **State Storage**:
  - Instance state در دیتابیس ذخیره شود
  - Resumption Data محفوظ شود

---

## 4. ویژگی‌های محیط بصری (UI/UX)

### الف) ویرایش و طراحی (Design Canvas)
- **Drag & Drop**:
  - عناصر از Toolbox به Canvas
  - Reorder شاخه‌ها (Drag to Reposition)
- **Node Sizing**:
  - سایز خودکار یا دستی
  - Responsive Design
- **Connection Management**:
  - رسم خطوط اتصال (Lines)
  - بازحذف اتصالات
  - Validation Visual Indicators

### ب) Navigation و Zoom
- **Zoom Controls**:
  - Zoom In/Out (Ctrl+Scroll)
  - Fit to Screen (Auto-Zoom)
  - 100% Reset
- **Panning**:
  - Space + Drag
  - Mouse Wheel Scroll
- **Overview Map** (Mini Map):
  - نمای کلی وورک‌فلو

### ج) Properties Panel
- **Activity Properties**:
  - نام فعالیت
  - Input/Output Mapping
  - Variable Assignments
- **Workflow Properties**:
  - نام، توضیح
  - Version
  - Tags, Metadata
- **Runtime Settings**:
  - Timeout
  - Retry Policy
  - Compensation

### د) Validation & Error Reporting
- **Design-time Validation**:
  - دقیق شود که هیچ خروجی disconnected نباشد
  - نام‌های منحصر بفرد
  - Valid JavaScript/Liquid
- **Visual Indicators**:
  - 🔴 Errors (Red)
  - 🟡 Warnings (Yellow)
  - 🟢 Valid (Green)

### ه) Import/Export
- **JSON Format**:
  ```json
  {
    "id": "workflow-1",
    "name": "Order Approval",
    "version": 1,
    "activities": [...],
    "connections": [...]
  }
  ```
- **Version Control**:
  - Export برای Git
  - Import از JSON
  - Merge Conflicts Detection

---

## 5. ساختار داده برای Storage

### الف) Workflow Definition
```json
{
  "id": "string",
  "name": "string",
  "description": "string",
  "version": "int",
  "isPublished": "boolean",
  "activities": [
    {
      "id": "string",
      "name": "string",
      "type": "string",
      "x": "number",
      "y": "number",
      "properties": {}
    }
  ],
  "connections": [
    {
      "source": "string",
      "sourceOutcome": "string",
      "target": "string"
    }
  ],
  "variables": [
    {
      "name": "string",
      "type": "string",
      "scope": "Global|Local",
      "defaultValue": "any"
    }
  ],
  "createdAt": "datetime",
  "updatedAt": "datetime",
  "createdBy": "string"
}
```

### ب) Workflow Instance
```json
{
  "id": "string (unique instance ID)",
  "definitionId": "string",
  "definitionVersion": "int",
  "status": "Running|Completed|Failed|Suspended",
  "variables": {},
  "currentActivityId": "string",
  "executionLog": [
    {
      "timestamp": "datetime",
      "activityId": "string",
      "activityName": "string",
      "outcome": "string",
      "duration": "int (ms)"
    }
  ],
  "incidents": [
    {
      "id": "string",
      "type": "Error|Warning",
      "message": "string",
      "activityId": "string",
      "timestamp": "datetime"
    }
  ],
  "startedAt": "datetime",
  "finishedAt": "datetime",
  "correlationId": "string"
}
```

### ج) Workflow Task (Human Interaction)
```json
{
  "id": "string",
  "instanceId": "string",
  "activityId": "string",
  "title": "string",
  "description": "string",
  "status": "Pending|Completed|Rejected|Escalated",
  "assignedTo": "string (user ID)",
  "assignedRole": "string",
  "priority": "Low|Normal|High|Urgent",
  "dueDate": "datetime",
  "createdAt": "datetime",
  "completedAt": "datetime",
  "contextData": "{}",
  "outcome": "string"
}
```

---

## 6. معماری و الگوهای طراحی

### الف) Activity Base Pattern
```csharp
public class BaseActivity : ActivityBase
{
    // Inputs (Properties)
    public Input<string> DisplayName { get; set; }
    
    // Outputs (Outcomes)
    protected override void Execute(ActivityExecutionContext context)
    {
        // Logic
        context.Outcomes.Add("Done");
        context.Outcomes.Add("Error");
    }
}
```

### ب) Composite Activity Pattern
- Activity که دیگر Activities را شامل می‌شود
- مثال: `ParallelForEach`, `Switch`
- State Management برای Child Activities

### ج) Persistence Strategy
- **Database Persistence**:
  - EF Core DBContext
  - Workflow + Instance + Task Tables
- **In-Memory Caching**:
  - Active instances
  - Workflow definitions
  - Task queues
- **Event Sourcing** (اختیاری):
  - تمام تغییرات log شوند
  - Replay برای Audit

### د) Correlation Mechanism
- **Correlation Token**:
  - شناسایی فعالیت‌ها در long-running workflows
  - Lookup by External ID
- **Signal/Resume**:
  - `WaitForSignal("correlationId")`
  - `ResumeWorkflow("correlationId", payload)`

---

## 📊 جدول خلاصه قابلیت‌ها

| قابلیت | پشتیبانی | توضیحات |
|--------|---------|--------|
| **منشعب شدن موازی** | ✅ | بله، از طریق Fork/Join |
| **تصمیم‌گیری شرطی** | ✅ | JavaScript, Liquid |
| **حلقه‌ها** | ✅ | While, For, ForEach |
| **مدیریت خطا** | ✅ | Exception Handlers, Retry |
| **متغیرها** | ✅ | Global, Local, Input/Output |
| **بازگشت به عقب (Loop Back)** | ✅ | از طریق connections |
| **نسخه‌بندی** | ✅ | Version control |
| **Custom Activities** | ✅ | C# Classes |
| **Integration** | ✅ | HTTP, RPC, Events |
| **اجرای موقوف (Suspend)** | ✅ | WaitForApproval, WaitForSignal |

---

## 🎯 اولویت‌های پیاده‌سازی

1. **Phase 1 (Core)**:
   - Visual Designer Canvas
   - Activity Toolbox
   - Basic Activities (Start, End, SetVariable)
   - Connections & Outcomes

2. **Phase 2 (Control Flow)**:
   - Decision/If Activity
   - Switch Activity
   - While Loop
   - ForEach Loop

3. **Phase 3 (Advanced)**:
   - Parallel Fork/Join
   - Error Handling
   - WaitForApproval
   - Composite Activities

4. **Phase 4 (Integration)**:
   - RPC Integration
   - Workflow Execution Engine
   - Task Management
   - Monitoring Dashboard
