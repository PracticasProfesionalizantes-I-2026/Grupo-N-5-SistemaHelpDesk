using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HelpDesk.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class FixSolutionRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Soluciones_Tickets_TicketId1",
                table: "Soluciones");

            migrationBuilder.DropIndex(
                name: "IX_Soluciones_TicketId1",
                table: "Soluciones");

            migrationBuilder.DropColumn(
                name: "TicketId1",
                table: "Soluciones");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "TicketId1",
                table: "Soluciones",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Soluciones_TicketId1",
                table: "Soluciones",
                column: "TicketId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Soluciones_Tickets_TicketId1",
                table: "Soluciones",
                column: "TicketId1",
                principalTable: "Tickets",
                principalColumn: "Id");
        }
    }
}
