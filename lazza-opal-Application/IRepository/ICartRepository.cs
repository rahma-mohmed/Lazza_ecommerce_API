namespace Lazza.opal.application.IRepository
{
     public interface ICartRepository
     {
            Task<CartItem> AddCartItemAsync(string userId , CartItem cartItem);
            Task<CartItem> UpdateCartItemsQuantityAsync(int cartId, int quantity);
			Task<bool> DeleteCartItemAsync(int cartItemId);
            Task<IEnumerable<Cart>> GetCartItemsByUserIdAsync(string userId);
            Task<Cart> GetCartByUserIdAsync(string userId);

     }
}

