using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Poort8.Dataspace.CoreManager.Migrations
{
    /// <inheritdoc />
    public partial class ProductFeatureRename : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ArResource",
                columns: table => new
                {
                    ResourceId = table.Column<string>(type: "TEXT", nullable: false),
                    UseCase = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArResource", x => x.ResourceId);
                });

            migrationBuilder.CreateTable(
                name: "ArResourceGroup",
                columns: table => new
                {
                    ResourceGroupId = table.Column<string>(type: "TEXT", nullable: false),
                    UseCase = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    Provider = table.Column<string>(type: "TEXT", nullable: true),
                    Url = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArResourceGroup", x => x.ResourceGroupId);
                });

            migrationBuilder.CreateTable(
                name: "ResourceProperty",
                columns: table => new
                {
                    PropertyId = table.Column<string>(type: "TEXT", nullable: false),
                    Key = table.Column<string>(type: "TEXT", nullable: false),
                    Value = table.Column<string>(type: "TEXT", nullable: false),
                    IsIdentifier = table.Column<bool>(type: "INTEGER", nullable: false),
                    ResourceId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResourceProperty", x => x.PropertyId);
                    table.ForeignKey(
                        name: "FK_ResourceProperty_ArResource_ResourceId",
                        column: x => x.ResourceId,
                        principalTable: "ArResource",
                        principalColumn: "ResourceId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ResourceGroupProperty",
                columns: table => new
                {
                    PropertyId = table.Column<string>(type: "TEXT", nullable: false),
                    Key = table.Column<string>(type: "TEXT", nullable: false),
                    Value = table.Column<string>(type: "TEXT", nullable: false),
                    IsIdentifier = table.Column<bool>(type: "INTEGER", nullable: false),
                    ResourceGroupId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResourceGroupProperty", x => x.PropertyId);
                    table.ForeignKey(
                        name: "FK_ResourceGroupProperty_ArResourceGroup_ResourceGroupId",
                        column: x => x.ResourceGroupId,
                        principalTable: "ArResourceGroup",
                        principalColumn: "ResourceGroupId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ResourceResourceGroup",
                columns: table => new
                {
                    ResourceGroupId = table.Column<string>(type: "TEXT", nullable: false),
                    ResourcesResourceId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResourceResourceGroup", x => new { x.ResourceGroupId, x.ResourcesResourceId });
                    table.ForeignKey(
                        name: "FK_ResourceResourceGroup_ArResourceGroup_ResourceGroupId",
                        column: x => x.ResourceGroupId,
                        principalTable: "ArResourceGroup",
                        principalColumn: "ResourceGroupId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ResourceResourceGroup_ArResource_ResourcesResourceId",
                        column: x => x.ResourcesResourceId,
                        principalTable: "ArResource",
                        principalColumn: "ResourceId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ResourceGroupProperty_ResourceGroupId",
                table: "ResourceGroupProperty",
                column: "ResourceGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_ResourceProperty_ResourceId",
                table: "ResourceProperty",
                column: "ResourceId");

            migrationBuilder.CreateIndex(
                name: "IX_ResourceResourceGroup_ResourcesResourceId",
                table: "ResourceResourceGroup",
                column: "ResourcesResourceId");

            migrationBuilder.Sql("INSERT INTO ArResource (ResourceId, UseCase, Name, Description) SELECT FeatureId, UseCase, Name, Description FROM ArFeature");
            migrationBuilder.Sql("INSERT INTO ArResourceGroup (ResourceGroupId, UseCase, Name, Description, Provider, Url) SELECT ProductId, UseCase, Name, Description, Provider, Url FROM ArProduct");
            migrationBuilder.Sql("INSERT INTO ResourceProperty (PropertyId, Key, Value, IsIdentifier, ResourceId) SELECT PropertyId, Key, Value, IsIdentifier, FeatureId FROM FeatureProperty");
            migrationBuilder.Sql("INSERT INTO ResourceGroupProperty (PropertyId, Key, Value, IsIdentifier, ResourceGroupId) SELECT PropertyId, Key, Value, IsIdentifier, ProductId FROM ProductProperty");
            migrationBuilder.Sql("INSERT INTO ResourceResourceGroup (ResourceGroupId, ResourcesResourceId) SELECT ProductId, FeaturesFeatureId FROM FeatureProduct");

            migrationBuilder.DropTable(
                name: "FeatureProduct");

            migrationBuilder.DropTable(
                name: "FeatureProperty");

            migrationBuilder.DropTable(
                name: "ProductProperty");

            migrationBuilder.DropTable(
                name: "ArFeature");

            migrationBuilder.DropTable(
                name: "ArProduct");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ArFeature",
                columns: table => new
                {
                    FeatureId = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    UseCase = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArFeature", x => x.FeatureId);
                });

            migrationBuilder.CreateTable(
                name: "ArProduct",
                columns: table => new
                {
                    ProductId = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Provider = table.Column<string>(type: "TEXT", nullable: true),
                    Url = table.Column<string>(type: "TEXT", nullable: true),
                    UseCase = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArProduct", x => x.ProductId);
                });

            migrationBuilder.CreateTable(
                name: "FeatureProperty",
                columns: table => new
                {
                    PropertyId = table.Column<string>(type: "TEXT", nullable: false),
                    FeatureId = table.Column<string>(type: "TEXT", nullable: false),
                    IsIdentifier = table.Column<bool>(type: "INTEGER", nullable: false),
                    Key = table.Column<string>(type: "TEXT", nullable: false),
                    Value = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FeatureProperty", x => x.PropertyId);
                    table.ForeignKey(
                        name: "FK_FeatureProperty_ArFeature_FeatureId",
                        column: x => x.FeatureId,
                        principalTable: "ArFeature",
                        principalColumn: "FeatureId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FeatureProduct",
                columns: table => new
                {
                    FeaturesFeatureId = table.Column<string>(type: "TEXT", nullable: false),
                    ProductId = table.Column<string>(type: "TEXT", nullable: false)
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
                    PropertyId = table.Column<string>(type: "TEXT", nullable: false),
                    IsIdentifier = table.Column<bool>(type: "INTEGER", nullable: false),
                    Key = table.Column<string>(type: "TEXT", nullable: false),
                    ProductId = table.Column<string>(type: "TEXT", nullable: false),
                    Value = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductProperty", x => x.PropertyId);
                    table.ForeignKey(
                        name: "FK_ProductProperty_ArProduct_ProductId",
                        column: x => x.ProductId,
                        principalTable: "ArProduct",
                        principalColumn: "ProductId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FeatureProduct_ProductId",
                table: "FeatureProduct",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_FeatureProperty_FeatureId",
                table: "FeatureProperty",
                column: "FeatureId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductProperty_ProductId",
                table: "ProductProperty",
                column: "ProductId");

            migrationBuilder.Sql("INSERT INTO ArFeature (FeatureId, UseCase, Name, Description) SELECT ResourceId, UseCase, Name, Description FROM ArResource");
            migrationBuilder.Sql("INSERT INTO ArProduct (ProductId, UseCase, Name, Description, Provider, Url) SELECT ResourceGroupId, UseCase, Name, Description, Provider, Url FROM ArResourceGroup");
            migrationBuilder.Sql("INSERT INTO FeatureProperty (PropertyId, Key, Value, IsIdentifier, FeatureId) SELECT PropertyId, Key, Value, IsIdentifier, ResourceId FROM ResourceProperty");
            migrationBuilder.Sql("INSERT INTO ProductProperty (PropertyId, Key, Value, IsIdentifier, ProductId) SELECT PropertyId, Key, Value, IsIdentifier, ResourceGroupId FROM ResourceGroupProperty");
            migrationBuilder.Sql("INSERT INTO FeatureProduct (ProductId, FeaturesFeatureId) SELECT ResourceGroupId, ResourcesResourceId FROM ResourceResourceGroup");

            migrationBuilder.DropTable(
                name: "ResourceGroupProperty");

            migrationBuilder.DropTable(
                name: "ResourceProperty");

            migrationBuilder.DropTable(
                name: "ResourceResourceGroup");

            migrationBuilder.DropTable(
                name: "ArResourceGroup");

            migrationBuilder.DropTable(
                name: "ArResource");
        }
    }
}
