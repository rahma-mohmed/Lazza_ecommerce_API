namespace Lazza.opal.persistence.Repository
{
    public class CartRepository : ICartRepository
    {
        private readonly AppDbContext _context;

        public CartRepository(AppDbContext context)
        {
            _context = context;
        }

        // Add Cart Item
        [Authorize]
        public async Task<CartItem> AddCartItemAsync(string userId,CartItem cartItem)
        {

            var cart = await _context.Carts.Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
            {
                cart = new Cart
                {
                    UserId = userId, 
                    CartItems = new List<CartItem>() 
                };
                _context.Carts.Add(cart);
                await _context.SaveChangesAsync();
            }

			var existingCartItem = cart.CartItems
		    .FirstOrDefault(ci => ci.ProductId == cartItem.ProductId && ci.Size == cartItem.Size);


			if (existingCartItem != null)
			{
				existingCartItem.Quantity += 1;
			}
			else
			{
				cartItem.Quantity = 1;
				cartItem.CartId = cart.CartId;
				cart.CartItems.Add(cartItem);
			}

			await _context.SaveChangesAsync();
			return existingCartItem ?? cartItem;
        }


        // Update Cart Item
        public async Task<CartItem> UpdateCartItemsQuantityAsync(int cartId, int quantity)
        {
            var existingCartItem = await _context.CartItems.FindAsync(cartId);
            existingCartItem.Quantity = quantity;
            _context.CartItems.Update(existingCartItem);
			await _context.SaveChangesAsync();
            return existingCartItem;
        }

        // Delete Cart Item
        public async Task<bool> DeleteCartItemAsync(int cartItemId)
        {
            var cartItem = await _context.CartItems.FindAsync(cartItemId);
            if (cartItem != null)
            {
                _context.CartItems.Remove(cartItem);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        // Get Cart Items by User ID
        public async Task<IEnumerable<Cart>> GetCartItemsByUserIdAsync(string userId)
        {
            return await _context.Carts
                .Where(c => c.UserId == userId)
                .Include(c => c.CartItems).
                ThenInclude(c => c.Products)
                .ToListAsync();
        }

		public async Task<Cart> GetCartByUserIdAsync(string userId)
        {
            return await _context.Carts.Where(c => c.UserId == userId).FirstOrDefaultAsync();

		}
	}
}
