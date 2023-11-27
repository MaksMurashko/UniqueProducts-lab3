using UniqueProducts.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace UniqueProducts.Services
{
    public interface ICachedProductsService
    {
        public IEnumerable<Product> GetProducts(int rowsNumber = 20);
        public void AddProducts(string cacheKey, int rowsNumber = 20);
        public IEnumerable<Product> GetProductsFromCache(string cacheKey, int rowsNumber = 20);
        public IEnumerable<string> GetColors(IEnumerable<Product> selectedProducts);
    }
}