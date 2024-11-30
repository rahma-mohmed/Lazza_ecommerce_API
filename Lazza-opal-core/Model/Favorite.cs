namespace Lazza.opal.core.Model
{
    public class Favorite
    {
        public int FavoriteId { get; set; }

        [ForeignKey("User")]
        public string? UserId { get; set; }

        [JsonIgnore]
        public ApplicationUser? User { get; set; }  

        [ForeignKey("Product")]
        public int ProductId { get; set; }
        [JsonIgnore]
        public Product? Product { get; set; }
    }
}
