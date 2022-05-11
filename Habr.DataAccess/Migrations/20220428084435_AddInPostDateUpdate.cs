using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Habr.DataAccess.Migrations
{
    public partial class AddInPostDateUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "Updated",
                table: "Posts",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "getdate()");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Updated",
                table: "Posts");
        }
    }
}
