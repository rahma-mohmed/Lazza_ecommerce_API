namespace Lazza.opal.application.IRepository
{
    public interface IReviewRepository
    {
        Task<Review> AddReviewAsync(Review review);
        Task<IEnumerable<ReviewDto>> GetReviewsByProductIdAsync(int productId);
    }
}
