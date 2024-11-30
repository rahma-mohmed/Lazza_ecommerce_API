namespace laza_opal.vercel.app.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductRepository _productRepository;

        public ProductController(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        [HttpPost]
        public async Task<IActionResult> AddProduct([FromForm] ProductDto productDto)
        {
            if (productDto == null)
            {
                return BadRequest("Product cannot be null.");
            }

            var product = new Product
            {
                ProductName = productDto.ProductName,
                Price = productDto.Price,
                SmallQuant = productDto.SmallQuant,
                MQuant = productDto.MQuant,
                LQuant = productDto.LQuant,
                XLQuant = productDto.XLQuant,
                TwoXLQuant = productDto.TwoXLQuant,
                Description = productDto.Description,
                Gender = productDto.Gender,
                BrandId = productDto.BrandId
            };

            var addedProduct = await _productRepository.AddProductAsync(product, productDto.Images);
            return CreatedAtAction(nameof(GetProductById), new { id = addedProduct.ProductId }, addedProduct);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetAllProducts()
        {
            var products = await _productRepository.GetAllProductsAsync() ?? new List<Product>(); 
			return Ok(products);
        }

        // READ: Get a product by its ID
        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProductById(int id)
        {
            var product = await _productRepository.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound($"Product with ID {id} not found.");
            }
            return Ok(product);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromForm] ProductDto productDto)
        {
            var existingProduct = await _productRepository.GetProductByIdAsync(id);
            if (existingProduct == null)
            {
                return NotFound();
            }
            existingProduct.ProductName = productDto.ProductName;
            existingProduct.Price = productDto.Price;
            existingProduct.SmallQuant = productDto.SmallQuant;
            existingProduct.MQuant = productDto.MQuant;
            existingProduct.LQuant = productDto.LQuant;
            existingProduct.XLQuant = productDto.XLQuant;
            existingProduct.TwoXLQuant = productDto.TwoXLQuant;
            existingProduct.Description = productDto.Description;
            existingProduct.Gender = productDto.Gender;
            existingProduct.BrandId = productDto.BrandId;

            // Handle image updates
            await _productRepository.UpdateProductAsync(existingProduct, productDto.Images);

            return Ok(existingProduct);
        }

        // DELETE: Delete a product by its ID
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var existingProduct = await _productRepository.GetProductByIdAsync(id);
            if (existingProduct == null)
            {
                return NotFound($"Product with ID {id} not found.");
            }

            await _productRepository.DeleteProductAsync(id);
            return NoContent(); 
        }

        [HttpGet("GetProductByBrandId/{brandid}")]
        public async Task<IActionResult> GetProductByBrandId(int brandid)
        {
            var res = await _productRepository.GetAllProductInBrandAsync(brandid) ?? new List<Product>(); 
			return Ok(res);
        }

		[HttpGet("Search")]
		public async Task<ActionResult<IEnumerable<Product>>> SearchProducts(
	    [FromQuery] string? productName, [FromQuery] string? brandName)
		{
			var products = await _productRepository.SearchProductsAsync(productName, brandName);
			if (!products.Any())
			{
				return NotFound("No products found matching the search criteria.");
			}

			return Ok(products);
		}
	}
}

