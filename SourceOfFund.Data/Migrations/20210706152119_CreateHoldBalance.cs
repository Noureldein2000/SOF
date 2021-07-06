using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SourceOfFund.Data.Migrations
{
    public partial class CreateHoldBalance : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HoldBalances",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationDate = table.Column<DateTime>(nullable: false),
                    UpdateDate = table.Column<DateTime>(nullable: true),
                    AccountID = table.Column<int>(nullable: false),
                    RequestID = table.Column<int>(nullable: false),
                    Balance = table.Column<decimal>(type: "decimal(18, 3)", nullable: false),
                    SourceID = table.Column<int>(nullable: false),
                    Status = table.Column<short>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HoldBalances", x => x.ID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HoldBalances");
        }
    }
}
