using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarsLoader.Migrations
{
    /// <inheritdoc />
    public partial class CarImage_IsDownloaded_Added : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDownloaded",
                table: "Images",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDownloaded",
                table: "Images");
        }
    }
}
