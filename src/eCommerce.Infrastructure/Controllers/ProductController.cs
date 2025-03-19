using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using eCommerce.Core.Interfaces;
using eCommerce.Core.Models;
using Microsoft.Extensions.Logging;

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
            var product = await _productService.GetProductById(id);
            if (product == null)
                return NotFound();

            return Ok(product);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var products = await _productService.GetAllProducts(page, pageSize);
            return Ok(products);
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<Product>>> SearchProducts(
            [FromQuery] string searchTerm,
            [FromQuery] string? category = null,
            [FromQuery] decimal? minPrice = null,
            [FromQuery] decimal? maxPrice = null,
            [FromQuery] string? sortBy = null)
        {
            try
            {
                var products = await _productService.SearchProducts(searchTerm, category ?? "", minPrice, maxPrice, sortBy ?? "");
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
            var products = await _productService.GetFeaturedProducts();
            return Ok(products);
        }

        [HttpGet("category/{category}")]
        public async Task<ActionResult<IEnumerable<Product>>> GetProductsByCategory(string category)
        {
            var products = await _productService.GetProductsByCategory(category);
            return Ok(products);
        }

        [HttpGet("{id}/related")]
        public async Task<ActionResult<IEnumerable<Product>>> GetRelatedProducts(int id)
        {
            var products = await _productService.GetRelatedProducts(id);
            return Ok(products);
        }

        [HttpGet("{id}/reviews")]
        public async Task<ActionResult<IEnumerable<Review>>> GetProductReviews(int id)
        {
            var reviews = await _productService.GetProductReviews(id);
            return Ok(reviews);
        }

        [HttpGet("{id}/rating")]
        public async Task<ActionResult<decimal>> GetProductRating(int id)
        {
            var rating = await _productService.GetProductRating(id);
            return Ok(rating);
        }

        [HttpPost]
        public async Task<ActionResult<Product>> CreateProduct([FromBody] Product product)
        {
            var createdProduct = await _productService.CreateProduct(product);
            return CreatedAtAction(nameof(GetProduct), new { id = createdProduct.Id }, createdProduct);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] Product product)
        {
            if (id != product.Id)
                return BadRequest();

            await _productService.UpdateProduct(product);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            await _productService.DeleteProduct(id);
            return NoContent();
        }

        [HttpPost("{id}/reviews")]
        public async Task<IActionResult> AddProductReview(int id, [FromBody] Review review)
        {
            if (id != review.ProductId)
                return BadRequest();

            var success = await _productService.AddProductReview(review);
            if (!success)
                return BadRequest();

            return NoContent();
        }

        [HttpPut("{id}/stock")]
        public async Task<IActionResult> UpdateStock(int id, [FromBody] int quantity)
        {
            var success = await _productService.UpdateStock(id, quantity);
            if (!success)
                return BadRequest();

            return NoContent();
        }

        [HttpGet("{id}/stock")]
        public async Task<ActionResult<bool>> IsInStock(int id, [FromQuery] int quantity = 1)
        {
            var inStock = await _productService.IsProductInStock(id, quantity);
            return Ok(inStock);
        }
    }
} 