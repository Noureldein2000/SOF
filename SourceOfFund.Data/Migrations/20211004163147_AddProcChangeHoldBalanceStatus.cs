using Microsoft.EntityFrameworkCore.Migrations;

namespace SourceOfFund.Data.Migrations
{
    public partial class AddProcChangeHoldBalanceStatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"Create proc [dbo].[ChangeHoldBalancStatus] @RequestID int, @AccountID int
                AS
                BEGIN
                    UPDATE HoldBalances SET Status = 4 where RequestID = @RequestID and AccountID = @AccountID
                END");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
