namespace Lazza.opal.core.Model
{
    public class CartItem
    {
        public int CartItemId { get; set; }
        public string? Size { get; set; }
        public int Quantity { get; set; }

        [ForeignKey("Products")]
        public int ProductId { get; set; }
        [JsonIgnore]
        public virtual Product? Products { get; set; }

        [ForeignKey("Cart")]
        public int CartId { get; set; }

        [JsonIgnore]
        public virtual Cart? Cart { get; set; }
    }
}
