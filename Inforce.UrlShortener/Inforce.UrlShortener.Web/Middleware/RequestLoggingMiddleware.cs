using System.Diagnostics;

namespace Inforce.UrlShortener.Web.Middleware;

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();
        var request = context.Request;
            
        _logger.LogInformation("Request: {Method} {Path}", request.Method, request.Path);
            
        await _next(context);
            
        stopwatch.Stop();
        var response = context.Response;
        var elapsedMs = stopwatch.ElapsedMilliseconds;

        _logger.LogInformation(
            "Response: {Method} {Path} responded {StatusCode} in {ElapsedMilliseconds}ms",
            request.Method,
            request.Path,
            response.StatusCode,
            elapsedMs
        );
    }
}