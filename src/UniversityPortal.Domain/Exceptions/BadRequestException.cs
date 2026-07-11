namespace UniversityPortal.Domain.Exceptions;

public class BadRequestException(string message, IEnumerable<string>? errors = null) : Exception(message)
{
    /// <summary>Danh sách lý do cụ thể (nếu có) — dùng khi 1 thao tác bị chặn bởi nhiều điều kiện cùng lúc.</summary>
    public IEnumerable<string>? Errors { get; } = errors;
}
