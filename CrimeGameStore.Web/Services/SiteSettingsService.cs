using CrimeGameStore.Web.Data;
using CrimeGameStore.Web.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace CrimeGameStore.Web.Services
{
    public interface ISiteSettingsService
    {
        Task<SiteSetting> GetSettingsAsync();
        void ClearCache();
    }

    public class SiteSettingsService : ISiteSettingsService
    {
        private readonly AppDbContext _context;
        private readonly IMemoryCache _cache;
        private const string CacheKey = "PoirotSiteSettings";

        public SiteSettingsService(AppDbContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        public async Task<SiteSetting> GetSettingsAsync()
        {
            if (_cache.TryGetValue(CacheKey, out SiteSetting? cached) && cached != null)
            {
                return cached;
            }

            var settings = await _context.SiteSettings.AsNoTracking().FirstOrDefaultAsync();
            if (settings == null)
            {
                settings = new SiteSetting();
            }

            _cache.Set(CacheKey, settings, TimeSpan.FromHours(1));
            return settings;
        }

        public void ClearCache()
        {
            _cache.Remove(CacheKey);
        }
    }
}