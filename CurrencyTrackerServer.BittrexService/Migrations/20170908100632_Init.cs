using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CurrencyTrackerServer.ChangeTrackerService.Migrations
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "History",
                columns: table => new
                {
                    Currency = table.Column<string>(nullable: false),
                    ChangeSource = table.Column<int>(nullable: false),
                    Id = table.Column<int>(nullable: false),
                    Message = table.Column<string>(nullable: true),
                    Percentage = table.Column<double>(nullable: false),
                    Time = table.Column<DateTime>(nullable: false),
                    Type = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_History", x => new { x.Currency, x.ChangeSource });
                });

            migrationBuilder.CreateTable(
                name: "States",
                columns: table => new
                {
                    Currency = table.Column<string>(nullable: false),
                    ChangeSource = table.Column<int>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    LastChangeTime = table.Column<DateTime>(nullable: false),
                    Threshold = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_States", x => new { x.Currency, x.ChangeSource });
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
