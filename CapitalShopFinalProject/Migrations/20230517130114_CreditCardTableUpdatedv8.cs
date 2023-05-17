using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CapitalShopFinalProject.Migrations
{
    public partial class CreditCardTableUpdatedv8 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "CreditCards",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CreditCards_UserId",
                table: "CreditCards",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_CreditCards_AspNetUsers_UserId",
                table: "CreditCards",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CreditCards_AspNetUsers_UserId",
                table: "CreditCards");

            migrationBuilder.DropIndex(
                name: "IX_CreditCards_UserId",
                table: "CreditCards");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "CreditCards");
        }
    }
}
