# 🎯 دستورالعمل اجرای ReactFlow Designer

## ✅ وضعیت فعلی

✅ **npm dependencies نصب شدند**
✅ **React app با موفقیت build شد**
✅ **ASP.NET Core middleware تنظیم شد**

## 🚀 مراحل بعدی

### مرحله 1: Server را Start کنید

**در Visual Studio:**
- `Ctrl+F5` (یا Debug > Start without Debugging)

**یا از Command Line:**
```bash
dotnet run --project AppEndHost/AppEndHost.csproj
```

### مرحله 2: Workflow Manager را باز کنید

1. Browser: `https://localhost:7000/` (یا پورت شما)
2. Workflow Management قسمت را پیدا کنید
3. یک workflow را انتخاب کنید
4. دکمه **"Flow"** (سبز) را کلیک کنید

### مرحله 3: ReactFlow Designer استفاده کنید

- **Nodes** و **Edges** را drag & drop کنید
- **Controls** (توپ راست) برای zoom و pan
- **Save** دکمه برای ذخیره workflow
- **Cancel** برای بستن بدون ذخیره

---

## 📁 ساختار فایل‌ها

```
a.ReactFlow/
├── dist/                          # ✅ Build output (auto-generated)
│   ├── index.html
│   ├── app.js
│   └── index.css
├── src/
│   ├── App.jsx                   # Main React component
│   ├── App.css                   # Styles
│   ├── index.jsx                 # Entry point
│   └── components/
│       └── WorkflowEditor.jsx
├── public/
│   └── index.html
├── package.json
├── vite.config.js
└── index.html                     # Vite entry point
```

---

## 🔧 اگر مشکل پیش آمد

### ❌ White screen یا loading
**راه‌حل**: Browser console (F12) باز کنید و errors را بررسی کنید

### ❌ "Cannot GET /a.ReactFlow/dist/index.html"
**راه‌حل**: 
```bash
cd AppEndHost/workspace/client/a.ReactFlow
npm run build
```
سپس server را دوباره start کنید.

### ❌ React errors
**راه‌حل**: Browser console میدهد مشکل چیست

---

## 🔄 اگر تغییرات داده‌ای بخواهید

### برای development (hot reload):
```bash
cd AppEndHost/workspace/client/a.ReactFlow
npm run dev
# Server در http://localhost:5173 باز می‌شود
```

سپس در `ReactFlowDesigner.vue` تغییر دهید:
```javascript
const baseUrl = "http://localhost:5173/";
```

⚠️ فقط برای development - برای production باید `npm run build` شود!

### برای production build:
```bash
npm run build
```

---

## 📞 مختصر فرمان‌ها

| فرمان | توضیح |
|--------|--------|
| `npm install` | نصب dependencies |
| `npm run build` | Production build |
| `npm run dev` | Development mode |
| `npm run preview` | Preview build output |
| `npm audit fix` | Fix vulnerabilities |

---

## 🔐 امنیت

✅ Origin checking در postMessage API
✅ Same-origin policy رعایت شده
✅ XSS prevention via iframe sandbox

---

## 📝 نکات مهم

1. **dist فولدر را commit نکنید** - `.gitignore` تنظیم شده است
2. **node_modules را commit نکنید** - فقط package.json commit شود
3. **Production:** همیشه `npm run build` کنید قبل deployment

---

## ✨ بیشتر توسعه

اگر می‌خواهید reactive features اضافه کنید:

1. **Custom Node Types**: `src/components/nodes/` فولدر بسازید
2. **Custom Edge Types**: `src/components/edges/` فولدر بسازید
3. **Styling**: `src/App.css` تغییر دهید
4. **Logic**: `src/App.jsx` بهتر کنید

