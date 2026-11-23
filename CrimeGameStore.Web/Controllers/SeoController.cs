using CrimeGameStore.Web.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Xml.Linq;

namespace CrimeGameStore.Web.Controllers
{
    public class SeoController : Controller
    {
        private readonly AppDbContext _context;

        public SeoController(AppDbContext context)
        {
            _context = context;
        }

        // GET: /sitemap.xml
        [HttpGet("/sitemap.xml")]
        [ResponseCache(Duration = 3600)]
        public async Task<IActionResult> Sitemap()
        {
            var baseUrl = $"{Request.Scheme}://{Request.Host}";
            XNamespace ns = "http://www.sitemaps.org/schemas/sitemap/0.9";

            var urlElements = new List<XElement>
            {
                CreateUrlElement(ns, $"{baseUrl}/", DateTime.UtcNow, "weekly", "1.0"),
                CreateUrlElement(ns, $"{baseUrl}/cases", DateTime.UtcNow, "daily", "0.9"),
                CreateUrlElement(ns, $"{baseUrl}/how-to-play", DateTime.UtcNow, "monthly", "0.8"),
                CreateUrlElement(ns, $"{baseUrl}/contact", DateTime.UtcNow, "monthly", "0.7")
            };

            // سحب جميع القضايا الفعالة تلقائياً من قاعدة البيانات
            var activeCases = await _context.CrimeCases
                .Where(c => c.IsActive)
                .OrderByDescending(c => c.CreatedAt)
                .Select(c => new { c.Slug, c.CreatedAt })
                .AsNoTracking()
                .ToListAsync();

            foreach (var c in activeCases)
            {
                var caseUrl = $"{baseUrl}/cases/{Uri.EscapeDataString(c.Slug)}";
                urlElements.Add(CreateUrlElement(ns, caseUrl, c.CreatedAt, "weekly", "0.9"));
            }

            var sitemap = new XDocument(
                new XDeclaration("1.0", "utf-8", "yes"),
                new XElement(ns + "urlset", urlElements)
            );

            return Content(sitemap.ToString(), "application/xml", Encoding.UTF8);
        }

        // GET: /robots.txt
        [HttpGet("/robots.txt")]
        [ResponseCache(Duration = 86400)]
        public IActionResult Robots()
        {
            var baseUrl = $"{Request.Scheme}://{Request.Host}";
            var sb = new StringBuilder();
            sb.AppendLine("User-agent: *");
            sb.AppendLine("Allow: /");
            sb.AppendLine("Disallow: /admin");
            sb.AppendLine("Disallow: /admin/");
            sb.AppendLine("Disallow: /cart");
            sb.AppendLine("Disallow: /cart/");
            sb.AppendLine();
            sb.AppendLine($"Sitemap: {baseUrl}/sitemap.xml");

            return Content(sb.ToString(), "text/plain", Encoding.UTF8);
        }

        private static XElement CreateUrlElement(XNamespace ns, string loc, DateTime lastMod, string changeFreq, string priority)
        {
            return new XElement(ns + "url",
                new XElement(ns + "loc", loc),
                new XElement(ns + "lastmod", lastMod.ToString("yyyy-MM-dd")),
                new XElement(ns + "changefreq", changeFreq),
                new XElement(ns + "priority", priority)
            );
        }
    }
}