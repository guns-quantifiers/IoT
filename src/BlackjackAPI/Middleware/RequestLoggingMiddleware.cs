using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BlackjackAPI.Middleware
{
    public class RequestResponseLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestResponseLoggingMiddleware> _logger;

        public RequestResponseLoggingMiddleware(RequestDelegate next, ILogger<RequestResponseLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            var originalBodyStream = context.Response.Body;

            await using var copyOfBody = new MemoryStream();
            context.Response.Body = copyOfBody;
            await _next(context);

            string response = await FormatResponse(context.Response);
            Debug.WriteLine(response);
            _logger.LogTrace(response);

            copyOfBody.Seek(0, SeekOrigin.Begin);
            await copyOfBody.CopyToAsync(originalBodyStream);
        }

        private async Task<string> FormatRequest(HttpRequest request, StreamReader requestBodyReader)
        {
            var buffer = new char[Convert.ToInt32(request.ContentLength)];
            await requestBodyReader.ReadAsync(buffer, 0, buffer.Length);

            return $"{Environment.NewLine}{request}{Environment.NewLine}{request.Scheme} {request.Host}{request.Path} {request.QueryString} {buffer}{Environment.NewLine}{Environment.NewLine}";
        }

        private async Task<string> FormatResponse(HttpResponse response)
        {
            response.Body.Seek(0, SeekOrigin.Begin);
            string text = await new StreamReader(response.Body).ReadToEndAsync();
            response.Body.Seek(0, SeekOrigin.Begin);

            return $"Headers: {string.Join("| ", response.Headers.Select(h => h.Key + ": " + h.Value))} {response.StatusCode}: {text}";
        }
    }
}
