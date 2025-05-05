using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CapacashApi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemovedCompanyIdFromTransactions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
          
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CompanyId",
                table: "Transactions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
