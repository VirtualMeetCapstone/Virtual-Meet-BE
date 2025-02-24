using FluentValidation;
using GOCAP.Api.Model;
using GOCAP.Common;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
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
        app.UseExceptionHandler(errorApp =>
        {
            errorApp.Run(async context =>
            {
                context.Response.OnStarting(PopulateSecurityHeaders, context);
                var exception = context.Features.Get<IExceptionHandlerFeature>()?.Error;
                var loggerFactory = app.ApplicationServices.GetService<ILoggerFactory>();
                var logger = loggerFactory?.CreateLogger("GlobalExceptionHandler");
                logger?.LogError("Unexpected error has occurred, details: {exception}", exception);

                var errorModel = new ErrorModel
                {
                    ErrorMessage = "Unexpected error has occurred.",
                    ErrorCode = (int)ErrorCode.InternalError,
                    ErrorDetails = ["An internal server error occurred. Please try again later."]
                };
                if (exception is not null && exception is ValidationException validationException)
                {
                    errorModel.ErrorMessage = "Validation failed.";
                    errorModel.ErrorCode = (int)ErrorCode.InvalidRequest;
                    errorModel.ErrorDetails = validationException.Errors.Select(error => error.ErrorMessage).ToList();
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                }
                if (exception is not null && exception is ApiExceptionBase apiException)
                {
                    errorModel.ErrorMessage = apiException.Message;
                    errorModel.ErrorCode = apiException.ErrorCode;
                    context.Response.StatusCode = (int)apiException.StatusCode;
                }
                context.Response.ContentType = MediaTypeNames.Application.Json;
                var responseText = JsonSerializer.Serialize(errorModel);
                context.Response.ContentLength = Encoding.UTF8.GetByteCount(responseText);
                await context.Response.WriteAsync(responseText);
            });
        });
        return app;
    }

    private static async Task PopulateSecurityHeaders(Object state)
    {
        var response = ((HttpContext)state).Response;
        response.Headers.XContentTypeOptions = new StringValues("nosniff");
        response.Headers.XFrameOptions = new StringValues("SAMEORIGIN");
        response.Headers.ContentSecurityPolicy = new StringValues("frame-ancestors 'self'");
        response.Headers["Permissions-Policy"] = new StringValues("geolocation=(self), microphone=()");
        response.Headers["Referrer-Policy"] = new StringValues("strict-origin-when-cross-origin");
        await Task.CompletedTask;
    }
}
