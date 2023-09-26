using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Poort8.Dataspace.OrganizationRegistry.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Organizations",
                columns: table => new
                {
                    Identifier = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Organizations", x => x.Identifier);
                });

            migrationBuilder.CreateTable(
                name: "OrganizationProperty",
                columns: table => new
                {
                    PropertyId = table.Column<string>(type: "TEXT", nullable: false),
                    Key = table.Column<string>(type: "TEXT", nullable: false),
                    Value = table.Column<string>(type: "TEXT", nullable: false),
                    OrganizationIdentifier = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganizationProperty", x => x.PropertyId);
                    table.ForeignKey(
                        name: "FK_OrganizationProperty_Organizations_OrganizationIdentifier",
                        column: x => x.OrganizationIdentifier,
                        principalTable: "Organizations",
                        principalColumn: "Identifier");
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationProperty_OrganizationIdentifier",
                table: "OrganizationProperty",
                column: "OrganizationIdentifier");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrganizationProperty");

            migrationBuilder.DropTable(
                name: "Organizations");
        }
    }
}
