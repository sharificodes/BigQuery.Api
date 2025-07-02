

## معرفی کلی

سرویس BigQuery.Api یک API مبتنی بر .NET است که دسترسی به داده‌های Google BigQuery را از طریق REST API فراهم می‌کند. این پروژه دارای کنترلرهای تخصصی برای مدیریت رویدادها و crashlytics فایربیس است.

---

## کنترلرها

### 1. FirebaseCrashlyticController

آدرس پایه:  
`/api/v{version}/firebase-crashlytic`

#### متدها:

- **GET fields**  
  مسیر: `/fields`  
  دریافت فیلدهای جدول crashlytics (به جز event_timestamp).
  ```http
  GET /api/v1.0/firebase-crashlytic/fields
  ```
  خروجی:  
  ```json
  {
    "success": true,
    "data": ["field1", "field2", ...]
  }
  ```

- **POST query**  
  مسیر: `/query`  
  اجرای جستجوی شرطی روی داده‌های crashlytics با پارامترهای دلخواه (فیلتر تاریخ، کاربر و ...).
  ```http
  POST /api/v1.0/firebase-crashlytic/query
  Content-Type: application/json

  {
    "fields": ["field1", "field2"],
    "parameters": [{"key": "col", "operator": "Equals", "value": "val"}],
    "fromDate": "2024-05-01",
    "toDate": "2024-05-15",
    "userId": "123"
  }
  ```
  خروجی:  
  ```json
  {
    "success": true,
    "data": [...],
    "executionTime": "00:00:00.123"
  }
  ```

- **GET query**  
  مسیر: `/query`  
  اجرای مستقیم یک query متنی روی داده‌های crashlytics.
  ```http
  GET /api/v1.0/firebase-crashlytic/query?query=SELECT * FROM ...
  ```
  خروجی:  
  ```json
  {
    "success": true,
    "data": [...]
  }
  ```

---

### 2. FirebaseEventController

آدرس پایه:  
`/api/v{version}/firebase-event`

#### متدها:

- **GET fields**  
  مسیر: `/fields`  
  دریافت فیلدهای جدول event فایربیس.
  ```http
  GET /api/v1.0/firebase-event/fields
  ```
  خروجی مشابه بالا.

- **POST query**  
  مسیر: `/query`  
  اجرای جستجوی شرطی روی داده‌های event با پارامترهای دلخواه (فیلتر تاریخ، کاربر و ...).
  ```http
  POST /api/v1.0/firebase-event/query
  Content-Type: application/json

  {
    "fields": ["event_name", "user_id"],
    "parameters": [{"key": "event_name", "operator": "Equals", "value": "login"}],
    "fromDate": "2024-05-01",
    "toDate": "2024-05-10",
    "userId": "123"
  }
  ```
  خروجی مشابه بالا.

- **GET query**  
  مسیر: `/query`  
  اجرای مستقیم یک query متنی روی داده‌های event.
  ```http
  GET /api/v1.0/firebase-event/query?query=SELECT * FROM ...
  ```
  خروجی مشابه بالا.

---

## ساختار ورودی و خروجی

- **FirebaseCrashlyticQueryDto** و **QueryDto** مدل‌های اصلی ورودی هستند که شامل:  
  - fields: لیست فیلدهای مورد نیاز  
  - parameters: لیست پارامترها به صورت کلید/عملگر/مقدار  
  - fromDate / toDate: تاریخ شروع و پایان (اختیاری)  
  - userId: شناسه کاربر (اختیاری)  
  - query: (در GET query) متن کامل کوئری SQL

- **Result** ساختار خروجی اصلی تمام متدها بوده و در صورت موفقیت شامل success=true و data و در صورت خطا، success=false و error/details خواهد بود.

---

## نمونه استفاده (curl)

```bash
curl -X GET "http://localhost:5000/api/v1.0/firebase-crashlytic/fields"
curl -X POST "http://localhost:5000/api/v1.0/firebase-event/query" \
  -H "Content-Type: application/json" \
  -d '{"fields":["event_name"],"parameters":[],"fromDate":"2024-05-01","toDate":"2024-05-10"}'
curl -G "http://localhost:5000/api/v1.0/firebase-crashlytic/query" --data-urlencode "query=SELECT * FROM ..."
```

---

## سایر نکات پروژه

- **پیکربندی**:  
  اطلاعات اتصال به GCP و BigQuery باید در فایل appsettings.json یا bq-secrets.json قرار گیرد.

- **امنیت**:  
  استفاده از ApiKeyActionFilter باعث الزام کلاینت به ارسال API Key در header می‌شود.

- **Swagger**:  
  مستندات کامل و امکان تست endpointها از طریق Swagger UI فراهم است (معمولاً روی http://localhost:5000/swagger).

- **توسعه‌دهندگان**:  
  افزودن endpoint جدید در کنترلرهای مربوطه و افزودن منطق آن در سرویس IBQ و BQ انجام می‌شود.  
  مدل‌ها در پوشه Models و فیلترها در Filters قرار دارند.

---

## مدیریت خطاها

در صورت بروز خطا، خروجی به شکل زیر خواهد بود:
```json
{
  "success": false,
  "error": "شرح خطا",
  "details": "جزئیات"
}
```

---
