﻿// <auto-generated />
using System;
using BCExplorer.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace BCExplorer.Model.Migrations
{
    [DbContext(typeof(BCExplorerContext))]
    partial class BCExplorerContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.4-servicing-10062")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("BCExplorer.Model.Address", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(34);

                    b.Property<decimal>("Balance")
                        .HasColumnType("decimal(18,8)");

                    b.Property<int>("LastModifiedBlockHeight");

                    b.Property<string>("TxIdBlob");

                    b.HasKey("Id");

                    b.ToTable("Addresses");
                });

            modelBuilder.Entity("BCExplorer.Model.Block", b =>
                {
                    b.Property<int>("Id");

                    b.Property<string>("BlockData");

                    b.Property<string>("BlockHash")
                        .HasMaxLength(64);

                    b.Property<int>("Height");

                    b.HasKey("Id");

                    b.HasIndex("BlockHash")
                        .IsUnique()
                        .HasFilter("[BlockHash] IS NOT NULL")
                        .HasAnnotation("SqlServer:Clustered", false);

                    b.ToTable("Blocks");
                });

            modelBuilder.Entity("BCExplorer.Model.Statistics", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("BestAdrIndexHeight");

                    b.Property<DateTime>("ModifiedDate");

                    b.HasKey("Id");

                    b.ToTable("Statistics");
                });

            modelBuilder.Entity("BCExplorer.Model.Transaction", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(64);

                    b.Property<int?>("BlockId");

                    b.Property<string>("TransactionData");

                    b.HasKey("Id");

                    b.HasIndex("BlockId");

                    b.ToTable("Transactions");
                });

            modelBuilder.Entity("BCExplorer.Model.Transaction", b =>
                {
                    b.HasOne("BCExplorer.Model.Block", "Block")
                        .WithMany("Transactions")
                        .HasForeignKey("BlockId");
                });
#pragma warning restore 612, 618
        }
    }
}
