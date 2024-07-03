using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Poort8.Dataspace.CoreManager.Migrations
{
    /// <inheritdoc />
    public partial class iShareOrganisationModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AdditionalDetails_CapabilitiesUrl",
                table: "OrOrganization",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AdditionalDetails_CompanyEmail",
                table: "OrOrganization",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AdditionalDetails_CompanyPhone",
                table: "OrOrganization",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AdditionalDetails_CountriesOfOperation",
                table: "OrOrganization",
                type: "TEXT",
                nullable: false,
                defaultValue: "[]");

            migrationBuilder.AddColumn<string>(
                name: "AdditionalDetails_Description",
                table: "OrOrganization",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AdditionalDetails_LogoUrl",
                table: "OrOrganization",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "AdditionalDetails_PubliclyPublishable",
                table: "OrOrganization",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AdditionalDetails_Sectors",
                table: "OrOrganization",
                type: "TEXT",
                nullable: false,
                defaultValue: "[]");

            migrationBuilder.AddColumn<string>(
                name: "AdditionalDetails_Tags",
                table: "OrOrganization",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AdditionalDetails_WebsiteUrl",
                table: "OrOrganization",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "CompliancyVerified",
                table: "OrganizationRole",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateOnly>(
                name: "EndDate",
                table: "OrganizationRole",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.AddColumn<bool>(
                name: "LegalAdherence",
                table: "OrganizationRole",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "LoA",
                table: "OrganizationRole",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateOnly>(
                name: "StartDate",
                table: "OrganizationRole",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.CreateTable(
                name: "Agreement",
                columns: table => new
                {
                    AgreementId = table.Column<string>(type: "TEXT", nullable: false),
                    Type = table.Column<string>(type: "TEXT", nullable: false),
                    Title = table.Column<string>(type: "TEXT", nullable: false),
                    Status = table.Column<string>(type: "TEXT", nullable: false),
                    DateOfSigning = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    DateOfExpiry = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    Framework = table.Column<string>(type: "TEXT", nullable: false),
                    ContractFile = table.Column<byte[]>(type: "BLOB", nullable: false),
                    HashOfSignedContract = table.Column<string>(type: "TEXT", nullable: false),
                    CompliancyVerified = table.Column<bool>(type: "INTEGER", nullable: true),
                    DataspaceId = table.Column<string>(type: "TEXT", nullable: true),
                    OrganizationIdentifier = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Agreement", x => x.AgreementId);
                    table.ForeignKey(
                        name: "FK_Agreement_OrOrganization_OrganizationIdentifier",
                        column: x => x.OrganizationIdentifier,
                        principalTable: "OrOrganization",
                        principalColumn: "Identifier",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AuthorizationRegistry",
                columns: table => new
                {
                    AuthorizationRegistryId = table.Column<string>(type: "TEXT", nullable: false),
                    AuthorizationRegistryOrganizationId = table.Column<string>(type: "TEXT", nullable: false),
                    AuthorizationRegistryUrl = table.Column<string>(type: "TEXT", nullable: false),
                    DataspaceId = table.Column<string>(type: "TEXT", nullable: true),
                    OrganizationIdentifier = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuthorizationRegistry", x => x.AuthorizationRegistryId);
                    table.ForeignKey(
                        name: "FK_AuthorizationRegistry_OrOrganization_OrganizationIdentifier",
                        column: x => x.OrganizationIdentifier,
                        principalTable: "OrOrganization",
                        principalColumn: "Identifier",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Certificate",
                columns: table => new
                {
                    CertificateId = table.Column<string>(type: "TEXT", nullable: false),
                    CertificateFile = table.Column<byte[]>(type: "BLOB", nullable: false),
                    EnabledFrom = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    OrganizationIdentifier = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Certificate", x => x.CertificateId);
                    table.ForeignKey(
                        name: "FK_Certificate_OrOrganization_OrganizationIdentifier",
                        column: x => x.OrganizationIdentifier,
                        principalTable: "OrOrganization",
                        principalColumn: "Identifier",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Service",
                columns: table => new
                {
                    ServiceId = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    Url = table.Column<string>(type: "TEXT", nullable: true),
                    LogoUrl = table.Column<string>(type: "TEXT", nullable: true),
                    Color = table.Column<string>(type: "TEXT", nullable: true),
                    OrganizationIdentifier = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Service", x => x.ServiceId);
                    table.ForeignKey(
                        name: "FK_Service_OrOrganization_OrganizationIdentifier",
                        column: x => x.OrganizationIdentifier,
                        principalTable: "OrOrganization",
                        principalColumn: "Identifier",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Agreement_OrganizationIdentifier",
                table: "Agreement",
                column: "OrganizationIdentifier");

            migrationBuilder.CreateIndex(
                name: "IX_AuthorizationRegistry_OrganizationIdentifier",
                table: "AuthorizationRegistry",
                column: "OrganizationIdentifier");

            migrationBuilder.CreateIndex(
                name: "IX_Certificate_OrganizationIdentifier",
                table: "Certificate",
                column: "OrganizationIdentifier");

            migrationBuilder.CreateIndex(
                name: "IX_Service_OrganizationIdentifier",
                table: "Service",
                column: "OrganizationIdentifier");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Agreement");

            migrationBuilder.DropTable(
                name: "AuthorizationRegistry");

            migrationBuilder.DropTable(
                name: "Certificate");

            migrationBuilder.DropTable(
                name: "Service");

            migrationBuilder.DropColumn(
                name: "AdditionalDetails_CapabilitiesUrl",
                table: "OrOrganization");

            migrationBuilder.DropColumn(
                name: "AdditionalDetails_CompanyEmail",
                table: "OrOrganization");

            migrationBuilder.DropColumn(
                name: "AdditionalDetails_CompanyPhone",
                table: "OrOrganization");

            migrationBuilder.DropColumn(
                name: "AdditionalDetails_CountriesOfOperation",
                table: "OrOrganization");

            migrationBuilder.DropColumn(
                name: "AdditionalDetails_Description",
                table: "OrOrganization");

            migrationBuilder.DropColumn(
                name: "AdditionalDetails_LogoUrl",
                table: "OrOrganization");

            migrationBuilder.DropColumn(
                name: "AdditionalDetails_PubliclyPublishable",
                table: "OrOrganization");

            migrationBuilder.DropColumn(
                name: "AdditionalDetails_Sectors",
                table: "OrOrganization");

            migrationBuilder.DropColumn(
                name: "AdditionalDetails_Tags",
                table: "OrOrganization");

            migrationBuilder.DropColumn(
                name: "AdditionalDetails_WebsiteUrl",
                table: "OrOrganization");

            migrationBuilder.DropColumn(
                name: "CompliancyVerified",
                table: "OrganizationRole");

            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "OrganizationRole");

            migrationBuilder.DropColumn(
                name: "LegalAdherence",
                table: "OrganizationRole");

            migrationBuilder.DropColumn(
                name: "LoA",
                table: "OrganizationRole");

            migrationBuilder.DropColumn(
                name: "StartDate",
                table: "OrganizationRole");
        }
    }
}
