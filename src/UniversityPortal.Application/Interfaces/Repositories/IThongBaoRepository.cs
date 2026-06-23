using UniversityPortal.Domain.Entities;

namespace UniversityPortal.Application.Interfaces.Repositories;

public interface IThongBaoRepository : IRepository<ThongBao>
{
    /// <summary>Lấy thông báo dành cho sinh viên: toàn trường + theo lớp của SV đó.</summary>
    Task<IEnumerable<ThongBao>> GetBySinhVienAsync(int lopId);

    /// <summary>Lấy tất cả thông báo (Admin).</summary>
    Task<IEnumerable<ThongBao>> GetAllWithDetailsAsync();

    /// <summary>Lấy một thông báo kèm trạng thái đọc của người dùng cụ thể.</summary>
    Task<ThongBao?> GetDetailAsync(int id);
}
