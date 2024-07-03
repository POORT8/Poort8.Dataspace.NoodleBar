using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Poort8.Dataspace.CoreManager.Migrations
{
    /// <inheritdoc />
    public partial class AuditIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_EntityAuditRecords_Timestamp",
                table: "EntityAuditRecords",
                column: "Timestamp",
                descending: Array.Empty<bool>());

            migrationBuilder.CreateIndex(
                name: "IX_EnforceAuditRecords_Timestamp",
                table: "EnforceAuditRecords",
                column: "Timestamp",
                descending: Array.Empty<bool>());
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_EntityAuditRecords_Timestamp",
                table: "EntityAuditRecords");

            migrationBuilder.DropIndex(
                name: "IX_EnforceAuditRecords_Timestamp",
                table: "EnforceAuditRecords");
        }
    }
}
