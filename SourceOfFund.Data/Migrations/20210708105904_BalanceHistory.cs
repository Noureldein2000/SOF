using Microsoft.EntityFrameworkCore.Migrations;

namespace SourceOfFund.Data.Migrations
{
    public partial class BalanceHistory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BalanceHistory_BalanceTypes_BalanceTypeID",
                table: "BalanceHistory");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BalanceHistory",
                table: "BalanceHistory");

            migrationBuilder.RenameTable(
                name: "BalanceHistory",
                newName: "BalanceHistories");

            migrationBuilder.RenameIndex(
                name: "IX_BalanceHistory_BalanceTypeID",
                table: "BalanceHistories",
                newName: "IX_BalanceHistories_BalanceTypeID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BalanceHistories",
                table: "BalanceHistories",
                column: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_BalanceHistories_BalanceTypes_BalanceTypeID",
                table: "BalanceHistories",
                column: "BalanceTypeID",
                principalTable: "BalanceTypes",
                principalColumn: "ID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BalanceHistories_BalanceTypes_BalanceTypeID",
                table: "BalanceHistories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BalanceHistories",
                table: "BalanceHistories");

            migrationBuilder.RenameTable(
                name: "BalanceHistories",
                newName: "BalanceHistory");

            migrationBuilder.RenameIndex(
                name: "IX_BalanceHistories_BalanceTypeID",
                table: "BalanceHistory",
                newName: "IX_BalanceHistory_BalanceTypeID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BalanceHistory",
                table: "BalanceHistory",
                column: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_BalanceHistory_BalanceTypes_BalanceTypeID",
                table: "BalanceHistory",
                column: "BalanceTypeID",
                principalTable: "BalanceTypes",
                principalColumn: "ID");
        }
    }
}
