namespace Lazza.opal.persistence.Repository
{
    public class ReviewRepository : IReviewRepository
    {
        private readonly AppDbContext _context; // Your EF DbContext

        public ReviewRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Review> AddReviewAsync(Review review)
        {
            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();
            return review;
        }

        public async Task<IEnumerable<ReviewDto>> GetReviewsByProductIdAsync(int productId)
        {
            return await _context.Reviews
            .Where(r => r.ProductId == productId)
            .Select(r => new ReviewDto
            {
                ReviewId = r.ReviewId,
                ReviewDate = r.ReviewDate,
                ProductId = r.ProductId,
                Comment = r.Comment,
                Rate = r.Rate,
                UserName = r.User != null ? r.User.Name : null,
                UserImage = r.User != null ? r.User.UserImage : null
            })
            .ToListAsync() ?? new List<ReviewDto>(); 
        }
    }
}
