using Microsoft.EntityFrameworkCore;
using UniversityPortal.Application.Interfaces.Repositories;
using UniversityPortal.Domain.Entities;
using UniversityPortal.Infrastructure.Persistence;

namespace UniversityPortal.Infrastructure.Repositories;

/// <summary>
/// Cài đặt repository tuần học — dùng CRUD cơ bản từ BaseRepository, GetAllAsync
/// nạp kèm năm học và sắp xếp theo thứ tự tuần để hiển thị dropdown.
/// </summary>
public class TuanHocRepository(AppDbContext context) : BaseRepository<TuanHoc>(context), ITuanHocRepository
{
    public async Task<IEnumerable<TuanHoc>> GetAllAsync()
        => await DbSet
            .Include(x => x.NamHoc)
            .OrderBy(x => x.NamHocId).ThenBy(x => x.SoThuTuTuan)
            .ToListAsync();

    public async Task<bool> ExistsByNamHocAsync(int namHocId)
        => await DbSet.AnyAsync(x => x.NamHocId == namHocId);
}
