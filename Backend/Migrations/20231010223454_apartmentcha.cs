using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend.Migrations
{
    /// <inheritdoc />
    public partial class apartmentcha : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // migrationBuilder.DropIndex(
            //     name: "IX_Apartments_FloorID",
            //     table: "Apartments");

            migrationBuilder.CreateIndex(
                name: "IX_Apartments_FloorID_Room",
                table: "Apartments",
                columns: new[] { "FloorID", "Room" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Apartments_FloorID_Room",
                table: "Apartments");

            migrationBuilder.CreateIndex(
                name: "IX_Apartments_FloorID",
                table: "Apartments",
                column: "FloorID");
        }
    }
}
