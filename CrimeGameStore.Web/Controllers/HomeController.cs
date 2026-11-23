using CrimeGameStore.Web.Data;
using CrimeGameStore.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace CrimeGameStore.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }

        // الصفحة الرئيسية: تجلب القضايا المميزة والفعالة من قاعدة البيانات
        public async Task<IActionResult> Index()
        {
            var featuredCases = await _context.CrimeCases
                .Where(c => c.IsActive && c.IsFeatured)
                .OrderByDescending(c => c.CreatedAt)
                .Take(6)
                .AsNoTracking()
                .ToListAsync();

            return View(featuredCases);
        }

        // صفحة كيف تلعب؟ + الأسئلة الشائعة برابط صديق للسيو
        [HttpGet("how-to-play")]
        public async Task<IActionResult> HowToPlay()
        {
            var faqs = await _context.FaqItems
                .Where(f => f.IsActive)
                .OrderBy(f => f.SortOrder)
                .ThenBy(f => f.Id)
                .AsNoTracking()
                .ToListAsync();

            return View(faqs);
        }

        // توجيه رابط "الأسئلة الشائعة" في الهيدر والفوتر مباشرة لقسم الأسئلة
        [HttpGet("faq")]
        public IActionResult Faq()
        {
            return Redirect("/how-to-play#faq-section");
        }

        // صفحة تواصل معنا
        [HttpGet("contact")]
        public IActionResult Contact()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}