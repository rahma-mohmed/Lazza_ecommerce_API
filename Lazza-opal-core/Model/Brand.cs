namespace Lazza.opal.core.Model
{
    public class Brand
    {
        public int BrandId { get; set; }
        public string? BrandName { get; set; }
        public string? BrandImage { get; set; }

        [JsonIgnore]
        public virtual ICollection<Product>? Products { get; set; }
    }
}
