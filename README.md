
```markdown
# راهنمای کامل استفاده از سرویس BigQuery API

## معرفی
این ریپوزیتوری یک سرویس API برای کار با Google BigQuery ارائه می‌دهد که امکان اجرای کوئری‌ها و مدیریت داده‌ها را از طریق REST API فراهم می‌کند.

## پیش‌نیازها
- .NET Core 3.1 یا بالاتر
- حساب Google Cloud Platform
- پروژه BigQuery
- فایل Credential سرویس حساب

## نصب و راه‌اندازی
```bash
git clone https://github.com/sharificodes/BigQuery.Api.git
cd BigQuery.Api
dotnet restore
dotnet build
```

## پیکربندی
فایل `appsettings.json` را با مقادیر خود تنظیم کنید:
```json
{
  "BigQuerySettings": {
    "CredentialFilePath": "path/to/credentials.json",
    "ProjectId": "your-project-id",
    "DatasetName": "your-dataset-name"
  }
}
```

## اجرای سرویس
```bash
dotnet run
```

## Endpointهای API

### 1. اجرای کوئری ساده
```
POST /api/query
Content-Type: application/json

{
  "query": "SELECT * FROM `project.dataset.table` LIMIT 10"
}
```

### 2. اجرای کوئری پارامتری
```
POST /api/query/parameterized
Content-Type: application/json

{
  "query": "SELECT * FROM table WHERE field = @param",
  "parameters": {
    "param": "value"
  }
}
```

### 3. دریافت اطلاعات دیتاست
```
GET /api/dataset/info
```

## مثال‌های استفاده

### با curl:
```bash
curl -X POST http://localhost:5000/api/query \
  -H "Content-Type: application/json" \
  -d '{"query":"SELECT * FROM mydataset.mytable LIMIT 5"}'
```

### با C#:
```csharp
var client = new HttpClient();
var response = await client.PostAsJsonAsync("http://localhost:5000/api/query", new {
    query = "SELECT name, age FROM users WHERE age > 18"
});
var result = await response.Content.ReadAsStringAsync();
```

## ساختار پاسخ
پاسخ‌ها به صورت JSON با ساختار زیر برگردانده می‌شوند:
```json
{
  "success": true,
  "data": [...],
  "executionTime": "00:00:00.123"
}
```

## خطاها
خطاها با کد وضعیت HTTP مناسب و بدنه زیر برگردانده می‌شوند:
```json
{
  "error": "Invalid query",
  "details": "Syntax error at line 1:15..."
}
```

## توسعه‌دهندگان
- برای اضافه کردن Endpoint جدید، به `Controllers/BigQueryController.cs` مراجعه کنید.
- منطق کسب‌وکار در `Services/BigQueryService.cs` قرار دارد.
- مدل‌های داده در پوشه `Models` قابل مشاهده هستند.

## مجوز
این پروژه تحت مجوز MIT منتشر شده است.
```
