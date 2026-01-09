using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;
    private static readonly string LogsFolder = Path.Combine(AppContext.BaseDirectory, "ErrorLogs");

    static ErrorHandlingMiddleware()
    {
        // Ensure folder exists once at type initialization. Best-effort.
        try { Directory.CreateDirectory(LogsFolder); } catch { /* suppress */ }
    }

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
        catch (AppException ex)
        {
            _logger.LogWarning(ex, "Handled application error");
            await WriteExceptionToFileAsync(context, ex, ex.StatusCode, ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled server error");
            await WriteExceptionToFileAsync(context, ex, StatusCodes.Status500InternalServerError,
                "An unexpected error occurred.");
        }
    }

    private static async Task WriteExceptionToFileAsync(HttpContext context, Exception? exception, int statusCode, string clientMessage)
    {
        context.Response.ContentType = "text/plain";
        context.Response.StatusCode = statusCode;

        var traceId = context.TraceIdentifier;
        var timestamp = DateTime.UtcNow;
        var safeTrace = string.IsNullOrWhiteSpace(traceId) ? Guid.NewGuid().ToString("N") : traceId;
        var dayString = timestamp.ToString("yyyyMMdd");
        var fileName = $"error_{dayString}.txt";
        var filePath = Path.Combine(LogsFolder, fileName);
        var requestPath = context?.Request?.Path.ToString() ?? "<unknown>";
        var cancellation = context?.RequestAborted ?? default;

        try
        {
            // Pre-size StringBuilder to avoid multiple resizes for typical error sizes.
            var sb = new StringBuilder(4096);
            sb.AppendLine($"Timestamp (UTC): {timestamp:O}");
            sb.AppendLine($"TraceId: {traceId}");
            sb.AppendLine($"Request Path: {requestPath}");
            sb.AppendLine($"Status Code: {statusCode}");
            sb.AppendLine();

            if (exception != null)
            {
                var current = exception;
                var level = 0;
                while (current != null)
                {
                    sb.AppendLine($"Exception Level: {level}");
                    sb.AppendLine($"Type: {current.GetType().FullName}");
                    sb.AppendLine($"Message: {current.Message}");
                    sb.AppendLine("StackTrace:");
                    sb.AppendLine(current.StackTrace ?? "<no stack trace>");
                    sb.AppendLine();
                    current = current.InnerException;
                    level++;
                }
            }
            else
            {
                sb.AppendLine("No exception object provided.");
            }

            // Append to the daily log file.
            await File.AppendAllTextAsync(filePath, sb.ToString(), Encoding.UTF8, cancellation).ConfigureAwait(false);

            var responseMessage = $"{clientMessage} TraceId: {traceId}";
            await context.Response.WriteAsync(responseMessage, cancellation).ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            // Request aborted; nothing to do.
        }
        catch (Exception)
        {
            // Best-effort fallback: try to respond to client without throwing.
            try
            {
                var fallbackMessage = $"{clientMessage} TraceId: {traceId}";
                await context.Response.WriteAsync(fallbackMessage).ConfigureAwait(false);
            }
            catch
            {
                // suppress any further failures
            }
        }
    }
    public class AppException : Exception
    {
        public int StatusCode { get; }

        public AppException(string message, int statusCode = StatusCodes.Status400BadRequest)
            : base(message)
        {
            StatusCode = statusCode;
        }
    }
}

