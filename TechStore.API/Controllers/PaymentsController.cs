using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using TechStore.API.Constants;
using TechStore.API.DTOs.Payments;
using TechStore.API.Helpers;
using TechStore.API.Services;

namespace TechStore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PaymentsController : ControllerBase
    {
        private readonly PaymentService _paymentService;

        public PaymentsController(PaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
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

            int? currentUserId = User.GetUserId();
            if (currentUserId == null)
            {
                return Unauthorized();
            }

            if (!User.IsInRole("Admin") && payment.UserId != currentUserId.Value)
            {
                return Forbid();
            }

            return Ok(payment);
        }

        [HttpPost("order/{orderId}")]
        public async Task<IActionResult> CreatePayment(int orderId, CreatePaymentDto dto)
        {
            int? currentUserId = User.GetUserId();
            if (currentUserId == null)
            {
                return Unauthorized();
            }

            if (orderId <= 0)
            {
                return BadRequest("OrderId must be greater than zero.");
            }

            if (string.IsNullOrWhiteSpace(dto.PaymentMethod))
            {
                return BadRequest("Payment method cannot be empty.");
            }

            if (!PaymentRules.ValidMethods.Contains(dto.PaymentMethod))
            {
                return BadRequest("Payment method is invalid.");
            }

            PaymentDto? payment = await _paymentService.CreatePaymentAsync(
                orderId,
                currentUserId.Value,
                User.IsInRole("Admin"),
                dto);

            if (payment == null)
            {
                return BadRequest("Order could not be found.");
            }

            return CreatedAtAction(nameof(GetPaymentById), new { paymentId = payment.Id }, payment);
        }

        [HttpPut("{paymentId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdatePayment(int paymentId, UpdatePaymentDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.PaymentStatus))
            {
                return BadRequest("Payment status cannot be empty.");
            }

            if (!PaymentRules.ValidStatuses.Contains(dto.PaymentStatus))
            {
                return BadRequest("Payment status is invalid.");
            }

            bool result = await _paymentService.UpdatePaymentAsync(paymentId, dto);

            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpDelete("{paymentId}")]
        [Authorize(Roles = "Admin")]
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
