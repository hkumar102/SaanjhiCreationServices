using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Shared.ErrorHandling;

public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task Invoke(HttpContext context)
    {
        // Capture request headers
        var requestHeaders = JsonSerializer.Serialize(context.Request.Headers);

        // Capture request body
        string requestBody = string.Empty;
        context.Request.EnableBuffering();
        if (context.Request.ContentLength > 0 && context.Request.Body.CanRead)
        {
            context.Request.Body.Position = 0;
            using var reader = new StreamReader(context.Request.Body, Encoding.UTF8, leaveOpen: true);
            requestBody = await reader.ReadToEndAsync();
            context.Request.Body.Position = 0;
        }

        // Capture response body
        var originalBodyStream = context.Response.Body;
        using var responseBody = new MemoryStream();
        context.Response.Body = responseBody;

        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            context.Response.Body.Seek(0, SeekOrigin.Begin);
            string responseBodyText = await new StreamReader(context.Response.Body).ReadToEndAsync();
            context.Response.Body.Seek(0, SeekOrigin.Begin);

            using (logger.BeginScope(new Dictionary<string, object>
                   {
                       ["RequestHeaders"] = requestHeaders,
                       ["RequestBody"] = requestBody,
                       ["ResponseBody"] = responseBodyText
                   }))
            {
                logger.LogError(ex, "Unhandled exception occurred");
            }

            var response = context.Response;
            response.ContentType = "application/json";

            var errorResponse = new ErrorResponse();
            response.StatusCode = errorResponse.StatusCode = ex switch
            {
                KeyNotFoundException => (int)HttpStatusCode.NotFound,
                UnauthorizedAccessException => (int)HttpStatusCode.Unauthorized,
                ArgumentException => (int)HttpStatusCode.BadRequest,
                _ => (int)HttpStatusCode.InternalServerError
            };

            errorResponse.Message = ex.Message;
            errorResponse.Details = ex.StackTrace;

            var result = JsonSerializer.Serialize(errorResponse);
            await response.WriteAsync(result);
        }
        finally
        {
            context.Response.Body.Seek(0, SeekOrigin.Begin);
            await responseBody.CopyToAsync(originalBodyStream);
            context.Response.Body = originalBodyStream;
        }
    }
}