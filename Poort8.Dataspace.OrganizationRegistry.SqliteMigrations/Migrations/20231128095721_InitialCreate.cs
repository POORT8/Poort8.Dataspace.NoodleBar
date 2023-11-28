using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Poort8.Dataspace.CoreManager.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AuditRecords",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "TEXT", nullable: false),
                    User = table.Column<string>(type: "TEXT", nullable: false),
                    EntityType = table.Column<string>(type: "TEXT", nullable: false),
                    EntityId = table.Column<string>(type: "TEXT", nullable: false),
                    Action = table.Column<string>(type: "TEXT", nullable: false),
                    Entity = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditRecords", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OrOrganization",
                columns: table => new
                {
                    Identifier = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Adherence_Status = table.Column<string>(type: "TEXT", nullable: false),
                    Adherence_ValidFrom = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    Adherence_ValidUntil = table.Column<DateOnly>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrOrganization", x => x.Identifier);
                });

            migrationBuilder.CreateTable(
                name: "OrganizationRole",
                columns: table => new
                {
                    RoleId = table.Column<string>(type: "TEXT", nullable: false),
                    Role = table.Column<string>(type: "TEXT", nullable: false),
                    OrganizationIdentifier = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganizationRole", x => x.RoleId);
                    table.ForeignKey(
                        name: "FK_OrganizationRole_OrOrganization_OrganizationIdentifier",
                        column: x => x.OrganizationIdentifier,
                        principalTable: "OrOrganization",
                        principalColumn: "Identifier",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Property",
                columns: table => new
                {
                    PropertyId = table.Column<string>(type: "TEXT", nullable: false),
                    Key = table.Column<string>(type: "TEXT", nullable: false),
                    Value = table.Column<string>(type: "TEXT", nullable: false),
                    IsIdentifier = table.Column<bool>(type: "INTEGER", nullable: false),
                    OrganizationIdentifier = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Property", x => x.PropertyId);
                    table.ForeignKey(
                        name: "FK_Property_OrOrganization_OrganizationIdentifier",
                        column: x => x.OrganizationIdentifier,
                        principalTable: "OrOrganization",
                        principalColumn: "Identifier",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationRole_OrganizationIdentifier",
                table: "OrganizationRole",
                column: "OrganizationIdentifier");

            migrationBuilder.CreateIndex(
                name: "IX_Property_OrganizationIdentifier",
                table: "Property",
                column: "OrganizationIdentifier");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuditRecords");

            migrationBuilder.DropTable(
                name: "OrganizationRole");

            migrationBuilder.DropTable(
                name: "Property");

            migrationBuilder.DropTable(
                name: "OrOrganization");
        }
    }
}
