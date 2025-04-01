using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CapacashApi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChangeCompanyIdToString : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CompanyId",
                table: "Kiosks",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "Kiosks");
        }
    }
}
