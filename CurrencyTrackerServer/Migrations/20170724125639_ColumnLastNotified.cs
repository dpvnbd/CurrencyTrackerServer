using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CurrencyTrackerServer.Migrations
{
    public partial class ColumnLastNotified : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Changes",
                columns: table => new
                {
                    Currency = table.Column<string>(nullable: false),
                    ChangeTime = table.Column<DateTime>(nullable: false),
                    CreatedTime = table.Column<DateTime>(nullable: false),
                    CurrentBid = table.Column<double>(nullable: false),
                    LastNotifiedChange = table.Column<DateTime>(nullable: true),
                    PreviousDayBid = table.Column<double>(nullable: false),
                    ReferenceCurrency = table.Column<string>(nullable: true),
                    Threshsold = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Changes", x => x.Currency);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Changes");
        }
    }
}
