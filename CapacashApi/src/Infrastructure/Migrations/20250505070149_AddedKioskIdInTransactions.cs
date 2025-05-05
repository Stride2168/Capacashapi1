using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CapacashApi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedKioskIdInTransactions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "KioskId",
                table: "Transactions",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_KioskId",
                table: "Transactions",
                column: "KioskId");

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Kiosks_KioskId",
                table: "Transactions",
                column: "KioskId",
                principalTable: "Kiosks",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Kiosks_KioskId",
                table: "Transactions");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_KioskId",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "KioskId",
                table: "Transactions");
        }
    }
}
