using System.Collections.Generic;
using System.Threading.Tasks;
using eCommerce.Core.Models;

namespace eCommerce.Core.Interfaces
{
    public interface ICategoryRepository : IGenericRepository<Category>
    {
        Task<IEnumerable<Category>> GetAllActiveCategoriesAsync();
        Task<Category> GetCategoryWithProductsAsync(int id);
        Task<IEnumerable<Category>> GetCategoriesWithProductCountAsync();
        Task<bool> IsCategoryNameUniqueAsync(string name);
        Task<IEnumerable<Category>> GetParentCategoriesAsync();
        Task<IEnumerable<Category>> GetChildCategoriesAsync(int parentId);
        Task<int> GetProductCountAsync(int categoryId);
        Task<decimal> GetAverageProductPriceAsync(int categoryId);
        Task<decimal> GetTotalRevenueAsync(int categoryId);
        Task<IEnumerable<Category>> SearchCategoriesAsync(string searchTerm);
        Task<bool> HasActiveProductsAsync(int categoryId);
        Task<IEnumerable<Category>> GetFeaturedCategoriesAsync(int count);
        Task<CategoryStatistics> GetCategoryStatisticsAsync(int categoryId);
    }
} 