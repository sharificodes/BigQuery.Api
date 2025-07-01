using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace BigQuery.Api.Filters
{
    public class ApiKeyActionFilter : ActionFilterAttribute
    {
        public bool Optional { get; set; }

        public ApiKeyActionFilter(bool optional = false)
        {
            Optional = optional;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var config = context.HttpContext.RequestServices.GetService<IConfiguration>();

            config = config ?? throw new ArgumentNullException(nameof(config));

            if (!context.HttpContext.Request.Headers.TryGetValue("X-API-KEY", out var apiKey))
            {
                context.Result = new BadRequestObjectResult("API key missing");
                return;
            }

            if (apiKey != config["private_key_id"])
            {
                context.Result = new BadRequestObjectResult("Invalid API key");
                return;
            }

            base.OnActionExecuting(context);
        }
    }
}
