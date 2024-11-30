namespace Lazza.opal.application.IRepository 
{ 

	public interface ICardRepository
    {
        Task<IEnumerable<Card>> GetAllCardsAsync();
        Task<Card> GetCardByIdAsync(int id);
        Task AddCardAsync(Card card);
    }
}
