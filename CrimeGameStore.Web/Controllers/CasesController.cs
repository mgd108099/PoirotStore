using CrimeGameStore.Web.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CrimeGameStore.Web.Controllers
{
    [Route("cases")]
    public class CasesController : Controller
    {
        private readonly AppDbContext _context;

        public CasesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: /cases
        // كتالوج القضايا مع دعم البحث والفلترة حسب الصعوبة ونوع النسخة
        [HttpGet("")]
        public async Task<IActionResult> Index(string? search, string? difficulty, string? edition)
        {
            var query = _context.CrimeCases
                .Where(c => c.IsActive)
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.Trim();
                query = query.Where(c =>
                    c.Title.Contains(term) ||
                    c.ShortDescription.Contains(term) ||
                    c.CaseCode.Contains(term));
            }

            if (!string.IsNullOrWhiteSpace(difficulty) && difficulty != "الكل")
            {
                query = query.Where(c => c.Difficulty == difficulty);
            }

            if (edition == "physical")
            {
                query = query.Where(c => c.IsPhysicalAvailable && c.PhysicalStock > 0);
            }
            else if (edition == "digital")
            {
                query = query.Where(c => c.IsDigitalAvailable);
            }

            var cases = await query
                .OrderByDescending(c => c.IsFeatured)
                .ThenByDescending(c => c.CreatedAt)
                .ToListAsync();

            // تمرير قيم الفلاتر الحالية للحفاظ عليها في الواجهة
            ViewData["CurrentSearch"] = search;
            ViewData["CurrentDifficulty"] = difficulty ?? "الكل";
            ViewData["CurrentEdition"] = edition ?? "all";

            return View(cases);
        }

        // GET: /cases/{slug}
        // يقبل الروابط العربية والإنجليزية الصديقة للسيو (مثل /cases/قضية-السم-النائم أو /cases/al-sam-al-naem)
        [HttpGet("{slug}")]
        public async Task<IActionResult> Details(string slug)
        {
            if (string.IsNullOrWhiteSpace(slug))
                return NotFound();

            // فك ترميز الرابط لضمان تطابق الروابط العربية في SQL Server
            var decodedSlug = Uri.UnescapeDataString(slug).Trim();

            var crimeCase = await _context.CrimeCases
                .Include(c => c.GalleryImages.OrderBy(g => g.SortOrder))
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.IsActive && c.Slug == decodedSlug);

            if (crimeCase == null)
                return NotFound();

            // جلب قضايا مقترحة أخرى (لزيادة بقاء الزائر في الموقع وتقوية الربط الداخلي للسيو)
            ViewBag.RelatedCases = await _context.CrimeCases
                .Where(c => c.IsActive && c.Id != crimeCase.Id)
                .OrderByDescending(c => c.CreatedAt)
                .Take(3)
                .AsNoTracking()
                .ToListAsync();

            return View(crimeCase);
        }
    }
}