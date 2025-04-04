using System.Net.Http.Json;
using eCommerce.BlazorWasm.Services.Interfaces;
using eCommerce.Core.DTOs.Product;

namespace eCommerce.BlazorWasm.Services
{
    public class ProductService : IProductService
    {
        private readonly HttpClient _httpClient;

        public ProductService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<ProductDto>> GetProductsAsync(int page = 1, int pageSize = 10)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<List<ProductDto>>($"api/products?page={page}&pageSize={pageSize}") ?? new List<ProductDto>();
            }
            catch
            {
                return new List<ProductDto>();
            }
        }

        public async Task<ProductDto> GetProductByIdAsync(int id)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<ProductDto>($"api/products/{id}") ?? new ProductDto();
            }
            catch
            {
                return new ProductDto();
            }
        }

        public async Task<List<ProductDto>> GetProductsByCategoryAsync(int categoryId)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<List<ProductDto>>($"api/products/category/{categoryId}") ?? new List<ProductDto>();
            }
            catch
            {
                return new List<ProductDto>();
            }
        }

        public async Task<List<CategoryDTO>> GetCategoriesAsync()
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<List<CategoryDTO>>("api/categories") ?? new List<CategoryDTO>();
            }
            catch
            {
                return new List<CategoryDTO>();
            }
        }

        public async Task<bool> CreateProductAsync(CreateProductDto productDto)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/products", productDto);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UpdateProductAsync(int id, UpdateProductDto productDto)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"api/products/{id}", productDto);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"api/products/{id}");
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public async Task<List<ProductDto>> SearchProductsAsync(string searchTerm)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<List<ProductDto>>($"api/products/search?term={searchTerm}") ?? new List<ProductDto>();
            }
            catch
            {
                return new List<ProductDto>();
            }
        }

        public async Task<List<ReviewDto>> GetProductReviewsAsync(int productId)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<List<ReviewDto>>($"api/products/{productId}/reviews") ?? new List<ReviewDto>();
            }
            catch
            {
                return new List<ReviewDto>();
            }
        }

        public async Task<bool> AddProductReviewAsync(int productId, CreateReviewDto reviewDto)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync($"api/products/{productId}/reviews", reviewDto);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        Task<List<Pages.CategoryDto>> IProductService.GetCategoriesAsync()
        {
            throw new NotImplementedException();
        }

        Task<List<ReviewDto>> IProductService.GetProductReviewsAsync(int productId)
        {
            throw new NotImplementedException();
        }

        Task<bool> IProductService.AddProductReviewAsync(int productId, ReviewDto reviewDto)
        {
            throw new NotImplementedException();
        }
    }
}