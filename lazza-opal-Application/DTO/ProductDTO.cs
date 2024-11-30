namespace Lazza.opal.Application.DTO
{
    public class ProductDto
    {
        public string? ProductName { get; set; }
        public decimal Price { get; set; }
        public int SmallQuant { get; set; }
        public int MQuant { get; set; }
        public int LQuant { get; set; }
        public int XLQuant { get; set; }
        public int TwoXLQuant { get; set; }
        public string? Description { get; set; }
        public bool Gender { get; set; }
        public int? BrandId { get; set; }

        // New property for images
        public IFormFile[]? Images { get; set; }
    }
}
