# Workflow Designer - پلن اجرایی تکمیل

## 📋 خلاصه پروژه

یک **Workflow Designer** پیشرفته شامل:
- **Canvas SVG** برای رسم workflow گرافیکی
- **12+ Node Type** با دسته‌بندی
- **Undo/Redo** سیستم
- **Drag & Drop** رابط کاربری
- **Properties Panel** دینامیک
- **Validation** خودکار

---

## 📁 ساختار فایل‌ها

```
AppEndStudio/workflows/
├── WorkflowEditor.vue                          # کامپوننت اصلی
├── components/
│   └── DesignerCanvas.vue                      # کامپوننت SVG Canvas
├── lib/
│   ├── workflowBuilder.js                      # Logic و History Management
│   ├── nodeTypes.js                            # Node Types و Categories
│   └── connectionManager.js                    # (آینده) Connection Logic
└── assets/
    └── workflow-designer.css                   # Custom Styles
```

---

## 🎯 Phases اجرا شده

### ✅ **فاز 1: زیرساخت‌های اصلی**
- [x] WorkflowBuilder class کامل‌تر شدن
- [x] Validation System
- [x] History Management (Undo/Redo)
- [x] Node و Connection Management

### ✅ **فاز 2: Node Types پیشرفته**
- [x] 20+ Node Type تعریف شده
- [x] 12 Category (Control, Branching, Looping, etc.)
- [x] Helper functions برای Node Management
- [x] Dynamic Node Rendering

### ✅ **فاز 3: DesignerCanvas بهبود‌شده**
- [x] SVG-based rendering
- [x] Color coding برای Node Types
- [x] Icons برای هر Node Type
- [x] Shape متفاوت (Circle, Rectangle, Diamond)
- [x] Drag & Drop

### ✅ **فاز 4: Properties Panel دینامیک**
- [x] Node-type specific properties
- [x] Configuration fields
- [x] Metadata editing
- [x] Position display

### ✅ **فاز 5: Styling و UX**
- [x] Custom CSS برای Designer
- [x] Responsive Layout
- [x] Animation و Transitions
- [x] Color schemes

---

## 🔌 Node Types موجود

### Control Flow
- `START` - شروع workflow (سبز)
- `END` - پایان workflow (قرمز)
- `TRY_CATCH` - مدیریت خطا

### Branching
- `DECISION` - تصمیم‌گیری (زرد)
- `IF_ELSE` - شرط if/else
- `SWITCH` - switch/case
- `FLOWCHART_DECISION` - Diamond shape

### Looping
- `FOR_LOOP` - حلقه for
- `FOREACH_LOOP` - حلقه foreach
- `WHILE_LOOP` - حلقه while
- `PARALLEL_LOOP` - حلقه موازی
- `BREAK` - خروج از حلقه

### Communication & Integration
- `HTTP_REQUEST` - درخواست HTTP
- `EMAIL` - ارسال ایمیل
- `DATABASE_QUERY` - Query دیتابیس
- `WORKFLOW_INVOKE` - فراخوانی workflow دیگر

### Execution
- `SCRIPT` - اجرای script
- `CONSOLE_LOG` - log کردن
- `DELAY` - تأخیر
- `TIMER` - تایمر

### Data Management
- `ASSIGN_VARIABLE` - تخصیص متغیر
- `VARIABLE_COUNTER` - شمارنده

### Primitives
- `TASK` - Task عمومی

---

## 💻 API و Functions

### WorkflowBuilder Class

```javascript
// ایجاد builder
const builder = new WorkflowBuilder(workflowData);

// Node Management
builder.addNode(nodeType, position)          // اضافه کردن node
builder.deleteNode(nodeId)                   // حذف node
builder.updateNode(node)                     // اپدیت node
builder.moveNode(nodeId, position)           // حرکت دادن node

// Connection Management
builder.addConnection(fromId, toId, label)   // اتصال ایجاد
builder.deleteConnection(connId)             // حذف اتصال
builder.updateConnection(connId, updates)    // اپدیت اتصال

// History
builder.undo()                               // برگشت
builder.redo()                               // جلو
builder.canUndo()                            // می‌تواند برگشت؟
builder.canRedo()                            // می‌تواند جلو؟

// Validation
builder.validateWorkflow()                   // بررسی validity

// Get Data
builder.getWorkflow()                        // دریافت workflow
builder.updateMetadata(metadata)             // اپدیت metadata
```

### Node Types API

```javascript
// دریافت یک Node Type
const nodeType = window.getNodeType(typeId);

// Node Type Properties
{
  id: 'task',
  type: 'task',
  label: 'Task',
  category: 'primitives',
  icon: 'fa-solid fa-square',
  color: '#007bff',
  description: 'Execute a task or action',
  shape: 'rectangle'  // circle, rectangle, diamond
}

// دریافت تمام Nodes یک Category
const nodes = window.getNodesByCategory('looping');

// دریافت تمام Categories
const categories = window.getAllCategories();
```

---

## 🎨 Rendering

### Shape Types
- **Circle**: Start, End nodes
- **Diamond**: Decision nodes
- **Rectangle**: All other types

### Colors per Category
- **Control**: #28a745 (سبز) / #dc3545 (قرمز)
- **Branching**: #ffc107 (زرد)
- **Looping**: #6f42c1 (بنفش)
- **HTTP**: #e83e8c (گلابی)
- **Storage**: #0dcaf0 (فیروزه‌ای)
- **Scripting**: #9b59b6 (بنفش)

---

## 📝 Keyboard Shortcuts

- `Ctrl+Z` - Undo
- `Ctrl+Y` / `Ctrl+Shift+Z` - Redo
- `Delete` - Delete selected node
- `Drag` - Move node / Create connection
- `Click` - Select node/connection

---

## 🔧 Configuration Examples

### HTTP Request Node
```javascript
{
  type: 'http_request',
  label: 'Get User',
  url: 'https://api.example.com/users/1',
  method: 'GET'
}
```

### Decision Node
```javascript
{
  type: 'decision',
  label: 'Check Status',
  condition: "response.status === 'approved'"
}
```

### Loop Node
```javascript
{
  type: 'foreach_loop',
  label: 'Process Items',
  configuration: '{"array": "items", "variable": "item"}'
}
```

---

## 🧪 Testing

برای تست کردن:

1. باز کردن WorkflowEditor component
2. Drag Node از palette به canvas
3. اتصال nodes توسط port‌ها
4. تغییر properties از panel
5. Save کردن workflow

---

## 📌 Future Enhancements (آینده)

- [ ] Custom node types
- [ ] Nested workflows
- [ ] Node grouping
- [ ] Advanced connection routing
- [ ] Performance optimization
- [ ] Export/Import formats (BPMN, etc.)
- [ ] Collaboration features
- [ ] Analytics dashboard

---

## 🐛 Known Issues & Fixes Needed

- [ ] Safari SVG rendering optimization
- [ ] Mobile touch support
- [ ] Large workflow performance
- [ ] Connection label positioning

---

## 📚 اسناد و منابع

- **Copilot Instructions**: `.github/copilot-instructions.md`
- **Target Framework**: .NET 10
- **UI Framework**: Vue 3 + Bootstrap 5
- **Icons**: FontAwesome

---

**آخرین بروزرسانی**: امروز
**وضعیت**: ✅ آماده برای آزمایش و توسعه بیشتر
