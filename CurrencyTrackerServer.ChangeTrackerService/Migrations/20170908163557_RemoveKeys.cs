using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace CurrencyTrackerServer.ChangeTrackerService.Migrations
{
    public partial class RemoveKeys : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "History",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ChangeSource = table.Column<int>(nullable: false),
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
