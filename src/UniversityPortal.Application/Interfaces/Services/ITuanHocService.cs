using UniversityPortal.Application.DTOs.TuanHoc;

namespace UniversityPortal.Application.Interfaces.Services;

public interface ITuanHocService
{
    /// <summary>Lấy tất cả tuần học (dùng cho dropdown chọn tuần khi tạo thời khoá biểu).</summary>
    Task<IEnumerable<TuanHocDto>> GetAllAsync();
}
