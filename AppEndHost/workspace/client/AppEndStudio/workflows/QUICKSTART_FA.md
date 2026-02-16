# ⚡ Workflow Designer - Quick Start Guide

## 🚀 شروع سریع

### 5 دقیقه برای شروع استفاده

#### 1️⃣ باز کردن Designer (1 دقیقه)

```javascript
// کل کاری که باید کنید:
openComponent('/AppEndStudio/workflows/WorkflowEditor.vue', {
    title: 'Workflow Designer',
    modalSize: 'modal-fullscreen',
    modal: true,
    params: {
        workflow: null  // یا workflow موجود
    },
    callback: (result) => {
        if (result?.success) {
            console.log('Workflow saved:', result.workflow);
        }
    }
});
```

---

#### 2️⃣ افزودن Nodes (2 دقیقه)

**در Canvas:**
1. Drag node از **Left Palette** → Drop به **Canvas**
2. یا از این اقسام شروع کنید:
   - ✅ **Start** (سبز) - همیشه اول
   - 📋 **Task** (آبی) - یک کار
   - ❓ **Decision** (زرد) - شرط
   - ❌ **End** (قرمز) - آخر

**مثال:**
```
Start → Check Status → Send Email → End
```

---

#### 3️⃣ اتصال Nodes (1 دقیقه)

**روش:**
1. کلیک بر روی Output Port (سمت راست node)
2. Drag کنید به Input Port (سمت چپ node بعدی)
3. Connection رسم می‌شود

**ویرایش Connection:**
- کلیک روی خط
- X قرمز می‌آید
- کلیک X برای حذف

---

#### 4️⃣ ویرایش Properties (1 دقیقه)

**در Right Panel:**

| Field | چه کنید |
|-------|---------|
| Label | نام node را تغییر دهید |
| Description | توضیح اضافه کنید |
| Condition | برای Decision node: شرط بنویسید |
| Configuration | برای Loop: تنظیمات JSON |

**مثال Decision:**
```
Condition: status === 'approved'
```

---

### 📋 Node Types - فوری

| نوع | رنگ | استفاده |
|-----|------|---------|
| Start/End | سبز/قرمز | شروع و پایان |
| Task | آبی | یک عمل |
| Decision | زرد | شرط/if |
| Loop | بنفش | تکرار |
| HTTP | صورتی | API call |
| Email | نارنجی | ایمیل |
| Database | فیروزه‌ای | DB query |
| Script | بنفش | کد custom |

---

### ⌨️ Keyboard Shortcuts

| کلید | اثر |
|------|------|
| `Ctrl+Z` | Undo |
| `Ctrl+Y` | Redo |
| `Delete` | حذف node انتخاب‌شده |
| `Drag` | حرکت node / اتصال |

---

### ✅ قبل از Save کردن

**چک list:**
- [ ] Start node دارد؟
- [ ] End node دارد؟
- [ ] تمام nodes متصل هستند؟
- [ ] Label‌ها معنی‌دار هستند؟

**نشان:**
- 🟢 **Green badge** = Valid
- 🔴 **Red badge** = Invalid

---

### 🎨 بهترین روش‌ها

#### ❌ اشتباه
```
Task1 → Task2 → Task3
```
(هیچ شرط، هیچ منطق)

#### ✅ صحیح
```
Start → Check Order → (Decision)
                    ├─ Approved → Send Email → End
                    └─ Rejected → Log Error → End
```

---

### 🔍 Test کردن

#### روش 1: Test Component
```javascript
// برای مدیریت‌گران
openComponent('/AppEndStudio/workflows/WorkflowDesignerTest.vue', {
    title: 'Workflow Designer Test',
    modal: true
});
```

#### روش 2: Console Debug
```javascript
// در Developer Tools Console:
window.NodeTypes              // تمام types
window.NodeCategories         // تمام categories
window.getNodeType('task')    // جزئیات یک type
```

---

### 🐛 مشکلات رایج و حل

#### مشکل: "Cannot connect"
**حل:**
- Start node از آن نمی‌توانید connect کنید
- End node به آن نمی‌توانید connect کنید

#### مشکل: "Workflow must have Start and End"
**حل:**
- ابتدا یک Start node اضافه کنید
- آخرش End node اضافه کنید

#### مشکل: "Nodes not appearing"
**حل:**
- صفحه refresh کنید (F5)
- DevTools میں error چیک کریں

#### مشکل: "Properties not updating"
**حل:**
- پس از تغییر، @change trigger شود
- اگر نشد، node دوباره select کنید

---

### 💡 نکات مهم

1. **Undo Unlimited**
   - هر تعداد `Ctrl+Z` می‌توانید انجام دهید
   - 99 سطح history

2. **Auto-Save Features**
   - Position خودکار ذخیره می‌شود
   - هنگام Save، تمام تاریخچه برای validation چک می‌شود

3. **Node Snapping**
   - Nodes خودکار به grid (10px) snap می‌شوند
   - Position دقیق‌تر است

4. **Categories**
   - Left Palette دسته‌بندی‌شده است
   - Search برای پیدا کردن node

---

### 📞 کمک و منابع

| منبع | مورد |
|------|------|
| README_FA.md | مستندات کامل |
| DEVELOPER_GUIDE_FA.md | برای توسعه‌دهندگان |
| IMPLEMENTATION_SUMMARY_FA.md | معماری سیستم |
| WorkflowDesignerTest.vue | نمونه‌های عملی |

---

### 🎓 مثال کامل: Order Processing Workflow

```
1. Start
   ↓
2. Receive Order (Task)
   - Label: "Receive Order"
   - Description: "Get order from queue"
   ↓
3. Check Order (Decision)
   - Label: "Check Order"
   - Condition: "order.total > 0"
   ↓
   ├─ Valid → Process Payment (Task) → Send Confirmation (Email) → End
   │
   └─ Invalid → Log Error (Script) → End
```

**درجه صحت:** ✅ Valid

---

### 🚨 Remember

```
⚠️ Rules:
├─ Must have 1 Start
├─ Must have 1 End
├─ All nodes must be connected
└─ No cycles allowed

✅ Workflow ready when:
├─ Green badge visible
├─ All nodes labeled
├─ All connections made
└─ Validation passes
```

---

**Ready? Go ahead and create your first workflow! 🎉**

```
💪 Start → Design → Test → Save → Deploy
```

---

**بیشتر سوالات؟**
📖 مستندات دیگر را بخوانید یا منتظر بمانید!
