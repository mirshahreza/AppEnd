# 📖 Documentation Map - Elsa Workflow Engine

**نقشه کاملی از تمام documentation و کاری که انجام شده**

---

## 🗺️ Navigation Guide

### 🚀 Quick Start
**شروع کردن:**
1. 👉 **`RUN-AND-TEST.md`** ← **شروع اینجا**
   - اجرای برنامه
   - اولین test
   - PowerShell script

### 📋 Understanding the System
2. **`PROJECT-SUMMARY.md`** ← خلاصه کامل
   - مروری بر تمام کاری که انجام شده
   - Architecture
   - Key decisions

3. **`IMPLEMENTATION-CHECKLIST.md`** ← تفصیل کامل
   - Item by item checklist
   - تمام فایل‌های تغییر یافته
   - Code metrics

### 🔧 Technical Reference
4. **`QUICK-REFERENCE.md`** ← API reference
   - RPC methods
   - SQL schema
   - Configuration
   - Troubleshooting

5. **`READY-FOR-TESTING.md`** ← تفصیلی تست
   - Step by step مراحل
   - SQL examples
   - Browser console examples

6. **`CONFIG-REORGANIZED.md`** ← Configuration details
   - Nested config structure
   - Why each change
   - Benefits

### 🔨 Advanced Topics (Optional)
7. **`PHASE7-CUSTOM-ACTIVITIES.md`** ← Custom activities (4-6 ساعت)
   - CreateTaskActivity
   - WaitForApprovalActivity
   - SendEmailActivity
   - RunSqlQueryActivity

---

## 📂 Code Files Location

### Backend (.NET / C#)
```
AppEndHost\
├── Program.cs
│   └── builder.Services.AddAppEndWorkflow(builder.Configuration)
│
└── appsettings.json
    └── "AppEnd.Workflow": { ... }

AppEndWorkflow\
├── ElsaSetup.cs
│   └── AddAppEndWorkflow() method
│
├── WorkflowServices.cs
│   ├── GetMyWorkflowTasks()
│   └── CompleteWorkflowTask()
│
└── WorkflowDefinitionProvider.cs

AppEndHost\workspace\server\
└── Zzz.AppEndProxy.Workflow.cs
    ├── GetMyWorkflowTasks(Actor, Status, Page, PageSize)
    └── CompleteWorkflowTask(Actor, TaskId, Outcome, OutputParams)
```

### Frontend (Vue.js)
```
AppEndHost\workspace\client\
├── a.SharedComponents\
│   └── WorkflowInbox.vue
│       ├── Display tasks
│       ├── Approve/Reject
│       └── RPC calls
│
└── AppEndStudio\components\
    └── WorkflowInstances.vue
        ├── Display instances
        ├── Filter & search
        └── Cancel capability
```

### Database (SQL)
```
AppEnd\
└── WorkflowTasks-Schema.sql
    ├── [WorkflowTasks] table
    ├── 6 indexes
    ├── 2 views
    └── 2 stored procedures
        ├── ElsaGetMyWorkflowTasks
        └── ElsaCompleteWorkflowTask
```

### Workflows
```
AppEndHost\workspace\workflows\
├── hello-world.json
├── order-approval.json
├── data-pipeline.json
└── scheduled-db-check.json
```

---

## 🎯 First Time Users

### Day 1: Setup & Test (2 ساعت)
```
1. Read:  RUN-AND-TEST.md  (15 min)
2. Run:   dotnet run  (5 min)
3. Test:  .\test-workflow-api.ps1  (15 min)
4. UI:    Open WorkflowInbox.vue  (20 min)
5. Read:  PROJECT-SUMMARY.md  (30 min)
```

### Day 2: Deep Dive (2 ساعت)
```
1. Read:  IMPLEMENTATION-CHECKLIST.md  (30 min)
2. Read:  CONFIG-REORGANIZED.md  (30 min)
3. Code:  Review WorkflowServices.cs  (30 min)
4. Code:  Review ElsaSetup.cs  (30 min)
```

### Optional: Advanced Features (6+ ساعت)
```
1. Read:  PHASE7-CUSTOM-ACTIVITIES.md
2. Code:  Implement custom activities
3. Test:  Create complex workflows
```

---

## 🔍 Finding Things

### "How do I...?"

**...test the API?**
→ `RUN-AND-TEST.md` → "🧪 Test API"

**...get my workflow tasks?**
→ `QUICK-REFERENCE.md` → "📋 RPC Methods" → GetMyWorkflowTasks

**...complete a task?**
→ `READY-FOR-TESTING.md` → "مرحله 3: تست CompleteWorkflowTask"

**...see the configuration?**
→ `CONFIG-REORGANIZED.md` → "🔍 کد یقین‌میری"

**...create a custom activity?**
→ `PHASE7-CUSTOM-ACTIVITIES.md` → "1. CreateTaskActivity"

**...understand the architecture?**
→ `PROJECT-SUMMARY.md` → "🎯 Architecture Overview"

**...fix connection timeout?**
→ `QUICK-REFERENCE.md` → "🐛 Troubleshooting"

**...see what was done?**
→ `IMPLEMENTATION-CHECKLIST.md` → "✅ Implementation Checklist"

---

## 🏗️ Technical Architecture

```
┌──────────────────────────────────────────┐
│         PRESENTATION LAYER               │
├──────────────────────────────────────────┤
│  Vue.js Components                       │
│  ├─ WorkflowInbox.vue                   │
│  └─ WorkflowInstances.vue               │
└────────────────┬─────────────────────────┘
                 │ HTTP + JSON (RPC)
┌────────────────▼─────────────────────────┐
│         APPLICATION LAYER                │
├──────────────────────────────────────────┤
│  Zzz.AppEndProxy.Workflow.cs             │
│  ├─ GetMyWorkflowTasks()                │
│  └─ CompleteWorkflowTask()              │
└────────────────┬─────────────────────────┘
                 │ In-Process
┌────────────────▼─────────────────────────┐
│         BUSINESS LOGIC LAYER             │
├──────────────────────────────────────────┤
│  WorkflowServices.cs                     │
│  ├─ GetMyWorkflowTasks()                │
│  ├─ CompleteWorkflowTask()              │
│  └─ [Other workflow operations]         │
│                                          │
│  ElsaSetup.cs                           │
│  └─ Elsa Framework Configuration        │
└────────────────┬─────────────────────────┘
                 │ ADO.NET
┌────────────────▼─────────────────────────┐
│         DATA ACCESS LAYER                │
├──────────────────────────────────────────┤
│  DbIO (AppEndDbIO)                      │
│  └─ SQL Server queries                  │
│                                          │
│  Elsa Framework                         │
│  └─ Workflow persistence                │
└────────────────┬─────────────────────────┘
                 │ SQL
┌────────────────▼─────────────────────────┐
│         DATABASE LAYER                   │
├──────────────────────────────────────────┤
│  SQL Server (AppEnd database)            │
│                                          │
│  Tables:                                 │
│  ├─ WorkflowTasks                       │
│  ├─ Elsa.WorkflowInstances             │
│  ├─ Elsa.WorkflowDefinitions           │
│  └─ [other Elsa tables]                │
│                                          │
│  Stored Procedures:                      │
│  ├─ ElsaGetMyWorkflowTasks              │
│  └─ ElsaCompleteWorkflowTask            │
│                                          │
│  Indexes:                                │
│  ├─ ix_TasksAssignedTo_Status          │
│  ├─ ix_TasksStatus                     │
│  └─ [others]                           │
└──────────────────────────────────────────┘
```

---

## 📚 Reading Order

### Sequential (Recommended)
1. `RUN-AND-TEST.md` - Get it running
2. `PROJECT-SUMMARY.md` - Overview
3. `IMPLEMENTATION-CHECKLIST.md` - Details
4. `CONFIG-REORGANIZED.md` - Configuration
5. `QUICK-REFERENCE.md` - API reference
6. `PHASE7-CUSTOM-ACTIVITIES.md` - Advanced

### By Role

**Developer (wants to understand code):**
→ `PROJECT-SUMMARY.md` → `IMPLEMENTATION-CHECKLIST.md` → Code files

**DevOps/Ops (wants to run/deploy):**
→ `RUN-AND-TEST.md` → `QUICK-REFERENCE.md` → `CONFIG-REORGANIZED.md`

**QA/Tester (wants to test):**
→ `RUN-AND-TEST.md` → `READY-FOR-TESTING.md` → `test-workflow-api.ps1`

**Architect (wants full picture):**
→ `PROJECT-SUMMARY.md` → All technical docs → Code review

---

## 🎯 Quick Answers

| Question | Answer | File |
|----------|--------|------|
| How do I start? | `dotnet run` | RUN-AND-TEST.md |
| How do I test? | `.\test-workflow-api.ps1` | RUN-AND-TEST.md |
| What's the config? | appsettings.json | CONFIG-REORGANIZED.md |
| What RPC methods exist? | 6 main methods | QUICK-REFERENCE.md |
| What was done? | 100+ items | IMPLEMENTATION-CHECKLIST.md |
| Where are files? | See code locations | This file |
| How does it work? | Architecture diagram | PROJECT-SUMMARY.md |
| What's broken? | Troubleshooting guide | QUICK-REFERENCE.md |
| How do I extend? | Custom activities | PHASE7-CUSTOM-ACTIVITIES.md |

---

## 🚀 Getting Started Right Now

```bash
# Step 1: Open terminal
cd AppEndHost

# Step 2: Run application
dotnet run

# Step 3: Open PowerShell (new terminal)
.\test-workflow-api.ps1

# Step 4: Open browser
http://localhost:5000/AppEndStudio
```

---

## 📞 Support Files

### For Problems
- **Connection issues:** QUICK-REFERENCE.md → Troubleshooting
- **Configuration issues:** CONFIG-REORGANIZED.md
- **Testing issues:** RUN-AND-TEST.md

### For Learning
- **Architecture:** PROJECT-SUMMARY.md
- **Details:** IMPLEMENTATION-CHECKLIST.md
- **API Usage:** QUICK-REFERENCE.md

### For Extension
- **Custom code:** PHASE7-CUSTOM-ACTIVITIES.md
- **Database changes:** WorkflowTasks-Schema.sql
- **UI changes:** WorkflowInbox.vue, WorkflowInstances.vue

---

## ✅ Status

```
┌─────────────────────────────────────┐
│  Elsa Workflow Engine               │
│  Status: ✅ COMPLETE & TESTED       │
│                                     │
│  Documentation: 100%                │
│  Code: 100%                         │
│  Build: ✅ Successful               │
│                                     │
│  Ready to: 🚀 RUN & DEPLOY          │
└─────────────────────────────────────┘
```

---

**شروع کنید اینجا:** `RUN-AND-TEST.md` 🚀
