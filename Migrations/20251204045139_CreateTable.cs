using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarAllowedApi.Migrations
{
    /// <inheritdoc />
    public partial class CreateTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ImageEmps",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Nickname = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Tel = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FileName = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ImageFile = table.Column<byte[]>(type: "longblob", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImageEmps", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "JobStatuses",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(95)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Status = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobStatuses", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "JobRequestCars",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    DateNow = table.Column<DateOnly>(type: "date", nullable: false),
                    TimeNow = table.Column<TimeOnly>(type: "time(0)", nullable: false),
                    EDateNow = table.Column<DateOnly>(type: "date", nullable: false),
                    ETimeNow = table.Column<TimeOnly>(type: "time(0)", nullable: false),
                    Requester = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DepartmentId = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Origin = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Destination = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    StartDate = table.Column<DateOnly>(type: "date", nullable: false),
                    StartTime = table.Column<TimeOnly>(type: "time(0)", nullable: false),
                    EndDate = table.Column<DateOnly>(type: "date", nullable: false),
                    EndTime = table.Column<TimeOnly>(type: "time(0)", nullable: false),
                    JobStatusId = table.Column<string>(type: "varchar(95)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    NumPer = table.Column<int>(type: "int", nullable: true),
                    Tel = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Note = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobRequestCars", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JobRequestCars_JobStatuses_JobStatusId",
                        column: x => x.JobStatusId,
                        principalTable: "JobStatuses",
                        principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "JobImages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    FileName = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ImageFile = table.Column<byte[]>(type: "longblob", nullable: false),
                    OpNoteDetailId = table.Column<int>(type: "int", nullable: false),
                    JobRequestCarId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobImages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JobImages_JobRequestCars_JobRequestCarId",
                        column: x => x.JobRequestCarId,
                        principalTable: "JobRequestCars",
                        principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_JobImages_JobRequestCarId",
                table: "JobImages",
                column: "JobRequestCarId");

            migrationBuilder.CreateIndex(
                name: "IX_JobRequestCars_JobStatusId",
                table: "JobRequestCars",
                column: "JobStatusId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ImageEmps");

            migrationBuilder.DropTable(
                name: "JobImages");

            migrationBuilder.DropTable(
                name: "JobRequestCars");

            migrationBuilder.DropTable(
                name: "JobStatuses");
        }
    }
}
