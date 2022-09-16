using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Habr.DataAccess.Migrations
{
    public partial class ChangedentityAvatarImageaddedentityPostIMagechangeattributeisRequiredinfieldwithdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "PathImage",
                table: "AvatarImages",
                type: "nvarchar(max)",
                nullable: true,
                defaultValue: "Images\\DefaultAvatar.png",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldDefaultValue: "C:\\Users\\ivan-\\source\\repos\\Habr\\Habr.Presentation\\Content\\Images\\DefaultAvatar.png");

            migrationBuilder.CreateTable(
                name: "PostImages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PathImage = table.Column<string>(type: "nvarchar(max)", nullable: false, defaultValue: ""),
                    LoadDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "getdate()"),
                    PostId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PostImages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PostImages_Posts_PostId",
                        column: x => x.PostId,
                        principalTable: "Posts",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_PostImages_PostId",
                table: "PostImages",
                column: "PostId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PostImages");

            migrationBuilder.AlterColumn<string>(
                name: "PathImage",
                table: "AvatarImages",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "C:\\Users\\ivan-\\source\\repos\\Habr\\Habr.Presentation\\Content\\Images\\DefaultAvatar.png",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true,
                oldDefaultValue: "Images\\DefaultAvatar.png");
        }
    }
}
