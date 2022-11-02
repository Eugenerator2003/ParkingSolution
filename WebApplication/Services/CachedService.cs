using Parking.Application;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace WebApplication.Services
{
    public class CachedService<T> : ICachedService<T> where T : class
    {
        private ParkingContext _db;
        private DbSet<T> table;
        private IMemoryCache cache;
        private int _rowsNumber;
        private int secondsCount = 2 * 7 + 240;

        public CachedService(ParkingContext db, IMemoryCache cache)
        {
            this._db = db;
            this.table = db.Set<T>();
            this.cache = cache;
            this._rowsNumber = 20;
        }

        public IEnumerable<T> GetItems()
        {
            return table.Take(_rowsNumber).ToList();
        }

        public IEnumerable<T> GetItems(string cacheKey)
        {
            IEnumerable<T> items = null;
            if (!cache.TryGetValue(cacheKey, out items))
            {
                items = table.Take(_rowsNumber).ToList();
                if (items != null)
                {
                    cache.Set(cacheKey, items,
                              new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromSeconds(secondsCount)));
                }
            }
            return items;
        }

        public void AddItems(string cacheKey)
        {
            IEnumerable<T> items = GetItems();
            cache.Set(cacheKey, items, new MemoryCacheEntryOptions()
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(secondsCount)
            });
        }
    }
}
