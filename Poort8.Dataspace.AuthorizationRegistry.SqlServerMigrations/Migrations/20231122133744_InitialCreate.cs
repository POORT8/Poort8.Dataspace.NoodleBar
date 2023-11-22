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
                name: "ArFeature",
                columns: table => new
                {
                    FeatureId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArFeature", x => x.FeatureId);
                });

            migrationBuilder.CreateTable(
                name: "ArOrganization",
                columns: table => new
                {
                    Identifier = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Url = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Representative = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    InvoicingContact = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArOrganization", x => x.Identifier);
                });

            migrationBuilder.CreateTable(
                name: "ArPolicy",
                columns: table => new
                {
                    PolicyId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UseCase = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IssuedAt = table.Column<long>(type: "bigint", nullable: false),
                    NotBefore = table.Column<long>(type: "bigint", nullable: false),
                    Expiration = table.Column<long>(type: "bigint", nullable: false),
                    IssuerId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SubjectId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ResourceId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Action = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArPolicy", x => x.PolicyId);
                });

            migrationBuilder.CreateTable(
                name: "ArProduct",
                columns: table => new
                {
                    ProductId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Provider = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Url = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArProduct", x => x.ProductId);
                });

            migrationBuilder.CreateTable(
                name: "EnforceAuditRecords",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    User = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UseCase = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SubjectId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ResourceId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Action = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Allow = table.Column<bool>(type: "bit", nullable: false),
                    Explain = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EnforceAuditRecords", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EntityAuditRecords",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    User = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EntityType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EntityId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Action = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Entity = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EntityAuditRecords", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FeatureProperty",
                columns: table => new
                {
                    Key = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsIdentifier = table.Column<bool>(type: "bit", nullable: false),
                    FeatureId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FeatureProperty", x => x.Key);
                    table.ForeignKey(
                        name: "FK_FeatureProperty_ArFeature_FeatureId",
                        column: x => x.FeatureId,
                        principalTable: "ArFeature",
                        principalColumn: "FeatureId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ArEmployee",
                columns: table => new
                {
                    EmployeeId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    GivenName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FamilyName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Telephone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OrganizationIdentifier = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArEmployee", x => x.EmployeeId);
                    table.ForeignKey(
                        name: "FK_ArEmployee_ArOrganization_OrganizationIdentifier",
                        column: x => x.OrganizationIdentifier,
                        principalTable: "ArOrganization",
                        principalColumn: "Identifier",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrganizationProperty",
                columns: table => new
                {
                    Key = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsIdentifier = table.Column<bool>(type: "bit", nullable: false),
                    OrganizationIdentifier = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganizationProperty", x => x.Key);
                    table.ForeignKey(
                        name: "FK_OrganizationProperty_ArOrganization_OrganizationIdentifier",
                        column: x => x.OrganizationIdentifier,
                        principalTable: "ArOrganization",
                        principalColumn: "Identifier",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PolicyProperty",
                columns: table => new
                {
                    Key = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsIdentifier = table.Column<bool>(type: "bit", nullable: false),
                    PolicyId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PolicyProperty", x => x.Key);
                    table.ForeignKey(
                        name: "FK_PolicyProperty_ArPolicy_PolicyId",
                        column: x => x.PolicyId,
                        principalTable: "ArPolicy",
                        principalColumn: "PolicyId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FeatureProduct",
                columns: table => new
                {
                    FeaturesFeatureId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProductId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FeatureProduct", x => new { x.FeaturesFeatureId, x.ProductId });
                    table.ForeignKey(
                        name: "FK_FeatureProduct_ArFeature_FeaturesFeatureId",
                        column: x => x.FeaturesFeatureId,
                        principalTable: "ArFeature",
                        principalColumn: "FeatureId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FeatureProduct_ArProduct_ProductId",
                        column: x => x.ProductId,
                        principalTable: "ArProduct",
                        principalColumn: "ProductId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductProperty",
                columns: table => new
                {
                    Key = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsIdentifier = table.Column<bool>(type: "bit", nullable: false),
                    ProductId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductProperty", x => x.Key);
                    table.ForeignKey(
                        name: "FK_ProductProperty_ArProduct_ProductId",
                        column: x => x.ProductId,
                        principalTable: "ArProduct",
                        principalColumn: "ProductId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeProperty",
                columns: table => new
                {
                    Key = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsIdentifier = table.Column<bool>(type: "bit", nullable: false),
                    EmployeeId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeProperty", x => x.Key);
                    table.ForeignKey(
                        name: "FK_EmployeeProperty_ArEmployee_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "ArEmployee",
                        principalColumn: "EmployeeId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ArEmployee_OrganizationIdentifier",
                table: "ArEmployee",
                column: "OrganizationIdentifier");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeProperty_EmployeeId",
                table: "EmployeeProperty",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_FeatureProduct_ProductId",
                table: "FeatureProduct",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_FeatureProperty_FeatureId",
                table: "FeatureProperty",
                column: "FeatureId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationProperty_OrganizationIdentifier",
                table: "OrganizationProperty",
                column: "OrganizationIdentifier");

            migrationBuilder.CreateIndex(
                name: "IX_PolicyProperty_PolicyId",
                table: "PolicyProperty",
                column: "PolicyId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductProperty_ProductId",
                table: "ProductProperty",
                column: "ProductId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmployeeProperty");

            migrationBuilder.DropTable(
                name: "EnforceAuditRecords");

            migrationBuilder.DropTable(
                name: "EntityAuditRecords");

            migrationBuilder.DropTable(
                name: "FeatureProduct");

            migrationBuilder.DropTable(
                name: "FeatureProperty");

            migrationBuilder.DropTable(
                name: "OrganizationProperty");

            migrationBuilder.DropTable(
                name: "PolicyProperty");

            migrationBuilder.DropTable(
                name: "ProductProperty");

            migrationBuilder.DropTable(
                name: "ArEmployee");

            migrationBuilder.DropTable(
                name: "ArFeature");

            migrationBuilder.DropTable(
                name: "ArPolicy");

            migrationBuilder.DropTable(
                name: "ArProduct");

            migrationBuilder.DropTable(
                name: "ArOrganization");
        }
    }
}
