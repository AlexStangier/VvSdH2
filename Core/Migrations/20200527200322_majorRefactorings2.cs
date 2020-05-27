using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Core.Migrations
{
    public partial class majorRefactorings2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Holydays",
                table: "Holydays");

            migrationBuilder.DropColumn(
                name: "Day",
                table: "Holydays");

            migrationBuilder.AddColumn<DateTime>(
                name: "Date",
                table: "Holydays",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddPrimaryKey(
                name: "PK_Holydays",
                table: "Holydays",
                column: "Date");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Holydays",
                table: "Holydays");

            migrationBuilder.DropColumn(
                name: "Date",
                table: "Holydays");

            migrationBuilder.AddColumn<DateTime>(
                name: "Day",
                table: "Holydays",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddPrimaryKey(
                name: "PK_Holydays",
                table: "Holydays",
                column: "Day");
        }
    }
}
