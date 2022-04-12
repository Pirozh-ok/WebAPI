using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Habr.DataAccess.Migrations
{
    public partial class RenameColumnRegistrDateFromUserToRegistrationDate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RegistrDate",
                table: "User",
                newName: "RegistrationDate"
                );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RegistrationDate",
                table: "User",
                newName: "RegistrDate"
                );
        }
    }
}
