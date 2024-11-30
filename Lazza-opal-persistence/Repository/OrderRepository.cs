namespace Lazza.opal.persistence.Repository
{
	public class OrderRepository : IOrderRepository
	{
		private readonly AppDbContext _context;
		private readonly UserManager<ApplicationUser> _userManager;

		public OrderRepository(AppDbContext context, UserManager<ApplicationUser> userManager)
		{
			_context = context;
			_userManager = userManager;
		}

		public async Task<IEnumerable<Order>> GetAllAsync(string userid)
		{

			var orders = await _context.Orders
			.Where(o => o.UserId == userid)
			.Include(c => c.Cart)
			.Include(o => o.OrderProducts).ThenInclude(o => o.Products)
			.ToListAsync() ?? new List<Order>();

			foreach (var order in orders)
			{
				if (order.Status != "Confirmed")
				{
					if (DateTime.Now >= order.OrderDate.AddHours(2))
						order.Status = "Delivered";
					else if (DateTime.Now >= order.OrderDate.AddHours(1))
						order.Status = "Shipped";
				}
			}
			_context.Orders.UpdateRange(orders);
			await _context.SaveChangesAsync();

			var ordersWithProducts = orders.Select(order => new Order
			{
				Id = order.Id,
				OrderDate = order.OrderDate,
				Status = order.Status,
				CartId = order.CartId,
				UserId = order.UserId,
				Cart = order.Cart,
				OrderProducts = order.OrderProducts.Select(op => new OrderProduct
				{
					Id = op.Id,
					ProductId = op.ProductId,
					OrderId = op.OrderId,
					Products = op.Products

				}).ToList(),
				Count = order.Count,
				TotalPrice = order.TotalPrice
			}).ToList();

			return ordersWithProducts;
		}

		public async Task<Order> ConfirmOrderAsync(Order order)
		{
			var cartdata = await _context.Carts
				.Where(u => u.UserId == order.UserId)
				.Include(c => c.CartItems)
				.ThenInclude(c => c.Products)
				.Select(c => new { c.CartItems, c.CartId })
				.SingleOrDefaultAsync();

			if (cartdata == null)
				throw new InvalidOperationException("Cart not found for this user");

			var user = await _context.Users.OfType<ApplicationUser>().SingleOrDefaultAsync(u => u.Id == order.UserId);
			if (user != null)
			{
				user.OrderCount += 1;
				_context.Users.Update(user);
			}

			order.CartId = cartdata.CartId;
			order.UserId = user.Id;
			order.OrderDate = DateTime.Now;
			order.Status = "Pending";
			order.Count = cartdata.CartItems.Sum(ci => ci.Quantity);

			_context.Orders.Add(order);
			await _context.SaveChangesAsync();

			order.TotalPrice = cartdata.CartItems
			.Sum(ci => ci.Products.Price * ci.Quantity);

			var orderProducts = cartdata.CartItems.Select(ci => new OrderProduct
			{
				ProductId = ci.ProductId,
				OrderId = order.Id
			}).ToList();


			await _context.orderProducts.AddRangeAsync(orderProducts);

			var itemsToRemove = cartdata.CartItems.ToList();
			_context.CartItems.RemoveRange(itemsToRemove);
			await _context.SaveChangesAsync();


			return new Order
			{
				Id = order.Id,
				OrderDate = order.OrderDate,
				Status = order.Status,
				CartId = cartdata.CartId,
				UserId = order.UserId,
				Cart = new Cart
				{
					CartItems = cartdata.CartItems,
					CartId = order.CartId,
					UserId = order.UserId
				},
				Count = order.Count,
				TotalPrice = order.TotalPrice
			};
		}

		public async Task<Order> CancelOrderAsync(int orderId, string userId)
		{
			var order = await _context.Orders
				.Include(o => o.OrderProducts)
				.SingleOrDefaultAsync(o => o.Id == orderId && o.UserId == userId);

			if (order == null)
			{
				throw new InvalidOperationException("Order not found or does not belong to this user.");
			}

			if (order.Status == "Delivered")
			{
				throw new InvalidOperationException("Cannot cancel a delivered order.");
			}

			if (order.Status == "Canceled")
			{
				throw new InvalidOperationException("Order is already canceled.");
			}

			var orderProducts = order.OrderProducts.ToList();
			_context.orderProducts.RemoveRange(orderProducts);

			order.Status = "Canceled";
			_context.Orders.Update(order);

			await _context.SaveChangesAsync();

			return new Order
			{
				Id = order.Id,
				OrderDate = order.OrderDate,
				Status = order.Status,
				CartId = order.CartId,
				UserId = order.UserId,
				Count = order.Count,
				TotalPrice = order.TotalPrice
			};
		}
	}
}