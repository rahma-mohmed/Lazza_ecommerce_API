namespace laza_opal.vercel.app.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class StripePaymentController : ControllerBase
	{
		private readonly StripePaymentService _stripePaymentService;
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly AppDbContext _context;

		public StripePaymentController(AppDbContext context, StripePaymentService stripePaymentService, UserManager<ApplicationUser> userManager)
		{
			_stripePaymentService = stripePaymentService;
			_userManager = userManager;
			_context = context;
		}

		[HttpPost("create-session")]
		[Authorize]
		public async Task<IActionResult> CreatePaymentSession()
		{

			string userId = _userManager.GetUserId(User); ;
			int cartId = await _context.Carts
								   .Where(c => c.UserId == userId)
								   .Select(c => c.CartId)
								   .FirstOrDefaultAsync();

			Cart cart = await _context.Carts.Where(c => c.CartId == cartId).Include(c => c.CartItems).ThenInclude(c => c.Products).FirstOrDefaultAsync();

			if (cart == null && cart.CartItems.Count == 0)
				return BadRequest("No items provided for payment.");

			var successUrl = "https://lazza-opal-vercel-app.runasp.net/api/StripePayment/payment-success?sessionId={CHECKOUT_SESSION_ID}";
			var cancelUrl = "https://lazza-opal-vercel-app.runasp.net/api/StripePayment/payment-cancel";

			try
			{
				var (sessionUrl, sessionId) = await _stripePaymentService.CreatePaymentSession(cart.CartItems.ToList(), "usd", successUrl, cancelUrl, userId, cartId);
				return Ok(new { url = sessionUrl, id = sessionId });
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { message = ex.Message });
			}
		}

		[HttpGet("payment-success")]
		public async Task<IActionResult> PaymentSuccess(string sessionId)
		{
			try
			{
				await _stripePaymentService.ProcessOrderAsync(sessionId);
				return Ok(new { message = "Payment successful" });
			}
			catch (Exception ex)
			{
				return BadRequest(new { message = ex.Message });
			}
		}

		[HttpGet("payment-cancel")]
		public IActionResult PaymentCancel()
		{
			return Ok(new { message = "Payment was canceled by the user." });
		}
	}
}