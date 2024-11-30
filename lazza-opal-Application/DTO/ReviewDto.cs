namespace Lazza.opal.Application.DTO
{
	public class ReviewDto
	{
		public int ReviewId { get; set; }
		public DateTime ReviewDate { get; set; }
		public int ProductId { get; set; }
		public string? Comment { get; set; }
		public float Rate { get; set; }
		public string? UserName { get; set; }
		public string? UserImage { get; set; }
	}
}
