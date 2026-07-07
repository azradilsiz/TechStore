using Microsoft.AspNetCore.Mvc;
using TechStore.API.DTOs.Payments;
using TechStore.API.Services;

namespace TechStore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentsController : ControllerBase
    {
        private readonly PaymentService _paymentService;

        public PaymentsController(PaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPayments()
        {
            List<PaymentDto> payments = await _paymentService.GetAllPaymentsAsync();

            return Ok(payments);
        }

        [HttpGet("{paymentId}")]
        public async Task<IActionResult> GetPaymentById(int paymentId)
        {
            PaymentDto? payment = await _paymentService.GetPaymentByIdAsync(paymentId);

            if (payment == null)
            {
                return NotFound();
            }

            return Ok(payment);
        }

        [HttpPost("order/{orderId}")]
        public async Task<IActionResult> CreatePayment(int orderId, CreatePaymentDto dto)
        {
            if (orderId <= 0)
            {
                return BadRequest("OrderId must be greater than zero.");
            }

            if (string.IsNullOrWhiteSpace(dto.PaymentMethod))
            {
                return BadRequest("Payment method cannot be empty.");
            }

            PaymentDto? payment = await _paymentService.CreatePaymentAsync(orderId, dto);

            if (payment == null)
            {
                return BadRequest("Order could not be found.");
            }

            return CreatedAtAction(nameof(GetPaymentById), new { paymentId = payment.Id }, payment);
        }

        [HttpPut("{paymentId}")]
        public async Task<IActionResult> UpdatePayment(int paymentId, UpdatePaymentDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.PaymentStatus))
            {
                return BadRequest("Payment status cannot be empty.");
            }

            bool result = await _paymentService.UpdatePaymentAsync(paymentId, dto);

            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpDelete("{paymentId}")]
        public async Task<IActionResult> DeletePayment(int paymentId)
        {
            bool result = await _paymentService.DeletePaymentAsync(paymentId);

            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
