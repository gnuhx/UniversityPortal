using UniversityPortal.Domain.Entities;

namespace UniversityPortal.Application.Interfaces.Repositories;

public interface ITuanHocRepository : IRepository<TuanHoc>
{
    Task<IEnumerable<TuanHoc>> GetAllAsync();

    /// <summary>Kiểm tra còn tuần học nào thuộc năm học này không (dùng khi xoá năm học).</summary>
    Task<bool> ExistsByNamHocAsync(int namHocId);
}
