using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RinhaBackend.Api.Migrations
{
    /// <inheritdoc />
    public partial class InitialDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "payments",
                columns: table => new
                {
                    correlationId = table.Column<Guid>(type: "uuid", nullable: false, comment: "Identificador"),
                    amount = table.Column<decimal>(type: "numeric", nullable: false, comment: "Valor do pagamento"),
                    requestedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, comment: "Data e hora da solicitação")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_payments", x => x.correlationId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "payments");
        }
    }
}
