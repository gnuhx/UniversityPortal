// AutoMapper profile — ánh xạ từ Entity sang DTO cho toàn bộ module Tuần 1 và Tuần 2
using AutoMapper;
using UniversityPortal.Application.DTOs.Auth;
using UniversityPortal.Application.DTOs.ChiTietCTDT;
using UniversityPortal.Application.DTOs.ChuongTrinhDT;
using UniversityPortal.Application.DTOs.DanhSachLopHP;
using UniversityPortal.Application.DTOs.GiaoVien;
using UniversityPortal.Application.DTOs.LopHocPhan;
using UniversityPortal.Application.DTOs.LopSinhHoat;
using UniversityPortal.Application.DTOs.MonHoc;
using UniversityPortal.Application.DTOs.NganhHoc;
using UniversityPortal.Application.DTOs.PhongBan;
using UniversityPortal.Application.DTOs.SinhVien;
using UniversityPortal.Application.DTOs.TaiKhoan;
using UniversityPortal.Application.DTOs.ThoiKhoaBieu;
using UniversityPortal.Application.DTOs.TuanHoc;
using UniversityPortal.Domain.Entities;

namespace UniversityPortal.Application.Mappings;

/// <summary>
/// Cấu hình ánh xạ AutoMapper cho tất cả Entity → DTO trong hệ thống.
/// Các thuộc tính navigation được ánh xạ tường minh để tránh null reference.
/// </summary>
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // ── Auth ──────────────────────────────────────────────────────────────
        CreateMap<TaiKhoan, UserInfoDto>()
            .ForMember(d => d.VaiTro, o => o.MapFrom(s => s.VaiTro.TenVaiTro));

        // ── TaiKhoan ──────────────────────────────────────────────────────────
        CreateMap<TaiKhoan, TaiKhoanDto>()
            .ForMember(d => d.TenVaiTro,   o => o.MapFrom(s => s.VaiTro != null ? s.VaiTro.TenVaiTro : string.Empty))
            .ForMember(d => d.TenPhongBan, o => o.MapFrom(s => s.PhongBan != null ? s.PhongBan.TenPhongBan : null));

        // ── GiaoVien ──────────────────────────────────────────────────────────
        CreateMap<GiaoVien, GiaoVienDto>()
            .ForMember(d => d.HoTen,      o => o.MapFrom(s => s.TaiKhoan.HoTen))
            .ForMember(d => d.Email,      o => o.MapFrom(s => s.TaiKhoan.Email))
            .ForMember(d => d.AnhDaiDien, o => o.MapFrom(s => s.TaiKhoan.AnhDaiDien))
            .ForMember(d => d.TrangThai,  o => o.MapFrom(s => s.TaiKhoan.TrangThai))
            .ForMember(d => d.PhongBanId, o => o.MapFrom(s => s.TaiKhoan.PhongBanId))
            .ForMember(d => d.TenPhongBan,o => o.MapFrom(s => s.TaiKhoan.PhongBan != null ? s.TaiKhoan.PhongBan.TenPhongBan : null))
            .ForMember(d => d.TaiKhoanId, o => o.MapFrom(s => s.TaiKhoanId))
            .ForMember(d => d.CreatedAt,  o => o.MapFrom(s => s.CreatedAt));

        // ── SinhVien ──────────────────────────────────────────────────────────
        CreateMap<SinhVien, SinhVienDto>()
            .ForMember(d => d.HoTen,      o => o.MapFrom(s => s.TaiKhoan.HoTen))
            .ForMember(d => d.Email,      o => o.MapFrom(s => s.TaiKhoan.Email))
            .ForMember(d => d.AnhDaiDien, o => o.MapFrom(s => s.TaiKhoan.AnhDaiDien))
            .ForMember(d => d.TrangThai,  o => o.MapFrom(s => s.TaiKhoan.TrangThai))
            .ForMember(d => d.TenLop,     o => o.MapFrom(s => s.Lop != null ? s.Lop.MaLop : null))
            .ForMember(d => d.TaiKhoanId, o => o.MapFrom(s => s.TaiKhoanId))
            .ForMember(d => d.CreatedAt,  o => o.MapFrom(s => s.CreatedAt));

        // ── NganhHoc ──────────────────────────────────────────────────────────
        CreateMap<NganhHoc, NganhHocDto>()
            .ForMember(d => d.TenNganhCha, o => o.MapFrom(s => s.NganhCha != null ? s.NganhCha.TenNganh : null))
            .ForMember(d => d.TenPhongBan, o => o.MapFrom(s => s.PhongBan != null ? s.PhongBan.TenPhongBan : null));

        // ── PhongBan ──────────────────────────────────────────────────────────
        CreateMap<PhongBan, PhongBanDto>();

        // ── ChuongTrinhDT ─────────────────────────────────────────────────────
        CreateMap<ChuongTrinhDT, ChuongTrinhDTDto>()
            .ForMember(d => d.TenNganh, o => o.MapFrom(s => s.Nganh != null ? s.Nganh.TenNganh : string.Empty));

        // ── MonHoc ────────────────────────────────────────────────────────────
        CreateMap<MonHoc, MonHocDto>();

        // ── ChiTietCTDT ───────────────────────────────────────────────────────
        CreateMap<ChiTietCTDT, ChiTietCTDTDto>()
            .ForMember(d => d.MaCtdt,   o => o.MapFrom(s => s.ChuongTrinhDT != null ? s.ChuongTrinhDT.MaCtdt : string.Empty))
            .ForMember(d => d.MaMon,    o => o.MapFrom(s => s.MonHoc != null ? s.MonHoc.MaMon : string.Empty))
            .ForMember(d => d.TenMon,   o => o.MapFrom(s => s.MonHoc != null ? s.MonHoc.TenMon : string.Empty))
            .ForMember(d => d.TenHocKy, o => o.MapFrom(s => s.HocKy != null ? s.HocKy.TenHocKy : string.Empty));

        // ── DanhSachLopHP ─────────────────────────────────────────────────────
        CreateMap<DanhSachLopHP, DanhSachLopHPDto>()
            .ForMember(d => d.MaLopHp,          o => o.MapFrom(s => s.LopHocPhan != null ? s.LopHocPhan.MaLopHp : string.Empty))
            .ForMember(d => d.MaMon,             o => o.MapFrom(s => s.LopHocPhan != null ? s.LopHocPhan.ChiTietCTDT.MonHoc.MaMon : string.Empty))
            .ForMember(d => d.TenMon,            o => o.MapFrom(s => s.LopHocPhan != null ? s.LopHocPhan.ChiTietCTDT.MonHoc.TenMon : string.Empty))
            .ForMember(d => d.HocKyId,           o => o.MapFrom(s => s.LopHocPhan != null ? s.LopHocPhan.HocKyId : 0))
            .ForMember(d => d.TenHocKy,          o => o.MapFrom(s => s.LopHocPhan != null ? s.LopHocPhan.HocKy.TenHocKy : string.Empty))
            .ForMember(d => d.TenGiaoVien,       o => o.MapFrom(s => s.LopHocPhan != null ? s.LopHocPhan.GiaoVien.TaiKhoan.HoTen : string.Empty))
            .ForMember(d => d.KhoaBangDiem,      o => o.MapFrom(s => s.LopHocPhan != null && s.LopHocPhan.KhoaBangDiem))
            .ForMember(d => d.TenSinhVien,       o => o.MapFrom(s => s.SinhVien != null ? s.SinhVien.TaiKhoan.HoTen : null))
            .ForMember(d => d.Mssv,              o => o.MapFrom(s => s.SinhVien != null ? s.SinhVien.Mssv : null));

        // ── LopHocPhan ────────────────────────────────────────────────────────
        CreateMap<LopHocPhan, LopHocPhanDto>()
            .ForMember(d => d.MaMon,       o => o.MapFrom(s => s.ChiTietCTDT != null ? s.ChiTietCTDT.MonHoc.MaMon : string.Empty))
            .ForMember(d => d.TenMon,      o => o.MapFrom(s => s.ChiTietCTDT != null ? s.ChiTietCTDT.MonHoc.TenMon : string.Empty))
            .ForMember(d => d.TenHocKy,    o => o.MapFrom(s => s.HocKy != null ? s.HocKy.TenHocKy : string.Empty))
            .ForMember(d => d.TenGiaoVien, o => o.MapFrom(s => s.GiaoVien != null ? s.GiaoVien.TaiKhoan.HoTen : string.Empty))
            .ForMember(d => d.SoSinhVien,  o => o.MapFrom(s => s.DanhSachLopHPs != null ? s.DanhSachLopHPs.Count : 0));

        // ── TuanHoc ───────────────────────────────────────────────────────────
        CreateMap<TuanHoc, TuanHocDto>()
            .ForMember(d => d.TenNamHoc, o => o.MapFrom(s => s.NamHoc != null ? s.NamHoc.TenNamHoc : string.Empty));

        // ── ThoiKhoaBieu ──────────────────────────────────────────────────────
        // Thu quy ước 2..8 (2 = Thứ Hai ... 7 = Thứ Bảy, 8 = Chủ nhật); NgayHoc = NgayBatDau (thứ Hai) + (Thu - 2) ngày.
        CreateMap<ThoiKhoaBieu, ThoiKhoaBieuDto>()
            .ForMember(d => d.MaLopHp,      o => o.MapFrom(s => s.LopHocPhan != null ? s.LopHocPhan.MaLopHp : string.Empty))
            .ForMember(d => d.MaMon,        o => o.MapFrom(s => s.LopHocPhan != null ? s.LopHocPhan.ChiTietCTDT.MonHoc.MaMon : string.Empty))
            .ForMember(d => d.TenMon,       o => o.MapFrom(s => s.LopHocPhan != null ? s.LopHocPhan.ChiTietCTDT.MonHoc.TenMon : string.Empty))
            .ForMember(d => d.TenGiaoVien,  o => o.MapFrom(s => s.LopHocPhan != null ? s.LopHocPhan.GiaoVien.TaiKhoan.HoTen : string.Empty))
            .ForMember(d => d.HocKyId,      o => o.MapFrom(s => s.LopHocPhan != null ? s.LopHocPhan.HocKyId : 0))
            .ForMember(d => d.TenHocKy,     o => o.MapFrom(s => s.LopHocPhan != null ? s.LopHocPhan.HocKy.TenHocKy : string.Empty))
            .ForMember(d => d.MaTuan,       o => o.MapFrom(s => s.TuanHoc != null ? s.TuanHoc.MaTuan : string.Empty))
            .ForMember(d => d.SoThuTuTuan,  o => o.MapFrom(s => s.TuanHoc != null ? s.TuanHoc.SoThuTuTuan : 0))
            .ForMember(d => d.NgayHoc,      o => o.MapFrom(s => s.TuanHoc != null ? s.TuanHoc.NgayBatDau.AddDays(s.Thu - 2) : default));

        // ── LopSinhHoat ───────────────────────────────────────────────────────
        CreateMap<LopSinhHoat, LopSinhHoatDto>()
            .ForMember(d => d.TenGvcn,  o => o.MapFrom(s => s.Gvcn != null && s.Gvcn.TaiKhoan != null ? s.Gvcn.TaiKhoan.HoTen : string.Empty))
            .ForMember(d => d.TenThuKy, o => o.MapFrom(s => s.ThuKy != null && s.ThuKy.TaiKhoan != null ? s.ThuKy.TaiKhoan.HoTen : null))
            .ForMember(d => d.MaCtdt,   o => o.MapFrom(s => s.ChuongTrinhDT != null ? s.ChuongTrinhDT.MaCtdt : string.Empty))
            .ForMember(d => d.SoSinhVien, o => o.MapFrom(s => s.SinhViens != null ? s.SinhViens.Count : 0));
    }
}
