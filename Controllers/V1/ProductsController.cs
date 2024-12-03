using Microsoft.AspNetCore.Mvc;
namespace ProductApi.Controllers.V1
{
    [ApiController]
    [Route("v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    public class ProductsController : ControllerBase
    {
        private static readonly List<Product> _products =
        [
            new Product { Id = 1, Name = "Classic Product", Price = 19.99m }
        ];

        [HttpGet]
        public ActionResult<IEnumerable<Product>> GetProducts()
        {
            return Ok(_products);
        }

        [HttpGet("{id}")]
        public ActionResult<Product> GetProduct(int id)
        {
            var product = _products.FirstOrDefault(p => p.Id == id);
            if (product == null) return NotFound();
            return Ok(product);
        }

        [HttpPost]
        public ActionResult<Product> CreateProduct(Product product)
        {
            product.Id = _products.Count + 1;
            _products.Add(product);
            return CreatedAtAction(nameof(GetProduct), new { id = product.Id, version = "1.0" }, product);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateProduct(int id, Product product)
        {
            if (id != product.Id) return BadRequest();

            var existingProduct = _products.FirstOrDefault(p => p.Id == id);
            if (existingProduct == null) return NotFound();

            var index = _products.IndexOf(existingProduct);
            _products[index] = product;

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteProduct(int id)
        {
            var product = _products.FirstOrDefault(p => p.Id == id);
            if (product == null) return NotFound();

            _products.Remove(product);
            return NoContent();
        }
    }
}