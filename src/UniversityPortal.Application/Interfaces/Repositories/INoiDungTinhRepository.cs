using UniversityPortal.Domain.Entities;

namespace UniversityPortal.Application.Interfaces.Repositories;

/// <summary>
/// Repository nội dung tĩnh — dùng chung cho các khu vực (vd "thu-vien", "hoc-vu").
/// </summary>
public interface INoiDungTinhRepository : IRepository<NoiDungTinh>
{
    /// <summary>Lấy các mục nội dung của 1 khu vực, sắp theo Thứ tự.</summary>
    Task<IEnumerable<NoiDungTinh>> GetByKhuVucAsync(string khuVuc);
}
