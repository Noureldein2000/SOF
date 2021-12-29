using Microsoft.EntityFrameworkCore.Migrations;

namespace SourceOfFund.Data.Migrations
{
    public partial class AddIndexAndUniqueKeys : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_AccountServiceBalances_AccountID_BalanceTypeID",
                table: "AccountServiceBalances",
                columns: new[] { "AccountID", "BalanceTypeID" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AccountServiceAvailableBalances_AccountID_BalanceTypeID",
                table: "AccountServiceAvailableBalances",
                columns: new[] { "AccountID", "BalanceTypeID" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AccountServiceBalances_AccountID_BalanceTypeID",
                table: "AccountServiceBalances");

            migrationBuilder.DropIndex(
                name: "IX_AccountServiceAvailableBalances_AccountID_BalanceTypeID",
                table: "AccountServiceAvailableBalances");
        }
    }
}
