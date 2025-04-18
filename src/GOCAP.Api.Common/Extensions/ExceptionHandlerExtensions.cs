using FluentValidation;
using GOCAP.Api.Model;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using System.Net;
using System.Net.Mime;
using System.Text;
using System.Text.Json;

namespace GOCAP.Api.Common;

public static class ExceptionHandlerExtensions
{
    public static IApplicationBuilder UseCustomExceptionHandler(this IApplicationBuilder app)
    {
        app.UseExceptionHandler(new ExceptionHandlerOptions
        {
            AllowStatusCode404Response = true,
            ExceptionHandler = async context =>
            {
                context.Response.OnStarting(PopulateSecurityHeaders, context);
                var exception = context.Features.Get<IExceptionHandlerFeature>()?.Error;
                var loggerFactory = app.ApplicationServices.GetService<ILoggerFactory>();
                var logger = loggerFactory?.CreateLogger("GlobalExceptionHandler");
                logger?.LogError("Unexpected error has occurred, details: {exception}", exception);

                var errorModel = new ErrorModel
                {
                    ErrorMessage = "Unexpected error has occurred.",
                    ErrorCode = (int)ErrorCode.InternalError
                };

                if (exception is ValidationException validationException)
                {
                    errorModel.ErrorMessage = "Validation failed.";
                    errorModel.ErrorCode = (int)ErrorCode.InvalidRequest;
                    errorModel.ErrorDetails = [.. validationException.Errors.Select(error => error.ErrorMessage)];
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                }
                else if (exception is ApiExceptionBase apiException)
                {
                    errorModel.ErrorMessage = apiException.Message;
                    errorModel.ErrorCode = apiException.ErrorCode;
                    context.Response.StatusCode = (int)apiException.StatusCode;
                }

                context.Response.ContentType = MediaTypeNames.Application.Json;
                var responseText = JsonSerializer.Serialize(errorModel);
                context.Response.ContentLength = Encoding.UTF8.GetByteCount(responseText);
                await context.Response.WriteAsync(responseText);
            }
        });

        return app;
    }

    private static async Task PopulateSecurityHeaders(object state)
    {
        var response = ((HttpContext)state).Response;

        // Prevent MIME type sniffing
        response.Headers.XContentTypeOptions = new StringValues("nosniff");

        // Prevent Clickjacking attacks
        response.Headers.XFrameOptions = new StringValues("SAMEORIGIN");

        // Content Security Policy - Restricts frame embedding
        response.Headers.ContentSecurityPolicy = new StringValues("frame-ancestors 'self'");

        // Permissions Policy - Controls access to browser APIs
        response.Headers["Permissions-Policy"] = new StringValues("geolocation=(self), microphone=()");

        // Referrer Policy - Limits referrer information sent
        response.Headers["Referrer-Policy"] = new StringValues("strict-origin-when-cross-origin");

        // Enforce HTTPS and prevent downgrade attacks
        response.Headers.StrictTransportSecurity = new StringValues("max-age=31536000; includeSubDomains; preload");

        // XSS Protection for older browsers
        response.Headers.XXSSProtection = new StringValues("1; mode=block");

        // Prevent caching of sensitive data
        response.Headers.CacheControl = new StringValues("no-store, no-cache, must-revalidate, max-age=0");
        response.Headers.Pragma = new StringValues("no-cache");
        response.Headers.Expires = new StringValues("0");

        await Task.CompletedTask;
    }
}
