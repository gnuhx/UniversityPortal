using UniversityPortal.Application.DTOs.ThongBao;
using UniversityPortal.Application.Interfaces;
using UniversityPortal.Application.Interfaces.Services;
using UniversityPortal.Domain.Entities;
using UniversityPortal.Domain.Exceptions;

namespace UniversityPortal.Application.Services;

public class ThongBaoService(IUnitOfWork uow) : IThongBaoService
{
    public async Task<ThongBaoDto> CreateAsync(int taiKhoanId, CreateThongBaoDto dto)
    {
        var thongBao = new ThongBao
        {
            TieuDe       = dto.TieuDe,
            NoiDung      = dto.NoiDung,
            LoaiThongBao = dto.LoaiThongBao,
            MucDo        = dto.MucDo,
            NguoiTaoId   = taiKhoanId,
            LopNhanId    = dto.LopNhanId,
            NgayTao      = DateTime.UtcNow,
        };

        await uow.ThongBaos.AddAsync(thongBao);
        await uow.CommitAsync();

        var saved = await uow.ThongBaos.GetDetailAsync(thongBao.Id)
            ?? throw new NotFoundException("Lỗi khi lưu thông báo.");

        return MapToDto(saved, null);
    }

    public async Task<IEnumerable<ThongBaoDto>> GetAllAsync()
    {
        var list = await uow.ThongBaos.GetAllWithDetailsAsync();
        return list.Select(x => MapToDto(x, null));
    }

    public async Task<IEnumerable<ThongBaoDto>> GetBySinhVienMeAsync(int taiKhoanId)
    {
        var sv = await uow.SinhViens.GetByTaiKhoanIdAsync(taiKhoanId)
            ?? throw new NotFoundException("Không tìm thấy hồ sơ sinh viên.");

        var lopId = sv.LopId ?? 0;
        var list = await uow.ThongBaos.GetBySinhVienAsync(lopId);

        var docSet = new HashSet<int>();
        if (sv.LopId.HasValue)
        {
            // Lấy danh sách thông báo đã đọc của user này từ DB
            foreach (var tb in list)
            {
                var daDoc = tb.ThongBaoDaDocs?.FirstOrDefault(x => x.TaiKhoanId == taiKhoanId);
                if (daDoc?.DaDoc == true) docSet.Add(tb.Id);
            }
        }

        return list.Select(x => MapToDto(x, docSet.Contains(x.Id)));
    }

    public async Task MarkAsReadAsync(int thongBaoId, int taiKhoanId)
    {
        var tb = await uow.ThongBaos.GetDetailAsync(thongBaoId)
            ?? throw new NotFoundException($"Không tìm thấy thông báo ID = {thongBaoId}.");

        var existing = await uow.ThongBaoDaDocs.GetAsync(thongBaoId, taiKhoanId);
        if (existing is null)
        {
            await uow.ThongBaoDaDocs.AddAsync(new ThongBaoDaDoc
            {
                ThongBaoId  = thongBaoId,
                TaiKhoanId  = taiKhoanId,
                DaDoc       = true,
                NgayDoc     = DateTime.UtcNow,
            });
        }
        else
        {
            existing.DaDoc  = true;
            existing.NgayDoc = DateTime.UtcNow;
        }

        await uow.CommitAsync();
    }

    public async Task DeleteAsync(int thongBaoId)
    {
        var tb = await uow.ThongBaos.GetByIdAsync(thongBaoId)
            ?? throw new NotFoundException($"Không tìm thấy thông báo ID = {thongBaoId}.");

        uow.ThongBaos.Delete(tb);
        await uow.CommitAsync();
    }

    private static ThongBaoDto MapToDto(ThongBao tb, bool? daDoc) => new()
    {
        Id           = tb.Id,
        TieuDe       = tb.TieuDe,
        NoiDung      = tb.NoiDung,
        LoaiThongBao = tb.LoaiThongBao,
        MucDo        = tb.MucDo,
        NguoiTaoId   = tb.NguoiTaoId,
        TenNguoiTao  = tb.NguoiTao?.HoTen ?? string.Empty,
        LopNhanId    = tb.LopNhanId,
        TenLopNhan   = tb.LopNhan?.MaLop,
        NgayTao      = tb.NgayTao,
        DaDoc        = daDoc,
    };
}
