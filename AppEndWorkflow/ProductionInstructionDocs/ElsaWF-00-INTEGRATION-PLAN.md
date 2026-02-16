# Elsa 3.5.3 Workflow Engine — Integration Plan

> **Branch:** `mohsen-workflow-engine`  
> **Date:** 2025-07  
> **Status:** Planning  
> **Elsa Version:** 3.5.3 (latest stable)  
> **Target Framework:** .NET 10

---

## 📚 Plan Documents (Split by Section)

این پلن برای جلوگیری از مصرف توکن زیاد به فایل‌های جداگانه تقسیم شده.  
هر فایل یک بخش خاص رو پوشش می‌ده — فقط همونی که نیاز داری رو بخون.

| # | فایل | محتوا | تقریبی خطوط |
|---|---|---|---|
| 0 | **[ElsaWF-00-Overview.md](ElsaWF-00-Overview.md)** | Project Context, Architecture Decisions | ~70 |
| 1 | **[ElsaWF-01-Phase1-Project.md](ElsaWF-01-Phase1-Project.md)** | Phase 1 — ساخت پروژه AppEndWorkflow | ~45 |
| 2 | **[ElsaWF-02-Phase2-ElsaSetup.md](ElsaWF-02-Phase2-ElsaSetup.md)** | Phase 2 — راه‌اندازی Elsa و SQL Server | ~55 |
| 3 | **[ElsaWF-03-Phase3-Integration.md](ElsaWF-03-Phase3-Integration.md)** | Phase 3 — اتصال به AppEndHost | ~45 |
| 4 | **[ElsaWF-04-Phase4-Samples.md](ElsaWF-04-Phase4-Samples.md)** | Phase 4 — نمونه Workflowها (4 تا) | ~150 |
| 5 | **[ElsaWF-05-Phase5-Verify.md](ElsaWF-05-Phase5-Verify.md)** | Phase 5 — Build و تست | ~20 |
| 6 | **[ElsaWF-06-Phase6-UI.md](ElsaWF-06-Phase6-UI.md)** | Phase 6 — رابط کاربری Vue.js | ~260 |
| 7 | **[ElsaWF-07-Phase7-Activities.md](ElsaWF-07-Phase7-Activities.md)** | Phase 7 — کتابخانه Activity سفارشی (48 تا) | ~1140 |
| 8 | **[ElsaWF-08-Structure-Notes.md](ElsaWF-08-Structure-Notes.md)** | ساختار فایل‌ها، نمودار وابستگی، نکات کلیدی | ~120 |

---

## ⚡ مرجع سریع

### ترتیب اجرا

| فاز | شرح | وابستگی |
|---|---|---|
| **Phase 1** | ساخت پروژه `AppEndWorkflow` + NuGet + references | هیچ |
| **Phase 2** | `ElsaSetup.cs` با DI و middleware | Phase 1 |
| **Phase 3** | اتصال به `AppEndHost` (csproj + Program.cs) | Phase 2 |
| **Phase 4** | 4 نمونه workflow (HelloWorld, ScheduledDbCheck, OrderApproval, DataPipeline) | Phase 2 |
| **Phase 5** | Build و تست کامل | Phase 3 + 4 |
| **Phase 6** | `WorkflowServices.cs` + 4 کامپوننت ادمین + 1 کامپوننت کارتابل مشترک | Phase 5 |
| **Phase 7** | کتابخانه Activity سفارشی — 48 activity در 14 دسته | Phase 5 |

**⚠️ هر فاز نیاز به تایید صریح قبل از شروع داره.**

### محدودیت‌های کلیدی

1. ❌ **Blazor نداریم** — فقط Vue.js + jQuery
2. ❌ **SQLite نداریم** — فقط SQL Server (دیتابیس موجود)
3. ❌ **Auto-migration نداریم** — اسکریپت‌های SQL دستی
4. ✅ **فقط RPC** — بدون route های Elsa، بدون `UseWorkflowsApi()`, بدون `UseHttp()`
5. 📦 **NuGet:** `Elsa` 3.5.3 + `Elsa.EntityFrameworkCore.SqlServer` 3.5.3

---

## 🎯 شروع کار

برای شروع، ابتدا [ElsaWF-00-Overview.md](ElsaWF-00-Overview.md) رو بخون تا با معماری آشنا بشی، بعد از [ElsaWF-01-Phase1-Project.md](ElsaWF-01-Phase1-Project.md) شروع کن.

---

## Project Context (خلاصه)

### Existing Solution Structure

| Project | Type | Role |
|---|---|---|
| `AppEndCommon` | Class Library | Settings, extensions, logging |
| `AppEndDynaCode` | Class Library | Dynamic code compilation (Roslyn) |
| `AppEndDbIO` | Class Library | DB access, `DbConf`, `DbDialog` |
| `AppEndServer` | Class Library | Services, RPC middleware, scheduling |
| `AppEndHost` | Web App | Entry point, `Program.cs`, static files |

### ویژگی‌های کلیدی

- **RPC Pattern:** Vue.js → `rpcAEP()` → `Zzz.AppEndProxy` → Static C# methods
- **DB Access:** `DbConf.FromSettings(AppEndSettings.DefaultDbConfName).ConnectionString`
- **UI Stack:** Vue.js + jQuery + Bootstrap 5 + FontAwesome
- **Styles:** `a..lib/append/css/` (reusable) or `AppEndStudio/assets/custom.css` (app-specific)

---

## 📋 پیوست: همه فایل‌های پلن

- [ElsaWF-00-Overview.md](ElsaWF-00-Overview.md)
- [ElsaWF-01-Phase1-Project.md](ElsaWF-01-Phase1-Project.md)
- [ElsaWF-02-Phase2-ElsaSetup.md](ElsaWF-02-Phase2-ElsaSetup.md)
- [ElsaWF-03-Phase3-Integration.md](ElsaWF-03-Phase3-Integration.md)
- [ElsaWF-04-Phase4-Samples.md](ElsaWF-04-Phase4-Samples.md)
- [ElsaWF-05-Phase5-Verify.md](ElsaWF-05-Phase5-Verify.md)
- [ElsaWF-06-Phase6-UI.md](ElsaWF-06-Phase6-UI.md)
- [ElsaWF-07-Phase7-Activities.md](ElsaWF-07-Phase7-Activities.md)
- [ElsaWF-08-Structure-Notes.md](ElsaWF-08-Structure-Notes.md)

---

## یادداشت‌های منتقل‌شده از چت

موارد مفید از چت در اینجا منتقل شد تا دیگر نیازی به نگه داشتن لاگ چت نباشد:

- فایل‌های تقسیم‌شده ایجاد شدند: `ElsaWF-00-Overview.md` تا `ElsaWF-08-Structure-Notes.md`.
- فایل پروژه ایجاد شد: `AppEndWorkflow/AppEndWorkflow.csproj` (TargetFramework=`net10.0`) با PackageReference به `Elsa` و `Elsa.EntityFrameworkCore.SqlServer` نسخه `3.5.3` و ProjectReference به `..\AppEndCommon\AppEndCommon.csproj` و `..\AppEndDbIO\AppEndDbIO.csproj`.
- فایل اصلی برنامه (`ELSA-INTEGRATION-PLAN.md`) به یک ایندکس سبک تقسیم شد و به فایل‌های `ElsaWF-*` لینک می‌دهد.
- کارهایی که هنوز انجام نشده‌اند (آگاه باشید):
  - پروژه هنوز به `AppEnd.sln` اضافه نشده.
  - `AppEndHost/Program.cs` و `AppEndHost/AppEndHost.csproj` هنوز تغییر داده نشده‌اند (قرار است پس از تایید فازها انجام شود).

پیشنهاد قدم‌های بعدی (برای اجرا وقتی آماده بودید):
- ایجاد `ElsaSetup.cs` در `AppEndWorkflow` (اضافه‌کردن `AddAppEndWorkflow` و `UseAppEndWorkflow`).
- ایجاد `WorkflowServices.cs` (پل RPC → Elsa SDK) و پیاده‌سازی متدهای پایه (ExecuteWorkflow، GetWorkflowDefinitions، CompleteWorkflowTask و غیره).
- اضافه‌کردن نمونه Workflow‌ها در `AppEndWorkflow/Workflows/` (HelloWorld, ScheduledDbCheck, OrderApproval, DataPipeline).
- استخراج و قرار دادن اسکریپت `ElsaSchema.sql` در پروژه تنظیمات دیتابیس تیم شما (auto-migrations غیرفعال است).

این بخش خلاصه‌ای از محتواست که از چت استخراج و داخل رپو ذخیره شد. اگر بخواهید بقیه لاگ کامل هم به یک فایل جدا منتقل شود، بگید تا اضافه کنم.
