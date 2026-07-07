using Microsoft.AspNetCore.Mvc;
using TechStore.API.DTOs.Orders;
using TechStore.API.Services;

namespace TechStore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly OrderService _orderService;

        public OrdersController(OrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet]
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

            return Ok(order);
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetOrdersByUserId(int userId)
        {
            List<OrderDto> orders = await _orderService.GetOrdersByUserIdAsync(userId);

            return Ok(orders);
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder(CreateOrderDto dto)
        {
            if (dto.UserId <= 0)
            {
                return BadRequest("UserId must be greater than zero.");
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

            OrderDto? order = await _orderService.CreateOrderAsync(dto);

            if (order == null)
            {
                return BadRequest("One or more products could not be found.");
            }

            return CreatedAtAction(nameof(GetOrderById), new { orderId = order.Id }, order);
        }

        [HttpPost("user/{userId}/from-cart")]
        public async Task<IActionResult> CreateOrderFromCart(int userId, CreateOrderFromCartDto dto)
        {
            if (userId <= 0)
            {
                return BadRequest("UserId must be greater than zero.");
            }

            if (dto.UserAddressId <= 0)
            {
                return BadRequest("UserAddressId must be greater than zero.");
            }

            OrderDto? order = await _orderService.CreateOrderFromCartAsync(userId, dto);

            if (order == null)
            {
                return BadRequest("Cart is empty or cart products could not be found.");
            }

            return CreatedAtAction(nameof(GetOrderById), new { orderId = order.Id }, order);
        }

        [HttpPut("{orderId}")]
        public async Task<IActionResult> UpdateOrder(int orderId, UpdateOrderDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Status))
            {
                return BadRequest("Status cannot be empty.");
            }

            bool result = await _orderService.UpdateOrderAsync(orderId, dto);

            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpDelete("{orderId}")]
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
