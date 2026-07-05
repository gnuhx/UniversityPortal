using UniversityPortal.Domain.Entities;

namespace UniversityPortal.Application.Interfaces.Repositories;

/// <summary>
/// Repository phòng ban / khoa — hiện chỉ cần các thao tác đọc kế thừa từ IRepository.
/// </summary>
public interface IPhongBanRepository : IRepository<PhongBan>
{
}
