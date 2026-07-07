using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UniversityPortal.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddGiayXacNhanVaPhanHoiToYeuCauHanhChinh : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ghi_chu_admin",
                table: "yeu_cau_hanh_chinh",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "loai_giay_xac_nhan",
                table: "yeu_cau_hanh_chinh",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ghi_chu_admin",
                table: "yeu_cau_hanh_chinh");

            migrationBuilder.DropColumn(
                name: "loai_giay_xac_nhan",
                table: "yeu_cau_hanh_chinh");
        }
    }
}
