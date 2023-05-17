using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CapitalShopFinalProject.Migrations
{
    public partial class OrdersTableUpdatedv5 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CreditCardId",
                table: "Orders",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Orders_CreditCardId",
                table: "Orders",
                column: "CreditCardId");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_CreditCards_CreditCardId",
                table: "Orders",
                column: "CreditCardId",
                principalTable: "CreditCards",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_CreditCards_CreditCardId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_CreditCardId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "CreditCardId",
                table: "Orders");
        }
    }
}
