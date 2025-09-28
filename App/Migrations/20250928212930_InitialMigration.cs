using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace App.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Brand",
                columns: table => new
                {
                    idBrand = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    brandName = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Brand", x => x.idBrand);
                });

            migrationBuilder.CreateTable(
                name: "TypeProduct",
                columns: table => new
                {
                    idTypeProduct = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    typeProductName = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TypeProduct", x => x.idTypeProduct);
                });

            migrationBuilder.CreateTable(
                name: "Product",
                columns: table => new
                {
                    idProduct = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    productName = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    description = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    photoName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    photoUri = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    idTypeProduct = table.Column<int>(type: "integer", nullable: true),
                    idBrand = table.Column<int>(type: "integer", nullable: true),
                    actualStock = table.Column<int>(type: "integer", nullable: false),
                    minStock = table.Column<int>(type: "integer", nullable: false),
                    maxStock = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Product", x => x.idProduct);
                    table.ForeignKey(
                        name: "FK_product_brand",
                        column: x => x.idBrand,
                        principalTable: "Brand",
                        principalColumn: "idBrand");
                    table.ForeignKey(
                        name: "FK_product_type",
                        column: x => x.idTypeProduct,
                        principalTable: "TypeProduct",
                        principalColumn: "idTypeProduct");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Brand_brandName",
                table: "Brand",
                column: "brandName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Brand_idBrand",
                table: "Brand",
                column: "idBrand",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Product_idBrand",
                table: "Product",
                column: "idBrand");

            migrationBuilder.CreateIndex(
                name: "IX_Product_idProduct",
                table: "Product",
                column: "idProduct",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Product_idTypeProduct",
                table: "Product",
                column: "idTypeProduct");

            migrationBuilder.CreateIndex(
                name: "IX_Product_productName",
                table: "Product",
                column: "productName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TypeProduct_idTypeProduct",
                table: "TypeProduct",
                column: "idTypeProduct",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TypeProduct_typeProductName",
                table: "TypeProduct",
                column: "typeProductName",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Product");

            migrationBuilder.DropTable(
                name: "Brand");

            migrationBuilder.DropTable(
                name: "TypeProduct");
        }
    }
}
