using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Flex.Securities.Api.Migrations
{
    /// <inheritdoc />
    public partial class InitSource : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DataProposed",
                table: "ISSUERREQUESTS");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "ISSUERREQUESTS");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "ISSUERS",
                newName: "Code");

            migrationBuilder.RenameColumn(
                name: "Type",
                table: "ISSUERREQUESTS",
                newName: "Code");

            migrationBuilder.AddColumn<int>(
                name: "ProcessStatus",
                table: "ISSUERS",
                type: "NUMBER(10)",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "ISSUERREQUESTS",
                type: "CLOB",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "ISSUERREQUESTS",
                type: "VARCHAR2(250)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProcessStatus",
                table: "ISSUERS");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "ISSUERREQUESTS");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "ISSUERREQUESTS");

            migrationBuilder.RenameColumn(
                name: "Code",
                table: "ISSUERS",
                newName: "Status");

            migrationBuilder.RenameColumn(
                name: "Code",
                table: "ISSUERREQUESTS",
                newName: "Type");

            migrationBuilder.AddColumn<string>(
                name: "DataProposed",
                table: "ISSUERREQUESTS",
                type: "CLOB",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "ISSUERREQUESTS",
                type: "VARCHAR2(50)",
                nullable: false,
                defaultValue: "");
        }
    }
}
