namespace Lazza.opal.core.Model
{
    public class Review
    {
        public int ReviewId { get; set; }
        public DateTime ReviewDate { get; set; } = DateTime.Now;

        [ForeignKey("Product")]
        public int ProductId { get; set; }
        [JsonIgnore]
        public Product? Product { get; set; } 

        [ForeignKey("User")]
        public string? UserId { get; set; }
        [JsonIgnore]
        public ApplicationUser? User { get; set; } 

        public string? Comment { get; set; }

        [Range(1, 5)]
        public float Rate { get; set; } 
    }
}
