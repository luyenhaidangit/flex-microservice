﻿// <auto-generated />
using System;
using Flex.Securities.Api.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Oracle.EntityFrameworkCore.Metadata;

#nullable disable

namespace Flex.Securities.Api.Migrations
{
    [DbContext(typeof(SecuritiesDbContext))]
    partial class SecuritiesDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            OracleModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Flex.Securities.Api.Entities.CatalogSecurities", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("NUMBER(19)");

                    OraclePropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"));

                    b.Property<DateTimeOffset>("CreatedDate")
                        .HasColumnType("TIMESTAMP(7) WITH TIME ZONE");

                    b.Property<string>("Description")
                        .HasColumnType("CLOB");

                    b.Property<DateTimeOffset?>("LastModifiedDate")
                        .HasColumnType("TIMESTAMP(7) WITH TIME ZONE");

                    b.Property<string>("Symbol")
                        .IsRequired()
                        .HasColumnType("VARCHAR2(250)");

                    b.Property<string>("TradePlace")
                        .IsRequired()
                        .HasColumnType("VARCHAR2(10)");

                    b.HasKey("Id");

                    b.ToTable("SECURITIES");
                });
#pragma warning restore 612, 618
        }
    }
}
