using CrimeGameStore.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace CrimeGameStore.Web.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<CrimeCase> CrimeCases => Set<CrimeCase>();
        public DbSet<CaseGallery> CaseGalleries => Set<CaseGallery>();
        public DbSet<Order> Orders => Set<Order>();
        public DbSet<OrderItem> OrderItems => Set<OrderItem>();
        public DbSet<SiteSetting> SiteSettings => Set<SiteSetting>();
        public DbSet<FaqItem> FaqItems => Set<FaqItem>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // منع تكرار الـ Slug لضمان قوة الروابط في السيو
            modelBuilder.Entity<CrimeCase>()
                .HasIndex(c => c.Slug)
                .IsUnique();

            // علاقة القضية بمعرض الصور (عند حذف قضية تُحذف صورها)
            modelBuilder.Entity<CaseGallery>()
                .HasOne(g => g.CrimeCase)
                .WithMany(c => c.GalleryImages)
                .HasForeignKey(g => g.CrimeCaseId)
                .OnDelete(DeleteBehavior.Cascade);

            // علاقة الطلب بعناصر الطلب
            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Order)
                .WithMany(o => o.Items)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            // منع حذف قضية مرتبطة بطلبات قديمة لحفظ سجلات المبيعات
            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.CrimeCase)
                .WithMany(c => c.OrderItems)
                .HasForeignKey(oi => oi.CrimeCaseId)
                .OnDelete(DeleteBehavior.Restrict);

            // ==========================================
            // بيانات تجريبية أولية للقضايا (Data Seeding)
            // ==========================================
            modelBuilder.Entity<CrimeCase>().HasData(
                new CrimeCase
                {
                    Id = 1,
                    CaseCode = "#511/B",
                    Title = "قضية السم النائم",
                    Slug = "al-sam-al-naem",
                    ShortDescription = "جريمة غامضة في أمسية هادئة؛ تقارير سموم متضاربة، مكالمة متأخرة، وشهود يخفون أكثر مما يبوحون به.",
                    FullStory = "عُثر على الضحية في ظروف مريبة تبدو للوهلة الأولى كوفاة طبيعية، لكن تقرير الطب الشرعي والسموم يكشف عن مادة نادرة لم تترك أثراً واضحاً. مهمتك كمحقق هي مراجعة إفادات المشتبه بهم، تدقيق سجل المكالمات، فك الشيفرات الرقمية، وكشف التناقض الزمني الذي يفضح القاتل الحقيقي.",
                    Difficulty = "صعب",
                    PlayersCount = "1 - 6 محققين",
                    EstimatedTime = "2 - 3 ساعات",
                    CoverImageUrl = "/images/logo.png",
                    PhysicalPrice = 15.00m,
                    PhysicalStock = 25,
                    IsPhysicalAvailable = true,
                    DigitalPrice = 8.00m,
                    IsDigitalAvailable = true,
                    MetaTitle = "قضية السم النائم (#511/B) | لعبة تحقيقات جنائية من بوارو",
                    MetaDescription = "اشترِ ملف قضية السم النائم من بوارو (Poirot) بنسخة ورقية مطبوعة أو نسخة إلكترونية فورية وحل لغز الجريمة.",
                    MetaKeywords = "قضية السم النائم, بوارو, لعبة تحقيقات جنائية, ملفات قضايا",
                    IsFeatured = true,
                    IsActive = true,
                    CreatedAt = new DateTime(2026, 11, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new CrimeCase
                {
                    Id = 2,
                    CaseCode = "#512/A",
                    Title = "لغز القصر الصامت",
                    Slug = "lughz-al-qasr-al-samit",
                    ShortDescription = "انقطاع مفاجئ للكهرباء أثناء عشاء عائلي لورثة ثري، وبعد عودة الإضاءة تختفي الوصية ويُسجّل بلاغ جريمة.",
                    FullStory = "داخل قصر مغلق الأبواب والنوافذ، اجتمع خمسة ورثة لكل منهم دافع قوي. بين خريطة القصر، إيصالات مشبوهة، ورسالة مشفرة تُركت في المكتب، هل ستتمكن من تحديد من تحرك في الظلام قبل فوات الأوان؟",
                    Difficulty = "متوسط",
                    PlayersCount = "2 - 5 محققين",
                    EstimatedTime = "90 - 120 دقيقة",
                    CoverImageUrl = "/images/logo.png",
                    PhysicalPrice = 12.00m,
                    PhysicalStock = 15,
                    IsPhysicalAvailable = true,
                    DigitalPrice = 6.00m,
                    IsDigitalAvailable = true,
                    MetaTitle = "لغز القصر الصامت (#512/A) | ألعاب بوارو للتحقيق الجنائي",
                    MetaDescription = "تحدَّ أصدقاءك في حل لغز القصر الصامت من بوارو Poirot - متوفرة بنسخة ورقية وإلكترونية.",
                    MetaKeywords = "لغز القصر الصامت, ألعاب غموض, بوارو Poirot, قضايا ورقية",
                    IsFeatured = true,
                    IsActive = true,
                    CreatedAt = new DateTime(2026, 11, 10, 0, 0, 0, DateTimeKind.Utc)
                }
            );

            modelBuilder.Entity<CaseGallery>().HasData(
                new CaseGallery
                {
                    Id = 1,
                    CrimeCaseId = 1,
                    ImageUrl = "/images/logo.png",
                    Caption = "ملف قضية السم النائم - الظرف السري",
                    SortOrder = 1
                },
                new CaseGallery
                {
                    Id = 2,
                    CrimeCaseId = 2,
                    ImageUrl = "/images/logo.png",
                    Caption = "ملف لغز القصر الصامت - مخطط القصر",
                    SortOrder = 1
                }
            );

            // ==========================================
            // إعدادات ومحتوى الموقع (قابلة للتعديل بالكامل من الأدمن)
            // ==========================================
            modelBuilder.Entity<SiteSetting>().HasData(new SiteSetting
            {
                Id = 1,
                SiteName = "بوارو | POIROT",
                SiteSubtitle = "ألعاب التحقيقات الجنائية",
                TopAnnouncement = "قضايا بوارو (Poirot) متوفرة بخيارين: ملف ورقي مطبوع يصلك للمنزل أو نسخة إلكترونية فورية",
                LogoUrl = "/images/logo.png",
                FooterAbout = "علامة متخصصة في صناعة قضايا التحقيق الجنائي الواقعية. نضع بين يديك تقارير طب شرعي، محاضر استجواب، خرائط مسرح جريمة، وأدلة مادية لتعيش مع أصدقائك تجربة المحقق الحقيقي.",
                PhoneDisplay = "0995 775 317",
                WhatsAppNumber = "963995775317",
                ShamCashAccount = "fbb86878018a1d41050dbfc85b135746",
                DepositNote = "رعبون تثبيت للطلب الورقي (يُخصم من إجمالي الفاتورة والباقي عند الاستلام)",
                HeroBadge = "ملفات جنائية مغلقة تنتظر محققاً ذكياً",
                HeroTitle = "لا تقرأ الرواية البوليسية فقط..",
                HeroHighlight = "عِش تفاصيل التحقيق بنفسك",
                HeroDescription = "في بوارو (Poirot)، نضع بين يديك ملف قضية متكاملاً يضم تقارير الطب الشرعي، إفادات المشتبه بهم، الشيفرات، والصور. اجمع الخيوط، اكتشف التناقض، وأدِن القاتل الحقيقي.",
                HomeCtaTitle = "جاهز لاختبار قوة ملاحظتك مع فريقك؟",
                HomeCtaDescription = "تصفح القضايا المتاحة الآن أو اطلع على خطوات اللعب بالتفصيل قبل بدء مهمتك الأولى.",
                DefaultMetaDescription = "بوارو (Poirot) لألعاب التحقيقات الجنائية والقضايا الغامضة. اطلب ملف القضية بنسخة ورقية مطبوعة أو نسخة إلكترونية فورية وابدأ التحقيق مع أصدقائك.",
                DefaultMetaKeywords = "بوارو, Poirot, لعبة تحقيقات جنائية, ألعاب بوارو, قضايا تحقيق ورقية, ألعاب ذكاء وغموض, حل جرائم",
                InstagramUrl = "https://instagram.com/",
                FacebookUrl = "https://facebook.com/"
            });

            // ==========================================
            // الأسئلة الشائعة الأولية (قابلة للإضافة والتعديل والحذف من الأدمن)
            // ==========================================
            modelBuilder.Entity<FaqItem>().HasData(
                new FaqItem
                {
                    Id = 1,
                    SortOrder = 1,
                    IsActive = true,
                    Question = "هل أحتاج للإنترنت أثناء اللعب؟",
                    Answer = "لا تحتاج للإنترنت أثناء فحص الأوراق وتحليل الأدلة؛ ستحتاج اتصالاً بسيطاً بالهاتف فقط في حال رغبت بمسح رمز QR لطلب تلميح أو التأكد من إجابتك النهائية في بوابة المحكمة."
                },
                new FaqItem
                {
                    Id = 2,
                    SortOrder = 2,
                    IsActive = true,
                    Question = "كم يستغرق حل القضية الواحدة؟",
                    Answer = "تتراوح مدة التحقيق عادةً بين ساعة ونصف إلى 3 ساعات حسب مستوى صعوبة القضية وعدد المحققين المشاركين، ويمكنك إيقاف اللعب واستكماله في وقت لاحق بكل حرية."
                },
                new FaqItem
                {
                    Id = 3,
                    SortOrder = 3,
                    IsActive = true,
                    Question = "ماذا لو علقنا عند لغز أو شيفرة ولم نعرف كيف نكمل؟",
                    Answer = "لا تقلق أبداً! صممنا في بوابة الحلول الخاصة بنا نظام تلميحات تدريجي لكل خطوة في القضية، يفتح لك طرف الخيط دون أن يحرق عليك هوية القاتل أو يفسد متعة التحقيق."
                },
                new FaqItem
                {
                    Id = 4,
                    SortOrder = 4,
                    IsActive = true,
                    Question = "هل يمكنني تمرير النسخة الورقية لأصدقائي بعد الانتهاء منها؟",
                    Answer = "نعم بالتأكيد! جميع مستندات القضية قابلة لإعادة الاستخدام بالكامل ما لم تكتب عليها مباشرة (ننصح باستخدام ورقة ملاحظات جانبية أثناء التحقيق)."
                },
                new FaqItem
                {
                    Id = 5,
                    SortOrder = 5,
                    IsActive = true,
                    Question = "ما هو العمر المناسب للعب قضايا بوارو؟",
                    Answer = "تُصمم القضايا عادةً لعمر 14 سنة فما فوق، لأنها تعتمد على التحليل المنطقي، القراءة الدقيقة، وربط التناقضات الزمنية، مع الحرص على أن تكون محتوياتها ملائمة تماماً للجلسات العائلية وجلسات الأصدقاء."
                }
            );
        }
    }
}