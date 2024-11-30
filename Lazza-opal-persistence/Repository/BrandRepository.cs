namespace Lazza.opal.persistence.Repository
{
    public class BrandRepository : IBrandRepository
    {
        private readonly AppDbContext _context; 

        public BrandRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Brand> AddBrandAsync(Brand brand)
        {
            _context.Brands.Add(brand);
            await _context.SaveChangesAsync();
            return brand;
        }

        public async Task<IEnumerable<Brand>> GetAllBrandsAsync()
        {
            return await _context.Brands.ToListAsync();
        }

        public async Task<Brand?> GetBrandByIdAsync(int id)
        {
            return await _context.Brands.FirstOrDefaultAsync(b => b.BrandId == id);
        }

        public async Task<Brand> UpdateBrandAsync(Brand brand)
        {
            var existingBrand = await _context.Brands.FindAsync(brand.BrandId);
            if (existingBrand != null)
            {
                existingBrand.BrandName = brand.BrandName;
                existingBrand.BrandImage = brand.BrandImage;
                await _context.SaveChangesAsync();
            }
            return existingBrand;
        }

        public async Task<bool> DeleteBrandAsync(int id)
        {
            var brand = await _context.Brands.FindAsync(id);
            if (brand != null)
            {
                _context.Brands.Remove(brand);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }
    }
}

