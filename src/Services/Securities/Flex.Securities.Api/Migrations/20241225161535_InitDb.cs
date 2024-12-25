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
                name: "SECURITIES",
                columns: table => new
                {
                    Id = table.Column<long>(type: "NUMBER(19)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    Symbol = table.Column<string>(type: "VARCHAR2(250)", nullable: false),
                    TradePlace = table.Column<string>(type: "VARCHAR2(10)", nullable: false),
                    Description = table.Column<string>(type: "CLOB", nullable: true),
                    CreatedDate = table.Column<DateTimeOffset>(type: "TIMESTAMP(7) WITH TIME ZONE", nullable: false),
                    LastModifiedDate = table.Column<DateTimeOffset>(type: "TIMESTAMP(7) WITH TIME ZONE", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SECURITIES", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SECURITIES");
        }
    }
}
