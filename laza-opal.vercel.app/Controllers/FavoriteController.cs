namespace laza_opal.vercel.app.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FavoriteController : ControllerBase
    {
        private readonly IFavoriteRepository _favoriteRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public FavoriteController(IFavoriteRepository favoriteRepository , UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
            _favoriteRepository = favoriteRepository;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddFavorite([FromBody] Favorite favorite)
        {
			string UserId = _userManager.GetUserId(User);
			try
            {
                favorite.UserId = UserId;
                var addedFavorite = await _favoriteRepository.AddFavoriteAsync(favorite);
				return CreatedAtAction(nameof(GetFavoritesByUserId), new { userId = UserId }, addedFavorite);
			}
			catch (KeyNotFoundException ex)
			{
				return NotFound(ex.Message);
			}
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Favorite>>> GetFavoritesByUserId()
        {
            string userId = _userManager.GetUserId(User);
            var favorites = await _favoriteRepository.GetFavoritesByUserIdAsync(userId) ?? new List<Favorite>(); 
            return Ok(favorites);
        }

		[HttpDelete]
		[Authorize]
		public async Task<IActionResult> Unfavorite([FromBody] Favorite favorite)
		{
			string userId = _userManager.GetUserId(User);
			favorite.UserId = userId;

			var success = await _favoriteRepository.UnfavoriteAsync(favorite);
			if (!success)
			{
				return NotFound("Favorite not found for the specified product and user.");
			}

			return NoContent();
		}

		[HttpGet("AllProductsWithFavourites")]
		[Authorize]
		public async Task<ActionResult<List<Product>>> AllProductWithFavourites()
		{
			string userId = _userManager.GetUserId(User);
			var products = await _favoriteRepository.AllProductWithFavourites(userId) ?? new List<Product>(); ;
			return Ok(products);
		}
	}
}

