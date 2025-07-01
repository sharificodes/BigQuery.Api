using BigQuery.Api.Configs;
using BigQuery.Api.Filters;
using BigQuery.Api.Helpers;
using BigQuery.Api.Services;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.BigQuery.V2;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);


if (!builder.Environment.IsDevelopment())
{
    builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
}
builder.Services.AddProblemDetails();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddTransient<IBQ, BQ>();
builder.Services.SwaggerConfiguration();
builder.Services.AddMvc(options =>
{
    options.Filters.Add(new ApiKeyActionFilter());
}).AddJsonOptions(opts =>
{
    var enumConverter = new JsonStringEnumConverter();
    opts.JsonSerializerOptions.Converters.Add(enumConverter);
});

builder.Configuration.SetBasePath(builder.Environment.ContentRootPath)
    .AddJsonFile("bq-secrets.json", optional: true, reloadOnChange: true);

ServiceAccountCredential serviceAccountCredential = null;
builder.Services.AddScoped(sp =>
{
    var config = Path.Combine(builder.Environment.ContentRootPath, "bq-secrets.json");

    GoogleCredential credential = null;
    using (var jsonStream = new FileStream(config, FileMode.Open, FileAccess.Read, FileShare.Read))
        credential = GoogleCredential.FromStream(jsonStream);

    //ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11;

    serviceAccountCredential = (ServiceAccountCredential)credential.UnderlyingCredential;

    return BigQueryClient.Create(serviceAccountCredential.ProjectId, credential);
});

builder.Services.AddOptions<ServiceAccountCredential>();

var app = builder.Build();
var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();

app.UseSwagger(options => options.RouteTemplate = "swagger/{documentName}/swagger.json");
app.UseSwaggerUI(o =>
{
    foreach (var description in provider.ApiVersionDescriptions)
    {
        o.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json",
            $"{description.GroupName.ToUpper()}");
    }
});

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseExceptionHandler();
}

app.UseAuthorization();

app.MapControllers();

app.Run();


