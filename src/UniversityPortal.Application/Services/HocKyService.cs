// Dịch vụ quản lý học kỳ — CRUD với ràng buộc xoá khi còn dữ liệu học vụ liên kết
using AutoMapper;
using UniversityPortal.Application.DTOs.Common;
using UniversityPortal.Application.DTOs.HocKy;
using UniversityPortal.Application.Interfaces;
using UniversityPortal.Application.Interfaces.Services;
using UniversityPortal.Domain.Entities;
using UniversityPortal.Domain.Exceptions;

namespace UniversityPortal.Application.Services;

/// <summary>
/// Cài đặt dịch vụ quản lý học kỳ.
/// </summary>
public class HocKyService(IUnitOfWork uow, IMapper mapper) : IHocKyService
{
    /// <summary>Lấy danh sách học kỳ có phân trang, lọc theo năm học.</summary>
    public async Task<PagedResultDto<HocKyDto>> GetPagedAsync(int page, int pageSize, int? namHocId)
    {
        var paged = await uow.HocKys.GetPagedFilterAsync(page, pageSize, namHocId);
        return new PagedResultDto<HocKyDto>
        {
            Data     = mapper.Map<IEnumerable<HocKyDto>>(paged.Data),
            Total    = paged.Total,
            Page     = paged.Page,
            PageSize = paged.PageSize
        };
    }

    /// <summary>Lấy tất cả học kỳ (không phân trang, dùng cho dropdown).</summary>
    public async Task<IEnumerable<HocKyDto>> GetAllAsync()
        => mapper.Map<IEnumerable<HocKyDto>>(await uow.HocKys.GetAllAsync());

    /// <summary>Lấy chi tiết học kỳ theo id. Ném NotFoundException nếu không tồn tại.</summary>
    public async Task<HocKyDto> GetByIdAsync(int id)
    {
        var hocKy = await uow.HocKys.GetDetailAsync(id)
            ?? throw new NotFoundException($"Không tìm thấy học kỳ id = {id}.");
        return mapper.Map<HocKyDto>(hocKy);
    }

    /// <summary>Tạo mới học kỳ.</summary>
    public async Task<HocKyDto> CreateAsync(UpsertHocKyDto dto)
    {
        var hocKy = new HocKy
        {
            TenHocKy   = dto.TenHocKy,
            NamHocId   = dto.NamHocId,
            NgayBatDau = dto.NgayBatDau
        };

        await uow.HocKys.AddAsync(hocKy);
        await uow.CommitAsync();

        return mapper.Map<HocKyDto>(await uow.HocKys.GetDetailAsync(hocKy.Id));
    }

    /// <summary>Cập nhật học kỳ.</summary>
    public async Task<HocKyDto> UpdateAsync(int id, UpsertHocKyDto dto)
    {
        var hocKy = await uow.HocKys.GetDetailAsync(id)
            ?? throw new NotFoundException($"Không tìm thấy học kỳ id = {id}.");

        hocKy.TenHocKy   = dto.TenHocKy;
        hocKy.NamHocId   = dto.NamHocId;
        hocKy.NgayBatDau = dto.NgayBatDau;

        uow.HocKys.Update(hocKy);
        await uow.CommitAsync();

        return mapper.Map<HocKyDto>(await uow.HocKys.GetDetailAsync(id));
    }

    /// <summary>Xoá học kỳ. Ném BadRequestException nếu còn lớp học phần, chi tiết CTĐT hoặc học phí liên kết.</summary>
    public async Task DeleteAsync(int id)
    {
        var hocKy = await uow.HocKys.GetDetailAsync(id)
            ?? throw new NotFoundException($"Không tìm thấy học kỳ id = {id}.");

        var coLopHocPhan = await uow.LopHocPhans.GetPagedByHocKyAsync(id, 1, 1);
        if (coLopHocPhan.Total > 0)
            throw new BadRequestException("Không thể xoá học kỳ đang có lớp học phần liên kết.");

        if (await uow.ChiTietCTDTs.ExistsByHocKyAsync(id))
            throw new BadRequestException("Không thể xoá học kỳ đang có chi tiết chương trình đào tạo liên kết.");

        if (await uow.HocPhis.ExistsByHocKyAsync(id))
            throw new BadRequestException("Không thể xoá học kỳ đang có khoản học phí liên kết.");

        uow.HocKys.Delete(hocKy);
        await uow.CommitAsync();
    }

    /// <summary>Lấy các học kỳ mà sinh viên đang đăng nhập có lớp học phần đã đăng ký.</summary>
    public async Task<IEnumerable<HocKyDto>> GetForSinhVienMeAsync(int taiKhoanId)
    {
        var sv = await uow.SinhViens.GetByTaiKhoanIdAsync(taiKhoanId)
            ?? throw new NotFoundException("Không tìm thấy hồ sơ sinh viên.");

        return mapper.Map<IEnumerable<HocKyDto>>(await uow.HocKys.GetForSinhVienAsync(sv.Id));
    }

    /// <summary>Lấy các học kỳ mà giáo viên đang đăng nhập có lớp học phần đang dạy.</summary>
    public async Task<IEnumerable<HocKyDto>> GetForGiaoVienMeAsync(int taiKhoanId)
    {
        var gv = await uow.GiaoViens.GetByTaiKhoanIdAsync(taiKhoanId)
            ?? throw new NotFoundException("Không tìm thấy hồ sơ giáo viên.");

        return mapper.Map<IEnumerable<HocKyDto>>(await uow.HocKys.GetForGiaoVienAsync(gv.Id));
    }
}
