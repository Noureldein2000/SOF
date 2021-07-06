using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SourceOfFund.Data.Migrations
{
    public partial class SeedBalanceType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "BalanceTypes",
                columns: new[] { "ID", "ArName", "CreationDate", "Name", "UpdateDate" },
                values: new object[] { 1, "رصيد ممكن", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Momkm Balance", null });

            migrationBuilder.InsertData(
                table: "BalanceTypes",
                columns: new[] { "ID", "ArName", "CreationDate", "Name", "UpdateDate" },
                values: new object[] { 2, "رصيد كاش", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Cash Balance", null });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "BalanceTypes",
                keyColumn: "ID",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "BalanceTypes",
                keyColumn: "ID",
                keyValue: 2);
        }
    }
}
