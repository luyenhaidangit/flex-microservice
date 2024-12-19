using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Flex.Securities.Api.Migrations
{
    /// <inheritdoc />
    public partial class ChangeTypeTableNull : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "ProcessStatus",
                table: "ISSUERS",
                type: "NUMBER(10)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "NUMBER(10)");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "ISSUERS",
                type: "VARCHAR2(250)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "VARCHAR2(250)");

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "ISSUERS",
                type: "VARCHAR2(50)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "VARCHAR2(50)");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "ISSUERREQUESTS",
                type: "VARCHAR2(250)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "VARCHAR2(250)");

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "ISSUERREQUESTS",
                type: "VARCHAR2(50)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "VARCHAR2(50)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "ProcessStatus",
                table: "ISSUERS",
                type: "NUMBER(10)",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "NUMBER(10)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "ISSUERS",
                type: "VARCHAR2(250)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "VARCHAR2(250)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "ISSUERS",
                type: "VARCHAR2(50)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "VARCHAR2(50)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "ISSUERREQUESTS",
                type: "VARCHAR2(250)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "VARCHAR2(250)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "ISSUERREQUESTS",
                type: "VARCHAR2(50)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "VARCHAR2(50)",
                oldNullable: true);
        }
    }
}
