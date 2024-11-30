namespace Lazza.opal.application.IRepository
{
    public interface IFavoriteRepository
    {
        Task<Favorite> AddFavoriteAsync(Favorite favorite);
        Task<IEnumerable<Favorite>> GetFavoritesByUserIdAsync(string userId);
        Task<bool> UnfavoriteAsync(Favorite favorite);
        Task<List<Product>> AllProductWithFavourites(string userId);
	}
}
