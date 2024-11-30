namespace Lazza.opal.core.Model
{
    public class Product
    {
        public int ProductId { get; set; }
        public string? ProductName { get; set; }
        public decimal Price { get; set; }
        public int SmallQuant { get; set; }
        public int MQuant { get; set; }
        public int LQuant { get; set; }
        public int XLQuant { get; set; }
        public int TwoXLQuant { get; set; }
        public string? Description { get; set; }
        public string? Img1 { get; set; }
        public string? Img2 { get; set; }
        public string? Img3 { get; set; }
        public string? Img4 { get; set; }
        public string? Img5 { get; set; }
        public bool Gender { get; set; }
        public bool Favourite { get; set; } = false;

		[ForeignKey("Brand")]
        public int? BrandId { get; set; }

        [JsonIgnore]
        public Brand? Brand { get; set; }

        [JsonIgnore]
        public virtual ICollection<Review>? Reviews { get; set; }
    }
}
