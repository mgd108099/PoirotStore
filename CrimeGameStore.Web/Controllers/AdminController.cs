using CrimeGameStore.Web.Data;
using CrimeGameStore.Web.Filters;
using CrimeGameStore.Web.Models;
using CrimeGameStore.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace CrimeGameStore.Web.Controllers
{
    [Route("admin")]
    public class AdminController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ISiteSettingsService _settingsService;
        private readonly IConfiguration _config;
        private readonly IWebHostEnvironment _env;

        public AdminController(
            AppDbContext context,
            ISiteSettingsService settingsService,
            IConfiguration config,
            IWebHostEnvironment env)
        {
            _context = context;
            _settingsService = settingsService;
            _config = config;
            _env = env;
        }

        // ==========================================
        // 1. تسجيل الدخول والخروج
        // ==========================================
        [HttpGet("login")]
        public IActionResult Login()
        {
            if (HttpContext.Session.GetString("PoirotAdminLoggedIn") == "true")
                return RedirectToAction(nameof(Index));

            return View();
        }

        [HttpPost("login")]
        [ValidateAntiForgeryToken]
        public IActionResult Login(string username, string password)
        {
            var validUser = _config["AdminAuth:Username"] ?? "admin";
            var validPass = _config["AdminAuth:Password"] ?? "Poirot@2026";

            if (username?.Trim() == validUser && password == validPass)
            {
                HttpContext.Session.SetString("PoirotAdminLoggedIn", "true");
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Error = "اسم المستخدم أو كلمة المرور غير صحيحة.";
            return View();
        }

        [HttpPost("logout")]
        [ValidateAntiForgeryToken]
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("PoirotAdminLoggedIn");
            return RedirectToAction(nameof(Login));
        }

        // ==========================================
        // 2. الصفحة الرئيسية للوحة التحكم (Dashboard)
        // ==========================================
        [AdminAuthorize]
        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            ViewBag.TotalCases = await _context.CrimeCases.CountAsync();
            ViewBag.ActiveCases = await _context.CrimeCases.CountAsync(c => c.IsActive);
            ViewBag.TotalOrders = await _context.Orders.CountAsync();
            ViewBag.PendingOrders = await _context.Orders.CountAsync(o => o.Status == OrderStatus.Pending);
            ViewBag.TotalRevenue = await _context.Orders
                .Where(o => o.Status != OrderStatus.Cancelled)
                .SumAsync(o => (decimal?)o.TotalAmount) ?? 0m;

            var latestOrders = await _context.Orders
                .Include(o => o.Items)
                .OrderByDescending(o => o.CreatedAt)
                .Take(6)
                .AsNoTracking()
                .ToListAsync();

            return View(latestOrders);
        }

        // ==========================================
        // 3. إدارة القضايا (إضافة / تعديل / حذف / صور)
        // ==========================================
        [AdminAuthorize]
        [HttpGet("cases")]
        public async Task<IActionResult> Cases()
        {
            var cases = await _context.CrimeCases
                .OrderByDescending(c => c.CreatedAt)
                .AsNoTracking()
                .ToListAsync();

            return View(cases);
        }

        [AdminAuthorize]
        [HttpGet("cases/create")]
        public IActionResult CreateCase()
        {
            return View("CaseForm", new CrimeCase());
        }

        [AdminAuthorize]
        [HttpPost("cases/create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCase(CrimeCase model, IFormFile? coverImage)
        {
            // توليد رابط صديق للسيو تلقائياً من العنوان إذا تُرك فارغاً
            model.Slug = GenerateSlug(string.IsNullOrWhiteSpace(model.Slug) ? model.Title : model.Slug);
            ModelState.Remove(nameof(model.Slug));
            ModelState.Remove(nameof(model.CoverImageUrl));

            if (await _context.CrimeCases.AnyAsync(c => c.Slug == model.Slug))
            {
                ModelState.AddModelError(nameof(model.Slug), "هذا الرابط (Slug) مستخدم في قضية أخرى، يرجى تغييره.");
            }

            if (!ModelState.IsValid)
                return View("CaseForm", model);

            if (coverImage != null && coverImage.Length > 0)
            {
                model.CoverImageUrl = await SaveImageAsync(coverImage, "cases");
            }
            else if (string.IsNullOrWhiteSpace(model.CoverImageUrl))
            {
                model.CoverImageUrl = "/images/logo.png";
            }

            model.CreatedAt = DateTime.UtcNow;
            _context.CrimeCases.Add(model);
            await _context.SaveChangesAsync();

            TempData["AdminSuccess"] = $"تمت إضافة قضية «{model.Title}» بنجاح وأُضيفت تلقائياً لـ sitemap.xml.";
            return RedirectToAction(nameof(EditCase), new { id = model.Id });
        }

        [AdminAuthorize]
        [HttpGet("cases/edit/{id:int}")]
        public async Task<IActionResult> EditCase(int id)
        {
            var crimeCase = await _context.CrimeCases
                .Include(c => c.GalleryImages.OrderBy(g => g.SortOrder))
                .FirstOrDefaultAsync(c => c.Id == id);

            if (crimeCase == null)
                return NotFound();

            return View("CaseForm", crimeCase);
        }

        [AdminAuthorize]
        [HttpPost("cases/edit/{id:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCase(int id, CrimeCase model, IFormFile? coverImage)
        {
            var existing = await _context.CrimeCases
                .Include(c => c.GalleryImages)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (existing == null)
                return NotFound();

            model.Slug = GenerateSlug(string.IsNullOrWhiteSpace(model.Slug) ? model.Title : model.Slug);
            ModelState.Remove(nameof(model.Slug));
            ModelState.Remove(nameof(model.CoverImageUrl));

            if (await _context.CrimeCases.AnyAsync(c => c.Slug == model.Slug && c.Id != id))
            {
                ModelState.AddModelError(nameof(model.Slug), "هذا الرابط (Slug) مستخدم في قضية أخرى.");
            }

            if (!ModelState.IsValid)
            {
                model.GalleryImages = existing.GalleryImages;
                return View("CaseForm", model);
            }

            existing.CaseCode = model.CaseCode.Trim();
            existing.Title = model.Title.Trim();
            existing.Slug = model.Slug;
            existing.ShortDescription = model.ShortDescription.Trim();
            existing.FullStory = model.FullStory.Trim();
            existing.Difficulty = model.Difficulty;
            existing.PlayersCount = model.PlayersCount.Trim();
            existing.EstimatedTime = model.EstimatedTime.Trim();
            existing.PhysicalPrice = model.PhysicalPrice;
            existing.PhysicalStock = model.PhysicalStock;
            existing.IsPhysicalAvailable = model.IsPhysicalAvailable;
            existing.DigitalPrice = model.DigitalPrice;
            existing.IsDigitalAvailable = model.IsDigitalAvailable;
            existing.MetaTitle = model.MetaTitle?.Trim();
            existing.MetaDescription = model.MetaDescription?.Trim();
            existing.MetaKeywords = model.MetaKeywords?.Trim();
            existing.IsFeatured = model.IsFeatured;
            existing.IsActive = model.IsActive;

            if (coverImage != null && coverImage.Length > 0)
            {
                existing.CoverImageUrl = await SaveImageAsync(coverImage, "cases");
            }

            await _context.SaveChangesAsync();
            TempData["AdminSuccess"] = $"تم تحديث بيانات قضية «{existing.Title}» بنجاح.";
            return RedirectToAction(nameof(EditCase), new { id = existing.Id });
        }

        [AdminAuthorize]
        [HttpPost("cases/delete/{id:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCase(int id)
        {
            var crimeCase = await _context.CrimeCases
                .Include(c => c.OrderItems)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (crimeCase == null)
                return NotFound();

            // إذا كانت القضية مرتبطة بطلبات سابقة، نُخفيها بدلاً من حذفها لحفظ سجلات المبيعات
            if (crimeCase.OrderItems.Any())
            {
                crimeCase.IsActive = false;
                crimeCase.IsFeatured = false;
                await _context.SaveChangesAsync();
                TempData["AdminSuccess"] = $"القضية «{crimeCase.Title}» مرتبطة بطلبات سابقة، لذلك تم إخفاؤها عن المتجر بدلاً من حذفها حفاظاً على سجل المبيعات.";
            }
            else
            {
                _context.CrimeCases.Remove(crimeCase);
                await _context.SaveChangesAsync();
                TempData["AdminSuccess"] = $"تم حذف قضية «{crimeCase.Title}» نهائياً.";
            }

            return RedirectToAction(nameof(Cases));
        }

        // إضافة صورة لمعرض القضية
        [AdminAuthorize]
        [HttpPost("cases/gallery/add")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddGalleryImage(int crimeCaseId, IFormFile? galleryFile, string? caption, int sortOrder = 1)
        {
            if (galleryFile == null || galleryFile.Length == 0)
            {
                TempData["AdminError"] = "يرجى اختيار صورة لرفعها للمعرض.";
                return RedirectToAction(nameof(EditCase), new { id = crimeCaseId });
            }

            var url = await SaveImageAsync(galleryFile, "gallery");
            _context.CaseGalleries.Add(new CaseGallery
            {
                CrimeCaseId = crimeCaseId,
                ImageUrl = url,
                Caption = string.IsNullOrWhiteSpace(caption) ? "صورة من ملف القضية" : caption.Trim(),
                SortOrder = sortOrder
            });

            await _context.SaveChangesAsync();
            TempData["AdminSuccess"] = "تمت إضافة الصورة إلى معرض القضية.";
            return RedirectToAction(nameof(EditCase), new { id = crimeCaseId });
        }

        // حذف صورة من معرض القضية
        [AdminAuthorize]
        [HttpPost("cases/gallery/delete/{id:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteGalleryImage(int id)
        {
            var img = await _context.CaseGalleries.FindAsync(id);
            if (img == null)
                return NotFound();

            int caseId = img.CrimeCaseId;
            _context.CaseGalleries.Remove(img);
            await _context.SaveChangesAsync();

            TempData["AdminSuccess"] = "تم حذف الصورة من المعرض.";
            return RedirectToAction(nameof(EditCase), new { id = caseId });
        }

        // ==========================================
        // 4. إدارة الطلبات (متابعة وتحديث الحالة)
        // ==========================================
        [AdminAuthorize]
        [HttpGet("orders")]
        public async Task<IActionResult> Orders(OrderStatus? status)
        {
            var query = _context.Orders
                .Include(o => o.Items)
                    .ThenInclude(i => i.CrimeCase)
                .AsNoTracking()
                .AsQueryable();

            if (status.HasValue)
            {
                query = query.Where(o => o.Status == status.Value);
            }

            ViewBag.CurrentStatus = status;
            var orders = await query.OrderByDescending(o => o.CreatedAt).ToListAsync();
            return View(orders);
        }

        [AdminAuthorize]
        [HttpGet("orders/{id:int}")]
        public async Task<IActionResult> OrderDetails(int id)
        {
            var order = await _context.Orders
                .Include(o => o.Items)
                    .ThenInclude(i => i.CrimeCase)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
                return NotFound();

            return View(order);
        }

        [AdminAuthorize]
        [HttpPost("orders/update-status")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateOrderStatus(int id, OrderStatus status)
        {
            var order = await _context.Orders
                .Include(o => o.Items)
                    .ThenInclude(i => i.CrimeCase)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
                return NotFound();

            var oldStatus = order.Status;
            order.Status = status;

            // إعادة المخزون الورقي تلقائياً عند إلغاء الطلب، وخصمه مجدداً عند إعادة تفعيله
            if (oldStatus != OrderStatus.Cancelled && status == OrderStatus.Cancelled)
            {
                foreach (var item in order.Items.Where(i => i.Edition == EditionType.Physical && i.CrimeCase != null))
                {
                    item.CrimeCase!.PhysicalStock += item.Quantity;
                }
            }
            else if (oldStatus == OrderStatus.Cancelled && status != OrderStatus.Cancelled)
            {
                foreach (var item in order.Items.Where(i => i.Edition == EditionType.Physical && i.CrimeCase != null))
                {
                    item.CrimeCase!.PhysicalStock = Math.Max(0, item.CrimeCase!.PhysicalStock - item.Quantity);
                }
            }

            await _context.SaveChangesAsync();
            TempData["AdminSuccess"] = $"تم تحديث حالة الطلب {order.OrderNumber} بنجاح.";
            return RedirectToAction(nameof(OrderDetails), new { id = order.Id });
        }

        // ==========================================
        // 5. إدارة إعدادات ومحتوى الموقع بالكامل (CMS)
        // ==========================================
        [AdminAuthorize]
        [HttpGet("settings")]
        public async Task<IActionResult> Settings()
        {
            var settings = await _context.SiteSettings.FirstOrDefaultAsync() ?? new SiteSetting();
            return View(settings);
        }

        [AdminAuthorize]
        [HttpPost("settings")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Settings(SiteSetting model, IFormFile? logoFile)
        {
            ModelState.Remove(nameof(model.LogoUrl));

            if (!ModelState.IsValid)
                return View(model);

            var existing = await _context.SiteSettings.FirstOrDefaultAsync();
            if (existing == null)
            {
                existing = new SiteSetting();
                _context.SiteSettings.Add(existing);
            }

            existing.SiteName = model.SiteName.Trim();
            existing.SiteSubtitle = model.SiteSubtitle.Trim();
            existing.TopAnnouncement = model.TopAnnouncement.Trim();
            existing.FooterAbout = model.FooterAbout.Trim();
            existing.PhoneDisplay = model.PhoneDisplay.Trim();
            existing.WhatsAppNumber = model.WhatsAppNumber.Trim().Replace("+", "").Replace(" ", "");
            existing.ShamCashAccount = model.ShamCashAccount.Trim();
            existing.DepositNote = model.DepositNote.Trim();
            existing.HeroBadge = model.HeroBadge.Trim();
            existing.HeroTitle = model.HeroTitle.Trim();
            existing.HeroHighlight = model.HeroHighlight.Trim();
            existing.HeroDescription = model.HeroDescription.Trim();
            existing.HomeCtaTitle = model.HomeCtaTitle.Trim();
            existing.HomeCtaDescription = model.HomeCtaDescription.Trim();
            existing.DefaultMetaDescription = model.DefaultMetaDescription.Trim();
            existing.DefaultMetaKeywords = model.DefaultMetaKeywords.Trim();
            existing.InstagramUrl = string.IsNullOrWhiteSpace(model.InstagramUrl) ? null : model.InstagramUrl.Trim();
            existing.FacebookUrl = string.IsNullOrWhiteSpace(model.FacebookUrl) ? null : model.FacebookUrl.Trim();

            if (logoFile != null && logoFile.Length > 0)
            {
                existing.LogoUrl = await SaveImageAsync(logoFile, "branding");
            }

            await _context.SaveChangesAsync();
            _settingsService.ClearCache(); // تحديث فوري لكل صفحات الموقع

            TempData["AdminSuccess"] = "تم حفظ إعدادات ومحتوى الموقع بنجاح وانعكست فوراً على جميع الصفحات.";
            return RedirectToAction(nameof(Settings));
        }

        // ==========================================
        // 6. إدارة الأسئلة الشائعة (FAQ)
        // ==========================================
        [AdminAuthorize]
        [HttpGet("faqs")]
        public async Task<IActionResult> Faqs()
        {
            var faqs = await _context.FaqItems
                .OrderBy(f => f.SortOrder)
                .ThenBy(f => f.Id)
                .AsNoTracking()
                .ToListAsync();

            return View(faqs);
        }

        [AdminAuthorize]
        [HttpPost("faqs/save")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveFaq(FaqItem model)
        {
            if (!ModelState.IsValid)
            {
                TempData["AdminError"] = "يرجى التأكد من ملء السؤال والإجابة بشكل صحيح.";
                return RedirectToAction(nameof(Faqs));
            }

            if (model.Id == 0)
            {
                _context.FaqItems.Add(model);
                TempData["AdminSuccess"] = "تمت إضافة السؤال الشائع بنجاح.";
            }
            else
            {
                var existing = await _context.FaqItems.FindAsync(model.Id);
                if (existing != null)
                {
                    existing.Question = model.Question.Trim();
                    existing.Answer = model.Answer.Trim();
                    existing.SortOrder = model.SortOrder;
                    existing.IsActive = model.IsActive;
                    TempData["AdminSuccess"] = "تم تعديل السؤال بنجاح.";
                }
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Faqs));
        }

        [AdminAuthorize]
        [HttpPost("faqs/delete/{id:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteFaq(int id)
        {
            var faq = await _context.FaqItems.FindAsync(id);
            if (faq != null)
            {
                _context.FaqItems.Remove(faq);
                await _context.SaveChangesAsync();
                TempData["AdminSuccess"] = "تم حذف السؤال بنجاح.";
            }
            return RedirectToAction(nameof(Faqs));
        }

        // ==========================================
        // دوال مساعدة (رفع الصور وتوليد الـ Slug)
        // ==========================================
        private async Task<string> SaveImageAsync(IFormFile file, string folder)
        {
            var allowedExts = new[] { ".jpg", ".jpeg", ".png", ".webp" };
            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!allowedExts.Contains(ext))
                ext = ".png";

            var uploadRoot = Path.Combine(_env.WebRootPath, "uploads", folder);
            Directory.CreateDirectory(uploadRoot);

            var fileName = $"{Guid.NewGuid():N}{ext}";
            var fullPath = Path.Combine(uploadRoot, fileName);

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return $"/uploads/{folder}/{fileName}";
        }

        private static string GenerateSlug(string input)
        {
            var cleaned = input.Trim().ToLowerInvariant();
            // الإبقاء على الأحرف العربية والإنجليزية والأرقام والشرطة فقط
            cleaned = Regex.Replace(cleaned, @"[^\u0621-\u064Aa-z0-9\s-]", "");
            cleaned = Regex.Replace(cleaned, @"\s+", "-");
            cleaned = Regex.Replace(cleaned, @"-+", "-").Trim('-');
            return string.IsNullOrWhiteSpace(cleaned) ? $"case-{Random.Shared.Next(1000, 9999)}" : cleaned;
        }
    }
}