using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using TechStore.API.Constants;
using TechStore.API.DTOs.Orders;
using TechStore.API.Helpers;
using TechStore.API.Services;

namespace TechStore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrdersController : ControllerBase
    {
        private readonly OrderService _orderService;

        public OrdersController(OrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet]
        [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllOrders()
        {
            List<OrderDto> orders = await _orderService.GetAllOrdersAsync();

            return Ok(orders);
        }

        [HttpGet("{orderId}")]
        public async Task<IActionResult> GetOrderById(int orderId)
        {
            OrderDto? order = await _orderService.GetOrderByIdAsync(orderId);

            if (order == null)
            {
                return NotFound();
            }

            int? currentUserId = User.GetUserId();
            if (currentUserId == null)
            {
                return Unauthorized();
            }

            if (!User.IsInRole("Admin") && order.UserId != currentUserId.Value)
            {
                return Forbid();
            }

            return Ok(order);
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetOrdersByUserId(int userId)
        {
            int? currentUserId = User.GetUserId();
            if (currentUserId == null)
            {
                return Unauthorized();
            }

            if (!User.IsInRole("Admin") && userId != currentUserId.Value)
            {
                return Forbid();
            }

            List<OrderDto> orders = await _orderService.GetOrdersByUserIdAsync(userId);

            return Ok(orders);
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder(CreateOrderDto dto)
        {
            int? currentUserId = User.GetUserId();
            if (currentUserId == null)
            {
                return Unauthorized();
            }

            if (dto.UserAddressId <= 0)
            {
                return BadRequest("UserAddressId must be greater than zero.");
            }

            if (dto.Items.Count == 0)
            {
                return BadRequest("Order must contain at least one item.");
            }

            if (dto.Items.Any(item => item.ProductId <= 0))
            {
                return BadRequest("ProductId must be greater than zero.");
            }

            if (dto.Items.Any(item => item.Quantity <= 0))
            {
                return BadRequest("Quantity must be greater than zero.");
            }

            OrderDto? order = await _orderService.CreateOrderAsync(currentUserId.Value, dto);

            if (order == null)
            {
                return BadRequest("One or more products could not be found.");
            }

            return CreatedAtAction(nameof(GetOrderById), new { orderId = order.Id }, order);
        }

        [HttpPost("user/{userId}/from-cart")]
        public async Task<IActionResult> CreateOrderFromCart(int userId, CreateOrderFromCartDto dto)
        {
            int? currentUserId = User.GetUserId();
            if (currentUserId == null)
            {
                return Unauthorized();
            }

            if (userId != currentUserId.Value)
            {
                return Forbid();
            }

            if (dto.UserAddressId <= 0)
            {
                return BadRequest("UserAddressId must be greater than zero.");
            }

            if (string.IsNullOrWhiteSpace(dto.PaymentMethod))
            {
                return BadRequest("Payment method cannot be empty.");
            }

            if (!PaymentRules.ValidMethods.Contains(dto.PaymentMethod))
            {
                return BadRequest("Payment method is invalid.");
            }

            OrderDto? order = await _orderService.CreateOrderFromCartAsync(userId, dto);

            if (order == null)
            {
                return BadRequest("Cart is empty or cart products could not be found.");
            }

            return CreatedAtAction(nameof(GetOrderById), new { orderId = order.Id }, order);
        }

        [HttpPost("guest")]
        [AllowAnonymous]
        public async Task<IActionResult> CreateGuestOrder(CreateGuestOrderDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.FirstName))
            {
                return BadRequest("First name cannot be empty.");
            }

            if (string.IsNullOrWhiteSpace(dto.LastName))
            {
                return BadRequest("Last name cannot be empty.");
            }

            if (string.IsNullOrWhiteSpace(dto.Email))
            {
                return BadRequest("Email cannot be empty.");
            }

            if (!InputValidationHelper.IsEmailValid(dto.Email))
            {
                return BadRequest("Email format is invalid.");
            }

            if (string.IsNullOrWhiteSpace(dto.Phone))
            {
                return BadRequest("Phone cannot be empty.");
            }

            if (!InputValidationHelper.IsPhoneValid(dto.Phone))
            {
                return BadRequest("Phone number must be 10 digits or 11 digits starting with 0.");
            }

            if (string.IsNullOrWhiteSpace(dto.City))
            {
                return BadRequest("City cannot be empty.");
            }

            if (string.IsNullOrWhiteSpace(dto.District))
            {
                return BadRequest("District cannot be empty.");
            }

            if (string.IsNullOrWhiteSpace(dto.AddressDetail))
            {
                return BadRequest("Address detail cannot be empty.");
            }

            if (string.IsNullOrWhiteSpace(dto.PaymentMethod))
            {
                return BadRequest("Payment method cannot be empty.");
            }

            if (!PaymentRules.ValidMethods.Contains(dto.PaymentMethod))
            {
                return BadRequest("Payment method is invalid.");
            }

            if (dto.Items.Count == 0)
            {
                return BadRequest("Order must contain at least one item.");
            }

            if (dto.Items.Any(item => item.ProductId <= 0))
            {
                return BadRequest("ProductId must be greater than zero.");
            }

            if (dto.Items.Any(item => item.Quantity <= 0))
            {
                return BadRequest("Quantity must be greater than zero.");
            }

            OrderDto? order = await _orderService.CreateGuestOrderAsync(dto);

            if (order == null)
            {
                return BadRequest("One or more products could not be found.");
            }

            return CreatedAtAction(nameof(GetOrderById), new { orderId = order.Id }, order);
        }

        [HttpPut("{orderId}")]
        [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateOrder(int orderId, UpdateOrderDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Status))
            {
                return BadRequest("Status cannot be empty.");
            }

            string[] validStatuses = ["Pending", "Processing", "Shipped", "Delivered", "Cancelled"];
            if (!validStatuses.Contains(dto.Status))
            {
                return BadRequest("Order status is invalid.");
            }

            bool result = await _orderService.UpdateOrderAsync(orderId, dto);

            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpDelete("{orderId}")]
        [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteOrder(int orderId)
        {
            bool result = await _orderService.DeleteOrderAsync(orderId);

            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
