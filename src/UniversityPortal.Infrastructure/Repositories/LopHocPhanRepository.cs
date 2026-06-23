using Microsoft.EntityFrameworkCore;
using UniversityPortal.Application.DTOs.Common;
using UniversityPortal.Application.Interfaces.Repositories;
using UniversityPortal.Domain.Entities;
using UniversityPortal.Infrastructure.Persistence;

namespace UniversityPortal.Infrastructure.Repositories;

public class LopHocPhanRepository(AppDbContext context) : BaseRepository<LopHocPhan>(context), ILopHocPhanRepository
{
    public async Task<PagedResultDto<LopHocPhan>> GetPagedByHocKyAsync(int hocKyId, int page, int pageSize)
    {
        var query = DbSet.Where(x => x.HocKyId == hocKyId);
        var total = await query.CountAsync();
        var data = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        return new PagedResultDto<LopHocPhan> { Data = data, Total = total, Page = page, PageSize = pageSize };
    }

    public async Task<PagedResultDto<LopHocPhan>> GetPagedByGiaoVienAsync(int giaoVienId, int page, int pageSize)
    {
        var query = DbSet.Where(x => x.GiaoVienId == giaoVienId);
        var total = await query.CountAsync();
        var data = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        return new PagedResultDto<LopHocPhan> { Data = data, Total = total, Page = page, PageSize = pageSize };
    }

    public async Task<IEnumerable<LopHocPhan>> GetByGiaoVienWithDetailsAsync(int giaoVienId)
        => await DbSet
            .Where(x => x.GiaoVienId == giaoVienId)
            .Include(x => x.ChiTietCTDT).ThenInclude(ct => ct.MonHoc)
            .Include(x => x.HocKy)
            .Include(x => x.GiaoVien).ThenInclude(gv => gv.TaiKhoan)
            .Include(x => x.DanhSachLopHPs)
            .ToListAsync();

    public async Task<LopHocPhan?> GetDetailAsync(int id)
        => await DbSet
            .Where(x => x.Id == id)
            .Include(x => x.ChiTietCTDT).ThenInclude(ct => ct.MonHoc)
            .Include(x => x.HocKy)
            .Include(x => x.GiaoVien).ThenInclude(gv => gv.TaiKhoan)
            .Include(x => x.DanhSachLopHPs)
                .ThenInclude(ds => ds.SinhVien)
                    .ThenInclude(sv => sv.TaiKhoan)
            .FirstOrDefaultAsync();
}
