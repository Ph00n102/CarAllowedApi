using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarAllowedApi.Migrations
{
    /// <inheritdoc />
    public partial class AddClass3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JobRequestCars_JobStatuses_JobStatusId",
                table: "JobRequestCars");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "JobStatuses",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(95)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn)
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "JobStatusId",
                table: "JobRequestCars",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(string),
                oldType: "varchar(95)",
                oldNullable: true)
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddForeignKey(
                name: "FK_JobRequestCars_JobStatuses_JobStatusId",
                table: "JobRequestCars",
                column: "JobStatusId",
                principalTable: "JobStatuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JobRequestCars_JobStatuses_JobStatusId",
                table: "JobRequestCars");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "JobStatuses",
                type: "varchar(95)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<string>(
                name: "JobStatusId",
                table: "JobRequestCars",
                type: "varchar(95)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddForeignKey(
                name: "FK_JobRequestCars_JobStatuses_JobStatusId",
                table: "JobRequestCars",
                column: "JobStatusId",
                principalTable: "JobStatuses",
                principalColumn: "Id");
        }
    }
}
