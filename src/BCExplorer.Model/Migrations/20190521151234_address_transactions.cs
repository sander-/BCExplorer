using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BCExplorer.Model.Migrations
{
    public partial class address_transactions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AddressTransactions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AddressId = table.Column<string>(nullable: true),
                    Balance = table.Column<decimal>(type: "decimal(18,8)", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,8)", nullable: false),
                    TransactionId = table.Column<string>(maxLength: 64, nullable: true),
                    TimeStamp = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AddressTransactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AddressTransactions_Addresses_AddressId",
                        column: x => x.AddressId,
                        principalTable: "Addresses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AddressTransactions_AddressId",
                table: "AddressTransactions",
                column: "AddressId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AddressTransactions");
        }
    }
}
