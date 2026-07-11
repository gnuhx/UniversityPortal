using System.Net;
using System.Text.Json;
using UniversityPortal.Application.DTOs.Common;
using UniversityPortal.Domain.Exceptions;

namespace UniversityPortal.API.Middleware;

/// <summary>
/// Middleware bắt toàn bộ exception chưa được xử lý trong pipeline.
/// Ánh xạ domain exception sang HTTP status code tương ứng và trả về JSON chuẩn.
/// </summary>
public class GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
{
    /// <summary>Bọc pipeline tiếp theo trong try/catch; chuyển mọi exception sang HandleExceptionAsync.</summary>
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled exception");
            await HandleExceptionAsync(context, ex);
        }
    }

    /// <summary>
    /// Ánh xạ loại exception sang HTTP status code và ghi response JSON chuẩn ApiResponseDto.
    /// Domain exception chứa thông báo nghiệp vụ thân thiện; exception không xác định trả 500.
    /// </summary>
    private static Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        var (status, message) = ex switch
        {
            NotFoundException           => (HttpStatusCode.NotFound,            ex.Message),
            BadRequestException         => (HttpStatusCode.BadRequest,           ex.Message),
            ForbiddenException          => (HttpStatusCode.Forbidden,            ex.Message),
            UnauthorizedAccessException => (HttpStatusCode.Unauthorized,         ex.Message),
            _                           => (HttpStatusCode.InternalServerError,  "Đã xảy ra lỗi không mong đợi.")
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode  = (int)status;

        var response = ApiResponseDto<object>.Fail(message, (ex as BadRequestException)?.Errors);
        return context.Response.WriteAsync(JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        }));
    }
}
