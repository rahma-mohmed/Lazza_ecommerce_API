namespace laza_opal.vercel.app.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CardController : ControllerBase
    {
        private readonly ICardRepository _cardRepository;

        public CardController(ICardRepository cardRepository)
        {
            _cardRepository = cardRepository;
        }

        [HttpPost]
        public async Task<IActionResult> AddCard([FromBody] Card card)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _cardRepository.AddCardAsync(card);
            return Ok(new { message = "Card added successfully" });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCard(int id)
        {
            var card = await _cardRepository.GetCardByIdAsync(id);
            if (card == null)
                return NotFound();

            return Ok(card);
        }
    }
}
