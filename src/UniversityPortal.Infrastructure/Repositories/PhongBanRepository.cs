using UniversityPortal.Application.Interfaces.Repositories;
using UniversityPortal.Domain.Entities;
using UniversityPortal.Infrastructure.Persistence;

namespace UniversityPortal.Infrastructure.Repositories;

/// <summary>
/// Cài đặt repository phòng ban / khoa — dùng CRUD cơ bản từ BaseRepository.
/// </summary>
public class PhongBanRepository(AppDbContext context) : BaseRepository<PhongBan>(context), IPhongBanRepository
{
}
