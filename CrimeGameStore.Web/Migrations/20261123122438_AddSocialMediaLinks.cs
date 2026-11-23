using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CrimeGameStore.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddSocialMediaLinks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FacebookUrl",
                table: "SiteSettings",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InstagramUrl",
                table: "SiteSettings",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: true);

            migrationBuilder.UpdateData(
                table: "SiteSettings",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "FacebookUrl", "InstagramUrl" },
                values: new object[] { "https://facebook.com/", "https://instagram.com/" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FacebookUrl",
                table: "SiteSettings");

            migrationBuilder.DropColumn(
                name: "InstagramUrl",
                table: "SiteSettings");
        }
    }
}
