namespace Lazza.opal.persistence.Repository
{
    public class ProductRepository : IProductRepository
    {
        private readonly AppDbContext _context;
        private readonly IImageService _imageService;

        public ProductRepository(AppDbContext context, IImageService imageService)
        {
            _context = context;
            _imageService = imageService;
        }

        public async Task<Product> AddProductAsync(Product product, IFormFile[] images)
        {

            if (images != null && images.Length > 0)
            {
                for (int i = 0; i < Math.Min(images.Length, 5); i++)
                {
                    string folderPath = @"Images/ProductImages/";
                    string savedImagePath = await _imageService.SaveImageAsync(images[i], folderPath);
                    if (i == 0) product.Img1 = savedImagePath;
                    else if (i == 1) product.Img2 = savedImagePath;
                    else if (i == 2) product.Img3 = savedImagePath;
                    else if (i == 3) product.Img4 = savedImagePath;
                    else if (i == 4) product.Img5 = savedImagePath;
                }
            }
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return product;
        }

        public async Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            return await _context.Products.Include(p => p.Brand).ToListAsync();
        }

        // READ: Get a product by its ID
        public async Task<Product> GetProductByIdAsync(int id)
        {
            return await _context.Products.Include(p => p.Brand).FirstOrDefaultAsync(p => p.ProductId == id);
        }

        // UPDATE: Update an existing product
        public async Task UpdateProductAsync(Product product, IFormFile[]? images)
        {
            if (images != null && images.Length > 0)
            {
                for (int i = 0; i < Math.Min(images.Length, 5); i++)
                {
                    string folderPath = @"Images/ProductImages/";
                    string savedImagePath = await _imageService.SaveImageAsync(images[i], folderPath);
                    if (i == 0) product.Img1 = savedImagePath;
                    else if (i == 1) product.Img2 = savedImagePath;
                    else if (i == 2) product.Img3 = savedImagePath;
                    else if (i == 3) product.Img4 = savedImagePath;
                    else if (i == 4) product.Img5 = savedImagePath;
                }
            }
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
        }

        // DELETE: Delete a product by its ID
        public async Task DeleteProductAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                if (product != null)
                {
                    // Optional: Delete images if needed
                    await _imageService.DeleteFileAsync(product.Img1);
                    await _imageService.DeleteFileAsync(product.Img2);
                    await _imageService.DeleteFileAsync(product.Img3);
                    await _imageService.DeleteFileAsync(product.Img4);
                    await _imageService.DeleteFileAsync(product.Img5);

                    _context.Products.Remove(product);
                    await _context.SaveChangesAsync();
                }
            }
        }

        public async Task<IEnumerable<Product>> GetAllProductInBrandAsync(int brandid)
        {
            var products = await _context.Products.Where(b => b.BrandId == brandid).ToListAsync();
            return products;
        }

		public async Task<IEnumerable<Product>> SearchProductsAsync(string? productName, string? brandName)
		{
			var query = _context.Products.AsQueryable();

			if (!string.IsNullOrEmpty(productName))
			{
				query = query.Where(p => p.ProductName.Contains(productName));
			}

			if (!string.IsNullOrEmpty(brandName))
			{
				query = query.Include(p => p.Brand)
							 .Where(p => p.Brand != null && p.Brand.BrandName.Contains(brandName));
			}
			return await query.ToListAsync();
		}
	}
}
