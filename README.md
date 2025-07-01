معرفی
ریپوزیتوری BigQuery.Api یک کلاینت ساده برای کار با Google BigQuery از زبان C# (یا احتمالا .NET) است. هدف آن آسان‌سازی عملیات رایج مانند ایجاد دیتاست، جدول، ارسال Query و دریافت نتایج است، بدون نیاز به نوشتن مستقیم REST API.

 پیش‌نیازها
اکانت Google Cloud با پروژه فعال.

فعال‌سازی BigQuery API در GCP Console 
github.com
+11
cloud.google.com
+11
console.cloud.google.com
+11
.

کریدنشال:

استفاده از Service Account و دانلود فایل JSON.

یا تنظیم Application Default Credentials via gcloud auth application-default login 
apidog.com
+2
cloud.google.com
+2
github.com
+2
.

 نصب
با استفاده از NuGet (در پروژه دات‌نت):

bash
Copy
Edit
dotnet add package BigQuery.Api
یا در فایل .csproj:

xml
Copy
Edit
<PackageReference Include="BigQuery.Api" Version="x.y.z" />
(نسخه‌ دقیق رو از قسمت Releases یا NuGet ببین و جایگزین کن)

 نحوه استفاده
1. پیکربندی و احراز هویت
csharp
Copy
Edit
using BigQuery.Api;

var client = new BigQueryClient(
    projectId: "my-gcp-project",
    credentialsJsonPath: "/مسیر/به/credentials.json"
);
یا اگر Application Default Credentials فعال باشد، کافیست credentialsJsonPath را خالی بگذاری.

2. ایجاد دیتاست
csharp
Copy
Edit
await client.CreateDatasetAsync("my_dataset");
اگر قبلا وجود داشته باشد، بررسی و جلوگیری از خطا انجام می‌شود.

3. ایجاد جدول
csharp
Copy
Edit
var schema = new List<BigQueryField> {
    new("id", "INTEGER"),
    new("name", "STRING"),
    new("created_at", "TIMESTAMP")
};
await client.CreateTableAsync("my_dataset", "user_table", schema);
4. اجرای Query
csharp
Copy
Edit
string sql = @"
  SELECT id, name
  FROM `my-gcp-project.my_dataset.user_table`
  WHERE created_at > TIMESTAMP_SUB(CURRENT_TIMESTAMP(), INTERVAL 7 DAY)
";
var result = await client.ExecuteQueryAsync(sql);

foreach (var row in result.Rows)
{
    Console.WriteLine($"ID={row["id"]}, Name={row["name"]}");
}
5. بارگذاری داده (Streaming Insert)
برای درج مقادیر جدید:

csharp
Copy
Edit
var rows = new[] {
    new Dictionary<string, object> {
        ["id"] = 1,
        ["name"] = "Alice",
        ["created_at"] = DateTime.UtcNow
    }
};
await client.InsertRowsAsync("my_dataset", "user_table", rows);
✅ نکات و بهترین شیوه‌ها
تایپ داده‌ها: نوع فیلدها باید با schema تعریف‌شده مطابقت داشته باشد.

Retry‌ها و خطاها: استفاده از سیاست‌های retry داخلی کلاینت برای جلوگیری از شکست‌های موقتی.

Job vs Streaming: Queryها به صورت Job اجرا می‌شود و بارگذاری (InsertRowsAsync) داده، بهصورت streaming انجام می‌شود.

نرخ و هزینه: BigQuery هزینه براساس حجم داده پردازش‌شده محاسبه می‌شود؛ پس کوئری بهینه اهمیت زیادی دارد .

 نمونهٔ کامل برنامه
csharp
Copy
Edit
public async Task RunExampleAsync()
{
    var client = new BigQueryClient("my-project", "./creds.json");

    await client.CreateDatasetAsync("test_ds");
    var schema = new List<BigQueryField> {
        new("id", "INTEGER"),
        new("value", "STRING")
    };
    await client.CreateTableAsync("test_ds", "tbl", schema);

    await client.InsertRowsAsync("test_ds", "tbl", new[]{
        new Dictionary<string, object>{
            ["id"] = 42,
            ["value"] = "Hello!"
        }
    });

    var res = await client.ExecuteQueryAsync(
      $"SELECT id, value FROM `{client.Project}.{ "test_ds" }.tbl`"
    );
    foreach (var r in res.Rows)
        Console.WriteLine($"{r["id"]}: {r["value"]}");
}
