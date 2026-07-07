using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UniversityPortal.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddNoiDungTinh : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "noi_dung_tinh",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    khu_vuc = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ma_muc = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    tieu_de = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    noi_dung = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    thu_tu = table.Column<int>(type: "int", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_noi_dung_tinh", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_noi_dung_tinh_khu_vuc_thu_tu",
                table: "noi_dung_tinh",
                columns: new[] { "khu_vuc", "thu_tu" });

            var seedTime = new DateTime(2026, 7, 7, 0, 0, 0, DateTimeKind.Utc);

            migrationBuilder.InsertData(
                table: "noi_dung_tinh",
                columns: new[] { "Id", "khu_vuc", "ma_muc", "tieu_de", "noi_dung", "thu_tu", "created_at", "updated_at" },
                values: new object[,]
                {
                    { 1,  "thu-vien", "gioi-thieu",       "Giới thiệu",           null, 1, seedTime, seedTime },
                    { 2,  "thu-vien", "huong-dan",         "Hướng Dẫn",            null, 2, seedTime, seedTime },
                    { 3,  "thu-vien", "tra-cuu",           "Tra Cứu",              null, 3, seedTime, seedTime },
                    { 4,  "thu-vien", "tai-lieu-moi",      "Tài liệu mới",         null, 4, seedTime, seedTime },
                    { 5,  "thu-vien", "dich-vu",           "Dịch vụ",              null, 5, seedTime, seedTime },
                    { 6,  "thu-vien", "hoat-dong",         "Hoạt động",            null, 6, seedTime, seedTime },
                    { 7,  "thu-vien", "co-so-du-lieu",     "Cơ sở dữ liệu",        null, 7, seedTime, seedTime },
                    { 8,  "thu-vien", "media",             "Media",                null, 8, seedTime, seedTime },
                    { 9,  "thu-vien", "lien-he",           "Liên hệ",              null, 9, seedTime, seedTime },
                    { 10, "hoc-vu",   "quy-che-quy-dinh",  "Quy Chế - Quy Định",   null, 1, seedTime, seedTime },
                    { 11, "hoc-vu",   "giang-vien",        "Giảng viên",           null, 2, seedTime, seedTime },
                    { 12, "hoc-vu",   "sinh-vien",         "Sinh Viên",            null, 3, seedTime, seedTime },
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "noi_dung_tinh");
        }
    }
}
