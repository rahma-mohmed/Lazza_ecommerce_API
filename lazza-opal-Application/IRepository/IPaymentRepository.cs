namespace Lazza.opal.application.IRepository
{
    public interface IPaymentRepository
    {
        Task<Payment> ProcessPaymentAsync(Payment payment);
    }
}
