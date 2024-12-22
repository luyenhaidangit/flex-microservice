using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Flex.Securities.Api.Migrations
{
    /// <inheritdoc />
    public partial class InitDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ISSUERREQUESTS",
                columns: table => new
                {
                    Id = table.Column<long>(type: "NUMBER(19)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    Name = table.Column<string>(type: "VARCHAR2(250)", nullable: true),
                    Code = table.Column<string>(type: "VARCHAR2(50)", nullable: true),
                    Description = table.Column<string>(type: "CLOB", nullable: true),
                    CreatedDate = table.Column<DateTimeOffset>(type: "TIMESTAMP(7) WITH TIME ZONE", nullable: false),
                    LastModifiedDate = table.Column<DateTimeOffset>(type: "TIMESTAMP(7) WITH TIME ZONE", nullable: true),
                    EntityId = table.Column<long>(type: "NUMBER(19)", nullable: true)
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
                    Name = table.Column<string>(type: "VARCHAR2(250)", nullable: true),
                    Code = table.Column<string>(type: "VARCHAR2(50)", nullable: true),
                    Description = table.Column<string>(type: "CLOB", nullable: true),
                    ProcessStatus = table.Column<string>(type: "VARCHAR2(50)", nullable: true),
                    CreatedDate = table.Column<DateTimeOffset>(type: "TIMESTAMP(7) WITH TIME ZONE", nullable: false),
                    LastModifiedDate = table.Column<DateTimeOffset>(type: "TIMESTAMP(7) WITH TIME ZONE", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ISSUERS", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SECURITIES",
                columns: table => new
                {
                    Id = table.Column<long>(type: "NUMBER(19)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    Symbol = table.Column<string>(type: "VARCHAR2(250)", nullable: false),
                    TradePlace = table.Column<string>(type: "VARCHAR2(10)", nullable: false),
                    Description = table.Column<string>(type: "CLOB", nullable: true),
                    IssuerId = table.Column<long>(type: "NUMBER(19)", nullable: true),
                    CreatedDate = table.Column<DateTimeOffset>(type: "TIMESTAMP(7) WITH TIME ZONE", nullable: false),
                    LastModifiedDate = table.Column<DateTimeOffset>(type: "TIMESTAMP(7) WITH TIME ZONE", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SECURITIES", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SECURITIES_ISSUERS_IssuerId",
                        column: x => x.IssuerId,
                        principalTable: "ISSUERS",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_SECURITIES_IssuerId",
                table: "SECURITIES",
                column: "IssuerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ISSUERREQUESTS");

            migrationBuilder.DropTable(
                name: "SECURITIES");

            migrationBuilder.DropTable(
                name: "ISSUERS");
        }
    }
}
