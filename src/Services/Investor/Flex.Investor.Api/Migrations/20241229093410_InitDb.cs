using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Flex.Investor.Api.Migrations
{
    /// <inheritdoc />
    public partial class InitDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Investors",
                columns: table => new
                {
                    ID = table.Column<long>(type: "NUMBER(19)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    NO = table.Column<string>(type: "VARCHAR2(150)", nullable: false),
                    FULLNAME = table.Column<string>(type: "VARCHAR2(250)", nullable: false),
                    EMAIL = table.Column<string>(type: "VARCHAR2(250)", nullable: true),
                    PHONE = table.Column<string>(type: "VARCHAR2(250)", nullable: false),
                    CREATEDDATE = table.Column<DateTimeOffset>(type: "TIMESTAMP(7) WITH TIME ZONE", nullable: false),
                    LASTMODIFIEDDATE = table.Column<DateTimeOffset>(type: "TIMESTAMP(7) WITH TIME ZONE", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Investors", x => x.ID);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Investors");
        }
    }
}
