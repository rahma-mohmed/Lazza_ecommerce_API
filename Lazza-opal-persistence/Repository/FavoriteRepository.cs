namespace Lazza.opal.persistence.Repository
{
    public class FavoriteRepository : IFavoriteRepository
    {
        private readonly AppDbContext _context; // Your EF DbContext

        public FavoriteRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Favorite> AddFavoriteAsync(Favorite favorite)
        {
            _context.Favorites.Add(favorite);
            await _context.SaveChangesAsync();
            return favorite;
        }

		public async Task<List<Product>> AllProductWithFavourites(string userId)
		{
			var products = await _context.Products.ToListAsync();

			var userFavorites = await _context.Favorites
				.Where(f => f.UserId == userId)
				.Select(f => f.ProductId)
				.ToListAsync();

			foreach (var product in products)
			{
				if (userFavorites.Contains(product.ProductId))
				{
					product.Favourite = true;
				}
				else
				{
					product.Favourite = false;
				}
			}

			return products;
		}
	

		public async Task<IEnumerable<Favorite>> GetFavoritesByUserIdAsync(string userId)
        {
            return await _context.Favorites
                .Where(f => f.UserId == userId).Include(f => f.Product)
                .ToListAsync();
        }

		public async Task<bool> UnfavoriteAsync(Favorite favorite)
		{
			var existingFavorite = await _context.Favorites
				.FirstOrDefaultAsync(f => f.UserId == favorite.UserId && f.ProductId == favorite.ProductId);

			if (existingFavorite == null)
			{
				return false; 
			}

			_context.Favorites.Remove(existingFavorite);

			var product = await _context.Products.FindAsync(favorite.ProductId);
			if (product != null)
			{
				product.Favourite = false;
			}

			await _context.SaveChangesAsync();
			return true;
		}
	}
}
