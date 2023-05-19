using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CapitalShopFinalProject.Migrations
{
    public partial class AppuserTableUpdatedV3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BlockedBy",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BlockedBy",
                table: "AspNetUsers");
        }
    }
}
