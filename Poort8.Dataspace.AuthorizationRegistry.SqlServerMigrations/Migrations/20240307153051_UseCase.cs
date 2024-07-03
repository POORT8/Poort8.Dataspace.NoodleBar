using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Poort8.Dataspace.CoreManager.Migrations
{
    /// <inheritdoc />
    public partial class UseCase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UseCase",
                table: "ArProduct",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "default");

            migrationBuilder.AddColumn<string>(
                name: "UseCase",
                table: "ArOrganization",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "default");

            migrationBuilder.AddColumn<string>(
                name: "UseCase",
                table: "ArFeature",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "default");

            migrationBuilder.AddColumn<string>(
                name: "UseCase",
                table: "ArEmployee",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "default");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UseCase",
                table: "ArProduct");

            migrationBuilder.DropColumn(
                name: "UseCase",
                table: "ArOrganization");

            migrationBuilder.DropColumn(
                name: "UseCase",
                table: "ArFeature");

            migrationBuilder.DropColumn(
                name: "UseCase",
                table: "ArEmployee");
        }
    }
}
