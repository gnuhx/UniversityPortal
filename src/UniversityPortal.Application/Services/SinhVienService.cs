// Dịch vụ quản lý sinh viên — tạo kèm tài khoản, cập nhật, xoá mềm
using AutoMapper;
using UniversityPortal.Application.DTOs.Common;
using UniversityPortal.Application.DTOs.SinhVien;
using UniversityPortal.Application.Interfaces;
using UniversityPortal.Application.Interfaces.Services;
using UniversityPortal.Domain.Entities;
using UniversityPortal.Domain.Exceptions;

namespace UniversityPortal.Application.Services;

/// <summary>
/// Cài đặt dịch vụ quản lý sinh viên.
/// Mỗi thao tác tạo/xoá sinh viên đồng thời tác động lên bảng TaiKhoan liên kết.
/// </summary>
public class SinhVienService(IUnitOfWork uow, IMapper mapper) : ISinhVienService
{
    /// <summary>Lấy danh sách sinh viên có phân trang và lọc theo keyword và lớp.</summary>
    public async Task<PagedResultDto<SinhVienDto>> GetPagedAsync(
        int page, int pageSize, string? keyword, int? lopId)
    {
        var paged = await uow.SinhViens.GetPagedFilterAsync(page, pageSize, keyword, lopId);
        return new PagedResultDto<SinhVienDto>
        {
            Data     = mapper.Map<IEnumerable<SinhVienDto>>(paged.Data),
            Total    = paged.Total,
            Page     = paged.Page,
            PageSize = paged.PageSize
        };
    }

    /// <summary>Lấy chi tiết sinh viên theo id. Ném NotFoundException nếu không tồn tại.</summary>
    public async Task<SinhVienDto> GetByIdAsync(int id)
    {
        var sv = await uow.SinhViens.GetDetailAsync(id)
            ?? throw new NotFoundException($"Không tìm thấy sinh viên id = {id}.");
        return mapper.Map<SinhVienDto>(sv);
    }

    /// <summary>
    /// Tạo mới sinh viên kèm tài khoản đăng nhập với vai trò Sinh viên (VaiTroId = 3).
    /// Ném BadRequestException nếu tên đăng nhập hoặc MSSV đã tồn tại.
    /// </summary>
    public async Task<SinhVienDto> CreateAsync(CreateSinhVienDto dto)
    {
        if (await uow.TaiKhoans.GetByTenDangNhapAsync(dto.TenDangNhap) is not null)
            throw new BadRequestException($"Tên đăng nhập '{dto.TenDangNhap}' đã tồn tại.");

        if (await uow.SinhViens.GetByMssvAsync(dto.Mssv) is not null)
            throw new BadRequestException($"MSSV '{dto.Mssv}' đã tồn tại.");

        // Tạo tài khoản trước để lấy Id, sau đó tạo SinhVien liên kết
        var taiKhoan = new TaiKhoan
        {
            TenDangNhap = dto.TenDangNhap,
            MatKhau     = BCrypt.Net.BCrypt.HashPassword(dto.MatKhau),
            VaiTroId    = 3, // Sinh viên
            HoTen       = dto.HoTen,
            Email       = dto.Email,
            TrangThai   = true
        };
        await uow.TaiKhoans.AddAsync(taiKhoan);
        await uow.CommitAsync();

        var sinhVien = new SinhVien
        {
            TaiKhoanId = taiKhoan.Id,
            Mssv       = dto.Mssv,
            LopId      = dto.LopId
        };
        await uow.SinhViens.AddAsync(sinhVien);
        await uow.CommitAsync();

        return mapper.Map<SinhVienDto>(await uow.SinhViens.GetDetailAsync(sinhVien.Id));
    }

    /// <summary>Cập nhật thông tin sinh viên và tài khoản liên kết.</summary>
    public async Task<SinhVienDto> UpdateAsync(int id, UpdateSinhVienDto dto)
    {
        var sv = await uow.SinhViens.GetDetailAsync(id)
            ?? throw new NotFoundException($"Không tìm thấy sinh viên id = {id}.");

        sv.LopId           = dto.LopId;
        sv.TaiKhoan.HoTen  = dto.HoTen;
        sv.TaiKhoan.Email  = dto.Email;
        sv.TaiKhoan.TrangThai = dto.TrangThai;

        uow.SinhViens.Update(sv);
        await uow.CommitAsync();

        return mapper.Map<SinhVienDto>(sv);
    }

    /// <summary>Lấy thông tin sinh viên theo taiKhoanId (cho endpoint /me).</summary>
    public async Task<SinhVienDto> GetMeAsync(int taiKhoanId)
    {
        var sv = await uow.SinhViens.GetByTaiKhoanIdAsync(taiKhoanId);
        if (sv is null) throw new NotFoundException("Không tìm thấy hồ sơ sinh viên.");
        // Load lop to get MaLop for TenLop mapping
        var detail = await uow.SinhViens.GetDetailAsync(sv.Id)
            ?? throw new NotFoundException("Không tìm thấy hồ sơ sinh viên.");
        return mapper.Map<SinhVienDto>(detail);
    }

    /// <summary>
    /// Xoá mềm sinh viên bằng cách khoá tài khoản liên kết.
    /// Không xoá bản ghi để giữ lại lịch sử điểm, học phí.
    /// </summary>
    public async Task DeleteAsync(int id)
    {
        var sv = await uow.SinhViens.GetDetailAsync(id)
            ?? throw new NotFoundException($"Không tìm thấy sinh viên id = {id}.");

        sv.TaiKhoan.TrangThai = false;
        uow.SinhViens.Update(sv);
        await uow.CommitAsync();
    }

    /// <summary>
    /// Kiểm tra điều kiện tốt nghiệp của sinh viên đang đăng nhập.
    /// Đối chiếu danh sách môn học bắt buộc của CTDT với các môn sinh viên đã học và đạt (đã duyệt, điểm tổng kết >= 5).
    /// CTDT được xác định qua lớp sinh hoạt hiện tại; nếu sinh viên không còn thuộc lớp nào (VD: đã tốt nghiệp),
    /// dùng CTDT ghi nhận trong bản ghi học bạ (HocBa) gần nhất.
    /// </summary>
    public async Task<TotNghiepDto> GetTotNghiepMeAsync(int taiKhoanId)
    {
        var sv = await uow.SinhViens.GetByTaiKhoanIdAsync(taiKhoanId)
            ?? throw new NotFoundException("Không tìm thấy hồ sơ sinh viên.");

        var detail = await uow.SinhViens.GetByIdWithLopCtdtAsync(sv.Id)
            ?? throw new NotFoundException("Không tìm thấy hồ sơ sinh viên.");

        int? ctdtId = detail.Lop?.ChuongTrinhDtId;
        if (ctdtId is null)
        {
            var hocBa = await uow.HocBas.GetLatestBySinhVienAsync(sv.Id);
            ctdtId = hocBa?.CtdtId;
        }

        if (ctdtId is null)
            throw new NotFoundException("Không xác định được chương trình đào tạo của sinh viên.");

        var monBatBuoc = await uow.ChiTietCTDTs.GetByCtdtIdAsync(ctdtId.Value);
        var danhSach   = await uow.DanhSachLopHPs.GetBySinhVienAsync(sv.Id);

        var daDatIds = danhSach
            .Where(x => x.TrangThaiDuyet == "Đã duyệt" && x.DiemTongKet >= 5)
            .Select(x => x.LopHocPhan.ChiTietCtdtId)
            .ToHashSet();

        var conThieu = monBatBuoc.Where(x => !daDatIds.Contains(x.Id)).ToList();

        return new TotNghiepDto
        {
            DuDieuKienTotNghiep   = monBatBuoc.Any() && conThieu.Count == 0,
            TongSoTinChiYeuCau    = monBatBuoc.Sum(x => x.SoTinChi),
            TongSoTinChiDaTichLuy = monBatBuoc.Where(x => daDatIds.Contains(x.Id)).Sum(x => x.SoTinChi),
            MonHocConThieu = conThieu.Select(x => new MonHocConThieuDto
            {
                MaMon    = x.MonHoc.MaMon,
                TenMon   = x.MonHoc.TenMon,
                SoTinChi = x.SoTinChi,
                TenHocKy = x.HocKy.TenHocKy
            }).ToList()
        };
    }
}
