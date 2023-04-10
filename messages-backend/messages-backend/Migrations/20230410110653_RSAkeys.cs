using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace messages_backend.Migrations
{
    /// <inheritdoc />
    public partial class RSAkeys : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ComainRSAKey",
                table: "Accounts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "MainRSAKey",
                table: "Accounts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ComainRSAKey",
                table: "Accounts");

            migrationBuilder.DropColumn(
                name: "MainRSAKey",
                table: "Accounts");
        }
    }
}
