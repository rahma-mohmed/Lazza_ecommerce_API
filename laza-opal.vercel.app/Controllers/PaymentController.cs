namespace laza_opal.vercel.app.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentRepository _paymentRepository;

        public PaymentController(IPaymentRepository paymentRepository)
        {
            _paymentRepository = paymentRepository;
        }

        [HttpPost]
        public async Task<IActionResult> ProcessPayment([FromBody] Payment payment)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var processedPayment = await _paymentRepository.ProcessPaymentAsync(payment);
            return Ok(new { message = "Payment processed successfully", processedPayment });
        }
    }
}
