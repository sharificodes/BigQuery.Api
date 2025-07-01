# BigQuery.Api

کتابخانه‌ای ساده برای اتصال و کار با Google BigQuery در پروژه‌های C# / .NET با تمرکز بر سهولت استفاده، سادگی پیکربندی و عملیات متداول مانند اجرای Query، ایجاد دیتاست و جدول و درج داده.

---

## ✅ ویژگی‌ها

- اتصال آسان به Google BigQuery با استفاده از JSON فایل Service Account  
- اجرای Queryهای ساده و پیچیده  
- ایجاد دیتاست و جدول با تعریف schema  
- درج داده‌ها با استفاده از Streaming Insert  
- دریافت نتیجه‌ی Query به‌صورت داینامیک  

---

## ⚙️ پیش‌نیازها

قبل از استفاده، موارد زیر را آماده کنید:

1. **Google Cloud Project** با فعال‌سازی API مربوط به BigQuery  
2. **Service Account JSON Key** برای احراز هویت  
3. نصب کتابخانه از طریق NuGet:

```bash
dotnet add package BigQuery.Api
🧪 نحوه استفاده
1. ایجاد نمونه‌ای از کلاینت
csharp
Copy
Edit
var client = new BigQueryClient(
    projectId: "your-project-id",
    credentialsJsonPath: "path/to/service-account.json"
);
اگر فایل credentialsJsonPath داده نشود، از Application Default Credentials استفاده می‌کند.

2. اجرای Query
csharp
Copy
Edit
var result = await client.ExecuteQueryAsync("SELECT * FROM `dataset.table`");

foreach (var row in result.Rows)
{
    Console.WriteLine(row["column_name"]);
}
3. ایجاد دیتاست
csharp
Copy
Edit
await client.CreateDatasetAsync("your_dataset");
4. ایجاد جدول با Schema
csharp
Copy
Edit
var schema = new List<BigQueryField>
{
    new("id", "INTEGER"),
    new("name", "STRING"),
    new("created_at", "TIMESTAMP")
};

await client.CreateTableAsync("your_dataset", "your_table", schema);
5. درج داده در جدول
csharp
Copy
Edit
var rows = new[]
{
    new Dictionary<string, object>
    {
        ["id"] = 1,
        ["name"] = "Alice",
        ["created_at"] = DateTime.UtcNow
    }
};

await client.InsertRowsAsync("your_dataset", "your_table", rows);
📌 نکات
ستون‌ها باید مطابق با schema جدول باشند.

نوع داده‌ها باید با نوع تعریف‌شده در BigQuery هم‌خوانی داشته باشد.

تابع ExecuteQueryAsync به‌صورت job اجرا می‌شود و ممکن است با تاخیر همراه باشد.

📁 ساختار پروژه
BigQueryClient.cs: کلاس اصلی برای مدیریت اتصال و عملیات‌ها

BigQueryResult.cs: ساختار داده بازگشتی از کوئری‌ها

BigQueryField.cs: تعریف فیلدهای جدول

🧩 نمونه کامل
csharp
Copy
Edit
var client = new BigQueryClient("project-id", "credentials.json");

await client.CreateDatasetAsync("sample_dataset");

var schema = new List<BigQueryField>
{
    new("id", "INTEGER"),
    new("value", "STRING")
};
await client.CreateTableAsync("sample_dataset", "sample_table", schema);

await client.InsertRowsAsync("sample_dataset", "sample_table", new[]
{
    new Dictionary<string, object> {
        ["id"] = 123,
        ["value"] = "Test"
    }
});

var result = await client.ExecuteQueryAsync("SELECT * FROM `project-id.sample_dataset.sample_table`");
foreach (var row in result.Rows)
{
    Console.WriteLine($"{row["id"]}: {row["value"]}");
}
