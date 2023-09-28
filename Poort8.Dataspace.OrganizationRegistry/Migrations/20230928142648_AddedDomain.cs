using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Poort8.Dataspace.OrganizationRegistry.Migrations
{
    /// <inheritdoc />
    public partial class AddedDomain : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Domain",
                table: "Organizations",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Domain",
                table: "Organizations");
        }
    }
}
