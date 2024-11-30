namespace Lazza.opal.core.Model
{
    public class Card
    {
        public int Id { get; set; }
        public string? CardOwner { get; set; }
        public string? CardNumber { get; set; }
        public string? Expiration { get; set; }
        public string? CVV { get; set; }
        public bool SaveCardInfo { get; set; }
    }
}
