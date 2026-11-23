using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CrimeGameStore.Web.Models
{
    public enum EditionType
    {
        Physical = 1, // نسخة ورقية (شحن)
        Digital = 2   // نسخة إلكترونية (PDF فوري)
    }

    public enum OrderStatus
    {
        Pending = 1,      // طلب جديد بانتظار التأكيد
        Confirmed = 2,    // تم التأكيد والتجهيز
        Shipped = 3,      // قيد التوصيل (للنسخ الورقية)
        Completed = 4,    // مكتمل / تم التسليم
        Cancelled = 5     // ملغي
    }

    public enum PaymentMethod
    {
        ShamCashDepositAndCod = 1, // رعبون عبر شام كاش + الباقي عند الاستلام (للورقي)
        ShamCashFull = 2           // الدفع الكامل عبر شام كاش (للورقي أو الرقمي)
    }

    public class Order
    {
        public int Id { get; set; }

        [Required, MaxLength(30)]
        public string OrderNumber { get; set; } = string.Empty; // مثال: PRT-20261121-001

        [Required(ErrorMessage = "الاسم الكامل مطلوب"), MaxLength(120)]
        [Display(Name = "اسم المشتري")]
        public string CustomerName { get; set; } = string.Empty;

        [Required(ErrorMessage = "رقم الهاتف / واتساب مطلوب"), MaxLength(30)]
        [Display(Name = "رقم الهاتف (واتساب)")]
        public string PhoneNumber { get; set; } = string.Empty;

        [EmailAddress, MaxLength(150)]
        [Display(Name = "البريد الإلكتروني")]
        public string? Email { get; set; }

        // حقول الشحن (اختيارية في حال كان الطلب إلكترونياً فقط)
        [MaxLength(80)]
        [Display(Name = "المحافظة / المدينة")]
        public string? City { get; set; }

        [MaxLength(350)]
        [Display(Name = "العنوان التفصيلي للشحن")]
        public string? ShippingAddress { get; set; }

        [MaxLength(500)]
        [Display(Name = "ملاحظات إضافية")]
        public string? Notes { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "المبلغ الإجمالي")]
        public decimal TotalAmount { get; set; }

        [Display(Name = "طريقة الدفع")]
        public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.ShamCashFull;

        [MaxLength(120)]
        [Display(Name = "رقم عملية شام كاش أو اسم المرسل")]
        public string? PaymentReference { get; set; }

        public OrderStatus Status { get; set; } = OrderStatus.Pending;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // تفاصيل المنتجات داخل الطلب
        public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
        [NotMapped]
        public decimal TotalSyp => Items
            .Where(i => i.Edition == EditionType.Physical)
            .Sum(i => i.UnitPrice * i.Quantity);

        [NotMapped]
        public decimal TotalUsd => Items
            .Where(i => i.Edition == EditionType.Digital)
            .Sum(i => i.UnitPrice * i.Quantity);
    }
}