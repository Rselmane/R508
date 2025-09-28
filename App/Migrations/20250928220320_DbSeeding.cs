using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace App.Migrations
{
    /// <inheritdoc />
    public partial class DbSeeding : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Brand",
                columns: new[] { "idBrand", "brandName" },
                values: new object[,]
                {
                    { 1, "Ikea" },
                    { 2, "Mobalpa" },
                    { 3, "LeRoyMerlin" },
                    { 4, "Lego" },
                    { 5, "Playmobil" },
                    { 6, "Hasbro" },
                    { 7, "Apple" },
                    { 8, "Samsung" },
                    { 9, "Sony" },
                    { 10, "Nike" },
                    { 11, "Adidas" },
                    { 12, "Puma" },
                    { 13, "Paypal" }
                });

            migrationBuilder.InsertData(
                table: "TypeProduct",
                columns: new[] { "idTypeProduct", "typeProductName" },
                values: new object[,]
                {
                    { 1, "Home" },
                    { 2, "Toys" },
                    { 3, "Electronics" },
                    { 4, "Clothes" },
                    { 5, "Other" }
                });

            migrationBuilder.InsertData(
                table: "Product",
                columns: new[] { "idProduct", "actualStock", "description", "idBrand", "idTypeProduct", "maxStock", "minStock", "photoName", "photoUri", "productName" },
                values: new object[,]
                {
                    { 1, 10, "A comfortable corner sofa for your living room.", 1, 1, 20, 2, "corner_sofa.jpg", "/images/corner_sofa.jpg", "Corner Sofa" },
                    { 2, 15, "Coffee table made of solid wood.", 1, 1, 30, 3, "coffee_table.jpg", "/images/coffee_table.jpg", "Coffee Table" },
                    { 3, 5, "Modern kitchen with built-in storage.", 2, 1, 10, 1, "kitchen.jpg", "/images/kitchen.jpg", "Fitted Kitchen" },
                    { 4, 20, "Durable drill with long-lasting battery.", 3, 1, 50, 5, "drill.jpg", "/images/drill.jpg", "Cordless Drill" },
                    { 5, 25, "Lego Star Wars collector's set.", 4, 2, 40, 5, "lego_starwars.jpg", "/images/lego_starwars.jpg", "Lego Star Wars" },
                    { 6, 30, "Lego City set for kids.", 4, 2, 60, 6, "lego_city.jpg", "/images/lego_city.jpg", "Lego City" },
                    { 7, 12, "Medieval Playmobil castle with figures.", 5, 2, 25, 2, "playmobil_castle.jpg", "/images/playmobil_castle.jpg", "Playmobil Castle" },
                    { 8, 40, "Classic Monopoly board game.", 6, 2, 80, 8, "monopoly.jpg", "/images/monopoly.jpg", "Monopoly" },
                    { 9, 50, "Latest iPhone model with OLED display.", 7, 3, 100, 10, "iphone15.jpg", "/images/iphone15.jpg", "iPhone 15" },
                    { 10, 20, "High performance laptop.", 7, 3, 30, 5, "macbook.jpg", "/images/macbook.jpg", "MacBook Pro" },
                    { 11, 45, "Samsung high-end smartphone.", 8, 3, 90, 8, "galaxy_s24.jpg", "/images/galaxy_s24.jpg", "Galaxy S24" },
                    { 12, 18, "High-definition QLED television.", 8, 3, 25, 3, "qled_tv.jpg", "/images/qled_tv.jpg", "QLED TV 55\"" },
                    { 13, 35, "Next-generation gaming console.", 9, 3, 50, 7, "ps5.jpg", "/images/ps5.jpg", "PlayStation 5" },
                    { 14, 25, "Wireless headphones with noise cancellation.", 9, 3, 40, 5, "sony_wh1000xm5.jpg", "/images/sony_wh1000xm5.jpg", "Sony WH-1000XM5 Headphones" },
                    { 15, 30, "Iconic sports shoes.", 10, 4, 50, 5, "airmax.jpg", "/images/airmax.jpg", "Air Max" },
                    { 16, 25, "Classic Adidas sneakers.", 11, 4, 40, 5, "stansmith.jpg", "/images/stansmith.jpg", "Stan Smith" },
                    { 17, 20, "Iconic Puma Suede sneakers.", 12, 4, 30, 4, "puma_suede.jpg", "/images/puma_suede.jpg", "Puma Suede" },
                    { 18, 100, "Digital gift card usable everywhere.", 13, 5, 200, 20, "paypal_gift.jpg", "/images/paypal_gift.jpg", "PayPal Gift Card" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Product",
                keyColumn: "idProduct",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Product",
                keyColumn: "idProduct",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Product",
                keyColumn: "idProduct",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Product",
                keyColumn: "idProduct",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Product",
                keyColumn: "idProduct",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Product",
                keyColumn: "idProduct",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Product",
                keyColumn: "idProduct",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Product",
                keyColumn: "idProduct",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Product",
                keyColumn: "idProduct",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Product",
                keyColumn: "idProduct",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Product",
                keyColumn: "idProduct",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Product",
                keyColumn: "idProduct",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Product",
                keyColumn: "idProduct",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Product",
                keyColumn: "idProduct",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Product",
                keyColumn: "idProduct",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "Product",
                keyColumn: "idProduct",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "Product",
                keyColumn: "idProduct",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "Product",
                keyColumn: "idProduct",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "Brand",
                keyColumn: "idBrand",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Brand",
                keyColumn: "idBrand",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Brand",
                keyColumn: "idBrand",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Brand",
                keyColumn: "idBrand",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Brand",
                keyColumn: "idBrand",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Brand",
                keyColumn: "idBrand",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Brand",
                keyColumn: "idBrand",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Brand",
                keyColumn: "idBrand",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Brand",
                keyColumn: "idBrand",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Brand",
                keyColumn: "idBrand",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Brand",
                keyColumn: "idBrand",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Brand",
                keyColumn: "idBrand",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Brand",
                keyColumn: "idBrand",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "TypeProduct",
                keyColumn: "idTypeProduct",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "TypeProduct",
                keyColumn: "idTypeProduct",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "TypeProduct",
                keyColumn: "idTypeProduct",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "TypeProduct",
                keyColumn: "idTypeProduct",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "TypeProduct",
                keyColumn: "idTypeProduct",
                keyValue: 5);
        }
    }
}
