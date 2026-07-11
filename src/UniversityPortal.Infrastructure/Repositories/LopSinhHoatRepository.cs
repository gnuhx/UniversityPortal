using Microsoft.EntityFrameworkCore;
using UniversityPortal.Application.DTOs.Common;
using UniversityPortal.Application.Interfaces.Repositories;
using UniversityPortal.Domain.Entities;
using UniversityPortal.Infrastructure.Persistence;

namespace UniversityPortal.Infrastructure.Repositories;

/// <summary>
/// Cài đặt repository lớp sinh hoạt — hỗ trợ phân trang, lọc theo GVCN và đếm sinh viên.
/// </summary>
public class LopSinhHoatRepository(AppDbContext context) : BaseRepository<LopSinhHoat>(context), ILopSinhHoatRepository
{
    /// <summary>Lấy danh sách lớp sinh hoạt phân trang, lọc theo mã lớp hoặc GVCN.</summary>
    public async Task<PagedResultDto<LopSinhHoat>> GetPagedFilterAsync(
        int page, int pageSize, string? keyword, int? gvcnId)
    {
        var query = DbSet
            .Include(x => x.Gvcn).ThenInclude(g => g.TaiKhoan)
            .Include(x => x.ThuKy).ThenInclude(s => s!.TaiKhoan)
            .Include(x => x.ChuongTrinhDT).ThenInclude(c => c.Nganh).ThenInclude(n => n.PhongBan)
            .Include(x => x.SinhViens).ThenInclude(sv => sv.TaiKhoan)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            var kw = keyword.ToLower();
            query = query.Where(x => x.MaLop.ToLower().Contains(kw));
        }

        if (gvcnId.HasValue)
            query = query.Where(x => x.GvcnId == gvcnId.Value);

        var total = await query.CountAsync();
        var data  = await query
            .OrderBy(x => x.MaLop)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResultDto<LopSinhHoat> { Data = data, Total = total, Page = page, PageSize = pageSize };
    }

    /// <summary>Tìm lớp theo mã lớp để kiểm tra trùng khi tạo mới.</summary>
    public async Task<LopSinhHoat?> GetByMaLopAsync(string maLop)
        => await DbSet.FirstOrDefaultAsync(x => x.MaLop == maLop);

    /// <summary>Lấy chi tiết lớp theo id, bao gồm GVCN, thư ký, CTDT (kèm Ngành/Khoa) và roster sinh viên.</summary>
    public async Task<LopSinhHoat?> GetDetailAsync(int id)
        => await DbSet
            .Include(x => x.Gvcn).ThenInclude(g => g.TaiKhoan)
            .Include(x => x.ThuKy).ThenInclude(s => s!.TaiKhoan)
            .Include(x => x.ChuongTrinhDT).ThenInclude(c => c.Nganh).ThenInclude(n => n.PhongBan)
            .Include(x => x.SinhViens).ThenInclude(sv => sv.TaiKhoan)
            .FirstOrDefaultAsync(x => x.Id == id);

    /// <summary>Lấy tất cả lớp kèm ChuongTrinhDT.Nganh.PhongBan, dùng để dropdown chọn lớp lọc theo Khoa/Ngành/Khoá học.</summary>
    public async Task<IEnumerable<LopSinhHoat>> GetAllDetailAsync()
        => await DbSet
            .Include(x => x.ChuongTrinhDT).ThenInclude(c => c.Nganh).ThenInclude(n => n.PhongBan)
            .ToListAsync();

    /// <summary>Lấy (các) lớp mà giáo viên này là GVCN — dùng cho trang "Lớp của tôi" phía Giáo viên.</summary>
    public async Task<IEnumerable<LopSinhHoat>> GetByGvcnIdAsync(int gvcnId)
        => await DbSet
            .Include(x => x.Gvcn).ThenInclude(g => g.TaiKhoan)
            .Include(x => x.ThuKy).ThenInclude(s => s!.TaiKhoan)
            .Include(x => x.ChuongTrinhDT).ThenInclude(c => c.Nganh).ThenInclude(n => n.PhongBan)
            .Include(x => x.SinhViens).ThenInclude(sv => sv.TaiKhoan)
            .Where(x => x.GvcnId == gvcnId)
            .ToListAsync();
}
