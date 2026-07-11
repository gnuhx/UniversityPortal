using UniversityPortal.Application.DTOs.Common;
using UniversityPortal.Application.DTOs.YeuCauSuaDiem;
using UniversityPortal.Application.Interfaces;
using UniversityPortal.Application.Interfaces.Services;
using UniversityPortal.Domain.Entities;
using UniversityPortal.Domain.Exceptions;

namespace UniversityPortal.Application.Services;

public class YeuCauSuaDiemService(IUnitOfWork uow) : IYeuCauSuaDiemService
{
    public async Task<IEnumerable<YeuCauSuaDiemDto>> GetByMeAsync(int taiKhoanId)
    {
        var gv = await uow.GiaoViens.GetByTaiKhoanIdAsync(taiKhoanId)
            ?? throw new NotFoundException("Không tìm thấy hồ sơ giáo viên.");

        var list = await uow.YeuCauSuaDiems.GetByGiaoVienAsync(gv.Id);
        return list.Select(MapToDto);
    }

    public async Task<PagedResultDto<YeuCauSuaDiemDto>> GetAllAsync(int page, int pageSize, string? trangThai)
    {
        var paged = await uow.YeuCauSuaDiems.GetAllPagedAsync(page, pageSize, trangThai);
        return new PagedResultDto<YeuCauSuaDiemDto>
        {
            Data     = paged.Data.Select(MapToDto),
            Total    = paged.Total,
            Page     = paged.Page,
            PageSize = paged.PageSize,
        };
    }

    public async Task<YeuCauSuaDiemDto> CreateAsync(int taiKhoanId, CreateYeuCauSuaDiemDto dto)
    {
        var gv = await uow.GiaoViens.GetByTaiKhoanIdAsync(taiKhoanId)
            ?? throw new NotFoundException("Không tìm thấy hồ sơ giáo viên.");

        var lhp = await uow.LopHocPhans.GetByIdAsync(dto.LopHpId)
            ?? throw new NotFoundException($"Không tìm thấy lớp học phần ID = {dto.LopHpId}.");

        if (lhp.GiaoVienId != gv.Id)
            throw new ForbiddenException("Bạn không có quyền tạo yêu cầu cho lớp này.");

        if (!lhp.KhoaBangDiem)
            throw new BadRequestException("Bảng điểm chưa bị khoá, không cần yêu cầu mở khoá.");

        var yc = new YeuCauSuaDiem
        {
            LopHpId    = dto.LopHpId,
            GiaoVienId = gv.Id,
            LyDo       = dto.LyDo,
            TrangThai  = "Chờ duyệt",
        };

        await uow.YeuCauSuaDiems.AddAsync(yc);
        await uow.CommitAsync();

        var saved = await uow.YeuCauSuaDiems.GetDetailAsync(yc.Id)
            ?? throw new NotFoundException("Lỗi khi lưu yêu cầu.");

        return MapToDto(saved);
    }

    public async Task<YeuCauSuaDiemDto> DuyetAsync(int id, int taiKhoanId, DuyetYeuCauSuaDiemDto dto)
    {
        var allowed = new[] { "Đã duyệt", "Từ chối" };
        if (!allowed.Contains(dto.TrangThai))
            throw new BadRequestException($"Trạng thái không hợp lệ. Chỉ chấp nhận: {string.Join(", ", allowed)}.");

        var yc = await uow.YeuCauSuaDiems.GetDetailAsync(id)
            ?? throw new NotFoundException($"Không tìm thấy yêu cầu ID = {id}.");

        if (yc.TrangThai != "Chờ duyệt")
            throw new BadRequestException("Yêu cầu này đã được xử lý rồi.");

        yc.TrangThai    = dto.TrangThai;
        yc.NguoiDuyetId = taiKhoanId;

        if (dto.TrangThai == "Đã duyệt")
        {
            var lhp = await uow.LopHocPhans.GetByIdAsync(yc.LopHpId)
                ?? throw new NotFoundException("Không tìm thấy lớp học phần liên kết.");
            lhp.KhoaBangDiem = false;
        }

        await uow.CommitAsync();

        var updated = await uow.YeuCauSuaDiems.GetDetailAsync(id);
        return MapToDto(updated!);
    }

    private static YeuCauSuaDiemDto MapToDto(YeuCauSuaDiem yc) => new()
    {
        Id             = yc.Id,
        LopHpId        = yc.LopHpId,
        MaLopHp        = yc.LopHocPhan?.MaLopHp ?? string.Empty,
        TenMon         = yc.LopHocPhan?.ChiTietCTDT?.MonHoc?.TenMon ?? string.Empty,
        TenHocKy       = yc.LopHocPhan?.HocKy?.TenHocKy ?? string.Empty,
        GiaoVienId     = yc.GiaoVienId,
        TenGiaoVien    = yc.GiaoVien?.TaiKhoan?.HoTen ?? string.Empty,
        LyDo           = yc.LyDo,
        TrangThai      = yc.TrangThai,
        NguoiDuyetId   = yc.NguoiDuyetId,
        TenNguoiDuyet  = yc.NguoiDuyet?.HoTen,
        CreatedAt      = yc.CreatedAt,
    };
}
