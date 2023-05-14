using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CapitalShopFinalProject.Migrations
{
    public partial class BasketTableUpdatedV2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "DiscountedPrice",
                table: "Baskets",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Image",
                table: "Baskets",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "Baskets",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DiscountedPrice",
                table: "Baskets");

            migrationBuilder.DropColumn(
                name: "Image",
                table: "Baskets");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "Baskets");
        }
    }
}
