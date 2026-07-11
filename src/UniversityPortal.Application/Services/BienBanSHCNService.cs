// Dịch vụ quản lý biên bản sinh hoạt chủ nhiệm — kiểm tra quyền sở hữu GVCN, ẩn vắng người khác với sinh viên
using AutoMapper;
using UniversityPortal.Application.DTOs.BienBanSHCN;
using UniversityPortal.Application.DTOs.Common;
using UniversityPortal.Application.Interfaces;
using UniversityPortal.Application.Interfaces.Services;
using UniversityPortal.Domain.Entities;
using UniversityPortal.Domain.Exceptions;

namespace UniversityPortal.Application.Services;

/// <summary>
/// Cài đặt dịch vụ quản lý biên bản sinh hoạt chủ nhiệm.
/// FluentValidation không được wiring vào pipeline ở dự án này (xem các Validator hiện có — không nơi nào
/// gọi IValidator), nên các kiểm tra dữ liệu đầu vào (tồn tại lớp/tuần học/thư ký/sinh viên, nội dung
/// không rỗng) được thực hiện trực tiếp ở đây, theo đúng convention check-then-throw đã dùng ở
/// SinhVienService/LopSinhHoatService.
/// </summary>
public class BienBanSHCNService(IUnitOfWork uow, IMapper mapper) : IBienBanSHCNService
{
    public async Task<PagedResultDto<BienBanSHCNDto>> GetPagedAsync(int? lopId, int page, int pageSize)
    {
        var paged = await uow.BienBanSHCNs.GetPagedFilterAsync(lopId, page, pageSize);
        return new PagedResultDto<BienBanSHCNDto>
        {
            Data     = mapper.Map<IEnumerable<BienBanSHCNDto>>(paged.Data),
            Total    = paged.Total,
            Page     = paged.Page,
            PageSize = paged.PageSize
        };
    }

    public async Task<BienBanSHCNDto> GetByIdAsync(int id)
    {
        var bb = await uow.BienBanSHCNs.GetDetailAsync(id)
            ?? throw new NotFoundException($"Không tìm thấy biên bản sinh hoạt id = {id}.");
        return mapper.Map<BienBanSHCNDto>(bb);
    }

    public async Task<PagedResultDto<BienBanSHCNDto>> GetPagedForGvcnAsync(int taiKhoanId, int lopId, int page, int pageSize)
    {
        await GetLopOwnedByGvcnAsync(taiKhoanId, lopId);
        return await GetPagedAsync(lopId, page, pageSize);
    }

    public async Task<BienBanSHCNDto> GetDetailForGvcnAsync(int taiKhoanId, int id)
    {
        var gv = await uow.GiaoViens.GetByTaiKhoanIdAsync(taiKhoanId)
            ?? throw new NotFoundException("Không tìm thấy hồ sơ giáo viên.");

        var bb = await uow.BienBanSHCNs.GetDetailAsync(id)
            ?? throw new NotFoundException($"Không tìm thấy biên bản sinh hoạt id = {id}.");

        if (bb.GvcnId != gv.Id)
            throw new ForbiddenException("Bạn không phải là GVCN của lớp này.");

        return mapper.Map<BienBanSHCNDto>(bb);
    }

    public async Task<BienBanSHCNDto> CreateAsync(int taiKhoanId, CreateBienBanSHCNDto dto)
    {
        var gv = await GetLopOwnedByGvcnAsync(taiKhoanId, dto.LopId);

        if (string.IsNullOrWhiteSpace(dto.NoiDung))
            throw new BadRequestException("Nội dung buổi sinh hoạt không được để trống.");

        if (string.IsNullOrWhiteSpace(dto.DiaDiem))
            throw new BadRequestException("Địa điểm không được để trống.");

        if (await uow.TuanHocs.GetByIdAsync(dto.TuanHocId) is null)
            throw new BadRequestException($"Không tìm thấy tuần học id = {dto.TuanHocId}.");

        var thuKy = await uow.SinhViens.GetByIdAsync(dto.ThuKyId);
        if (thuKy is null || thuKy.LopId != dto.LopId)
            throw new BadRequestException("Thư ký phải là sinh viên thuộc lớp này.");

        foreach (var v in dto.DanhSachVang)
        {
            var sv = await uow.SinhViens.GetByIdAsync(v.SinhVienId);
            if (sv is null || sv.LopId != dto.LopId)
                throw new BadRequestException($"Sinh viên id = {v.SinhVienId} không thuộc lớp này.");
        }

        var bienBan = new BienBanSHCN
        {
            LopId       = dto.LopId,
            TuanHocId   = dto.TuanHocId,
            ThoiGian    = dto.ThoiGian,
            DiaDiem     = dto.DiaDiem,
            GvcnId      = gv.Id,
            ThuKyId     = dto.ThuKyId,
            NoiDung     = dto.NoiDung,
            PhanHoiGvcn = dto.PhanHoiGvcn,
            ChiTietCongViecs = dto.CongViecs.Select(c => new ChiTietCongViec
            {
                TenCongViec   = c.TenCongViec,
                TrangThaiViec = c.TrangThaiViec
            }).ToList(),
            ChiTietVangSHCNs = dto.DanhSachVang.Select(v => new ChiTietVangSHCN
            {
                SinhVienId = v.SinhVienId,
                CoPhep     = v.CoPhep,
                LyDo       = v.LyDo
            }).ToList()
        };

        await uow.BienBanSHCNs.AddAsync(bienBan);
        await uow.CommitAsync();

        return mapper.Map<BienBanSHCNDto>(await uow.BienBanSHCNs.GetDetailAsync(bienBan.Id));
    }

    public async Task<IEnumerable<BienBanSHCNSinhVienDto>> GetMeAsync(int taiKhoanId)
    {
        var sv = await uow.SinhViens.GetByTaiKhoanIdAsync(taiKhoanId)
            ?? throw new NotFoundException("Không tìm thấy hồ sơ sinh viên.");

        if (sv.LopId is null)
            return [];

        var danhSach = await uow.BienBanSHCNs.GetByLopAsync(sv.LopId.Value);

        return danhSach.Select(bb =>
        {
            var vang = bb.ChiTietVangSHCNs.FirstOrDefault(v => v.SinhVienId == sv.Id);
            return new BienBanSHCNSinhVienDto
            {
                Id              = bb.Id,
                TuanHocId       = bb.TuanHocId,
                MaTuan          = bb.TuanHoc.MaTuan,
                ThoiGian        = bb.ThoiGian,
                DiaDiem         = bb.DiaDiem,
                TenGvcn         = bb.Gvcn.TaiKhoan.HoTen,
                TenThuKy        = bb.ThuKy.TaiKhoan.HoTen,
                NoiDung         = bb.NoiDung,
                PhanHoiGvcn     = bb.PhanHoiGvcn,
                CongViecs       = bb.ChiTietCongViecs.Select(c => new CongViecItemDto
                {
                    Id            = c.Id,
                    TenCongViec   = c.TenCongViec,
                    TrangThaiViec = c.TrangThaiViec
                }).ToList(),
                TinhTrangCuaToi  = vang is null ? "CoMat" : (vang.CoPhep ? "VangCoPhep" : "VangKhongPhep"),
                LyDoVangCuaToi   = vang?.LyDo
            };
        }).ToList();
    }

    /// <summary>Xác thực giáo viên đang đăng nhập tồn tại và là GVCN của lớp truyền vào; trả về GiaoVien để dùng tiếp.</summary>
    private async Task<GiaoVien> GetLopOwnedByGvcnAsync(int taiKhoanId, int lopId)
    {
        var gv = await uow.GiaoViens.GetByTaiKhoanIdAsync(taiKhoanId)
            ?? throw new NotFoundException("Không tìm thấy hồ sơ giáo viên.");

        var lop = await uow.LopSinhHoats.GetByIdAsync(lopId)
            ?? throw new NotFoundException($"Không tìm thấy lớp sinh hoạt id = {lopId}.");

        if (lop.GvcnId != gv.Id)
            throw new ForbiddenException("Bạn không phải là GVCN của lớp này.");

        return gv;
    }
}
