namespace Lazza.opal.persistence.Repository
{
    public class CardRepository : ICardRepository
    {
        private readonly AppDbContext _context;

        public CardRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Card>> GetAllCardsAsync()
        {
            return await _context.Cards.ToListAsync();
        }

        public async Task<Card> GetCardByIdAsync(int id)
        {
            return await _context.Cards.FindAsync(id);
        }

        public async Task AddCardAsync(Card card)
        {
            await _context.Cards.AddAsync(card);
            await _context.SaveChangesAsync();
        }
    }
}
