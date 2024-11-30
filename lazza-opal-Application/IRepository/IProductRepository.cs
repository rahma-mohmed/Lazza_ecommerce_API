namespace Lazza.opal.application.IRepository
{
    public interface IProductRepository
    {
        Task<Product> AddProductAsync(Product product, IFormFile[] images);
        Task<IEnumerable<Product>> GetAllProductsAsync();
        Task<Product> GetProductByIdAsync(int id);
        Task UpdateProductAsync(Product product, IFormFile[]? images);
        Task DeleteProductAsync(int id);
        Task<IEnumerable<Product>> GetAllProductInBrandAsync(int brandid);
		Task<IEnumerable<Product>> SearchProductsAsync(string? productName, string? brandName);
	}
}
