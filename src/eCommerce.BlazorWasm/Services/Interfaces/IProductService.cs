using eCommerce.Core.DTOs.Product;

namespace eCommerce.BlazorWasm.Services.Interfaces
{
    public interface IProductService
    {
        Task<List<ProductDto>> GetProductsAsync(int page = 1, int pageSize = 10);
        Task<ProductDto> GetProductByIdAsync(int id);
        Task<List<ProductDto>> GetProductsByCategoryAsync(int categoryId);
        Task<List<Pages.CategoryDto>> GetCategoriesAsync();
        Task<bool> CreateProductAsync(CreateProductDto productDto);
        Task<bool> UpdateProductAsync(int id, UpdateProductDto productDto);
        Task<bool> DeleteProductAsync(int id);
        Task<List<ProductDto>> SearchProductsAsync(string searchTerm);
        Task<List<ReviewDto>> GetProductReviewsAsync(int productId);
        Task<bool> AddProductReviewAsync(int productId, ReviewDto reviewDto);
    }
}