using System.ComponentModel.DataAnnotations;

namespace CrimeGameStore.Web.Models
{
    public class SiteSetting
    {
        public int Id { get; set; }

        // 1. الهوية العامة والهيدر والفوتر
        [Required, MaxLength(100)]
        [Display(Name = "اسم الموقع")]
        public string SiteName { get; set; } = "بوارو | POIROT";

        [Required, MaxLength(120)]
        [Display(Name = "الوصف المختصر تحت الشعار")]
        public string SiteSubtitle { get; set; } = "ألعاب التحقيقات الجنائية";

        [Required, MaxLength(300)]
        [Display(Name = "نص الشريط الإعلاني العلوي")]
        public string TopAnnouncement { get; set; } = "قضايا بوارو (Poirot) متوفرة بخيارين: ملف ورقي مطبوع يصلك للمنزل أو نسخة إلكترونية فورية";

        [Required, MaxLength(250)]
        [Display(Name = "مسار صورة الشعار (Logo)")]
        public string LogoUrl { get; set; } = "/images/logo.png";

        [Required, MaxLength(600)]
        [Display(Name = "نبذة عن بوارو في التذييل (Footer)")]
        public string FooterAbout { get; set; } = "علامة متخصصة في صناعة قضايا التحقيق الجنائي الواقعية. نضع بين يديك تقارير طب شرعي، محاضر استجواب، خرائط مسرح جريمة، وأدلة مادية لتعيش مع أصدقائك تجربة المحقق الحقيقي.";

        // 2. بيانات التواصل وشام كاش
        [Required, MaxLength(40)]
        [Display(Name = "رقم الهاتف المعروض")]
        public string PhoneDisplay { get; set; } = "09XX XXX XXX";

        [Required, MaxLength(30)]
        [Display(Name = "رقم الواتساب الدولي (بدون +)")]
        public string WhatsAppNumber { get; set; } = "9639XXXXXXXX";

        [MaxLength(250)]
        [Display(Name = "رابط صفحة إنستغرام")]
        public string? InstagramUrl { get; set; } = "https://instagram.com/";

        [MaxLength(250)]
        [Display(Name = "رابط صفحة فيسبوك")]
        public string? FacebookUrl { get; set; } = "https://facebook.com/";

        [Required, MaxLength(200)]
        [Display(Name = "حساب أو معرّف شام كاش (يُستخدم أيضاً لرسم الباركود)")]
        public string ShamCashAccount { get; set; } = "حساب-شام-كاش-هنا";

        [Required, MaxLength(300)]
        [Display(Name = "ملاحظة رعبون النسخة الورقية")]
        public string DepositNote { get; set; } = "رعبون تثبيت للطلب الورقي عبر شام كاش (يُخصم من إجمالي الفاتورة والباقي عند الاستلام)";

        // 3. نصوص الصفحة الرئيسية (Hero Section & CTA)
        [Required, MaxLength(120)]
        [Display(Name = "شارة البانر الرئيسية")]
        public string HeroBadge { get; set; } = "ملفات جنائية مغلقة تنتظر محققاً ذكياً";

        [Required, MaxLength(160)]
        [Display(Name = "العنوان الرئيسي (الجزء الأول)")]
        public string HeroTitle { get; set; } = "لا تقرأ الرواية البوليسية فقط..";

        [Required, MaxLength(120)]
        [Display(Name = "العنوان الرئيسي (الجزء المميّز)")]
        public string HeroHighlight { get; set; } = "عِش تفاصيل التحقيق بنفسك";

        [Required, MaxLength(500)]
        [Display(Name = "الوصف التعريفي في البانر")]
        public string HeroDescription { get; set; } = "في بوارو (Poirot)، نضع بين يديك ملف قضية متكاملاً يضم تقارير الطب الشرعي، إفادات المشتبه بهم، الشيفرات، والصور. اجمع الخيوط، اكتشف التناقض، وأدِن القاتل الحقيقي.";

        [Required, MaxLength(200)]
        [Display(Name = "عنوان قسم الدعوة للتحقيق أسفل الرئيسية")]
        public string HomeCtaTitle { get; set; } = "جاهز لاختبار قوة ملاحظتك مع فريقك؟";

        [Required, MaxLength(400)]
        [Display(Name = "وصف قسم الدعوة للتحقيق")]
        public string HomeCtaDescription { get; set; } = "تصفح القضايا المتاحة الآن أو اطلع على خطوات اللعب بالتفصيل قبل بدء مهمتك الأولى.";

        // 4. إعدادات السيو الافتراضية للموقع
        [Required, MaxLength(320)]
        [Display(Name = "وصف السيو الافتراضي (Meta Description)")]
        public string DefaultMetaDescription { get; set; } = "بوارو (Poirot) لألعاب التحقيقات الجنائية والقضايا الغامضة. اطلب ملف القضية بنسخة ورقية مطبوعة أو نسخة إلكترونية فورية وابدأ التحقيق مع أصدقائك.";

        [Required, MaxLength(350)]
        [Display(Name = "الكلمات المفتاحية الافتراضية (Meta Keywords)")]
        public string DefaultMetaKeywords { get; set; } = "بوارو, Poirot, لعبة تحقيقات جنائية, ألعاب بوارو, قضايا تحقيق ورقية, ألعاب ذكاء وغموض, حل جرائم";
    }
}