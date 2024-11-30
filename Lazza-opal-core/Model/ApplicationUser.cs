using Microsoft.AspNetCore.Identity;

namespace Lazza.opal.core.Model
{
    public class ApplicationUser : IdentityUser
    {
        public bool Gender { get; set; }
        public string Name { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string Address { get; set; }
        public string? UserImage { get; set; }
        public string? VerificationCode { get; set; }
        public DateTime? VerificationCodeExpiry { get; set; }
        public string? Provider {get; set; }
		public string? Token { get; set; }
        public int OrderCount { get; set; }

		[JsonIgnore]
        public virtual ICollection<Favorite>? Favorites { get; set; }
        [JsonIgnore]
        public virtual ICollection<Cart>? Carts { get; set; }
        [JsonIgnore]
        public virtual ICollection<Review>? Reviews { get; set; }
		[JsonIgnore]
		public virtual ICollection<Order>? Orders { get; set; }
	}
}
