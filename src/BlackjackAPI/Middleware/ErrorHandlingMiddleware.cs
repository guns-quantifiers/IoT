using Core.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Threading.Tasks;

namespace BlackjackAPI.Middleware
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlingMiddleware> _logger;

        public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex, _logger);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception ex, ILogger<ErrorHandlingMiddleware> logger)
        {
            logger.LogWarning(ex, ex.Message);
            var code = HttpStatusCode.InternalServerError; // 500 if unexpected

            switch (ex)
            {
                case NotFoundException _:
                    code = HttpStatusCode.NotFound;
                    break;
                case BlackjackBadRequestException _:
                case BadHttpRequestException _:
                case DuplicateKeyException _:
                case DealEndedException _:
                case EmptyHandException _:
                    code = HttpStatusCode.BadRequest;
                    break;
            }

            var result = JsonConvert.SerializeObject(new
            {
                type = ex.GetType().Name,
                error = ex.Message
            }, new JsonSerializerSettings() { Formatting = Formatting.Indented });
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)code;
            return context.Response.WriteAsync(result);
        }
    }
}
