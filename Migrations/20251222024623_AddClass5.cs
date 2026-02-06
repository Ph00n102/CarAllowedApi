using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarAllowedApi.Migrations
{
    /// <inheritdoc />
    public partial class AddClass5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateOnly>(
                name: "JDDate",
                table: "JobRequestCars",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.AddColumn<TimeOnly>(
                name: "JDTime",
                table: "JobRequestCars",
                type: "time(0)",
                nullable: false,
                defaultValue: new TimeOnly(0, 0, 0));

            migrationBuilder.AddColumn<DateOnly>(
                name: "StatusDate",
                table: "JobRequestCars",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.AddColumn<TimeOnly>(
                name: "StatusTime",
                table: "JobRequestCars",
                type: "time(0)",
                nullable: false,
                defaultValue: new TimeOnly(0, 0, 0));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "JDDate",
                table: "JobRequestCars");

            migrationBuilder.DropColumn(
                name: "JDTime",
                table: "JobRequestCars");

            migrationBuilder.DropColumn(
                name: "StatusDate",
                table: "JobRequestCars");

            migrationBuilder.DropColumn(
                name: "StatusTime",
                table: "JobRequestCars");
        }
    }
}
