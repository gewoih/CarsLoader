using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarsLoader.Migrations
{
    /// <inheritdoc />
    public partial class Car_Indexes_Added : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Cars_EncarId",
                table: "Cars",
                column: "EncarId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Cars_Manufacturer_Model_Series_ProductionDate_EngineCapacit~",
                table: "Cars",
                columns: new[] { "Manufacturer", "Model", "Series", "ProductionDate", "EngineCapacity", "Color", "Mileage" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Cars_EncarId",
                table: "Cars");

            migrationBuilder.DropIndex(
                name: "IX_Cars_Manufacturer_Model_Series_ProductionDate_EngineCapacit~",
                table: "Cars");
        }
    }
}
