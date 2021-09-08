using Microsoft.EntityFrameworkCore.Migrations;

namespace SourceOfFund.Data.Migrations
{
    public partial class ModifyAvailableBalanceBefore : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "AvailableBalanceBefore",
                table: "HoldBalances",
                type: "decimal(18, 3)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "BalanceTypeID",
                table: "HoldBalances",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalBalance",
                table: "BalanceHistory",
                type: "decimal(18,3)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateIndex(
                name: "IX_HoldBalances_BalanceTypeID",
                table: "HoldBalances",
                column: "BalanceTypeID");

            migrationBuilder.AddForeignKey(
                name: "FK_HoldBalances_BalanceTypes_BalanceTypeID",
                table: "HoldBalances",
                column: "BalanceTypeID",
                principalTable: "BalanceTypes",
                principalColumn: "ID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HoldBalances_BalanceTypes_BalanceTypeID",
                table: "HoldBalances");

            migrationBuilder.DropIndex(
                name: "IX_HoldBalances_BalanceTypeID",
                table: "HoldBalances");

            migrationBuilder.DropColumn(
                name: "AvailableBalanceBefore",
                table: "HoldBalances");

            migrationBuilder.DropColumn(
                name: "BalanceTypeID",
                table: "HoldBalances");

            migrationBuilder.DropColumn(
                name: "TotalBalance",
                table: "BalanceHistory");
        }
    }
}
