using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarAllowedApi.Migrations
{
    /// <inheritdoc />
    public partial class AddClass4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AlongWith",
                table: "JobRequestCars",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "For",
                table: "JobRequestCars",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Location",
                table: "JobRequestCars",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<TimeOnly>(
                name: "LocationTime",
                table: "JobRequestCars",
                type: "time(0)",
                nullable: false,
                defaultValue: new TimeOnly(0, 0, 0));

            migrationBuilder.CreateTable(
                name: "ImageFiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    FileName = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FileData = table.Column<byte[]>(type: "longblob", nullable: false),
                    ContentType = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FileSize = table.Column<long>(type: "bigint", nullable: false),
                    JobRequestCarId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImageFiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ImageFiles_JobRequestCars_JobRequestCarId",
                        column: x => x.JobRequestCarId,
                        principalTable: "JobRequestCars",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_ImageFiles_JobRequestCarId",
                table: "ImageFiles",
                column: "JobRequestCarId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ImageFiles");

            migrationBuilder.DropColumn(
                name: "AlongWith",
                table: "JobRequestCars");

            migrationBuilder.DropColumn(
                name: "For",
                table: "JobRequestCars");

            migrationBuilder.DropColumn(
                name: "Location",
                table: "JobRequestCars");

            migrationBuilder.DropColumn(
                name: "LocationTime",
                table: "JobRequestCars");
        }
    }
}
