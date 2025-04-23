using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Drinkbox.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Brands",
                columns: table => new
                {
                    BrandId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BrandName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Brands", x => x.BrandId);
                });

            migrationBuilder.CreateTable(
                name: "Coins",
                columns: table => new
                {
                    CoinId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Denomination = table.Column<string>(type: "nvarchar(12)", maxLength: 12, nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Coins", x => x.CoinId);
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    OrderId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TotalSum = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.OrderId);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    ProductId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Price = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    BrandId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.ProductId);
                    table.ForeignKey(
                        name: "FK_Products_Brands_BrandId",
                        column: x => x.BrandId,
                        principalTable: "Brands",
                        principalColumn: "BrandId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrderItems",
                columns: table => new
                {
                    OrderItemId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    UnitPrice = table.Column<int>(type: "int", nullable: false),
                    TotalPrice = table.Column<int>(type: "int", nullable: false),
                    OrderId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderItems", x => x.OrderItemId);
                    table.ForeignKey(
                        name: "FK_OrderItems_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "OrderId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderItems_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "ProductId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_OrderId",
                table: "OrderItems",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_ProductId",
                table: "OrderItems",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_BrandId",
                table: "Products",
                column: "BrandId");

            migrationBuilder.InsertData(
                table: "Brands",
                columns: new[] { "BrandName" },
                values: new object[,] { { "Coca-Cola" }, { "Pepsi" },
                            { "Fanta" }, { "Dr Pepper" }, { "Sprite" }, { "Добрый" } }
                );

            migrationBuilder.InsertData(
                table: "Coins",
                columns: new[] { "Denomination", "Quantity" },
                values: new object[,]
                {
                    { "1 рубль", 100 },
                    { "2 рубля", 100 },
                    { "5 рублей", 50 },
                    { "10 рублей", 30 }
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "BrandId", "ProductName", "Price",
                    "Quantity", "ImageUrl", "IsActive" },
                values: new object[,]
                {
                    { 1, "Coca-Cola 0.3л", 80, 18, "https://novosib.luding.ru/upload/iblock/f70/pve97zbvuzfmpsj06vzm7f1lk85j28eb.png", true },
                    { 1, "Coca-Cola 0.5л", 120, 15, "https://www.globalalco.ru/i/Coca-Cola-Company/Coca-Cola-Classic-Russia-in-pet-0-5-l--13796/22961_source_trim.png", true },
                    { 2, "Pepsi 0.3л", 75, 0, "https://www.globalalco.ru/i/PepsiCo-Inc/Pepsi-Russia-in-can-0-33-l--14308/23454_source_trim.png", false },
                    { 3, "Fanta Апельсин 0.5л", 70, 10, "https://www.globalalco.ru/i/Coca-Cola-Company/Fanta-Orange-Russia-in-pet-0-5-l--13832/22927_source_trim.png", true },
                    { 5, "Sprite 0.5л", 70, 8, "https://www.globalalco.ru/i/Coca-Cola-Company/Sprite-Russia-in-pet-0-5-l--13820/22944_source_trim.png", true },
                    { 4, "Dr Pepper 0.5л", 90, 20, "https://www.globalalco.ru/i/Dr-Pepper-Snapple-Group/Dr-Pepper-Poland-in-pet-0-5-l--13901/22837_source_trim.png", true },
                    { 6, "Добрый cola ледяной лимон 0.5л", 80, 15, "https://dobry.ru/local/templates/dobry/bundles/gazirovka/images/banner-bottles/cola-lemon.png", true },
                    { 6, "Добрый киви-виноград 0.3л", 120, 14, "https://dobry.ru/local/templates/dobry/bundles/gazirovka/images/banner-bottles/kiwi-grape.png", true },
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Coins");

            migrationBuilder.DropTable(
                name: "OrderItems");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "Brands");
        }
    }
}
