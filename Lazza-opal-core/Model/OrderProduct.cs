namespace Lazza.opal.core.Model
{
	public class OrderProduct
	{
		public int Id { get; set; }

		[ForeignKey("Order")]
		public int OrderId { get; set; }
		public Order? Order { get; set; }

		[ForeignKey("Products")]
		public int ProductId { get; set; }
		public Product Products { get; set; }
	}
}
