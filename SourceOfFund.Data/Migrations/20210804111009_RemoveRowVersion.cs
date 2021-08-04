using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SourceOfFund.Data.Migrations
{
    public partial class RemoveRowVersion : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "AccountServiceBalances");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "AccountServiceAvailableBalances");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "AccountServiceBalances",
                type: "rowversion",
                rowVersion: true,
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "AccountServiceAvailableBalances",
                type: "rowversion",
                rowVersion: true,
                nullable: true);
        }
    }
}
