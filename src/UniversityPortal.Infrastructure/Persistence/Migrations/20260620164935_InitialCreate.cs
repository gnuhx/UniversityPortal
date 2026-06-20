using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UniversityPortal.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "mon_hoc",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ma_mon = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ten_mon = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mon_hoc", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "nam_hoc",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ten_nam_hoc = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_nam_hoc", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "nganh_hoc",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ma_nganh = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ten_nganh = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    nganh_cha_id = table.Column<int>(type: "int", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_nganh_hoc", x => x.Id);
                    table.ForeignKey(
                        name: "FK_nganh_hoc_nganh_hoc_nganh_cha_id",
                        column: x => x.nganh_cha_id,
                        principalTable: "nganh_hoc",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "phong_ban",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ten_phong_ban = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_phong_ban", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "vai_tro",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ten_vai_tro = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_vai_tro", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "hoc_ky",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ten_hoc_ky = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    nam_hoc_id = table.Column<int>(type: "int", nullable: false),
                    ngay_bat_dau = table.Column<DateOnly>(type: "date", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_hoc_ky", x => x.Id);
                    table.ForeignKey(
                        name: "FK_hoc_ky_nam_hoc_nam_hoc_id",
                        column: x => x.nam_hoc_id,
                        principalTable: "nam_hoc",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tuan_hoc",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nam_hoc_id = table.Column<int>(type: "int", nullable: false),
                    ma_tuan = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    so_thu_tu_tuan = table.Column<int>(type: "int", nullable: false),
                    ngay_bat_dau = table.Column<DateOnly>(type: "date", nullable: false),
                    ngay_ket_thuc = table.Column<DateOnly>(type: "date", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tuan_hoc", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tuan_hoc_nam_hoc_nam_hoc_id",
                        column: x => x.nam_hoc_id,
                        principalTable: "nam_hoc",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "chuong_trinh_dt",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ma_ctdt = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    nganh_id = table.Column<int>(type: "int", nullable: false),
                    khoa_hoc = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_chuong_trinh_dt", x => x.Id);
                    table.ForeignKey(
                        name: "FK_chuong_trinh_dt_nganh_hoc_nganh_id",
                        column: x => x.nganh_id,
                        principalTable: "nganh_hoc",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tai_khoan",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ten_dang_nhap = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    mat_khau = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    vai_tro_id = table.Column<int>(type: "int", nullable: false),
                    phong_ban_id = table.Column<int>(type: "int", nullable: true),
                    ho_ten = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    email = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    anh_dai_dien = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    trang_thai = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    refresh_token = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    refresh_token_expiry = table.Column<DateTime>(type: "datetime2", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tai_khoan", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tai_khoan_phong_ban_phong_ban_id",
                        column: x => x.phong_ban_id,
                        principalTable: "phong_ban",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_tai_khoan_vai_tro_vai_tro_id",
                        column: x => x.vai_tro_id,
                        principalTable: "vai_tro",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "chi_tiet_ctdt",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ctdt_id = table.Column<int>(type: "int", nullable: false),
                    mon_hoc_id = table.Column<int>(type: "int", nullable: false),
                    hoc_ky_id = table.Column<int>(type: "int", nullable: false),
                    so_tin_chi = table.Column<int>(type: "int", nullable: false),
                    tinh_diem_tb = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_chi_tiet_ctdt", x => x.Id);
                    table.ForeignKey(
                        name: "FK_chi_tiet_ctdt_chuong_trinh_dt_ctdt_id",
                        column: x => x.ctdt_id,
                        principalTable: "chuong_trinh_dt",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_chi_tiet_ctdt_hoc_ky_hoc_ky_id",
                        column: x => x.hoc_ky_id,
                        principalTable: "hoc_ky",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_chi_tiet_ctdt_mon_hoc_mon_hoc_id",
                        column: x => x.mon_hoc_id,
                        principalTable: "mon_hoc",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "giao_vien",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    tai_khoan_id = table.Column<int>(type: "int", nullable: false),
                    ma_gv = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_giao_vien", x => x.Id);
                    table.ForeignKey(
                        name: "FK_giao_vien_tai_khoan_tai_khoan_id",
                        column: x => x.tai_khoan_id,
                        principalTable: "tai_khoan",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "dat_phong_thuc_hanh",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    giao_vien_id = table.Column<int>(type: "int", nullable: false),
                    phong_hoc = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ngay_dat = table.Column<DateOnly>(type: "date", nullable: false),
                    ca_hoc = table.Column<int>(type: "int", nullable: false),
                    ly_do = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    trang_thai = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    nguoi_duyet_id = table.Column<int>(type: "int", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dat_phong_thuc_hanh", x => x.Id);
                    table.ForeignKey(
                        name: "FK_dat_phong_thuc_hanh_giao_vien_giao_vien_id",
                        column: x => x.giao_vien_id,
                        principalTable: "giao_vien",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_dat_phong_thuc_hanh_tai_khoan_nguoi_duyet_id",
                        column: x => x.nguoi_duyet_id,
                        principalTable: "tai_khoan",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "lop_hoc_phan",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    chi_tiet_ctdt_id = table.Column<int>(type: "int", nullable: false),
                    hoc_ky_id = table.Column<int>(type: "int", nullable: false),
                    giao_vien_id = table.Column<int>(type: "int", nullable: false),
                    ma_lop_hp = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    khoa_bang_diem = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    trang_thai_ket_thuc = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_lop_hoc_phan", x => x.Id);
                    table.ForeignKey(
                        name: "FK_lop_hoc_phan_chi_tiet_ctdt_chi_tiet_ctdt_id",
                        column: x => x.chi_tiet_ctdt_id,
                        principalTable: "chi_tiet_ctdt",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_lop_hoc_phan_giao_vien_giao_vien_id",
                        column: x => x.giao_vien_id,
                        principalTable: "giao_vien",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_lop_hoc_phan_hoc_ky_hoc_ky_id",
                        column: x => x.hoc_ky_id,
                        principalTable: "hoc_ky",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "thoi_khoa_bieu",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    lop_hp_id = table.Column<int>(type: "int", nullable: false),
                    tuan_hoc_id = table.Column<int>(type: "int", nullable: false),
                    thu = table.Column<int>(type: "int", nullable: false),
                    tiet_bat_dau = table.Column<int>(type: "int", nullable: false),
                    tiet_ket_thuc = table.Column<int>(type: "int", nullable: false),
                    phong_hoc = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_thoi_khoa_bieu", x => x.Id);
                    table.ForeignKey(
                        name: "FK_thoi_khoa_bieu_lop_hoc_phan_lop_hp_id",
                        column: x => x.lop_hp_id,
                        principalTable: "lop_hoc_phan",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_thoi_khoa_bieu_tuan_hoc_tuan_hoc_id",
                        column: x => x.tuan_hoc_id,
                        principalTable: "tuan_hoc",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "yeu_cau_sua_diem",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    lop_hp_id = table.Column<int>(type: "int", nullable: false),
                    giao_vien_id = table.Column<int>(type: "int", nullable: false),
                    ly_do = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    trang_thai = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    nguoi_duyet_id = table.Column<int>(type: "int", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_yeu_cau_sua_diem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_yeu_cau_sua_diem_giao_vien_giao_vien_id",
                        column: x => x.giao_vien_id,
                        principalTable: "giao_vien",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_yeu_cau_sua_diem_lop_hoc_phan_lop_hp_id",
                        column: x => x.lop_hp_id,
                        principalTable: "lop_hoc_phan",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_yeu_cau_sua_diem_tai_khoan_nguoi_duyet_id",
                        column: x => x.nguoi_duyet_id,
                        principalTable: "tai_khoan",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "bien_ban_shcn",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    lop_id = table.Column<int>(type: "int", nullable: false),
                    tuan_hoc_id = table.Column<int>(type: "int", nullable: false),
                    thoi_gian = table.Column<DateTime>(type: "datetime2", nullable: false),
                    dia_diem = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    gvcn_id = table.Column<int>(type: "int", nullable: false),
                    thu_ky_id = table.Column<int>(type: "int", nullable: false),
                    noi_dung = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    phan_hoi_gvcn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_bien_ban_shcn", x => x.Id);
                    table.ForeignKey(
                        name: "FK_bien_ban_shcn_giao_vien_gvcn_id",
                        column: x => x.gvcn_id,
                        principalTable: "giao_vien",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_bien_ban_shcn_tuan_hoc_tuan_hoc_id",
                        column: x => x.tuan_hoc_id,
                        principalTable: "tuan_hoc",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "chi_tiet_cong_viec",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    bien_ban_id = table.Column<int>(type: "int", nullable: false),
                    ten_cong_viec = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    trang_thai_viec = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_chi_tiet_cong_viec", x => x.Id);
                    table.ForeignKey(
                        name: "FK_chi_tiet_cong_viec_bien_ban_shcn_bien_ban_id",
                        column: x => x.bien_ban_id,
                        principalTable: "bien_ban_shcn",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "binh_luan_thong_bao",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    thong_bao_id = table.Column<int>(type: "int", nullable: false),
                    tai_khoan_id = table.Column<int>(type: "int", nullable: false),
                    noi_dung = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ngay_binh_luan = table.Column<DateTime>(type: "datetime2", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_binh_luan_thong_bao", x => x.Id);
                    table.ForeignKey(
                        name: "FK_binh_luan_thong_bao_tai_khoan_tai_khoan_id",
                        column: x => x.tai_khoan_id,
                        principalTable: "tai_khoan",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "chi_tiet_vang_shcn",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    bien_ban_id = table.Column<int>(type: "int", nullable: false),
                    sinh_vien_id = table.Column<int>(type: "int", nullable: false),
                    ly_do = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    co_phep = table.Column<bool>(type: "bit", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_chi_tiet_vang_shcn", x => x.Id);
                    table.ForeignKey(
                        name: "FK_chi_tiet_vang_shcn_bien_ban_shcn_bien_ban_id",
                        column: x => x.bien_ban_id,
                        principalTable: "bien_ban_shcn",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "danh_sach_lop_hp",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    sinh_vien_id = table.Column<int>(type: "int", nullable: false),
                    lop_hp_id = table.Column<int>(type: "int", nullable: false),
                    loai_dang_ky = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    trang_thai_duyet = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    nguoi_duyet_id = table.Column<int>(type: "int", nullable: true),
                    diem_qt1 = table.Column<float>(type: "real", nullable: true),
                    diem_qt2 = table.Column<float>(type: "real", nullable: true),
                    diem_thi = table.Column<float>(type: "real", nullable: true),
                    diem_tong_ket = table.Column<float>(type: "real", nullable: true),
                    so_tien_phai_dong = table.Column<decimal>(type: "decimal(12,2)", nullable: true),
                    trang_thai_dong_tien = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_danh_sach_lop_hp", x => x.Id);
                    table.ForeignKey(
                        name: "FK_danh_sach_lop_hp_lop_hoc_phan_lop_hp_id",
                        column: x => x.lop_hp_id,
                        principalTable: "lop_hoc_phan",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_danh_sach_lop_hp_tai_khoan_nguoi_duyet_id",
                        column: x => x.nguoi_duyet_id,
                        principalTable: "tai_khoan",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "danh_sach_thi_lai",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    sinh_vien_id = table.Column<int>(type: "int", nullable: false),
                    lop_hp_id = table.Column<int>(type: "int", nullable: false),
                    diem_thi_lai = table.Column<float>(type: "real", nullable: true),
                    so_tien_phai_dong = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    trang_thai_dong_tien = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    nguoi_duyet_id = table.Column<int>(type: "int", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_danh_sach_thi_lai", x => x.Id);
                    table.ForeignKey(
                        name: "FK_danh_sach_thi_lai_lop_hoc_phan_lop_hp_id",
                        column: x => x.lop_hp_id,
                        principalTable: "lop_hoc_phan",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_danh_sach_thi_lai_tai_khoan_nguoi_duyet_id",
                        column: x => x.nguoi_duyet_id,
                        principalTable: "tai_khoan",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "diem_ren_luyen",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    sinh_vien_id = table.Column<int>(type: "int", nullable: false),
                    hoc_ky_id = table.Column<int>(type: "int", nullable: false),
                    diem_tong = table.Column<int>(type: "int", nullable: false),
                    xep_loai = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_diem_ren_luyen", x => x.Id);
                    table.ForeignKey(
                        name: "FK_diem_ren_luyen_hoc_ky_hoc_ky_id",
                        column: x => x.hoc_ky_id,
                        principalTable: "hoc_ky",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "dien_dan_giao_vien",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    lop_id = table.Column<int>(type: "int", nullable: false),
                    giao_vien_id = table.Column<int>(type: "int", nullable: false),
                    noi_dung = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ngay_gui = table.Column<DateTime>(type: "datetime2", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dien_dan_giao_vien", x => x.Id);
                    table.ForeignKey(
                        name: "FK_dien_dan_giao_vien_giao_vien_giao_vien_id",
                        column: x => x.giao_vien_id,
                        principalTable: "giao_vien",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "hoc_ba",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    sinh_vien_id = table.Column<int>(type: "int", nullable: false),
                    ctdt_id = table.Column<int>(type: "int", nullable: false),
                    diem_tbc_tich_luy = table.Column<float>(type: "real", nullable: false),
                    so_tin_chi_tich_luy = table.Column<int>(type: "int", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_hoc_ba", x => x.Id);
                    table.ForeignKey(
                        name: "FK_hoc_ba_chuong_trinh_dt_ctdt_id",
                        column: x => x.ctdt_id,
                        principalTable: "chuong_trinh_dt",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "hoc_phi",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    sinh_vien_id = table.Column<int>(type: "int", nullable: false),
                    hoc_ky_id = table.Column<int>(type: "int", nullable: false),
                    so_tien = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    trang_thai_dong = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_hoc_phi", x => x.Id);
                    table.ForeignKey(
                        name: "FK_hoc_phi_hoc_ky_hoc_ky_id",
                        column: x => x.hoc_ky_id,
                        principalTable: "hoc_ky",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ket_qua_anh_van_dau_vao",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    sinh_vien_id = table.Column<int>(type: "int", nullable: false),
                    hinh_thuc_xet = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    diem_thi = table.Column<float>(type: "real", nullable: true),
                    diem_ta1 = table.Column<float>(type: "real", nullable: true),
                    diem_ta2 = table.Column<float>(type: "real", nullable: true),
                    diem_ta3 = table.Column<float>(type: "real", nullable: true),
                    ghi_chu = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ket_qua_anh_van_dau_vao", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "khao_sat_y_kien",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    sinh_vien_id = table.Column<int>(type: "int", nullable: false),
                    lop_hp_id = table.Column<int>(type: "int", nullable: false),
                    diem_danh_gia = table.Column<int>(type: "int", nullable: false),
                    gop_y = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_khao_sat_y_kien", x => x.Id);
                    table.ForeignKey(
                        name: "FK_khao_sat_y_kien_lop_hoc_phan_lop_hp_id",
                        column: x => x.lop_hp_id,
                        principalTable: "lop_hoc_phan",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "lop_sinh_hoat",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ma_lop = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    gvcn_id = table.Column<int>(type: "int", nullable: false),
                    thu_ky_id = table.Column<int>(type: "int", nullable: true),
                    chuong_trinh_dt_id = table.Column<int>(type: "int", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_lop_sinh_hoat", x => x.Id);
                    table.ForeignKey(
                        name: "FK_lop_sinh_hoat_chuong_trinh_dt_chuong_trinh_dt_id",
                        column: x => x.chuong_trinh_dt_id,
                        principalTable: "chuong_trinh_dt",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_lop_sinh_hoat_giao_vien_gvcn_id",
                        column: x => x.gvcn_id,
                        principalTable: "giao_vien",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "sinh_vien",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    tai_khoan_id = table.Column<int>(type: "int", nullable: false),
                    mssv = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    lop_id = table.Column<int>(type: "int", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sinh_vien", x => x.Id);
                    table.ForeignKey(
                        name: "FK_sinh_vien_lop_sinh_hoat_lop_id",
                        column: x => x.lop_id,
                        principalTable: "lop_sinh_hoat",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_sinh_vien_tai_khoan_tai_khoan_id",
                        column: x => x.tai_khoan_id,
                        principalTable: "tai_khoan",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "thong_bao",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    loai_thong_bao = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    muc_do = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    tieu_de = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    noi_dung = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    nguoi_tao_id = table.Column<int>(type: "int", nullable: false),
                    lop_nhan_id = table.Column<int>(type: "int", nullable: true),
                    ngay_tao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_thong_bao", x => x.Id);
                    table.ForeignKey(
                        name: "FK_thong_bao_lop_sinh_hoat_lop_nhan_id",
                        column: x => x.lop_nhan_id,
                        principalTable: "lop_sinh_hoat",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_thong_bao_tai_khoan_nguoi_tao_id",
                        column: x => x.nguoi_tao_id,
                        principalTable: "tai_khoan",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "yeu_cau_hanh_chinh",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    sinh_vien_id = table.Column<int>(type: "int", nullable: false),
                    loai_yeu_cau = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    noi_dung = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    file_dinh_kem = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    trang_thai = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    nguoi_duyet_id = table.Column<int>(type: "int", nullable: true),
                    ngay_tao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_yeu_cau_hanh_chinh", x => x.Id);
                    table.ForeignKey(
                        name: "FK_yeu_cau_hanh_chinh_sinh_vien_sinh_vien_id",
                        column: x => x.sinh_vien_id,
                        principalTable: "sinh_vien",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_yeu_cau_hanh_chinh_tai_khoan_nguoi_duyet_id",
                        column: x => x.nguoi_duyet_id,
                        principalTable: "tai_khoan",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "thong_bao_da_doc",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    thong_bao_id = table.Column<int>(type: "int", nullable: false),
                    tai_khoan_id = table.Column<int>(type: "int", nullable: false),
                    da_doc = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    ngay_doc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_thong_bao_da_doc", x => x.Id);
                    table.ForeignKey(
                        name: "FK_thong_bao_da_doc_tai_khoan_tai_khoan_id",
                        column: x => x.tai_khoan_id,
                        principalTable: "tai_khoan",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_thong_bao_da_doc_thong_bao_thong_bao_id",
                        column: x => x.thong_bao_id,
                        principalTable: "thong_bao",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_bien_ban_shcn_gvcn_id",
                table: "bien_ban_shcn",
                column: "gvcn_id");

            migrationBuilder.CreateIndex(
                name: "IX_bien_ban_shcn_lop_id",
                table: "bien_ban_shcn",
                column: "lop_id");

            migrationBuilder.CreateIndex(
                name: "IX_bien_ban_shcn_thu_ky_id",
                table: "bien_ban_shcn",
                column: "thu_ky_id");

            migrationBuilder.CreateIndex(
                name: "IX_bien_ban_shcn_tuan_hoc_id",
                table: "bien_ban_shcn",
                column: "tuan_hoc_id");

            migrationBuilder.CreateIndex(
                name: "IX_binh_luan_thong_bao_tai_khoan_id",
                table: "binh_luan_thong_bao",
                column: "tai_khoan_id");

            migrationBuilder.CreateIndex(
                name: "IX_binh_luan_thong_bao_thong_bao_id",
                table: "binh_luan_thong_bao",
                column: "thong_bao_id");

            migrationBuilder.CreateIndex(
                name: "IX_chi_tiet_cong_viec_bien_ban_id",
                table: "chi_tiet_cong_viec",
                column: "bien_ban_id");

            migrationBuilder.CreateIndex(
                name: "IX_chi_tiet_ctdt_ctdt_id",
                table: "chi_tiet_ctdt",
                column: "ctdt_id");

            migrationBuilder.CreateIndex(
                name: "IX_chi_tiet_ctdt_hoc_ky_id",
                table: "chi_tiet_ctdt",
                column: "hoc_ky_id");

            migrationBuilder.CreateIndex(
                name: "IX_chi_tiet_ctdt_mon_hoc_id",
                table: "chi_tiet_ctdt",
                column: "mon_hoc_id");

            migrationBuilder.CreateIndex(
                name: "IX_chi_tiet_vang_shcn_bien_ban_id",
                table: "chi_tiet_vang_shcn",
                column: "bien_ban_id");

            migrationBuilder.CreateIndex(
                name: "IX_chi_tiet_vang_shcn_sinh_vien_id",
                table: "chi_tiet_vang_shcn",
                column: "sinh_vien_id");

            migrationBuilder.CreateIndex(
                name: "IX_chuong_trinh_dt_ma_ctdt",
                table: "chuong_trinh_dt",
                column: "ma_ctdt",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_chuong_trinh_dt_nganh_id",
                table: "chuong_trinh_dt",
                column: "nganh_id");

            migrationBuilder.CreateIndex(
                name: "IX_danh_sach_lop_hp_lop_hp_id",
                table: "danh_sach_lop_hp",
                column: "lop_hp_id");

            migrationBuilder.CreateIndex(
                name: "IX_danh_sach_lop_hp_nguoi_duyet_id",
                table: "danh_sach_lop_hp",
                column: "nguoi_duyet_id");

            migrationBuilder.CreateIndex(
                name: "IX_danh_sach_lop_hp_sinh_vien_id_lop_hp_id",
                table: "danh_sach_lop_hp",
                columns: new[] { "sinh_vien_id", "lop_hp_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_danh_sach_thi_lai_lop_hp_id",
                table: "danh_sach_thi_lai",
                column: "lop_hp_id");

            migrationBuilder.CreateIndex(
                name: "IX_danh_sach_thi_lai_nguoi_duyet_id",
                table: "danh_sach_thi_lai",
                column: "nguoi_duyet_id");

            migrationBuilder.CreateIndex(
                name: "IX_danh_sach_thi_lai_sinh_vien_id",
                table: "danh_sach_thi_lai",
                column: "sinh_vien_id");

            migrationBuilder.CreateIndex(
                name: "IX_dat_phong_thuc_hanh_giao_vien_id",
                table: "dat_phong_thuc_hanh",
                column: "giao_vien_id");

            migrationBuilder.CreateIndex(
                name: "IX_dat_phong_thuc_hanh_nguoi_duyet_id",
                table: "dat_phong_thuc_hanh",
                column: "nguoi_duyet_id");

            migrationBuilder.CreateIndex(
                name: "IX_diem_ren_luyen_hoc_ky_id",
                table: "diem_ren_luyen",
                column: "hoc_ky_id");

            migrationBuilder.CreateIndex(
                name: "IX_diem_ren_luyen_sinh_vien_id_hoc_ky_id",
                table: "diem_ren_luyen",
                columns: new[] { "sinh_vien_id", "hoc_ky_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_dien_dan_giao_vien_giao_vien_id",
                table: "dien_dan_giao_vien",
                column: "giao_vien_id");

            migrationBuilder.CreateIndex(
                name: "IX_dien_dan_giao_vien_lop_id",
                table: "dien_dan_giao_vien",
                column: "lop_id");

            migrationBuilder.CreateIndex(
                name: "IX_giao_vien_ma_gv",
                table: "giao_vien",
                column: "ma_gv",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_giao_vien_tai_khoan_id",
                table: "giao_vien",
                column: "tai_khoan_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_hoc_ba_ctdt_id",
                table: "hoc_ba",
                column: "ctdt_id");

            migrationBuilder.CreateIndex(
                name: "IX_hoc_ba_sinh_vien_id_ctdt_id",
                table: "hoc_ba",
                columns: new[] { "sinh_vien_id", "ctdt_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_hoc_ky_nam_hoc_id",
                table: "hoc_ky",
                column: "nam_hoc_id");

            migrationBuilder.CreateIndex(
                name: "IX_hoc_phi_hoc_ky_id",
                table: "hoc_phi",
                column: "hoc_ky_id");

            migrationBuilder.CreateIndex(
                name: "IX_hoc_phi_sinh_vien_id_hoc_ky_id",
                table: "hoc_phi",
                columns: new[] { "sinh_vien_id", "hoc_ky_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ket_qua_anh_van_dau_vao_sinh_vien_id",
                table: "ket_qua_anh_van_dau_vao",
                column: "sinh_vien_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_khao_sat_y_kien_lop_hp_id",
                table: "khao_sat_y_kien",
                column: "lop_hp_id");

            migrationBuilder.CreateIndex(
                name: "IX_khao_sat_y_kien_sinh_vien_id_lop_hp_id",
                table: "khao_sat_y_kien",
                columns: new[] { "sinh_vien_id", "lop_hp_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_lop_hoc_phan_chi_tiet_ctdt_id",
                table: "lop_hoc_phan",
                column: "chi_tiet_ctdt_id");

            migrationBuilder.CreateIndex(
                name: "IX_lop_hoc_phan_giao_vien_id",
                table: "lop_hoc_phan",
                column: "giao_vien_id");

            migrationBuilder.CreateIndex(
                name: "IX_lop_hoc_phan_hoc_ky_id",
                table: "lop_hoc_phan",
                column: "hoc_ky_id");

            migrationBuilder.CreateIndex(
                name: "IX_lop_hoc_phan_ma_lop_hp",
                table: "lop_hoc_phan",
                column: "ma_lop_hp",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_lop_sinh_hoat_chuong_trinh_dt_id",
                table: "lop_sinh_hoat",
                column: "chuong_trinh_dt_id");

            migrationBuilder.CreateIndex(
                name: "IX_lop_sinh_hoat_gvcn_id",
                table: "lop_sinh_hoat",
                column: "gvcn_id");

            migrationBuilder.CreateIndex(
                name: "IX_lop_sinh_hoat_ma_lop",
                table: "lop_sinh_hoat",
                column: "ma_lop",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_lop_sinh_hoat_thu_ky_id",
                table: "lop_sinh_hoat",
                column: "thu_ky_id");

            migrationBuilder.CreateIndex(
                name: "IX_mon_hoc_ma_mon",
                table: "mon_hoc",
                column: "ma_mon",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_nganh_hoc_ma_nganh",
                table: "nganh_hoc",
                column: "ma_nganh",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_nganh_hoc_nganh_cha_id",
                table: "nganh_hoc",
                column: "nganh_cha_id");

            migrationBuilder.CreateIndex(
                name: "IX_sinh_vien_lop_id",
                table: "sinh_vien",
                column: "lop_id");

            migrationBuilder.CreateIndex(
                name: "IX_sinh_vien_mssv",
                table: "sinh_vien",
                column: "mssv",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_sinh_vien_tai_khoan_id",
                table: "sinh_vien",
                column: "tai_khoan_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_tai_khoan_email",
                table: "tai_khoan",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_tai_khoan_phong_ban_id",
                table: "tai_khoan",
                column: "phong_ban_id");

            migrationBuilder.CreateIndex(
                name: "IX_tai_khoan_ten_dang_nhap",
                table: "tai_khoan",
                column: "ten_dang_nhap",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_tai_khoan_vai_tro_id",
                table: "tai_khoan",
                column: "vai_tro_id");

            migrationBuilder.CreateIndex(
                name: "IX_thoi_khoa_bieu_lop_hp_id",
                table: "thoi_khoa_bieu",
                column: "lop_hp_id");

            migrationBuilder.CreateIndex(
                name: "IX_thoi_khoa_bieu_tuan_hoc_id",
                table: "thoi_khoa_bieu",
                column: "tuan_hoc_id");

            migrationBuilder.CreateIndex(
                name: "IX_thong_bao_lop_nhan_id",
                table: "thong_bao",
                column: "lop_nhan_id");

            migrationBuilder.CreateIndex(
                name: "IX_thong_bao_nguoi_tao_id",
                table: "thong_bao",
                column: "nguoi_tao_id");

            migrationBuilder.CreateIndex(
                name: "IX_thong_bao_da_doc_tai_khoan_id",
                table: "thong_bao_da_doc",
                column: "tai_khoan_id");

            migrationBuilder.CreateIndex(
                name: "IX_thong_bao_da_doc_thong_bao_id_tai_khoan_id",
                table: "thong_bao_da_doc",
                columns: new[] { "thong_bao_id", "tai_khoan_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_tuan_hoc_ma_tuan",
                table: "tuan_hoc",
                column: "ma_tuan",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_tuan_hoc_nam_hoc_id",
                table: "tuan_hoc",
                column: "nam_hoc_id");

            migrationBuilder.CreateIndex(
                name: "IX_yeu_cau_hanh_chinh_nguoi_duyet_id",
                table: "yeu_cau_hanh_chinh",
                column: "nguoi_duyet_id");

            migrationBuilder.CreateIndex(
                name: "IX_yeu_cau_hanh_chinh_sinh_vien_id",
                table: "yeu_cau_hanh_chinh",
                column: "sinh_vien_id");

            migrationBuilder.CreateIndex(
                name: "IX_yeu_cau_sua_diem_giao_vien_id",
                table: "yeu_cau_sua_diem",
                column: "giao_vien_id");

            migrationBuilder.CreateIndex(
                name: "IX_yeu_cau_sua_diem_lop_hp_id",
                table: "yeu_cau_sua_diem",
                column: "lop_hp_id");

            migrationBuilder.CreateIndex(
                name: "IX_yeu_cau_sua_diem_nguoi_duyet_id",
                table: "yeu_cau_sua_diem",
                column: "nguoi_duyet_id");

            migrationBuilder.AddForeignKey(
                name: "FK_bien_ban_shcn_lop_sinh_hoat_lop_id",
                table: "bien_ban_shcn",
                column: "lop_id",
                principalTable: "lop_sinh_hoat",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_bien_ban_shcn_sinh_vien_thu_ky_id",
                table: "bien_ban_shcn",
                column: "thu_ky_id",
                principalTable: "sinh_vien",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_binh_luan_thong_bao_thong_bao_thong_bao_id",
                table: "binh_luan_thong_bao",
                column: "thong_bao_id",
                principalTable: "thong_bao",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_chi_tiet_vang_shcn_sinh_vien_sinh_vien_id",
                table: "chi_tiet_vang_shcn",
                column: "sinh_vien_id",
                principalTable: "sinh_vien",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_danh_sach_lop_hp_sinh_vien_sinh_vien_id",
                table: "danh_sach_lop_hp",
                column: "sinh_vien_id",
                principalTable: "sinh_vien",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_danh_sach_thi_lai_sinh_vien_sinh_vien_id",
                table: "danh_sach_thi_lai",
                column: "sinh_vien_id",
                principalTable: "sinh_vien",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_diem_ren_luyen_sinh_vien_sinh_vien_id",
                table: "diem_ren_luyen",
                column: "sinh_vien_id",
                principalTable: "sinh_vien",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_dien_dan_giao_vien_lop_sinh_hoat_lop_id",
                table: "dien_dan_giao_vien",
                column: "lop_id",
                principalTable: "lop_sinh_hoat",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_hoc_ba_sinh_vien_sinh_vien_id",
                table: "hoc_ba",
                column: "sinh_vien_id",
                principalTable: "sinh_vien",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_hoc_phi_sinh_vien_sinh_vien_id",
                table: "hoc_phi",
                column: "sinh_vien_id",
                principalTable: "sinh_vien",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ket_qua_anh_van_dau_vao_sinh_vien_sinh_vien_id",
                table: "ket_qua_anh_van_dau_vao",
                column: "sinh_vien_id",
                principalTable: "sinh_vien",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_khao_sat_y_kien_sinh_vien_sinh_vien_id",
                table: "khao_sat_y_kien",
                column: "sinh_vien_id",
                principalTable: "sinh_vien",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_lop_sinh_hoat_sinh_vien_thu_ky_id",
                table: "lop_sinh_hoat",
                column: "thu_ky_id",
                principalTable: "sinh_vien",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_lop_sinh_hoat_giao_vien_gvcn_id",
                table: "lop_sinh_hoat");

            migrationBuilder.DropForeignKey(
                name: "FK_sinh_vien_lop_sinh_hoat_lop_id",
                table: "sinh_vien");

            migrationBuilder.DropTable(
                name: "binh_luan_thong_bao");

            migrationBuilder.DropTable(
                name: "chi_tiet_cong_viec");

            migrationBuilder.DropTable(
                name: "chi_tiet_vang_shcn");

            migrationBuilder.DropTable(
                name: "danh_sach_lop_hp");

            migrationBuilder.DropTable(
                name: "danh_sach_thi_lai");

            migrationBuilder.DropTable(
                name: "dat_phong_thuc_hanh");

            migrationBuilder.DropTable(
                name: "diem_ren_luyen");

            migrationBuilder.DropTable(
                name: "dien_dan_giao_vien");

            migrationBuilder.DropTable(
                name: "hoc_ba");

            migrationBuilder.DropTable(
                name: "hoc_phi");

            migrationBuilder.DropTable(
                name: "ket_qua_anh_van_dau_vao");

            migrationBuilder.DropTable(
                name: "khao_sat_y_kien");

            migrationBuilder.DropTable(
                name: "thoi_khoa_bieu");

            migrationBuilder.DropTable(
                name: "thong_bao_da_doc");

            migrationBuilder.DropTable(
                name: "yeu_cau_hanh_chinh");

            migrationBuilder.DropTable(
                name: "yeu_cau_sua_diem");

            migrationBuilder.DropTable(
                name: "bien_ban_shcn");

            migrationBuilder.DropTable(
                name: "thong_bao");

            migrationBuilder.DropTable(
                name: "lop_hoc_phan");

            migrationBuilder.DropTable(
                name: "tuan_hoc");

            migrationBuilder.DropTable(
                name: "chi_tiet_ctdt");

            migrationBuilder.DropTable(
                name: "hoc_ky");

            migrationBuilder.DropTable(
                name: "mon_hoc");

            migrationBuilder.DropTable(
                name: "nam_hoc");

            migrationBuilder.DropTable(
                name: "giao_vien");

            migrationBuilder.DropTable(
                name: "lop_sinh_hoat");

            migrationBuilder.DropTable(
                name: "chuong_trinh_dt");

            migrationBuilder.DropTable(
                name: "sinh_vien");

            migrationBuilder.DropTable(
                name: "nganh_hoc");

            migrationBuilder.DropTable(
                name: "tai_khoan");

            migrationBuilder.DropTable(
                name: "phong_ban");

            migrationBuilder.DropTable(
                name: "vai_tro");
        }
    }
}
