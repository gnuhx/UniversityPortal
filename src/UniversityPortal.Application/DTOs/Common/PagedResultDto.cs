namespace UniversityPortal.Application.DTOs.Common;

public class PagedResultDto<T>
{
    public IEnumerable<T> Data { get; set; } = [];
    public int Total { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
}
