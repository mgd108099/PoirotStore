using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CrimeGameStore.Web.Models
{
    public class CrimeCase
    {
        public int Id { get; set; }

        [Required, MaxLength(30)]
        [Display(Name = "رمز القضية")]
        public string CaseCode { get; set; } = string.Empty; // مثال: #511/B

        [Required, MaxLength(150)]
        [Display(Name = "عنوان القضية")]
        public string Title { get; set; } = string.Empty;

        [Required, MaxLength(160)]
        [Display(Name = "الرابط الصديق لمحركات البحث (Slug)")]
        public string Slug { get; set; } = string.Empty; // مثال: al-sam-al-naem

        [Required, MaxLength(350)]
        [Display(Name = "وصف مختصر")]
        public string ShortDescription { get; set; } = string.Empty;

        [Required]
        [Display(Name = "ملخص القضية (بدون حرق)")]
        public string FullStory { get; set; } = string.Empty;

        [Required, MaxLength(50)]
        [Display(Name = "مستوى الصعوبة")]
        public string Difficulty { get; set; } = "متوسط"; // سهل، متوسط، صعب، للمحترفين

        [Required, MaxLength(50)]
        [Display(Name = "عدد المحققين")]
        public string PlayersCount { get; set; } = "1 - 5 محققين";

        [Required, MaxLength(50)]
        [Display(Name = "المدة المتوقعة")]
        public string EstimatedTime { get; set; } = "2 - 3 ساعات";

        [Required, MaxLength(250)]
        [Display(Name = "صورة الغلاف")]
        public string CoverImageUrl { get; set; } = "/images/logo.png";

        // تسعير ومخزون النسخة الورقية
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "سعر النسخة الورقية")]
        public decimal PhysicalPrice { get; set; }

        [Display(Name = "الكمية المتوفرة (ورقي)")]
        public int PhysicalStock { get; set; }

        public bool IsPhysicalAvailable { get; set; } = true;

        // تسعير النسخة الإلكترونية
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "سعر النسخة الإلكترونية")]
        public decimal DigitalPrice { get; set; }

        public bool IsDigitalAvailable { get; set; } = true;

        // حقول السيو المخصصة لكل قضية
        [MaxLength(160)]
        public string? MetaTitle { get; set; }

        [MaxLength(320)]
        public string? MetaDescription { get; set; }

        [MaxLength(300)]
        public string? MetaKeywords { get; set; }

        public bool IsFeatured { get; set; } = false;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // العلاقات
        public ICollection<CaseGallery> GalleryImages { get; set; } = new List<CaseGallery>();
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}