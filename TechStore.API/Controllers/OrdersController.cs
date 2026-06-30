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
            var orders = await _orderService.GetAllOrdersAsync();

            return Ok(orders);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderById(int id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);

            if (order == null)
            {
                return NotFound();
            }

            return Ok(order);
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetOrdersByUserId(int userId)
        {
            var orders = await _orderService.GetOrdersByUserIdAsync(userId);

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

            var order = await _orderService.CreateOrderAsync(dto);

            if (order == null)
            {
                return BadRequest("One or more products could not be found.");
            }

            return CreatedAtAction(nameof(GetOrderById), new { id = order.Id }, order);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrder(int id, UpdateOrderDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Status))
            {
                return BadRequest("Status cannot be empty.");
            }

            var result = await _orderService.UpdateOrderAsync(id, dto);

            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var result = await _orderService.DeleteOrderAsync(id);

            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}