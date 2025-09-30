using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RinhaBackend.Api.Migrations
{
    /// <inheritdoc />
    public partial class ColumnIsProcessed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "isProcessed",
                table: "payments",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                comment: "Pagamento processado");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "isProcessed",
                table: "payments");
        }
    }
}
