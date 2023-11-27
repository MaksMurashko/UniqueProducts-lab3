using UniqueProducts.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using UniqueProducts.Data;

namespace UniqueProducts.Services
{
    public class CachedProductsService : ICachedProductsService
    {
        private readonly UniqueProductsContext _dbContext;
        private readonly IMemoryCache _memoryCache;
        private readonly int _savingTime;

        public CachedProductsService(UniqueProductsContext dbContext, IMemoryCache memoryCache)
        {
            _dbContext = dbContext;
            _memoryCache = memoryCache;
            _savingTime = 2 * 23 + 240;
        }
        // получение списка материалов из базы
        public IEnumerable<Product> GetProducts(int rowsNumber = 20)
        {
            return _dbContext.Products.Take(rowsNumber).ToList();
        }

        // добавление списка материалов в кэш
        public void AddProducts(string cacheKey, int rowsNumber = 20)
        {
            if (!_memoryCache.TryGetValue(cacheKey, out IEnumerable<Product> cachedProducts))
            {
                cachedProducts = _dbContext.Products.Take(rowsNumber).ToList();

                if (cachedProducts != null)
                {
                    _memoryCache.Set(cacheKey, cachedProducts, new MemoryCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(_savingTime)
                    });
                }
                Console.WriteLine("Таблица занесена в кеш");
            }
            else
            {
                Console.WriteLine("Таблица уже находится в кеше");
            }
        }
        // получение списка матреиалов из кэша или из базы, если нет в кэше
        public IEnumerable<Product> GetProductsFromCache(string cacheKey, int rowsNumber = 20)
        {
            IEnumerable<Product> products;
            if (!_memoryCache.TryGetValue(cacheKey, out products))
            {
                products = _dbContext.Products.Take(rowsNumber).ToList();
                if (products != null)
                {
                    _memoryCache.Set(cacheKey, products,
                    new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromSeconds(_savingTime)));
                }
            }
            return products;
        }
        //Получение списка уникальных цветов изделий
        public IEnumerable<string> GetColors(IEnumerable<Product> selectedProducts)
        {
            IEnumerable<string> colors = selectedProducts.Select(p => p.ProductColor).Distinct().ToList();
            return colors;
        }

    }
}