namespace laza_opal.vercel.app.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ICartRepository _cartRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public CartController(ICartRepository cartRepository , UserManager<ApplicationUser>usermanager)
        {
            _cartRepository = cartRepository;
            _userManager = usermanager;
        }


        [HttpPost("add-item")]
        [Authorize]
        public async Task<IActionResult> AddCartItem([FromBody] CartItem cartItem)
        {
            var userId = _userManager.GetUserId(User);

            if (cartItem == null)
            {
                return BadRequest("Cart item cannot be null.");
            }
            var addedItem = await _cartRepository.AddCartItemAsync(userId , cartItem);
            return CreatedAtAction(nameof(GetCartItemsByUserId), new { userId = addedItem.Cart.UserId }, addedItem);
        }

        [HttpPut("{ItemId}")]
        [Authorize]
		public async Task<IActionResult> UpdateCartItemsQuantity(int ItemId, [FromBody] int quantity)
		{
			try
			{
				var updatedCartItems = await _cartRepository.UpdateCartItemsQuantityAsync(ItemId, quantity);
				return Ok(updatedCartItems);
			}
			catch (Exception ex)
			{
				return NotFound(ex.Message); 
			}
		}


		[HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteCartItem(int id)
        {
            var result = await _cartRepository.DeleteCartItemAsync(id);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Cart>>> GetCartItemsByUserId()
        {
            var userId = _userManager.GetUserId(User);
            var items = await _cartRepository.GetCartItemsByUserIdAsync(userId) ?? new List<Cart>();
            return Ok(items);
        }
    }
}
