using eCommerce.Core.DTOs.Product;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace eCommerce.Core.Interfaces
{
    public interface IProductService
    {
        Task<ProductListDto> GetProductsAsync(ProductFilterDto filter, int page = 1, int pageSize = 10);
        Task<ProductDto> GetProductByIdAsync(int id);
        Task<IEnumerable<ProductDto>> GetProductsByCategoryAsync(int categoryId, int page = 1, int pageSize = 10);
        Task<IEnumerable<ProductDto>> GetProductsByVendorAsync(int vendorId, int page = 1, int pageSize = 10);
        Task<IEnumerable<ProductDto>> GetFeaturedProductsAsync(int count = 10);
        Task<IEnumerable<ProductDto>> GetNewArrivalsAsync(int count = 10);
        Task<IEnumerable<ProductDto>> GetBestSellersAsync(int count = 10);
        Task<IEnumerable<ProductDto>> GetRelatedProductsAsync(int productId, int count = 4);
        Task<IEnumerable<ProductDto>> SearchProductsAsync(string searchTerm, int page = 1, int pageSize = 10);
        Task<ProductDto> CreateProductAsync(CreateProductDto productDto);
        Task<ProductDto> UpdateProductAsync(int id, UpdateProductDto productDto);
        Task<bool> DeleteProductAsync(int id);
        Task<bool> UpdateProductStockAsync(int id, int quantity);
        Task<bool> UpdateProductStatusAsync(int id, bool isActive);
    }
} 