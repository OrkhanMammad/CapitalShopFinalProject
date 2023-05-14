using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CapitalShopFinalProject.Migrations
{
    public partial class BasketTableUpdatedV3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Count",
                table: "Baskets",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Count",
                table: "Baskets",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");
        }
    }
}
