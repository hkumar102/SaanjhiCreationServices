// Shared/ErrorHandling/ExceptionHandlingMiddleware.cs

using System;
using System.Collections.Generic;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Shared.ErrorHandling;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
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
            _logger.LogError(ex, "Unhandled exception occurred");

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
#if DEBUG
            errorResponse.Details = ex.StackTrace;
#endif

            var result = JsonSerializer.Serialize(errorResponse);
            await response.WriteAsync(result);
        }
    }
}