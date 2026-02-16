# ⚡ Quick Reference - Workflow Tasks

**برای استفاده سریع - یادداشت برای بعد**

---

## 🚀 Deploy Checklist

```
☐ Run SQL: WorkflowTasks-Schema.sql
  USE AppEndDB
  GO
  -- Paste content from WorkflowTasks-Schema.sql

☐ Build: dotnet build AppEnd.sln

☐ Start App: dotnet run --project AppEndHost

☐ Test in Browser:
  rpcAEP("GetMyWorkflowTasks", {}, console.log)
```

---

## 📋 API Quick Calls

### Get My Tasks
```javascript
rpcAEP("GetMyWorkflowTasks", { 
    Status: "Pending", Page: 1, PageSize: 25 
}, console.log)
```

### Complete a Task
```javascript
rpcAEP("CompleteWorkflowTask", {
    TaskId: "GUID",
    Outcome: "Approve",
    OutputParams: { comment: "OK" }
}, console.log)
```

---

## 📁 Files Changed

| فایل | تغییر | سطر |
|------|-------|-----|
| WorkflowServices.cs | +2 methods | 764-883 |
| Zzz.AppEndProxy.Workflow.cs | +2 methods | 137-178 |
| WorkflowInstances.vue | updateAPI | ~300 |
| WorkflowInbox.vue | updateAPI | ~331 |

---

## 🔧 Key Methods

```csharp
// Get user tasks
GetMyWorkflowTasks(Status?, Page, PageSize, UserId)
  ↓ calls → sp_GetMyWorkflowTasks

// Complete task  
CompleteWorkflowTask(TaskId, Outcome, OutputParams, UserId)
  ↓ calls → sp_CompleteWorkflowTask
```

---

## ✅ Status Summary

| Item | Status |
|------|--------|
| Code | ✅ Done |
| Build | ✅ Pass |
| Tests | ⏳ Ready |
| Deploy | ⏳ Step 1 |

---

## 📞 If Issues:

1. **SQL Error?** → Check WorkflowTasks-Schema.sql syntax
2. **API 404?** → Rebuild & restart app
3. **Data Missing?** → Insert test data (see TESTING-GUIDE.md)
4. **TypeError?** → Check browser console logs

---

## 📚 Full Docs

- `COMPLETION-REPORT.md` - مکمل
- `RPC-API-REFERENCE.md` - API guide
- `TESTING-GUIDE.md` - تست
- `IMPLEMENTATION-SUMMARY.md` - جزئیات

---

**Ready to Deploy! 🚀**
