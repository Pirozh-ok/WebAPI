using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Habr.DataAccess.Migrations
{
    public partial class AddInPostColumnIsPublished : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Text",
                table: "Posts",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldMaxLength: 5000,
                oldDefaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsPublished",
                table: "Posts",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPublished",
                table: "Posts");

            migrationBuilder.AlterColumn<string>(
                name: "Text",
                table: "Posts",
                type: "nvarchar(max)",
                maxLength: 5000,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(2000)",
                oldMaxLength: 2000,
                oldDefaultValue: "");
        }
    }
}
