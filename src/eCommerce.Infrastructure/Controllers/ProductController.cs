using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using eCommerce.Core.Interfaces;
using eCommerce.Core.Models;
using Microsoft.Extensions.Logging;
using eCommerce.Core.DTOs.Product;
using System.Drawing.Printing;

namespace eCommerce.Infrastructure.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly ILogger<ProductController> _logger;

        public ProductController(IProductService productService, ILogger<ProductController> logger)
        {
            _productService = productService;
            _logger = logger;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
                return NotFound();

            return Ok(product);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts([FromQuery] ProductFilterDto filter, [FromQuery] int page=1 , [FromQuery] int pageSize = 10)
        {
            var products = await _productService.GetProductsAsync(filter,page, pageSize);
            return Ok(products);
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<Product>>> SearchProducts(
            [FromQuery] string searchTerm,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)

        {
            try
            {
                var products = await _productService.SearchProductsAsync(searchTerm, page, pageSize);
                return Ok(products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching products");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("featured")]
        public async Task<ActionResult<IEnumerable<Product>>> GetFeaturedProducts()
        {
            var products = await _productService.GetFeaturedProductsAsync();
            return Ok(products);
        }

        [HttpGet("category/{category}")]
        public async Task<ActionResult<IEnumerable<Product>>> GetProductsByCategory(int category)
        {
            var products = await _productService.GetProductsByCategoryAsync(category);
            return Ok(products);
        }

        [HttpGet("{id}/related")]
        public async Task<ActionResult<IEnumerable<Product>>> GetRelatedProducts(int id)
        {
            var products = await _productService.GetRelatedProductsAsync(id);
            return Ok(products);
        }

      

        [HttpPost]
        public async Task<ActionResult<Product>> CreateProduct([FromBody] CreateProductDto product)
        {
            var createdProduct = await _productService.CreateProductAsync(product);
            return CreatedAtAction(nameof(GetProduct), new { id = createdProduct.Id }, createdProduct);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int Id, [FromBody] UpdateProductDto product)
        {

            await _productService.UpdateProductAsync(Id, product);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            await _productService.DeleteProductAsync(id);
            return NoContent();
        }


        [HttpPut("{id}/stock")]
        public async Task<IActionResult> UpdateStock(int id, [FromBody] int quantity)
        {
            var success = await _productService.UpdateProductStockAsync(id, quantity);
            if (!success)
                return BadRequest();

            return NoContent();
        }

        [HttpGet("{id}/stock")]
        public async Task<ActionResult<bool>> IsInStock(int id, [FromQuery] bool isActive = true)
        {
            var inStock = await _productService.UpdateProductStatusAsync(id, isActive);
            return Ok(inStock);
        }
    }
} 