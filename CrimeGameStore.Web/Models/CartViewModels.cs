using System.ComponentModel.DataAnnotations;

namespace CrimeGameStore.Web.Models
{
    // عنصر واحد داخل السلة (يُحفظ كـ JSON داخل Session)
    public class CartItem
    {
        public int CrimeCaseId { get; set; }
        public string CaseCode { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string CoverImageUrl { get; set; } = "/images/logo.png";
        public EditionType Edition { get; set; } // Physical = 1, Digital = 2
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; } = 1;

        public decimal LineTotal => UnitPrice * Quantity;
        public string CurrencySymbol => Edition == EditionType.Physical ? "ل.س" : "$";
        public string FormattedUnitPrice => Edition == EditionType.Physical
            ? $"{UnitPrice:N0} ل.س"
            : $"{UnitPrice:0.##} $";
        public string FormattedLineTotal => Edition == EditionType.Physical
            ? $"{LineTotal:N0} ل.س"
            : $"{LineTotal:0.##} $";
    }

    // نموذج صفحة عرض السلة
    public class CartViewModel
    {
        public List<CartItem> Items { get; set; } = new();
        public decimal TotalAmount => Items.Sum(i => i.LineTotal);
        public int TotalItemsCount => Items.Sum(i => i.Quantity);
        public bool HasPhysicalItems => Items.Any(i => i.Edition == EditionType.Physical);
        public bool HasDigitalItems => Items.Any(i => i.Edition == EditionType.Digital);
        public decimal TotalSyp => Items
           .Where(i => i.Edition == EditionType.Physical)
           .Sum(i => i.LineTotal);
        public decimal TotalUsd => Items
            .Where(i => i.Edition == EditionType.Digital)
            .Sum(i => i.LineTotal);
    }

    // نموذج صفحة إتمام الطلب (Checkout)
    public class CheckoutViewModel
    {
        [Required(ErrorMessage = "يرجى إدخال الاسم الكامل")]
        [MaxLength(120)]
        [Display(Name = "الاسم الكامل")]
        public string CustomerName { get; set; } = string.Empty;

        [Required(ErrorMessage = "يرجى إدخال رقم الهاتف أو الواتساب للتواصل")]
        [MaxLength(30)]
        [Display(Name = "رقم الهاتف / واتساب")]
        public string PhoneNumber { get; set; } = string.Empty;

        [EmailAddress(ErrorMessage = "يرجى إدخال بريد إلكتروني صحيح")]
        [MaxLength(150)]
        [Display(Name = "البريد الإلكتروني (اختياري)")]
        public string? Email { get; set; }

        // حقول الشحن (تُطلب وتُفحص برمجياً فقط إذا كانت السلة تحتوي على نسخة ورقية)
        [MaxLength(80)]
        [Display(Name = "المحافظة / المدينة")]
        public string? City { get; set; }

        [MaxLength(350)]
        [Display(Name = "العنوان التفصيلي للشحن")]
        public string? ShippingAddress { get; set; }

        [MaxLength(500)]
        [Display(Name = "ملاحظات إضافية (اختياري)")]
        public string? Notes { get; set; }

        [Display(Name = "طريقة الدفع")]
        public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.ShamCashDepositAndCod;

        [MaxLength(120)]
        [Display(Name = "رقم عملية التحويل / اسم الحساب على شام كاش (اختياري)")]
        public string? PaymentReference { get; set; }

        // ملخص السلة للعرض بجانب الفورم
        public CartViewModel Cart { get; set; } = new();
    }
}