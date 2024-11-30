namespace Lazza.opal.application.IRepository
{
    public interface IOrderRepository
    {
        Task<Order> ConfirmOrderAsync(Order order);
        Task<IEnumerable<Order>> GetAllAsync(string userid);
        Task<Order> CancelOrderAsync(int orderId, string userId);
	}
}
