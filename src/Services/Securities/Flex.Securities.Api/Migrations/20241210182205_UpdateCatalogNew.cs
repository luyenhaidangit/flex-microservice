using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Flex.Securities.Api.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCatalogNew : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "No",
                table: "SECURITIES");

            migrationBuilder.RenameColumn(
                name: "IssuerNo",
                table: "SECURITIES",
                newName: "IssuerId");

            migrationBuilder.AlterColumn<int>(
                name: "TradePlace",
                table: "SECURITIES",
                type: "NUMBER(10)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "VARCHAR2(250)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IssuerId",
                table: "SECURITIES",
                newName: "IssuerNo");

            migrationBuilder.AlterColumn<string>(
                name: "TradePlace",
                table: "SECURITIES",
                type: "VARCHAR2(250)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "NUMBER(10)");

            migrationBuilder.AddColumn<string>(
                name: "No",
                table: "SECURITIES",
                type: "VARCHAR2(150)",
                nullable: false,
                defaultValue: "");
        }
    }
}
