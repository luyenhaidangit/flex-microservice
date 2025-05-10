using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Flex.AspNetIdentity.Api.Migrations
{
    /// <inheritdoc />
    public partial class Continue : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "BRANCH_ID",
                table: "Users",
                type: "NUMBER(19)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FULL_NAME",
                table: "Users",
                type: "NVARCHAR2(250)",
                maxLength: 250,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BRANCH_ID",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "FULL_NAME",
                table: "Users");
        }
    }
}
