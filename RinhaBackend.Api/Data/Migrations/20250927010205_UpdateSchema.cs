using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RinhaBackend.Api.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "requestedAt",
                table: "payments",
                type: "timestamp with time zone",
                nullable: true,
                comment: "Data e hora da solicitação",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldComment: "Data e hora da solicitação");

            migrationBuilder.AddColumn<int>(
                name: "processedBy",
                table: "payments",
                type: "integer",
                nullable: true,
                comment: "Onde o pagamento foi processado");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "processedBy",
                table: "payments");

            migrationBuilder.AlterColumn<DateTime>(
                name: "requestedAt",
                table: "payments",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                comment: "Data e hora da solicitação",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true,
                oldComment: "Data e hora da solicitação");
        }
    }
}
