using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Flex.Securities.Api.Migrations
{
    /// <inheritdoc />
    public partial class ChangeDataTypeProcessType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ProcessStatus",
                table: "ISSUERS",
                type: "VARCHAR2(50)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "NUMBER(10)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SECURITIES_IssuerId",
                table: "SECURITIES",
                column: "IssuerId");

            migrationBuilder.AddForeignKey(
                name: "FK_SECURITIES_ISSUERS_IssuerId",
                table: "SECURITIES",
                column: "IssuerId",
                principalTable: "ISSUERS",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SECURITIES_ISSUERS_IssuerId",
                table: "SECURITIES");

            migrationBuilder.DropIndex(
                name: "IX_SECURITIES_IssuerId",
                table: "SECURITIES");

            migrationBuilder.AlterColumn<int>(
                name: "ProcessStatus",
                table: "ISSUERS",
                type: "NUMBER(10)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "VARCHAR2(50)",
                oldNullable: true);
        }
    }
}
