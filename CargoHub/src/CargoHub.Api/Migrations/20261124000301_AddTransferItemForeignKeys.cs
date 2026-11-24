using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace CargoHub.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddTransferItemForeignKeys : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_TransferItems_ItemId",
                table: "TransferItems",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_TransferItems_TransferId",
                table: "TransferItems",
                column: "TransferId");

            migrationBuilder.AddForeignKey(
                name: "FK_TransferItems_Items_ItemId",
                table: "TransferItems",
                column: "ItemId",
                principalTable: "Items",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TransferItems_Transfers_TransferId",
                table: "TransferItems",
                column: "TransferId",
                principalTable: "Transfers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.DropForeignKey(
                name: "FK_TransferItems_Items_ItemId",
                table: "TransferItems");

            migrationBuilder.DropForeignKey(
                name: "FK_TransferItems_Transfers_TransferId",
                table: "TransferItems");

            migrationBuilder.DropIndex(
                name: "IX_TransferItems_ItemId",
                table: "TransferItems");

            migrationBuilder.DropIndex(
                name: "IX_TransferItems_TransferId",
                table: "TransferItems");
        }
    }
}
