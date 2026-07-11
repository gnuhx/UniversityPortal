using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UniversityPortal.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddPhongBanToNganhHoc : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "phong_ban_id",
                table: "nganh_hoc",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_nganh_hoc_phong_ban_id",
                table: "nganh_hoc",
                column: "phong_ban_id");

            migrationBuilder.AddForeignKey(
                name: "FK_nganh_hoc_phong_ban_phong_ban_id",
                table: "nganh_hoc",
                column: "phong_ban_id",
                principalTable: "phong_ban",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_nganh_hoc_phong_ban_phong_ban_id",
                table: "nganh_hoc");

            migrationBuilder.DropIndex(
                name: "IX_nganh_hoc_phong_ban_id",
                table: "nganh_hoc");

            migrationBuilder.DropColumn(
                name: "phong_ban_id",
                table: "nganh_hoc");
        }
    }
}
