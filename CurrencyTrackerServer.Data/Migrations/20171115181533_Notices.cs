using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace CurrencyTrackerServer.Data.Migrations
{
  public partial class Notices : Migration
  {
    protected override void Up(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.AlterColumn<DateTimeOffset>(
          name: "LastChangeTime",
          table: "States",
          type: "timestamptz",
          nullable: false,
          oldClrType: typeof(DateTime));

      migrationBuilder.AlterColumn<DateTimeOffset>(
          name: "Created",
          table: "States",
          type: "timestamptz",
          nullable: false,
          oldClrType: typeof(DateTime));

      migrationBuilder.AlterColumn<DateTimeOffset>(
          name: "Time",
          table: "History",
          type: "timestamptz",
          nullable: false,
          oldClrType: typeof(DateTime));

      migrationBuilder.CreateTable(
          name: "Notices",
          columns: table => new
          {
            Id = table.Column<int>(type: "int", nullable: false)
              .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
            Source = table.Column<int>(type: "int", nullable: false),
            Message = table.Column<string>(type: "text", nullable: true),
            Time = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_Notices", x => new { x.Id, x.Source });
          });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.DropTable(
          name: "Notices");

      migrationBuilder.AlterColumn<DateTime>(
          name: "LastChangeTime",
          table: "States",
          nullable: false,
          oldClrType: typeof(DateTimeOffset),
          oldType: "datetimeoffset");

      migrationBuilder.AlterColumn<DateTime>(
          name: "Created",
          table: "States",
          nullable: false,
          oldClrType: typeof(DateTimeOffset),
          oldType: "datetimeoffset");

      migrationBuilder.AlterColumn<DateTime>(
          name: "Time",
          table: "History",
          nullable: false,
          oldClrType: typeof(DateTimeOffset),
          oldType: "datetimeoffset");
    }
  }
}
