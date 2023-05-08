using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CapitalShopFinalProject.Migrations
{
    public partial class reviewTableUpdated : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "email",
                table: "ReviewTests",
                newName: "Email");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "ReviewTests",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "ReviewTests");

            migrationBuilder.RenameColumn(
                name: "Email",
                table: "ReviewTests",
                newName: "email");
        }
    }
}
