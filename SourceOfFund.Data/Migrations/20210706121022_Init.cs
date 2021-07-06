using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SourceOfFund.Data.Migrations
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BalanceTypes",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationDate = table.Column<DateTime>(nullable: false),
                    UpdateDate = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: false),
                    ArName = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BalanceTypes", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "AccountServiceAvailableBalances",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationDate = table.Column<DateTime>(nullable: false),
                    UpdateDate = table.Column<DateTime>(nullable: true),
                    AccountID = table.Column<int>(nullable: false),
                    Balance = table.Column<decimal>(nullable: false),
                    BalanceTypeID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountServiceAvailableBalances", x => x.ID);
                    table.ForeignKey(
                        name: "FK_AccountServiceAvailableBalances_BalanceTypes_BalanceTypeID",
                        column: x => x.BalanceTypeID,
                        principalTable: "BalanceTypes",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "AccountServiceBalances",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationDate = table.Column<DateTime>(nullable: false),
                    UpdateDate = table.Column<DateTime>(nullable: true),
                    AccountID = table.Column<int>(nullable: false),
                    Balance = table.Column<decimal>(nullable: false),
                    BalanceTypeID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountServiceBalances", x => x.ID);
                    table.ForeignKey(
                        name: "FK_AccountServiceBalances_BalanceTypes_BalanceTypeID",
                        column: x => x.BalanceTypeID,
                        principalTable: "BalanceTypes",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "BalanceHistory",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationDate = table.Column<DateTime>(nullable: false),
                    UpdateDate = table.Column<DateTime>(nullable: true),
                    TransactionID = table.Column<int>(nullable: false),
                    BalanceBefore = table.Column<decimal>(nullable: false),
                    AccountID = table.Column<int>(nullable: false),
                    BalanceTypeID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BalanceHistory", x => x.ID);
                    table.ForeignKey(
                        name: "FK_BalanceHistory_BalanceTypes_BalanceTypeID",
                        column: x => x.BalanceTypeID,
                        principalTable: "BalanceTypes",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccountServiceAvailableBalances_BalanceTypeID",
                table: "AccountServiceAvailableBalances",
                column: "BalanceTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_AccountServiceBalances_BalanceTypeID",
                table: "AccountServiceBalances",
                column: "BalanceTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_BalanceHistory_BalanceTypeID",
                table: "BalanceHistory",
                column: "BalanceTypeID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccountServiceAvailableBalances");

            migrationBuilder.DropTable(
                name: "AccountServiceBalances");

            migrationBuilder.DropTable(
                name: "BalanceHistory");

            migrationBuilder.DropTable(
                name: "BalanceTypes");
        }
    }
}
