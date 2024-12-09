using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SignDesign_App.Data.Migrations
{
    public partial class ExtendIdentityUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ContactFirstName",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ContactLastName",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ContactOrganizationType",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContactFirstName",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ContactLastName",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ContactOrganizationType",
                table: "AspNetUsers");
        }
    }
}
