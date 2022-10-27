using Microsoft.EntityFrameworkCore.Migrations;

namespace Pustok.Migrations
{
    public partial class UpdatedProductTableOriginal : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ProductPrice",
                table: "Products",
                newName: "Price");

            migrationBuilder.AlterColumn<decimal>(
                name: "DiscountPrice",
                table: "Products",
                type: "money",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float",
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsArrival",
                table: "Products",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsFeature",
                table: "Products",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsMostView",
                table: "Products",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsArrival",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "IsFeature",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "IsMostView",
                table: "Products");

            migrationBuilder.RenameColumn(
                name: "Price",
                table: "Products",
                newName: "ProductPrice");

            migrationBuilder.AlterColumn<double>(
                name: "DiscountPrice",
                table: "Products",
                type: "float",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "money",
                oldNullable: true);
        }
    }
}
