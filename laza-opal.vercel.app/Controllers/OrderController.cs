namespace laza_opal.vercel.app.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderRepository _orderRepository;
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly AppDbContext _context;

		public OrderController(IOrderRepository orderRepository , UserManager<ApplicationUser> userManager , AppDbContext context)
        {
            _orderRepository = orderRepository;
            _userManager = userManager;
            _context = context;
        }

        [HttpGet("GetOrder")]
		[Authorize]
		public async Task<IActionResult> GetCard()
        {
			string userId = _userManager.GetUserId(User);
            var orders = await _orderRepository.GetAllAsync(userId);
			var result = new
			{
				Count = orders.Count(),
				Orders = orders
			};

			return Ok(result);
		}
        
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> ConfirmOrder(Order order)
        {
			string userId = _userManager.GetUserId(User);

			if (!ModelState.IsValid)
                return BadRequest(ModelState);

            order.UserId = userId;
			var confirmedOrder = await _orderRepository.ConfirmOrderAsync(order);
			return Ok(new { message = "Order confirmed successfully", confirmedOrder });
        }

		[HttpDelete("CancelOrder/{orderId}")]
		[Authorize]
		public async Task<IActionResult> CancelOrder(int orderId)
		{
			string userId = _userManager.GetUserId(User);

			try
			{
				var canceledOrder = await _orderRepository.CancelOrderAsync(orderId, userId);
				return Ok(new { message = "Order canceled successfully", canceledOrder });
			}
			catch (InvalidOperationException ex)
			{
				return BadRequest(new { message = ex.Message });
			}
		}
	}
}
