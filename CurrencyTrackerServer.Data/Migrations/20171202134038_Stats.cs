using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace CurrencyTrackerServer.Data.Migrations
{
    public partial class Stats : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "StatsStates",
                columns: table => new
                {
                    Currency = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UpdateSource = table.Column<int>(type: "int", nullable: false),
                    Created = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastChangeTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    Percentage = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StatsStates", x => new { x.Currency, x.UpdateSource });
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StatsStates");
        }
    }
}
