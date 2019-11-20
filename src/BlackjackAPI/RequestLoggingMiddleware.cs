﻿using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace BlackjackAPI
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

            using var copyOfBody = new MemoryStream();
            context.Response.Body = copyOfBody;
            await _next(context);

            //await context.Response.Body.CopyToAsync(copyOfBody);
            //using var reader = new StreamReader(copyOfBody);

            var response = await FormatResponse(context.Response);
            Debug.WriteLine(response);
            _logger.LogTrace(response);

            copyOfBody.Seek(0, SeekOrigin.Begin);
            await copyOfBody.CopyToAsync(originalBodyStream);
            ////Copy a pointer to the original response body stream
            //var originalBodyStream = context.Response.Body;

            ////Create a new memory stream...
            ////...and use that for the temporary response body
            //context.Response.Body = responseBody;

            ////Continue down the Middleware pipeline, eventually returning to this class
            //await _next(context);

            ////Format the response from the server
            //var response = await FormatResponse(context.Response);

            ////TODO: Save log to chosen datastore
            //_logger.LogTrace(response);
            //Debug.WriteLine(response);
            ////Copy the contents of the new memory stream (which contains the response) to the original stream, which is then returned to the client.
            //await responseBody.CopyToAsync(originalBodyStream);
        }

        //private async Task<string> FormatRequest(HttpRequest request)
        //{
        //    var body = request.Body;

        //    //This line allows us to set the reader for the request back at the beginning of its stream.
        //    request.EnableRewind();

        //    //We now need to read the request stream.  First, we create a new byte[] with the same length as the request stream...
        //    var buffer = new byte[Convert.ToInt32(request.ContentLength)];

        //    //...Then we copy the entire request stream into the new buffer.
        //    await request.Body.ReadAsync(buffer, 0, buffer.Length);

        //    //We convert the byte[] into a string using UTF8 encoding...
        //    var bodyAsText = Encoding.UTF8.GetString(buffer);

        //    //..and finally, assign the read body back to the request body, which is allowed because of EnableRewind()
        //    request.Body = body;

        //    return $"{request.Scheme} {request.Host}{request.Path} {request.QueryString} {bodyAsText}";
        //}

        private async Task<string> FormatRequest(HttpRequest request, StreamReader requestBodyReader)
        {
            var buffer = new char[Convert.ToInt32(request.ContentLength)];
            await requestBodyReader.ReadAsync(buffer, 0, buffer.Length);

            return $"{Environment.NewLine}{request}{Environment.NewLine}{request.Scheme} {request.Host}{request.Path} {request.QueryString} {buffer}{Environment.NewLine}{Environment.NewLine}";
        }

        private async Task<string> FormatResponse(HttpResponse response)
        {
            //We need to read the response stream from the beginning...
            response.Body.Seek(0, SeekOrigin.Begin);

            //...and copy it into a string
            string text = await new StreamReader(response.Body).ReadToEndAsync();

            //We need to reset the reader for the response so that the client can read it.
            response.Body.Seek(0, SeekOrigin.Begin);

            //Return the string for the response, including the status code (e.g. 200, 404, 401, etc.)
            return $"{response.StatusCode}: {text}";
        }
    }
}
