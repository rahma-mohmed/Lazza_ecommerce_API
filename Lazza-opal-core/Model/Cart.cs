namespace Lazza.opal.core.Model
{
    public class Cart
    {
        public int CartId { get; set; }

        [ForeignKey("User")]
        public string? UserId { get; set; }

        [JsonIgnore]
        public ApplicationUser? User { get; set; }  

        public virtual ICollection<CartItem>? CartItems { get; set; } = new List<CartItem>();
    }
}
