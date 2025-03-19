using Microsoft.AspNetCore.Mvc;
using eCommerce.Core.Interfaces;
using eCommerce.Core.Models;

namespace eCommerce.API.Controllers
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
            try
            {
                var product = await _productService.GetProductById(id);
                if (product == null)
                    return NotFound();

                return Ok(product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving product with ID {id}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var products = await _productService.GetAllProducts(page, pageSize);
                return Ok(products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving products");
                return StatusCode(500, "Internal server error");
            }
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
                var products = await _productService.SearchProducts(searchTerm, category, minPrice, maxPrice, sortBy);
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
            try
            {
                var products = await _productService.GetFeaturedProducts();
                return Ok(products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving featured products");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("category/{category}")]
        public async Task<ActionResult<IEnumerable<Product>>> GetProductsByCategory(string category)
        {
            try
            {
                var products = await _productService.GetProductsByCategory(category);
                return Ok(products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving products for category {category}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        public async Task<ActionResult<Product>> CreateProduct([FromBody] Product product)
        {
            try
            {
                var createdProduct = await _productService.CreateProduct(product);
                return CreatedAtAction(nameof(GetProduct), new { id = createdProduct.Id }, createdProduct);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating product");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] Product product)
        {
            try
            {
                if (id != product.Id)
                    return BadRequest();

                await _productService.UpdateProduct(product);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating product with ID {id}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            try
            {
                await _productService.DeleteProduct(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting product with ID {id}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}/reviews")]
        public async Task<ActionResult<IEnumerable<Review>>> GetProductReviews(int id)
        {
            try
            {
                var reviews = await _productService.GetProductReviews(id);
                return Ok(reviews);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving reviews for product with ID {id}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("{id}/reviews")]
        public async Task<IActionResult> AddProductReview(int id, [FromBody] Review review)
        {
            try
            {
                if (id != review.ProductId)
                    return BadRequest();

                var success = await _productService.AddProductReview(review);
                if (!success)
                    return BadRequest();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error adding review for product with ID {id}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}/rating")]
        public async Task<ActionResult<decimal>> GetProductRating(int id)
        {
            try
            {
                var rating = await _productService.GetProductRating(id);
                return Ok(rating);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving rating for product with ID {id}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}/stock")]
        public async Task<IActionResult> UpdateStock(int id, [FromBody] int quantity)
        {
            try
            {
                var success = await _productService.UpdateStock(id, quantity);
                if (!success)
                    return BadRequest();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating stock for product with ID {id}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}/stock")]
        public async Task<ActionResult<bool>> IsInStock(int id, [FromQuery] int quantity = 1)
        {
            try
            {
                var inStock = await _productService.IsProductInStock(id, quantity);
                return Ok(inStock);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error checking stock for product with ID {id}");
                return StatusCode(500, "Internal server error");
            }
        }
    }
} 