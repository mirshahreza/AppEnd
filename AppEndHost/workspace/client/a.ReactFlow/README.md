# ReactFlow Designer - آموزش ساختار پروژه

## ساختار فولدر

```
a.ReactFlow/
├── public/
│   └── index.html              # HTML entry point
├── src/
│   ├── components/
│   │   └── WorkflowEditor.jsx  # Workflow editor component
│   ├── App.jsx                 # Main React app
│   ├── App.css                 # App styles
│   ├── index.jsx               # React root
│   └── [...فایل‌های دیگر]
├── package.json                # Dependencies
├── vite.config.js              # Vite configuration
└── README.md                   # This file
```

## راه اندازی و Build

### نصب Dependencies
```bash
cd AppEndHost/workspace/client/a.ReactFlow
npm install
```

### Development Mode
```bash
npm run dev
# Server در http://localhost:5173 راه‌اندازی می‌شود
```

### Production Build
```bash
npm run build
# Output در `dist` فولدر ایجاد می‌شود
```

## ارتباط Vue و React

### آرکیتکچر ارتباط:
1. **Vue Component** (ReactFlowDesigner.vue) یک iframe سازمان‌دهی می‌کند
2. **iframe** React app (a.ReactFlow) را load می‌کند
3. **postMessage API** برای دو‌طرفه ارتباط استفاده می‌شود

### Message Types:

**Vue → React:**
- `LOAD_WORKFLOW`: workflow را بار گذاری کنید
- `GET_WORKFLOW`: workflow فعلی را بازگردانید

**React → Vue:**
- `WORKFLOW_LOADED`: workflow سفلی load شد
- `WORKFLOW_CHANGED`: workflow تغییر کرد
- `ERROR`: خطایی رخ داد

## فایل‌های اصلی

### App.jsx
- Setup ارتباط postMessage
- مدیریت nodes و edges ReactFlow
- دریافت اطلاعات workflow از URL parameters

### ReactFlowDesigner.vue
- Iframe management
- دریافت و ارسال اطلاعات workflow
- Buttons برای Save/Cancel

## توجهات امنیتی

- ✅ Origin checking برای تمام messages
- ✅ Same-origin policy رعایت شده
- ✅ XSS prevention via postMessage

## مراحل بعدی

1. [ ] واقعی RPC backend integration برای fetch/save workflow
2. [ ] Node types پیشرفته (triggers, actions, conditions)
3. [ ] Workflow validation
4. [ ] Version history support
5. [ ] Collaborative editing
