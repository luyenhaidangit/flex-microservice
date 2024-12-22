using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Flex.Securities.Api.Migrations
{
    /// <inheritdoc />
    public partial class DropTable1221 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SECURITIES_ISSUERS_IssuerId",
                table: "SECURITIES");

            migrationBuilder.DropTable(
                name: "ISSUERREQUESTS");

            migrationBuilder.DropTable(
                name: "ISSUERS");

            migrationBuilder.DropIndex(
                name: "IX_SECURITIES_IssuerId",
                table: "SECURITIES");

            migrationBuilder.DropColumn(
                name: "IssuerId",
                table: "SECURITIES");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "IssuerId",
                table: "SECURITIES",
                type: "NUMBER(19)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ISSUERREQUESTS",
                columns: table => new
                {
                    Id = table.Column<long>(type: "NUMBER(19)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    Code = table.Column<string>(type: "VARCHAR2(50)", nullable: true),
                    CreatedDate = table.Column<DateTimeOffset>(type: "TIMESTAMP(7) WITH TIME ZONE", nullable: false),
                    Description = table.Column<string>(type: "CLOB", nullable: true),
                    EntityId = table.Column<long>(type: "NUMBER(19)", nullable: true),
                    LastModifiedDate = table.Column<DateTimeOffset>(type: "TIMESTAMP(7) WITH TIME ZONE", nullable: true),
                    Name = table.Column<string>(type: "VARCHAR2(250)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ISSUERREQUESTS", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ISSUERS",
                columns: table => new
                {
                    Id = table.Column<long>(type: "NUMBER(19)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    Code = table.Column<string>(type: "VARCHAR2(50)", nullable: true),
                    CreatedDate = table.Column<DateTimeOffset>(type: "TIMESTAMP(7) WITH TIME ZONE", nullable: false),
                    Description = table.Column<string>(type: "CLOB", nullable: true),
                    LastModifiedDate = table.Column<DateTimeOffset>(type: "TIMESTAMP(7) WITH TIME ZONE", nullable: true),
                    Name = table.Column<string>(type: "VARCHAR2(250)", nullable: true),
                    ProcessStatus = table.Column<string>(type: "VARCHAR2(50)", nullable: true)
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
                principalColumn: "Id");
        }
    }
}
