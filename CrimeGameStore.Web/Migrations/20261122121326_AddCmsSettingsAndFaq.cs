using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CrimeGameStore.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddCmsSettingsAndFaq : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FaqItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Question = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Answer = table.Column<string>(type: "nvarchar(1500)", maxLength: 1500, nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FaqItems", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SiteSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SiteName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SiteSubtitle = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    TopAnnouncement = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    LogoUrl = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    FooterAbout = table.Column<string>(type: "nvarchar(600)", maxLength: 600, nullable: false),
                    PhoneDisplay = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    WhatsAppNumber = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    ShamCashAccount = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    DepositNote = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    HeroBadge = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    HeroTitle = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: false),
                    HeroHighlight = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    HeroDescription = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    HomeCtaTitle = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    HomeCtaDescription = table.Column<string>(type: "nvarchar(400)", maxLength: 400, nullable: false),
                    DefaultMetaDescription = table.Column<string>(type: "nvarchar(320)", maxLength: 320, nullable: false),
                    DefaultMetaKeywords = table.Column<string>(type: "nvarchar(350)", maxLength: 350, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SiteSettings", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "FaqItems",
                columns: new[] { "Id", "Answer", "IsActive", "Question", "SortOrder" },
                values: new object[,]
                {
                    { 1, "لا تحتاج للإنترنت أثناء فحص الأوراق وتحليل الأدلة؛ ستحتاج اتصالاً بسيطاً بالهاتف فقط في حال رغبت بمسح رمز QR لطلب تلميح أو التأكد من إجابتك النهائية في بوابة المحكمة.", true, "هل أحتاج للإنترنت أثناء اللعب؟", 1 },
                    { 2, "تتراوح مدة التحقيق عادةً بين ساعة ونصف إلى 3 ساعات حسب مستوى صعوبة القضية وعدد المحققين المشاركين، ويمكنك إيقاف اللعب واستكماله في وقت لاحق بكل حرية.", true, "كم يستغرق حل القضية الواحدة؟", 2 },
                    { 3, "لا تقلق أبداً! صممنا في بوابة الحلول الخاصة بنا نظام تلميحات تدريجي لكل خطوة في القضية، يفتح لك طرف الخيط دون أن يحرق عليك هوية القاتل أو يفسد متعة التحقيق.", true, "ماذا لو علقنا عند لغز أو شيفرة ولم نعرف كيف نكمل؟", 3 },
                    { 4, "نعم بالتأكيد! جميع مستندات القضية قابلة لإعادة الاستخدام بالكامل ما لم تكتب عليها مباشرة (ننصح باستخدام ورقة ملاحظات جانبية أثناء التحقيق).", true, "هل يمكنني تمرير النسخة الورقية لأصدقائي بعد الانتهاء منها؟", 4 },
                    { 5, "تُصمم القضايا عادةً لعمر 14 سنة فما فوق، لأنها تعتمد على التحليل المنطقي، القراءة الدقيقة، وربط التناقضات الزمنية، مع الحرص على أن تكون محتوياتها ملائمة تماماً للجلسات العائلية وجلسات الأصدقاء.", true, "ما هو العمر المناسب للعب قضايا بوارو؟", 5 }
                });

            migrationBuilder.InsertData(
                table: "SiteSettings",
                columns: new[] { "Id", "DefaultMetaDescription", "DefaultMetaKeywords", "DepositNote", "FooterAbout", "HeroBadge", "HeroDescription", "HeroHighlight", "HeroTitle", "HomeCtaDescription", "HomeCtaTitle", "LogoUrl", "PhoneDisplay", "ShamCashAccount", "SiteName", "SiteSubtitle", "TopAnnouncement", "WhatsAppNumber" },
                values: new object[] { 1, "بوارو (Poirot) لألعاب التحقيقات الجنائية والقضايا الغامضة. اطلب ملف القضية بنسخة ورقية مطبوعة أو نسخة إلكترونية فورية وابدأ التحقيق مع أصدقائك.", "بوارو, Poirot, لعبة تحقيقات جنائية, ألعاب بوارو, قضايا تحقيق ورقية, ألعاب ذكاء وغموض, حل جرائم", "رعبون تثبيت للطلب الورقي (يُخصم من إجمالي الفاتورة والباقي عند الاستلام)", "علامة متخصصة في صناعة قضايا التحقيق الجنائي الواقعية. نضع بين يديك تقارير طب شرعي، محاضر استجواب، خرائط مسرح جريمة، وأدلة مادية لتعيش مع أصدقائك تجربة المحقق الحقيقي.", "ملفات جنائية مغلقة تنتظر محققاً ذكياً", "في بوارو (Poirot)، نضع بين يديك ملف قضية متكاملاً يضم تقارير الطب الشرعي، إفادات المشتبه بهم، الشيفرات، والصور. اجمع الخيوط، اكتشف التناقض، وأدِن القاتل الحقيقي.", "عِش تفاصيل التحقيق بنفسك", "لا تقرأ الرواية البوليسية فقط..", "تصفح القضايا المتاحة الآن أو اطلع على خطوات اللعب بالتفصيل قبل بدء مهمتك الأولى.", "جاهز لاختبار قوة ملاحظتك مع فريقك؟", "/images/logo.png", "0995 775 317", "fbb86878018a1d41050dbfc85b135746", "بوارو | POIROT", "ألعاب التحقيقات الجنائية", "قضايا بوارو (Poirot) متوفرة بخيارين: ملف ورقي مطبوع يصلك للمنزل أو نسخة إلكترونية فورية", "963995775317" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FaqItems");

            migrationBuilder.DropTable(
                name: "SiteSettings");
        }
    }
}
