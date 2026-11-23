using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CrimeGameStore.Web.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CrimeCases",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CaseCode = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Slug = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: false),
                    ShortDescription = table.Column<string>(type: "nvarchar(350)", maxLength: 350, nullable: false),
                    FullStory = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Difficulty = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PlayersCount = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    EstimatedTime = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CoverImageUrl = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    PhysicalPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PhysicalStock = table.Column<int>(type: "int", nullable: false),
                    IsPhysicalAvailable = table.Column<bool>(type: "bit", nullable: false),
                    DigitalPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IsDigitalAvailable = table.Column<bool>(type: "bit", nullable: false),
                    MetaTitle = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: true),
                    MetaDescription = table.Column<string>(type: "nvarchar(320)", maxLength: 320, nullable: true),
                    MetaKeywords = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    IsFeatured = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CrimeCases", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderNumber = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    CustomerName = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    City = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: true),
                    ShippingAddress = table.Column<string>(type: "nvarchar(350)", maxLength: 350, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CaseGalleries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CrimeCaseId = table.Column<int>(type: "int", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Caption = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CaseGalleries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CaseGalleries_CrimeCases_CrimeCaseId",
                        column: x => x.CrimeCaseId,
                        principalTable: "CrimeCases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrderItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderId = table.Column<int>(type: "int", nullable: false),
                    CrimeCaseId = table.Column<int>(type: "int", nullable: false),
                    Edition = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderItems_CrimeCases_CrimeCaseId",
                        column: x => x.CrimeCaseId,
                        principalTable: "CrimeCases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrderItems_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "CrimeCases",
                columns: new[] { "Id", "CaseCode", "CoverImageUrl", "CreatedAt", "Difficulty", "DigitalPrice", "EstimatedTime", "FullStory", "IsActive", "IsDigitalAvailable", "IsFeatured", "IsPhysicalAvailable", "MetaDescription", "MetaKeywords", "MetaTitle", "PhysicalPrice", "PhysicalStock", "PlayersCount", "ShortDescription", "Slug", "Title" },
                values: new object[,]
                {
                    { 1, "#511/B", "/images/logo.png", new DateTime(2026, 11, 1, 0, 0, 0, 0, DateTimeKind.Utc), "صعب", 8.00m, "2 - 3 ساعات", "عُثر على الضحية في ظروف مريبة تبدو للوهلة الأولى كوفاة طبيعية، لكن تقرير الطب الشرعي والسموم يكشف عن مادة نادرة لم تترك أثراً واضحاً. مهمتك كمحقق هي مراجعة إفادات المشتبه بهم، تدقيق سجل المكالمات، فك الشيفرات الرقمية، وكشف التناقض الزمني الذي يفضح القاتل الحقيقي.", true, true, true, true, "اشترِ ملف قضية السم النائم من بوارو (Poirot) بنسخة ورقية مطبوعة أو نسخة إلكترونية فورية وحل لغز الجريمة.", "قضية السم النائم, بوارو, لعبة تحقيقات جنائية, ملفات قضايا", "قضية السم النائم (#511/B) | لعبة تحقيقات جنائية من بوارو", 15.00m, 25, "1 - 6 محققين", "جريمة غامضة في أمسية هادئة؛ تقارير سموم متضاربة، مكالمة متأخرة، وشهود يخفون أكثر مما يبوحون به.", "al-sam-al-naem", "قضية السم النائم" },
                    { 2, "#512/A", "/images/logo.png", new DateTime(2026, 11, 10, 0, 0, 0, 0, DateTimeKind.Utc), "متوسط", 6.00m, "90 - 120 دقيقة", "داخل قصر مغلق الأبواب والنوافذ، اجتمع خمسة ورثة لكل منهم دافع قوي. بين خريطة القصر، إيصالات مشبوهة، ورسالة مشفرة تُركت في المكتب، هل ستتمكن من تحديد من تحرك في الظلام قبل فوات الأوان؟", true, true, true, true, "تحدَّ أصدقاءك في حل لغز القصر الصامت من بوارو Poirot - متوفرة بنسخة ورقية وإلكترونية.", "لغز القصر الصامت, ألعاب غموض, بوارو Poirot, قضايا ورقية", "لغز القصر الصامت (#512/A) | ألعاب بوارو للتحقيق الجنائي", 12.00m, 15, "2 - 5 محققين", "انقطاع مفاجئ للكهرباء أثناء عشاء عائلي لورثة ثري، وبعد عودة الإضاءة تختفي الوصية ويُسجّل بلاغ جريمة.", "lughz-al-qasr-al-samit", "لغز القصر الصامت" }
                });

            migrationBuilder.InsertData(
                table: "CaseGalleries",
                columns: new[] { "Id", "Caption", "CrimeCaseId", "ImageUrl", "SortOrder" },
                values: new object[,]
                {
                    { 1, "ملف قضية السم النائم - الظرف السري", 1, "/images/logo.png", 1 },
                    { 2, "ملف لغز القصر الصامت - مخطط القصر", 2, "/images/logo.png", 1 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_CaseGalleries_CrimeCaseId",
                table: "CaseGalleries",
                column: "CrimeCaseId");

            migrationBuilder.CreateIndex(
                name: "IX_CrimeCases_Slug",
                table: "CrimeCases",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_CrimeCaseId",
                table: "OrderItems",
                column: "CrimeCaseId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_OrderId",
                table: "OrderItems",
                column: "OrderId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CaseGalleries");

            migrationBuilder.DropTable(
                name: "OrderItems");

            migrationBuilder.DropTable(
                name: "CrimeCases");

            migrationBuilder.DropTable(
                name: "Orders");
        }
    }
}
