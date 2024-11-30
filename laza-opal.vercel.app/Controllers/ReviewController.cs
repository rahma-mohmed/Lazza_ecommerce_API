namespace laza_opal.vercel.app.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewController : ControllerBase
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public ReviewController(IReviewRepository reviewRepository , UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
            _reviewRepository = reviewRepository;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddReview([FromBody] Review review)
        {
            string userId = _userManager.GetUserId(User);
            review.UserId = userId;
            var addedReview = await _reviewRepository.AddReviewAsync(review);
            return CreatedAtAction(nameof(GetReviewsByProductId), new { productId = addedReview.ProductId }, addedReview);
        }

        [HttpGet("{productId}")]
        public async Task<ActionResult<IEnumerable<Review>>> GetReviewsByProductId(int productId)
        {
            var reviews = await _reviewRepository.GetReviewsByProductIdAsync(productId) ?? new List<ReviewDto>(); 
            return Ok(reviews);
        }
    }
}
