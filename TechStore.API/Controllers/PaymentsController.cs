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
            var payments = await _paymentService.GetAllPaymentsAsync();

            return Ok(payments);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPaymentById(int id)
        {
            var payment = await _paymentService.GetPaymentByIdAsync(id);

            if (payment == null)
            {
                return NotFound();
            }

            return Ok(payment);
        }

        [HttpPost]
        public async Task<IActionResult> CreatePayment(CreatePaymentDto dto)
        {
            if (dto.OrderId <= 0)
            {
                return BadRequest("OrderId must be greater than zero.");
            }

            if (dto.Amount <= 0)
            {
                return BadRequest("Amount must be greater than zero.");
            }

            if (string.IsNullOrWhiteSpace(dto.PaymentMethod))
            {
                return BadRequest("Payment method cannot be empty.");
            }

            var payment = await _paymentService.CreatePaymentAsync(dto);

            if (payment == null)
            {
                return BadRequest("Order could not be found.");
            }

            return CreatedAtAction(nameof(GetPaymentById), new { id = payment.Id }, payment);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePayment(int id, UpdatePaymentDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.PaymentStatus))
            {
                return BadRequest("Payment status cannot be empty.");
            }

            var result = await _paymentService.UpdatePaymentAsync(id, dto);

            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePayment(int id)
        {
            var result = await _paymentService.DeletePaymentAsync(id);

            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}