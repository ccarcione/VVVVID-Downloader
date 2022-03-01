using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;
using SimpleExceptionHandling;
using System.Net;

namespace VVVVID_Downloader.WebApi
{
    public class GlobalExceptionFilter : IExceptionFilter
    {
        private readonly ILogger<GlobalExceptionFilter> _logger;

        public GlobalExceptionFilter(ILogger<GlobalExceptionFilter> logger)
        {
            _logger = logger;
        }

        public void OnException(ExceptionContext context)
        {
            int statusCode = 0;
            string message = null;

            ModelStateDictionary modelState = context.ModelState;   // contiene anche query param
            string httpMethod = context.HttpContext.Request.Method;
            string controllerName = context.HttpContext.Request.Path.Value.Split('/')[2];

            IHandlingResult<object> result =
                Handling.Prepare()
                .Catch(context.Exception, throwIfNotHandled: false);

            if (!result.Handled)
            {
                message = "Exception unknown has been thrown";
                statusCode = (int)HttpStatusCode.InternalServerError;
            }

            context.HttpContext.Response.StatusCode = statusCode;
            context.Result = new ObjectResult(message);
        }
    }
}
