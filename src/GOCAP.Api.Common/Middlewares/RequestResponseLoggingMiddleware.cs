using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Text;

namespace GOCAP.Api.Common;

public class RequestResponseLoggingMiddleware(RequestDelegate next, ILogger<RequestResponseLoggingMiddleware> logger, IWebHostEnvironment env)
{
    private const int MaxLogSize = 4096;
    private readonly RequestDelegate _next = next;
    private readonly ILogger<RequestResponseLoggingMiddleware> _logger = logger;
    private readonly IWebHostEnvironment _env = env;

    public async Task Invoke(HttpContext context)
    {
        var request = context.Request;
        var requestTime = DateTime.UtcNow;

        string? requestBody = null;
        if (request.Method is "POST" or "PUT" or "PATCH")
        {
            requestBody = await ReadRequestBodyAsync(request);
        }

        using (_logger.BeginScope("Request {Method} {Path}", request.Method, request.Path))
        {
            _logger.LogInformation("➡️ Request {Method} {Path} | Body: {Body} | Time: {Time}",
                request.Method, request.Path, Truncate(requestBody), requestTime);
        }

        var originalBodyStream = context.Response.Body;
        using var responseBody = new MemoryStream();
        context.Response.Body = responseBody;

        await _next(context); 

        var responseTime = DateTime.UtcNow;
        var responseBodyText = context.Response.StatusCode >= 400
            ? await ReadResponseBodyAsync(responseBody)
            : null;

        if (responseBodyText is not null)
        {
            _logger.LogInformation("⬅️ Response {StatusCode} | Body: {Body} | Time: {Time}",
                context.Response.StatusCode, Truncate(responseBodyText), responseTime);
        }

        responseBody.Seek(0, SeekOrigin.Begin);
        await responseBody.CopyToAsync(originalBodyStream);
        context.Response.Body = originalBodyStream;
    }

    private static async Task<string> ReadRequestBodyAsync(HttpRequest request)
    {
        request.EnableBuffering();
        using var reader = new StreamReader(request.Body, Encoding.UTF8, leaveOpen: true);
        var body = await reader.ReadToEndAsync();
        request.Body.Position = 0;
        return body;
    }

    private static async Task<string> ReadResponseBodyAsync(Stream responseBody)
    {
        responseBody.Seek(0, SeekOrigin.Begin);
        using var reader = new StreamReader(responseBody, Encoding.UTF8, leaveOpen: true);
        var body = await reader.ReadToEndAsync();
        responseBody.Seek(0, SeekOrigin.Begin);
        return body;
    }

    private static string? Truncate(string? input)
    {
        if (string.IsNullOrEmpty(input)) return input;
        return input.Length > MaxLogSize
            ? string.Concat(input.AsSpan(0, MaxLogSize), "... (truncated)")
            : input;
    }
}
