# Child Applications - مستندات

## معرفی
سیستم Child Applications امکان ایجاد و مدیریت اپلیکیشن‌های مستقل را فراهم می‌کند. هر اپلیکیشن روی پورت مجزا اجرا می‌شود و می‌تواند تنظیمات خاص خود را داشته باشد.

## قابلیت‌ها

### 1. ایجاد اپلیکیشن جدید
- نام اپلیکیشن (بدون فاصله و کاراکترهای خاص)
- شماره پورت (پیشنهاد خودکار بین 5000-6000)
- توضیحات
- **انتخاب تمپلیت:**
  - **Empty Web API**: یک پروژه API خالی با Swagger
  - **API + Client App**: یک API با endpoint های تست + یک صفحه HTML کامل

### 2. مشاهده لیست اپلیکیشن‌ها
- نمایش نام، پورت، وضعیت اجرا و تاریخ ایجاد
- فیلتر کردن براساس نام یا توضیحات

### 3. اجرا و توقف اپلیکیشن
- Run: اجرای اپلیکیشن روی پورت مشخص شده
- Stop: توقف اپلیکیشن در حال اجرا
- Open: باز کردن اپلیکیشن در مرورگر

### 4. تنظیمات اپلیکیشن
- تغییر پورت
- ویرایش توضیحات
- تنظیم متغیرهای محیطی (Environment Variables)
- فعال‌سازی اجرای خودکار (Auto Start)

### 5. حذف اپلیکیشن
- حذف کامل اپلیکیشن و فولدر مربوطه

## تمپلیت‌های موجود

### Empty Web API
فایل‌های ایجاد شده:
- `Program.cs` - تنظیمات اولیه ASP.NET Core
- `appsettings.json` - پیکربندی‌های پایه
- `README.md` - راهنمای استفاده

ویژگی‌ها:
- Controllers پشتیبانی
- Swagger UI فعال
- پورت قابل تنظیم

### API + Client App
فایل‌های ایجاد شده:
- `Program.cs` - API با endpoint های تست
- `appsettings.json` - پیکربندی‌های پایه
- `wwwroot/index.html` - یک صفحه HTML زیبا و کاربردی
- `README.md` - راهنمای کامل

API Endpoints:
- `GET /api/hello` - پیام خوشامدگویی
- `GET /api/status` - وضعیت اپلیکیشن

ویژگی‌های صفحه HTML:
- طراحی مدرن و واکنش‌گرا (Responsive)
- نمایش اطلاعات اپلیکیشن (نام، پورت، وضعیت)
- دکمه‌های تست API
- نمایش نتایج به صورت JSON

## ساختار فایل‌ها

### مسیر ذخیره‌سازی
تمام اپلیکیشن‌ها در مسیر زیر ذخیره می‌شوند:
```
{ProjectRoot}/workspace/ChildApps/{AppName}/
```

### فایل پیکربندی
اطلاعات تمام اپلیکیشن‌ها در فایل زیر ذخیره می‌شود:
```
{ProjectRoot}/workspace/childapps.json
```

## API های موجود

### GetAvailablePort
پورت تصادفی خالی بین 5000-6000 پیدا می‌کند.
```javascript
rpcAEP("GetAvailablePort", {}, callback);
```

### GetChildApps
لیست تمام اپلیکیشن‌های فرزند را برمی‌گرداند.

### CreateChildApp
```javascript
rpcAEP("CreateChildApp", {
    "AppName": "MyApp",
    "Port": 5001,
    "Description": "توضیحات",
    "Template": "EmptyWebApi" // or "ApiWithClient"
}, callback);
```

### DeleteChildApp
```javascript
rpcAEP("DeleteChildApp", {
    "AppName": "MyApp"
}, callback);
```

### RunChildApp
```javascript
rpcAEP("RunChildApp", {
    "AppName": "MyApp"
}, callback);
```

### StopChildApp
```javascript
rpcAEP("StopChildApp", {
    "AppName": "MyApp"
}, callback);
```

### GetChildAppConfig
```javascript
rpcAEP("GetChildAppConfig", {
    "AppName": "MyApp"
}, callback);
```

### UpdateChildAppConfig
```javascript
rpcAEP("UpdateChildAppConfig", {
    "AppName": "MyApp",
    "Port": 5002,
    "Description": "توضیحات جدید",
    "AutoStart": true,
    "EnvironmentVariables": "KEY1=VALUE1\nKEY2=VALUE2"
}, callback);
```

## فایل‌های ایجاد شده

### کامپوننت‌های Vue
1. `AppEndHost/workspace/client/AppEndStudio/components/ChildApplications.vue`
   - کامپوننت اصلی برای مدیریت لیست اپلیکیشن‌ها

2. `AppEndHost/workspace/client/AppEndStudio/components/ChildApplicationSettings.vue`
   - کامپوننت تنظیمات هر اپلیکیشن

3. `AppEndHost/workspace/client/AppEndStudio/components/CreateChildAppDialog.vue`
   - Dialog ایجاد اپلیکیشن جدید با انتخاب تمپلیت

### سرویس‌های سرور
1. `AppEndServer/ChildApplicationServices.cs`
   - تمام منطق مدیریت Child Applications
   - ایجاد فایل‌های تمپلیت
   - مدیریت process ها

### Proxy
تغییرات در `AppEndHost/workspace/server/Zzz.AppEndProxy.cs`
- اضافه شدن region جدید: `#region ChildApplicationServices`
- متد `GetAvailablePort` برای پیدا کردن پورت خالی

### منو
تغییرات در `AppEndHost/workspace/client/AppEndStudio/app.json`
- اضافه شدن منوی "Child Applications" زیر "Sub Applications"

## نکات مهم

1. هر اپلیکیشن باید پورت یونیک داشته باشد
2. نام اپلیکیشن نباید شامل فاصله یا کاراکترهای خاص باشد (فقط حروف، اعداد، - و _)
3. متغیرهای محیطی باید به فرمت `KEY=VALUE` باشند (یک خط برای هر متغیر)
4. اپلیکیشن‌های Auto Start به صورت خودکار با شروع برنامه اصلی اجرا می‌شوند
5. قبل از حذف اپلیکیشن، ابتدا متوقف می‌شود
6. پورت پیشنهادی به صورت خودکار و تصادفی انتخاب می‌شود

## نحوه استفاده

1. از منوی "Server" گزینه "Child Applications" را انتخاب کنید
2. برای ایجاد اپلیکیشن جدید روی دکمه "Create App" کلیک کنید
3. در Dialog باز شده:
   - نام اپلیکیشن را وارد کنید (مثلاً: MyTestApp)
   - پورت پیشنهادی یا پورت دلخواه را انتخاب کنید
   - توضیحات را وارد کنید (اختیاری)
   - یکی از تمپلیت‌ها را انتخاب کنید
4. روی "Create Application" کلیک کنید
5. برای اجرا روی دکمه "Run" کلیک کنید
6. با کلیک روی "Open" می‌توانید اپلیکیشن را در مرورگر باز کنید
7. برای تنظیمات بیشتر روی "Settings" کلیک کنید

## مثال استفاده

### ایجاد یک API ساده
1. نام: `TestApi`
2. پورت: `5001` (پیشنهادی خودکار)
3. تمپلیت: `Empty Web API`
4. نتیجه: یک API خالی با Swagger در `http://localhost:5001/swagger`

### ایجاد یک اپلیکیشن کامل
1. نام: `DemoApp`
2. پورت: `5002` (پیشنهادی خودکار)
3. تمپلیت: `API + Client App`
4. نتیجه: 
   - صفحه HTML زیبا در `http://localhost:5002`
   - API endpoints در `http://localhost:5002/api/hello`
   - Swagger در `http://localhost:5002/swagger`
