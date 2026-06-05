using Microsoft.EntityFrameworkCore;
using UniversityPortal.Application.DTOs.Common;
using UniversityPortal.Application.Interfaces.Repositories;
using UniversityPortal.Domain.Entities;
using UniversityPortal.Infrastructure.Persistence;

namespace UniversityPortal.Infrastructure.Repositories;

public class SinhVienRepository(AppDbContext context) : BaseRepository<SinhVien>(context), ISinhVienRepository
{
    public async Task<SinhVien?> GetByMssvAsync(string mssv)
        => await DbSet.Include(x => x.TaiKhoan).FirstOrDefaultAsync(x => x.Mssv == mssv);

    public async Task<SinhVien?> GetByTaiKhoanIdAsync(int taiKhoanId)
        => await DbSet.FirstOrDefaultAsync(x => x.TaiKhoanId == taiKhoanId);

    public async Task<PagedResultDto<SinhVien>> GetPagedByLopAsync(int lopId, int page, int pageSize)
    {
        var query = DbSet.Where(x => x.LopId == lopId);
        var total = await query.CountAsync();
        var data = await query.Include(x => x.TaiKhoan).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        return new PagedResultDto<SinhVien> { Data = data, Total = total, Page = page, PageSize = pageSize };
    }
}
