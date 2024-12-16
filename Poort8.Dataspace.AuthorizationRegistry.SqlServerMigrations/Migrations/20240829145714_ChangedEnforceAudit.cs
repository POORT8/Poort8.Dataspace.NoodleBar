using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Poort8.Dataspace.CoreManager.Migrations
{
    /// <inheritdoc />
    public partial class ChangedEnforceAudit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Attribute",
                table: "EnforceAuditRecords",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IssuerId",
                table: "EnforceAuditRecords",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RequestContext",
                table: "EnforceAuditRecords",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ServiceProvider",
                table: "EnforceAuditRecords",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "EnforceAuditRecords",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Attribute",
                table: "EnforceAuditRecords");

            migrationBuilder.DropColumn(
                name: "IssuerId",
                table: "EnforceAuditRecords");

            migrationBuilder.DropColumn(
                name: "RequestContext",
                table: "EnforceAuditRecords");

            migrationBuilder.DropColumn(
                name: "ServiceProvider",
                table: "EnforceAuditRecords");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "EnforceAuditRecords");
        }
    }
}
