namespace Lazza.opal.core.Model
{
    public class Order
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }

		[ForeignKey("Cart")]
		public int CartId { get; set; }
		[JsonIgnore]
		public Cart? Cart { get; set; }

		[JsonIgnore]
		public List<OrderProduct> OrderProducts { get; set; } = new List<OrderProduct>();

		[ForeignKey("User")]
		public string? UserId { get; set; }
		[JsonIgnore]
		public ApplicationUser? User { get; set; }

		public string? Status
		{
			get;

			//            if (OrderDate.AddMinutes(15) >= DateTime.Now)
			//            {
			//	if (OrderDate.AddMinutes(15).AddMinutes(15) >= DateTime.Now)
			//	{
			//		return "Delivered";
			//	}
			//	return "Shipped";
			//}
			//            return "Pending";

			set;
		}
		public int Count { get; set; }
		public decimal TotalPrice { get; set; }
	}
}
