using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarAllowedApi.Migrations
{
    /// <inheritdoc />
    public partial class AddClass1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "GarageId",
                table: "JobRequestCars",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ImageEmpId",
                table: "JobRequestCars",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EmpStatusId",
                table: "ImageEmps",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Garages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    CarRegistration = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Carmodel = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Cartype = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CarStatusId = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FileName = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ImageFile = table.Column<byte[]>(type: "longblob", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Garages", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_JobRequestCars_GarageId",
                table: "JobRequestCars",
                column: "GarageId");

            migrationBuilder.CreateIndex(
                name: "IX_JobRequestCars_ImageEmpId",
                table: "JobRequestCars",
                column: "ImageEmpId");

            migrationBuilder.AddForeignKey(
                name: "FK_JobRequestCars_Garages_GarageId",
                table: "JobRequestCars",
                column: "GarageId",
                principalTable: "Garages",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_JobRequestCars_ImageEmps_ImageEmpId",
                table: "JobRequestCars",
                column: "ImageEmpId",
                principalTable: "ImageEmps",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JobRequestCars_Garages_GarageId",
                table: "JobRequestCars");

            migrationBuilder.DropForeignKey(
                name: "FK_JobRequestCars_ImageEmps_ImageEmpId",
                table: "JobRequestCars");

            migrationBuilder.DropTable(
                name: "Garages");

            migrationBuilder.DropIndex(
                name: "IX_JobRequestCars_GarageId",
                table: "JobRequestCars");

            migrationBuilder.DropIndex(
                name: "IX_JobRequestCars_ImageEmpId",
                table: "JobRequestCars");

            migrationBuilder.DropColumn(
                name: "GarageId",
                table: "JobRequestCars");

            migrationBuilder.DropColumn(
                name: "ImageEmpId",
                table: "JobRequestCars");

            migrationBuilder.DropColumn(
                name: "EmpStatusId",
                table: "ImageEmps");
        }
    }
}
