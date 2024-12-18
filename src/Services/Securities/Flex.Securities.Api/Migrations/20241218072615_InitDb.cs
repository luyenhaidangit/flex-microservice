﻿using System;
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
                    Type = table.Column<string>(type: "VARCHAR2(50)", nullable: false),
                    Status = table.Column<string>(type: "VARCHAR2(50)", nullable: false),
                    DataProposed = table.Column<string>(type: "CLOB", nullable: false),
                    CreatedDate = table.Column<DateTimeOffset>(type: "TIMESTAMP(7) WITH TIME ZONE", nullable: false),
                    LastModifiedDate = table.Column<DateTimeOffset>(type: "TIMESTAMP(7) WITH TIME ZONE", nullable: true),
                    EntityId = table.Column<long>(type: "NUMBER(19)", nullable: false)
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
                    Name = table.Column<string>(type: "VARCHAR2(250)", nullable: false),
                    Status = table.Column<string>(type: "VARCHAR2(50)", nullable: false),
                    Description = table.Column<string>(type: "CLOB", nullable: true),
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
                    IssuerId = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    TradePlace = table.Column<int>(type: "NUMBER(10)", nullable: false),
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
                name: "ISSUERREQUESTS");

            migrationBuilder.DropTable(
                name: "ISSUERS");

            migrationBuilder.DropTable(
                name: "SECURITIES");
        }
    }
}
