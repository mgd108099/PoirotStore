using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CrimeGameStore.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddShamCashPayment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PaymentMethod",
                table: "Orders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "PaymentReference",
                table: "Orders",
                type: "nvarchar(120)",
                maxLength: 120,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PaymentMethod",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "PaymentReference",
                table: "Orders");
        }
    }
}
