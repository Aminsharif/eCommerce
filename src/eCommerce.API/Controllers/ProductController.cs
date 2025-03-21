using eCommerce.Core.DTOs.Product;
using eCommerce.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace eCommerce.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public async Task<ActionResult<ProductListDto>> GetProducts([FromQuery] ProductFilterDto filter)
        {
            var products = await _productService.GetProductsAsync(filter);
            return Ok(products);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDto>> GetProduct(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
                return NotFound();

            return Ok(product);
        }

        [HttpGet("category/{categoryId}")]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetProductsByCategory(int categoryId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var products = await _productService.GetProductsByCategoryAsync(categoryId, page, pageSize);
            return Ok(products);
        }

        [HttpGet("vendor/{vendorId}")]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetProductsByVendor(int vendorId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var products = await _productService.GetProductsByVendorAsync(vendorId, page, pageSize);
            return Ok(products);
        }

        [HttpGet("featured")]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetFeaturedProducts([FromQuery] int count = 10)
        {
            var products = await _productService.GetFeaturedProductsAsync(count);
            return Ok(products);
        }

        [HttpGet("new-arrivals")]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetNewArrivals([FromQuery] int count = 10)
        {
            var products = await _productService.GetNewArrivalsAsync(count);
            return Ok(products);
        }

        [HttpGet("best-sellers")]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetBestSellers([FromQuery] int count = 10)
        {
            var products = await _productService.GetBestSellersAsync(count);
            return Ok(products);
        }

        [HttpGet("{id}/related")]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetRelatedProducts(int id, [FromQuery] int count = 4)
        {
            var products = await _productService.GetRelatedProductsAsync(id, count);
            return Ok(products);
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<ProductDto>>> SearchProducts([FromQuery] string searchTerm, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var products = await _productService.SearchProductsAsync(searchTerm, page, pageSize);
            return Ok(products);
        }

        [Authorize(Roles = "Admin,Vendor")]
        [HttpPost]
        public async Task<ActionResult<ProductDto>> CreateProduct(CreateProductDto productDto)
        {
            var product = await _productService.CreateProductAsync(productDto);
            return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
        }

        [Authorize(Roles = "Admin,Vendor")]
        [HttpPut("{id}")]
        public async Task<ActionResult<ProductDto>> UpdateProduct(int id, UpdateProductDto productDto)
        {
            var product = await _productService.UpdateProductAsync(id, productDto);
            if (product == null)
                return NotFound();

            return Ok(product);
        }

        [Authorize(Roles = "Admin,Vendor")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var result = await _productService.DeleteProductAsync(id);
            if (!result)
                return NotFound();

            return NoContent();
        }

        [Authorize(Roles = "Admin,Vendor")]
        [HttpPut("{id}/stock")]
        public async Task<IActionResult> UpdateStock(int id, [FromBody] int quantity)
        {
            var result = await _productService.UpdateProductStockAsync(id, quantity);
            if (!result)
                return NotFound();

            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] bool isActive)
        {
            var result = await _productService.UpdateProductStatusAsync(id, isActive);
            if (!result)
                return NotFound();

            return NoContent();
        }
    }
} 