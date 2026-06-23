using UniversityPortal.Application.DTOs.HocPhi;
using UniversityPortal.Application.Interfaces;
using UniversityPortal.Application.Interfaces.Services;
using UniversityPortal.Domain.Entities;
using UniversityPortal.Domain.Exceptions;

namespace UniversityPortal.Application.Services;

public class HocPhiService(IUnitOfWork uow) : IHocPhiService
{
    public async Task<IEnumerable<HocPhiDto>> GetByMeAsync(int taiKhoanId)
    {
        var sv = await uow.SinhViens.GetByTaiKhoanIdAsync(taiKhoanId)
            ?? throw new NotFoundException("Không tìm thấy hồ sơ sinh viên.");

        var list = await uow.HocPhis.GetBySinhVienAsync(sv.Id);
        return list.Select(MapToDto);
    }

    public async Task<IEnumerable<HocPhiDto>> GetByHocKyAsync(int hocKyId)
    {
        var list = await uow.HocPhis.GetByHocKyAsync(hocKyId);
        return list.Select(MapToDto);
    }

    public async Task<HocPhiDto> CreateAsync(CreateHocPhiDto dto)
    {
        var existing = await uow.HocPhis.GetBySinhVienAndHocKyAsync(dto.SinhVienId, dto.HocKyId);
        if (existing is not null)
            throw new BadRequestException("Sinh viên đã có học phí cho học kỳ này.");

        var hocPhi = new HocPhi
        {
            SinhVienId   = dto.SinhVienId,
            HocKyId      = dto.HocKyId,
            SoTien       = dto.SoTien,
            TrangThaiDong = "Chưa đóng",
        };

        await uow.HocPhis.AddAsync(hocPhi);
        await uow.CommitAsync();

        var saved = (await uow.HocPhis.GetBySinhVienAsync(dto.SinhVienId))
            .FirstOrDefault(x => x.HocKyId == dto.HocKyId)
            ?? throw new NotFoundException("Lỗi khi lưu học phí.");

        return MapToDto(saved);
    }

    public async Task<HocPhiDto> UpdateTrangThaiAsync(int id, string trangThai)
    {
        var hocPhi = await uow.HocPhis.GetByIdAsync(id)
            ?? throw new NotFoundException($"Không tìm thấy học phí ID = {id}.");

        hocPhi.TrangThaiDong = trangThai;
        await uow.CommitAsync();

        var updated = (await uow.HocPhis.GetBySinhVienAsync(hocPhi.SinhVienId))
            .First(x => x.Id == id);

        return MapToDto(updated);
    }

    private static HocPhiDto MapToDto(HocPhi hp) => new()
    {
        Id            = hp.Id,
        SinhVienId    = hp.SinhVienId,
        TenSinhVien   = hp.SinhVien?.TaiKhoan?.HoTen ?? string.Empty,
        Mssv          = hp.SinhVien?.Mssv ?? string.Empty,
        HocKyId       = hp.HocKyId,
        TenHocKy      = hp.HocKy?.TenHocKy ?? string.Empty,
        SoTien        = hp.SoTien,
        TrangThaiDong = hp.TrangThaiDong,
        CreatedAt     = hp.CreatedAt,
    };
}
