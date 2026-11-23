using CrimeGameStore.Web.Data;
using CrimeGameStore.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace CrimeGameStore.Web.Controllers
{
    [Route("cart")]
    public class CartController : Controller
    {
        private readonly AppDbContext _context;
        private const string CartSessionKey = "PoirotCartItems";

        public CartController(AppDbContext context)
        {
            _context = context;
        }

        // دالة مساعدة لقراءة السلة من الـ Session
        private List<CartItem> GetCartItems()
        {
            var json = HttpContext.Session.GetString(CartSessionKey);
            if (string.IsNullOrEmpty(json))
                return new List<CartItem>();

            return JsonSerializer.Deserialize<List<CartItem>>(json) ?? new List<CartItem>();
        }

        // دالة مساعدة لحفظ السلة في الـ Session
        private void SaveCartItems(List<CartItem> items)
        {
            var json = JsonSerializer.Serialize(items);
            HttpContext.Session.SetString(CartSessionKey, json);
        }

        // GET: /cart/count (لتحديث عداد الحقيبة في الشريط العلوي عبر JS)
        [HttpGet("count")]
        public IActionResult GetCount()
        {
            var count = GetCartItems().Sum(i => i.Quantity);
            return Json(new { count });
        }

        // GET: /cart (صفحة عرض حقيبة القضايا)
        [HttpGet("")]
        public IActionResult Index()
        {
            var vm = new CartViewModel { Items = GetCartItems() };
            return View(vm);
        }

        // POST: /cart/add (تستقبل الطلب من زر الإضافة في صفحة تفاصيل القضية)
        [HttpPost("add")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(int caseId, int edition, int quantity = 1)
        {
            var crimeCase = await _context.CrimeCases
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == caseId && c.IsActive);

            if (crimeCase == null)
                return NotFound();

            var editionType = edition == (int)EditionType.Digital
                ? EditionType.Digital
                : EditionType.Physical;

            if (quantity < 1) quantity = 1;

            // التحقق من التوفر والمخزون
            if (editionType == EditionType.Physical)
            {
                if (!crimeCase.IsPhysicalAvailable || crimeCase.PhysicalStock <= 0)
                {
                    TempData["CartError"] = "عذراً، النسخة الورقية من هذه القضية غير متوفرة حالياً.";
                    return RedirectToAction("Details", "Cases", new { slug = crimeCase.Slug });
                }
            }
            else
            {
                if (!crimeCase.IsDigitalAvailable)
                {
                    TempData["CartError"] = "عذراً، النسخة الإلكترونية من هذه القضية غير متوفرة حالياً.";
                    return RedirectToAction("Details", "Cases", new { slug = crimeCase.Slug });
                }
                // النسخة الإلكترونية تُضاف بكمية 1 دائماً
                quantity = 1;
            }

            var items = GetCartItems();
            var existingItem = items.FirstOrDefault(i => i.CrimeCaseId == caseId && i.Edition == editionType);

            if (existingItem != null)
            {
                if (editionType == EditionType.Physical)
                {
                    existingItem.Quantity = Math.Min(existingItem.Quantity + quantity, crimeCase.PhysicalStock);
                }
            }
            else
            {
                items.Add(new CartItem
                {
                    CrimeCaseId = crimeCase.Id,
                    CaseCode = crimeCase.CaseCode,
                    Title = crimeCase.Title,
                    Slug = crimeCase.Slug,
                    CoverImageUrl = crimeCase.CoverImageUrl,
                    Edition = editionType,
                    UnitPrice = editionType == EditionType.Physical ? crimeCase.PhysicalPrice : crimeCase.DigitalPrice,
                    Quantity = editionType == EditionType.Physical ? Math.Min(quantity, crimeCase.PhysicalStock) : 1
                });
            }

            SaveCartItems(items);
            TempData["CartSuccess"] = $"تمت إضافة «{crimeCase.Title}» إلى حقيبة القضايا بنجاح.";
            return RedirectToAction(nameof(Index));
        }

        // POST: /cart/update (زيادة أو إنقاص كمية النسخة الورقية)
        [HttpPost("update")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateQuantity(int caseId, EditionType edition, int delta)
        {
            var items = GetCartItems();
            var item = items.FirstOrDefault(i => i.CrimeCaseId == caseId && i.Edition == edition);

            if (item != null && edition == EditionType.Physical)
            {
                var crimeCase = await _context.CrimeCases.AsNoTracking().FirstOrDefaultAsync(c => c.Id == caseId);
                int maxStock = crimeCase?.PhysicalStock ?? 1;

                int newQty = item.Quantity + delta;
                if (newQty <= 0)
                {
                    items.Remove(item);
                }
                else
                {
                    item.Quantity = Math.Min(newQty, maxStock);
                }

                SaveCartItems(items);
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: /cart/remove (حذف عنصر من الحقيبة)
        [HttpPost("remove")]
        [ValidateAntiForgeryToken]
        public IActionResult Remove(int caseId, EditionType edition)
        {
            var items = GetCartItems();
            var item = items.FirstOrDefault(i => i.CrimeCaseId == caseId && i.Edition == edition);
            if (item != null)
            {
                items.Remove(item);
                SaveCartItems(items);
            }
            return RedirectToAction(nameof(Index));
        }

        // POST: /cart/clear (إفراغ الحقيبة)
        [HttpPost("clear")]
        [ValidateAntiForgeryToken]
        public IActionResult Clear()
        {
            HttpContext.Session.Remove(CartSessionKey);
            return RedirectToAction(nameof(Index));
        }

        // GET: /cart/checkout (صفحة إتمام الطلب)
        [HttpGet("checkout")]
        public IActionResult Checkout()
        {
            var items = GetCartItems();
            if (!items.Any())
            {
                TempData["CartError"] = "حقيبة القضايا فارغة، يرجى إضافة قضية أولاً.";
                return RedirectToAction(nameof(Index));
            }

            var cartVm = new CartViewModel { Items = items };
            var vm = new CheckoutViewModel
            {
                Cart = cartVm,
                PaymentMethod = cartVm.HasPhysicalItems
                    ? PaymentMethod.ShamCashDepositAndCod
                    : PaymentMethod.ShamCashFull
            };

            return View(vm);
        }

        // POST: /cart/checkout (التحقق وحفظ الطلب في SQL Server)
        [HttpPost("checkout")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(CheckoutViewModel model)
        {
            var cartItems = GetCartItems();
            if (!cartItems.Any())
            {
                TempData["CartError"] = "حقيبة القضايا فارغة.";
                return RedirectToAction(nameof(Index));
            }

            model.Cart = new CartViewModel { Items = cartItems };
            bool hasPhysical = model.Cart.HasPhysicalItems;

            // التحقق الشرطي: إذا كانت السلة تحتوي على نسخة ورقية فالعنوان والمدينة إجباريان
            if (hasPhysical)
            {
                if (string.IsNullOrWhiteSpace(model.City))
                    ModelState.AddModelError(nameof(model.City), "يرجى إدخال المحافظة أو المدينة لتوصيل النسخة الورقية.");

                if (string.IsNullOrWhiteSpace(model.ShippingAddress))
                    ModelState.AddModelError(nameof(model.ShippingAddress), "يرجى إدخال العنوان التفصيلي لشحن الملف الورقي.");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // جلب القضايا الفعلية من قاعدة البيانات للتحقق من الأسعار والمخزون الحقيقي
            var caseIds = cartItems.Select(i => i.CrimeCaseId).Distinct().ToList();
            var dbCases = await _context.CrimeCases
                .Where(c => caseIds.Contains(c.Id) && c.IsActive)
                .ToDictionaryAsync(c => c.Id);

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var order = new Order
                {
                    OrderNumber = $"PRT-{DateTime.UtcNow:yyyyMMdd}-{Random.Shared.Next(1000, 9999)}",
                    CustomerName = model.CustomerName.Trim(),
                    PhoneNumber = model.PhoneNumber.Trim(),
                    Email = string.IsNullOrWhiteSpace(model.Email) ? null : model.Email.Trim(),
                    City = hasPhysical ? model.City?.Trim() : null,
                    ShippingAddress = hasPhysical ? model.ShippingAddress?.Trim() : null,
                    Notes = string.IsNullOrWhiteSpace(model.Notes) ? null : model.Notes.Trim(),
                    PaymentMethod = hasPhysical ? model.PaymentMethod : PaymentMethod.ShamCashFull,
                    PaymentReference = string.IsNullOrWhiteSpace(model.PaymentReference) ? null : model.PaymentReference.Trim(),
                    Status = OrderStatus.Pending,
                    CreatedAt = DateTime.UtcNow
                };

                decimal verifiedTotal = 0;

                foreach (var item in cartItems)
                {
                    if (!dbCases.TryGetValue(item.CrimeCaseId, out var dbCase))
                    {
                        ModelState.AddModelError("", $"القضية «{item.Title}» لم تعد متوفرة حالياً.");
                        return View(model);
                    }

                    decimal unitPrice;
                    int qty = item.Quantity;

                    if (item.Edition == EditionType.Physical)
                    {
                        if (!dbCase.IsPhysicalAvailable || dbCase.PhysicalStock < qty)
                        {
                            ModelState.AddModelError("", $"عذراً، الكمية المطلوبة من النسخة الورقية لقضية «{dbCase.Title}» غير متوفرة حالياً (المتبقي: {dbCase.PhysicalStock}).");
                            return View(model);
                        }

                        unitPrice = dbCase.PhysicalPrice;
                        // خصم الكمية من مخزون SQL Server
                        dbCase.PhysicalStock -= qty;
                    }
                    else
                    {
                        if (!dbCase.IsDigitalAvailable)
                        {
                            ModelState.AddModelError("", $"عذراً، النسخة الإلكترونية لقضية «{dbCase.Title}» غير متوفرة حالياً.");
                            return View(model);
                        }

                        unitPrice = dbCase.DigitalPrice;
                        qty = 1;
                    }

                    verifiedTotal += unitPrice * qty;

                    order.Items.Add(new OrderItem
                    {
                        CrimeCaseId = dbCase.Id,
                        Edition = item.Edition,
                        Quantity = qty,
                        UnitPrice = unitPrice
                    });
                }

                order.TotalAmount = verifiedTotal;

                _context.Orders.Add(order);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                // إفراغ السلة بعد نجاح حفظ الطلب
                HttpContext.Session.Remove(CartSessionKey);

                return RedirectToAction(nameof(OrderSuccess), new { orderNumber = order.OrderNumber });
            }
            catch
            {
                await transaction.RollbackAsync();
                ModelState.AddModelError("", "حدث خطأ غير متوقع أثناء حفظ الطلب، يرجى المحاولة مجدداً.");
                return View(model);
            }
        }

        // GET: /cart/success/{orderNumber} (صفحة تأكيد نجاح الطلب)
        [HttpGet("success/{orderNumber}")]
        public async Task<IActionResult> OrderSuccess(string orderNumber)
        {
            if (string.IsNullOrWhiteSpace(orderNumber))
                return RedirectToAction("Index", "Home");

            var order = await _context.Orders
                .Include(o => o.Items)
                    .ThenInclude(i => i.CrimeCase)
                .AsNoTracking()
                .FirstOrDefaultAsync(o => o.OrderNumber == orderNumber);

            if (order == null)
                return NotFound();

            return View(order);
        }
    }
}