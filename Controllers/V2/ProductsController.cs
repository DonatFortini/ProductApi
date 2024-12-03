using Microsoft.AspNetCore.Mvc;
namespace ProductApi.Controllers.V2
{
    [ApiController]
    [Route("v{version:apiVersion}/[controller]")]
    [ApiVersion("2.0")]
    public class ProductsController : ControllerBase
    {
        private static readonly List<ProductV2> _products =
        [
            new ProductV2
            {
                Id = 1,
                Name = "Enhanced Product",
                Price = 29.99m,
                Description = "An enhanced version of our classic product",
                Category = "Premium",
                StockQuantity = 100,
                CreatedDate = DateTime.UtcNow,
                LastModifiedDate = DateTime.UtcNow
            },
            new ProductV2
            {
                Id = 2,
                Name = "Premium Product",
                Price = 39.99m,
                Description = "Our premium offering with advanced features",
                Category = "Luxury",
                StockQuantity = 50,
                CreatedDate = DateTime.UtcNow,
                LastModifiedDate = DateTime.UtcNow
            }
        ];

        [HttpGet]
        public ActionResult<IEnumerable<ProductV2>> GetProducts([FromQuery] string? category = null)
        {
            var query = _products.AsQueryable();

            if (!string.IsNullOrWhiteSpace(category))
            {
                query = query.Where(p => p.Category.Equals(category, StringComparison.OrdinalIgnoreCase));
            }

            return Ok(query.ToList());
        }

        [HttpGet("{id}")]
        public ActionResult<ProductV2> GetProduct(int id)
        {
            var product = _products.FirstOrDefault(p => p.Id == id);
            if (product == null) return NotFound();
            return Ok(product);
        }

        [HttpPost]
        public ActionResult<ProductV2> CreateProduct(ProductV2 product)
        {
            product.Id = _products.Count + 1;
            product.CreatedDate = DateTime.UtcNow;
            product.LastModifiedDate = DateTime.UtcNow;

            if (product.StockQuantity < 0)
                return BadRequest("Stock quantity cannot be negative");

            _products.Add(product);
            return CreatedAtAction(nameof(GetProduct), new { id = product.Id, version = "2.0" }, product);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateProduct(int id, ProductV2 product)
        {
            if (id != product.Id) return BadRequest();

            var existingProduct = _products.FirstOrDefault(p => p.Id == id);
            if (existingProduct == null) return NotFound();

            if (product.StockQuantity < 0)
                return BadRequest("Stock quantity cannot be negative");

            product.CreatedDate = existingProduct.CreatedDate;
            product.LastModifiedDate = DateTime.UtcNow;

            var index = _products.IndexOf(existingProduct);
            _products[index] = product;

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteProduct(int id)
        {
            var product = _products.FirstOrDefault(p => p.Id == id);
            if (product == null) return NotFound();

            if (product.StockQuantity > 0)
                return BadRequest("Cannot delete product with remaining stock");

            _products.Remove(product);
            return NoContent();
        }

        [HttpPatch("{id}/stock")]
        public IActionResult UpdateStock(int id, [FromBody] int quantity)
        {
            var product = _products.FirstOrDefault(p => p.Id == id);
            if (product == null) return NotFound();

            if (product.StockQuantity + quantity < 0)
                return BadRequest("Cannot reduce stock below 0");

            product.StockQuantity += quantity;
            product.LastModifiedDate = DateTime.UtcNow;

            return Ok(product);
        }
    }
}