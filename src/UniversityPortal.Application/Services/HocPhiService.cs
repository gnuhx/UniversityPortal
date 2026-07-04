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

    public async Task<IEnumerable<HocPhiDto>> GetAllAsync()
    {
        var list = await uow.HocPhis.GetAllWithDetailsAsync();
        return list.Select(MapToDto);
    }

    public async Task<GenerateHocPhiResultDto> GenerateAsync(GenerateHocPhiDto dto)
    {
        // All enrollments in the semester with ChiTietCTDT for credit count
        var enrollments = await uow.DanhSachLopHPs.GetByHocKyWithDetailsAsync(dto.HocKyId);

        // Group by student, summing credits across all enrolled courses
        var grouped = enrollments
            .GroupBy(e => e.SinhVienId)
            .Select(g => new
            {
                SinhVienId = g.Key,
                TotalTinChi = g.Sum(e => e.LopHocPhan.ChiTietCTDT?.SoTinChi ?? 0),
            });

        int created = 0, skipped = 0;

        foreach (var student in grouped)
        {
            var existing = await uow.HocPhis.GetBySinhVienAndHocKyAsync(student.SinhVienId, dto.HocKyId);
            if (existing is not null)
            {
                skipped++;
                continue;
            }

            var soTien = student.TotalTinChi * dto.TienMotTinChi;
            await uow.HocPhis.AddAsync(new HocPhi
            {
                SinhVienId    = student.SinhVienId,
                HocKyId       = dto.HocKyId,
                SoTien        = soTien,
                TrangThaiDong = "Chưa đóng",
            });
            created++;
        }

        if (created > 0) await uow.CommitAsync();

        return new GenerateHocPhiResultDto { Created = created, Skipped = skipped };
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
