using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Poort8.Dataspace.OrganizationRegistry.Migrations
{
    /// <inheritdoc />
    public partial class ChangedModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Domain",
                table: "Organizations",
                newName: "AdherenceId");

            migrationBuilder.CreateTable(
                name: "Adherence",
                columns: table => new
                {
                    AdherenceId = table.Column<string>(type: "TEXT", nullable: false),
                    Status = table.Column<string>(type: "TEXT", nullable: false),
                    ValidFrom = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    ValidUntil = table.Column<DateOnly>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Adherence", x => x.AdherenceId);
                });

            migrationBuilder.CreateTable(
                name: "OrganizationRole",
                columns: table => new
                {
                    RoleId = table.Column<string>(type: "TEXT", nullable: false),
                    Role = table.Column<string>(type: "TEXT", nullable: false),
                    OrganizationIdentifier = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganizationRole", x => x.RoleId);
                    table.ForeignKey(
                        name: "FK_OrganizationRole_Organizations_OrganizationIdentifier",
                        column: x => x.OrganizationIdentifier,
                        principalTable: "Organizations",
                        principalColumn: "Identifier");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Organizations_AdherenceId",
                table: "Organizations",
                column: "AdherenceId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationRole_OrganizationIdentifier",
                table: "OrganizationRole",
                column: "OrganizationIdentifier");

            migrationBuilder.AddForeignKey(
                name: "FK_Organizations_Adherence_AdherenceId",
                table: "Organizations",
                column: "AdherenceId",
                principalTable: "Adherence",
                principalColumn: "AdherenceId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Organizations_Adherence_AdherenceId",
                table: "Organizations");

            migrationBuilder.DropTable(
                name: "Adherence");

            migrationBuilder.DropTable(
                name: "OrganizationRole");

            migrationBuilder.DropIndex(
                name: "IX_Organizations_AdherenceId",
                table: "Organizations");

            migrationBuilder.RenameColumn(
                name: "AdherenceId",
                table: "Organizations",
                newName: "Domain");
        }
    }
}
