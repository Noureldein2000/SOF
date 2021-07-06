﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SourceOfFund.Data;

namespace SourceOfFund.Data.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.16")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("SourceOfFund.Data.Entities.AccountServiceAvailableBalance", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("AccountID")
                        .HasColumnType("int");

                    b.Property<decimal>("Balance")
                        .HasColumnType("decimal(18,3)");

                    b.Property<int>("BalanceTypeID")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreationDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("UpdateDate")
                        .HasColumnType("datetime2");

                    b.HasKey("ID");

                    b.HasIndex("BalanceTypeID");

                    b.ToTable("AccountServiceAvailableBalances");
                });

            modelBuilder.Entity("SourceOfFund.Data.Entities.AccountServiceBalance", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("AccountID")
                        .HasColumnType("int");

                    b.Property<decimal>("Balance")
                        .HasColumnType("decimal(18,3)");

                    b.Property<int>("BalanceTypeID")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreationDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("UpdateDate")
                        .HasColumnType("datetime2");

                    b.HasKey("ID");

                    b.HasIndex("BalanceTypeID");

                    b.ToTable("AccountServiceBalances");
                });

            modelBuilder.Entity("SourceOfFund.Data.Entities.BalanceHistory", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("AccountID")
                        .HasColumnType("int");

                    b.Property<decimal>("BalanceBefore")
                        .HasColumnType("decimal(18,3)");

                    b.Property<int>("BalanceTypeID")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreationDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("TransactionID")
                        .HasColumnType("int");

                    b.Property<DateTime?>("UpdateDate")
                        .HasColumnType("datetime2");

                    b.HasKey("ID");

                    b.HasIndex("BalanceTypeID");

                    b.ToTable("BalanceHistory");
                });

            modelBuilder.Entity("SourceOfFund.Data.Entities.BalanceType", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ArName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreationDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("UpdateDate")
                        .HasColumnType("datetime2");

                    b.HasKey("ID");

                    b.ToTable("BalanceTypes");

                    b.HasData(
                        new
                        {
                            ID = 1,
                            ArName = "رصيد ممكن",
                            CreationDate = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Name = "Momkm Balance"
                        },
                        new
                        {
                            ID = 2,
                            ArName = "رصيد كاش",
                            CreationDate = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Name = "Cash Balance"
                        });
                });

            modelBuilder.Entity("SourceOfFund.Data.Entities.HoldBalance", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("AccountID")
                        .HasColumnType("int");

                    b.Property<decimal>("Balance")
                        .HasColumnType("decimal(18, 3)");

                    b.Property<DateTime>("CreationDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("RequestID")
                        .HasColumnType("int");

                    b.Property<int>("SourceID")
                        .HasColumnType("int");

                    b.Property<short>("Status")
                        .HasColumnType("smallint");

                    b.Property<DateTime?>("UpdateDate")
                        .HasColumnType("datetime2");

                    b.HasKey("ID");

                    b.ToTable("HoldBalances");
                });

            modelBuilder.Entity("SourceOfFund.Data.Entities.AccountServiceAvailableBalance", b =>
                {
                    b.HasOne("SourceOfFund.Data.Entities.BalanceType", "BalanceType")
                        .WithMany("AccountServiceAvailableBalances")
                        .HasForeignKey("BalanceTypeID")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();
                });

            modelBuilder.Entity("SourceOfFund.Data.Entities.AccountServiceBalance", b =>
                {
                    b.HasOne("SourceOfFund.Data.Entities.BalanceType", "BalanceType")
                        .WithMany("AccountServiceBalances")
                        .HasForeignKey("BalanceTypeID")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();
                });

            modelBuilder.Entity("SourceOfFund.Data.Entities.BalanceHistory", b =>
                {
                    b.HasOne("SourceOfFund.Data.Entities.BalanceType", "BalanceType")
                        .WithMany("BalanceHistorys")
                        .HasForeignKey("BalanceTypeID")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
