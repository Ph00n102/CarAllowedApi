using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarAllowedApi.Migrations
{
    /// <inheritdoc />
    public partial class AddClass6 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateOnly>(
                name: "IssueDate",
                table: "JobRequestCars",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.AddColumn<TimeOnly>(
                name: "IssueTime",
                table: "JobRequestCars",
                type: "time(0)",
                nullable: false,
                defaultValue: new TimeOnly(0, 0, 0));

            migrationBuilder.AddColumn<string>(
                name: "MileageBack",
                table: "JobRequestCars",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "MileageOut",
                table: "JobRequestCars",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "NumOil",
                table: "JobRequestCars",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Ot",
                table: "JobRequestCars",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Price",
                table: "JobRequestCars",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<DateOnly>(
                name: "ReturnDate",
                table: "JobRequestCars",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.AddColumn<TimeOnly>(
                name: "ReturnTime",
                table: "JobRequestCars",
                type: "time(0)",
                nullable: false,
                defaultValue: new TimeOnly(0, 0, 0));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IssueDate",
                table: "JobRequestCars");

            migrationBuilder.DropColumn(
                name: "IssueTime",
                table: "JobRequestCars");

            migrationBuilder.DropColumn(
                name: "MileageBack",
                table: "JobRequestCars");

            migrationBuilder.DropColumn(
                name: "MileageOut",
                table: "JobRequestCars");

            migrationBuilder.DropColumn(
                name: "NumOil",
                table: "JobRequestCars");

            migrationBuilder.DropColumn(
                name: "Ot",
                table: "JobRequestCars");

            migrationBuilder.DropColumn(
                name: "Price",
                table: "JobRequestCars");

            migrationBuilder.DropColumn(
                name: "ReturnDate",
                table: "JobRequestCars");

            migrationBuilder.DropColumn(
                name: "ReturnTime",
                table: "JobRequestCars");
        }
    }
}
