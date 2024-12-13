using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Flex.Securities.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddTableIssuer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "IssuerId",
                table: "SECURITIES",
                type: "NUMBER(10)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "VARCHAR2(250)");

            migrationBuilder.CreateTable(
                name: "ISSUERS",
                columns: table => new
                {
                    Id = table.Column<long>(type: "NUMBER(19)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    Name = table.Column<string>(type: "VARCHAR2(250)", nullable: false),
                    Status = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    Description = table.Column<string>(type: "CLOB", nullable: true),
                    CreatedDate = table.Column<DateTimeOffset>(type: "TIMESTAMP(7) WITH TIME ZONE", nullable: false),
                    LastModifiedDate = table.Column<DateTimeOffset>(type: "TIMESTAMP(7) WITH TIME ZONE", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ISSUERS", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SECURITIES_IssuerId",
                table: "SECURITIES",
                column: "IssuerId");

            migrationBuilder.AddForeignKey(
                name: "FK_SECURITIES_ISSUERS_IssuerId",
                table: "SECURITIES",
                column: "IssuerId",
                principalTable: "ISSUERS",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SECURITIES_ISSUERS_IssuerId",
                table: "SECURITIES");

            migrationBuilder.DropTable(
                name: "ISSUERS");

            migrationBuilder.DropIndex(
                name: "IX_SECURITIES_IssuerId",
                table: "SECURITIES");

            migrationBuilder.AlterColumn<string>(
                name: "IssuerId",
                table: "SECURITIES",
                type: "VARCHAR2(250)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "NUMBER(10)");
        }
    }
}
