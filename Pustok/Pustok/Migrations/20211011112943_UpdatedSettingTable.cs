using Microsoft.EntityFrameworkCore.Migrations;

namespace Pustok.Migrations
{
    public partial class UpdatedSettingTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "Settings",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Settings",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FacebookUrl",
                table: "Settings",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GoogleUrl",
                table: "Settings",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Phone",
                table: "Settings",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TwitterUrl",
                table: "Settings",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "YoutubeUrl",
                table: "Settings",
                maxLength: 255,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "FacebookUrl",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "GoogleUrl",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "Phone",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "TwitterUrl",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "YoutubeUrl",
                table: "Settings");
        }
    }
}
