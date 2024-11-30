namespace laza_opal.vercel.app.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BrandController : ControllerBase
    {
        private readonly IBrandRepository _brandRepository;

        public BrandController(IBrandRepository brandRepository)
        {
            _brandRepository = brandRepository;
        }

        [HttpPost]
        public async Task<IActionResult> AddBrand([FromBody] Brand brand)
        {
            var addedBrand = await _brandRepository.AddBrandAsync(brand);
            return CreatedAtAction(nameof(GetAllBrands), new { id = addedBrand.BrandId }, addedBrand);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Brand>>> GetAllBrands()
        {
            var brands = await _brandRepository.GetAllBrandsAsync();
            return Ok(brands);
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<Brand>> GetBrandById(int id)
        {
            var brand = await _brandRepository.GetBrandByIdAsync(id);
            if (brand == null)
            {
                return NotFound();
            }
            return Ok(brand);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBrand(int id, [FromBody] Brand brand)
        {
            if (id != brand.BrandId)
            {
                return BadRequest("Brand ID mismatch.");
            }

            var updatedBrand = await _brandRepository.UpdateBrandAsync(brand);
            if (updatedBrand == null)
            {
                return NotFound();
            }

            return Ok(updatedBrand);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBrand(int id)
        {
            var result = await _brandRepository.DeleteBrandAsync(id);
            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}

