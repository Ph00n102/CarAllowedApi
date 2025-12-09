using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarAllowedApi.Migrations
{
    /// <inheritdoc />
    public partial class AddClass2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CarProvince",
                table: "Garages",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CarProvince",
                table: "Garages");
        }
    }
}
