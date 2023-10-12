using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend.Migrations
{
    /// <inheritdoc />
    public partial class aparmentthings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Apartments_Floors_FloorID",
                table: "Apartments");

            migrationBuilder.AlterColumn<int>(
                name: "FloorID",
                table: "Apartments",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Apartments_Floors_FloorID",
                table: "Apartments",
                column: "FloorID",
                principalTable: "Floors",
                principalColumn: "FloorID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Apartments_Floors_FloorID",
                table: "Apartments");

            migrationBuilder.AlterColumn<int>(
                name: "FloorID",
                table: "Apartments",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Apartments_Floors_FloorID",
                table: "Apartments",
                column: "FloorID",
                principalTable: "Floors",
                principalColumn: "FloorID");
        }
    }
}
