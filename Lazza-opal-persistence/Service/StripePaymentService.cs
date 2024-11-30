using Stripe;
using Stripe.Checkout;
using Microsoft.Extensions.Options;

namespace Lazza.opal.persistence.Service.StripePaymentService
{
	public class StripePaymentService
	{
		private readonly AppDbContext _context;
		private readonly StripeSettings _stripeSettings;

		public StripePaymentService(AppDbContext context, IOptions<StripeSettings> stripeOptions)
		{
			_context = context;
			_stripeSettings = stripeOptions.Value;
			StripeConfiguration.ApiKey = _stripeSettings.SecretKey;
		}

		// Create payment session
		public async Task<(string sessionUrl, string sessionId)> CreatePaymentSession(List<CartItem> cartItems, string currency, string successUrl, string cancelUrl, string userId, int cartId)
		{
			var options = new SessionCreateOptions
			{
				PaymentMethodTypes = new List<string> { "card" },
				LineItems = cartItems.Select(item =>
				{
					// Check if Products is null
					if (item.Products == null)
					{
						throw new ArgumentNullException(nameof(item.Products), "Product details are missing for cart item.");
					}

					return new SessionLineItemOptions
					{
						PriceData = new SessionLineItemPriceDataOptions
						{
							UnitAmount = (long)(item.Products.Price * 100), // Amount in cents
							Currency = currency,
							ProductData = new SessionLineItemPriceDataProductDataOptions
							{
								Name = item.Products.ProductName
							},
						},
						Quantity = item.Quantity,
					};
				}).ToList(),
				Mode = "payment",
				SuccessUrl = successUrl,
				CancelUrl = cancelUrl,
				Metadata = new Dictionary<string, string>
				{
					{ "userId", userId },
					{ "cartId", cartId.ToString() }
				}
			};

			var service = new SessionService();
			var session = await service.CreateAsync(options);
			return (session.Url , session.Id); 
		}

		public async Task ProcessOrderAsync(string sessionId)
		{
			var service = new SessionService();
			var session = await service.GetAsync(sessionId);

			if (session == null)
				throw new Exception("Payment session not found.");

			var userId = session.Metadata["userId"];


			//var orders = await _context.Orders.Where(o => o.UserId == userId).ToListAsync();

			//if (orders.Any())
			//{
			//	foreach (var order in orders)
			//	{
			//		order.Status = "Confirmed";
			//	}

			//	_context.Orders.UpdateRange(orders);
			//	await _context.SaveChangesAsync();
			//}


			var user = await _context.Users.OfType<ApplicationUser>().SingleOrDefaultAsync(u => u.Id == userId);
			if (user != null)
			{
				user.OrderCount += 1;
				_context.Users.Update(user);
			}

			var cartId = int.Parse(session.Metadata["cartId"]);

			var cart = await _context.Carts
				.Include(c => c.CartItems)
				.ThenInclude(ci => ci.Products)
				.FirstOrDefaultAsync(c => c.CartId == cartId && c.UserId == userId);

			if (cart.CartItems == null)
				throw new Exception("No Product in Cart");

			decimal Price = cart.CartItems
			.Sum(ci => ci.Products.Price * ci.Quantity);

			var order = new Order
			{
				UserId = userId,
				CartId = cart.CartId,
				OrderDate = DateTime.UtcNow,
				Status = "Confirmed",
				Count = cart.CartItems.Sum(ci => ci.Quantity),
				TotalPrice = Price
			};

			await _context.Orders.AddAsync(order);
			await _context.SaveChangesAsync();

			var orderProducts = cart.CartItems.Select(ci => new OrderProduct
			{
				ProductId = ci.ProductId,
				OrderId = order.Id
			}).ToList();

			await _context.orderProducts.AddRangeAsync(orderProducts);
			_context.CartItems.RemoveRange(cart.CartItems);
			await _context.SaveChangesAsync();
		}

		public async Task<Session> GetSessionAsync(string sessionId)
		{
			var service = new SessionService();
			var session = await service.GetAsync(sessionId);
			return session;
		}
	}
}