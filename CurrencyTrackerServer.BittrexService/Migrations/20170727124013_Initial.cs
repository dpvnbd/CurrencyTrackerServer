using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CurrencyTrackerServer.ChangeTrackerService.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "History",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Currency = table.Column<string>(nullable: true),
                    Message = table.Column<string>(nullable: true),
                    Percentage = table.Column<double>(nullable: false),
                    Time = table.Column<DateTime>(nullable: false),
                    Type = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_History", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "States",
                columns: table => new
                {
                    Currency = table.Column<string>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    LastChangeTime = table.Column<DateTime>(nullable: false),
                    Threshold = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_States", x => x.Currency);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "History");

            migrationBuilder.DropTable(
                name: "States");
        }
    }
}
