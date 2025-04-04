using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using eCommerce.Core.DTOs;
using eCommerce.Core.Models;
using Microsoft.AspNetCore.Components.Web;

namespace eCommerce.BlazorWasm.Pages
{
    public partial class Products
    {
        [Parameter]
        public int? CategoryId { get; set; }

        private List<ProductDto>? _products;
        private List<CategoryDto>? _categories;
        private List<string>? _brands;
        private HashSet<string> _selectedBrands = new();
        private int? _selectedCategoryId;
        private Range<decimal> _priceRange = new(0, 1000);
        private bool _discountedOnly;
        private bool _inStockOnly = true;
        private string _sortBy = "newest";
        private int _pageSize = 12;
        private int _currentPage = 1;
        private int _totalPages = 1;
        private bool _isLoading = true;
        private string _viewMode = "grid";

        protected override async Task OnInitializedAsync()
        {
            await LoadCategoriesAsync();
            await LoadBrandsAsync();
            
            if (CategoryId.HasValue)
            {
                _selectedCategoryId = CategoryId;
            }
            
            await LoadProductsAsync();
        }

        protected override async Task OnParametersSetAsync()
        {
            if (CategoryId.HasValue && _selectedCategoryId != CategoryId)
            {
                _selectedCategoryId = CategoryId;
                await LoadProductsAsync();
            }
        }

        private async Task LoadCategoriesAsync()
        {
            try
            {
                _categories = await Http.GetFromJsonAsync<List<CategoryDto>>("api/categories");
            }
            catch (Exception ex)
            {
                Snackbar.Add($"Error loading categories: {ex.Message}", Severity.Error);
            }
        }

        private async Task LoadBrandsAsync()
        {
            try
            {
                _brands = await Http.GetFromJsonAsync<List<string>>("api/products/brands");
            }
            catch (Exception ex)
            {
                Snackbar.Add($"Error loading brands: {ex.Message}", Severity.Error);
            }
        }

        private async Task LoadProductsAsync()
        {
            _isLoading = true;
            try
            {
                var queryParams = new Dictionary<string, string>();
                
                if (_selectedCategoryId.HasValue)
                    queryParams.Add("categoryId", _selectedCategoryId.Value.ToString());
                
                queryParams.Add("minPrice", _priceRange.Start.ToString());
                queryParams.Add("maxPrice", _priceRange.End.ToString());
                
                if (_selectedBrands.Count > 0)
                    queryParams.Add("brands", string.Join(",", _selectedBrands));
                
                if (_discountedOnly)
                    queryParams.Add("discounted", "true");
                
                if (_inStockOnly)
                    queryParams.Add("inStock", "true");
                
                queryParams.Add("sortBy", _sortBy);
                queryParams.Add("page", _currentPage.ToString());
                queryParams.Add("pageSize", _pageSize.ToString());
                
                var queryString = string.Join("&", queryParams.Select(p => $"{p.Key}={Uri.EscapeDataString(p.Value)}"));
                var response = await Http.GetFromJsonAsync<ProductsResponseDto>($"api/products?{queryString}");
                
                if (response != null)
                {
                    _products = response.Products;
                    _totalPages = response.TotalPages;
                }
            }
            catch (Exception ex)
            {
                Snackbar.Add($"Error loading products: {ex.Message}", Severity.Error);
                _products = new List<ProductDto>();
            }
            finally
            {
                _isLoading = false;
                StateHasChanged();
            }
        }

        private void OnCategoryChanged(int? categoryId)
        {
            _selectedCategoryId = categoryId;
            _currentPage = 1;
        }

        private void OnPriceRangeChanged(Range<decimal> range)
        {
            _priceRange = range;
            _currentPage = 1;
        }

        private void OnBrandChanged(string brand, bool isChecked)
        {
            if (isChecked)
                _selectedBrands.Add(brand);
            else
                _selectedBrands.Remove(brand);

            _currentPage = 1;
        }

        private void OnDiscountedOnlyChanged(bool value)
        {
            _discountedOnly = value;
            _currentPage = 1;
        }

        private void OnInStockOnlyChanged(bool value)
        {
            _inStockOnly = value;
            _currentPage = 1;
        }

        private void OnSortByChanged(string value)
        {
            _sortBy = value;
            _currentPage = 1;
        }

        private void OnPageSizeChanged(int value)
        {
            _pageSize = value;
            _currentPage = 1;
            LoadProductsAsync();
        }

        private async Task OnPageChanged(int page)
        {
            _currentPage = page;
            await LoadProductsAsync();
        }

        private async Task ApplyFilters()
        {
            _currentPage = 1;
            await LoadProductsAsync();
        }

        private async Task ResetFilters()
        {
            _selectedCategoryId = null;
            _priceRange = new Range<decimal>(0, 1000);
            _selectedBrands.Clear();
            _discountedOnly = false;
            _inStockOnly = true;
            _sortBy = "newest";
            _currentPage = 1;
            await LoadProductsAsync();
        }

        private void NavigateToProduct(int productId)
        {
            NavigationManager.NavigateTo($"/product/{productId}");
        }

        private async Task AddToCart(int productId, MouseEventArgs e)
        {
            try
            {
                await Http.PostAsync($"api/cart/add/{productId}?quantity=1", null);
                Snackbar.Add("Product added to cart", Severity.Success);
            }
            catch (Exception ex)
            {
                Snackbar.Add($"Error adding product to cart: {ex.Message}", Severity.Error);
            }
        }

        private async Task AddToWishlist(int productId, MouseEventArgs e)
        {
            
            try
            {
                await Http.PostAsync($"api/wishlist/items/{productId}", null);
                Snackbar.Add("Product added to wishlist", Severity.Success);
            }
            catch (Exception ex)
            {
                Snackbar.Add($"Error adding product to wishlist: {ex.Message}", Severity.Error);
            }
        }
    }

    public class ProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public decimal? DiscountPrice { get; set; }
        public decimal Rating { get; set; }
        public int TotalReviews { get; set; }
        public string Brand { get; set; } = string.Empty;
        public int StockQuantity { get; set; }
        public List<string> Images { get; set; } = new();
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
    }

    public class CategoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    public class ProductsResponseDto
    {
        public List<ProductDto> Products { get; set; } = new();
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
    }
}