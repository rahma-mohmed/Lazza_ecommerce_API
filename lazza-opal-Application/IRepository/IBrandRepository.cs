namespace Lazza.opal.application.IRepository
{
    public interface IBrandRepository
    {
        Task<Brand> AddBrandAsync(Brand brand);
        Task<IEnumerable<Brand>> GetAllBrandsAsync();
        Task<Brand?> GetBrandByIdAsync(int id); 
        Task<Brand> UpdateBrandAsync(Brand brand); 
        Task<bool> DeleteBrandAsync(int id); 
    }
}
