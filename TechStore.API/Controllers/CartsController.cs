using Microsoft.AspNetCore.Mvc;
using TechStore.API.DTOs.Carts;
using TechStore.API.Services;

namespace TechStore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartsController : ControllerBase
    {
        private readonly CartService _cartService;

        public CartsController(CartService cartService)
        {
            _cartService = cartService;
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetCartByUserId(int userId)
        {
            CartDto? cart = await _cartService.GetCartByUserIdAsync(userId);

            if (cart == null)
            {
                return NotFound();
            }

            return Ok(cart);
        }

        [HttpPost("add-item")]
        public async Task<IActionResult> AddItemToCart(AddCartItemDto dto)
        {
            if (dto.UserId <= 0)
            {
                return BadRequest("UserId must be greater than zero.");
            }

            if (dto.ProductId <= 0)
            {
                return BadRequest("ProductId must be greater than zero.");
            }

            if (dto.Quantity <= 0)
            {
                return BadRequest("Quantity must be greater than zero.");
            }

            CartDto cart = await _cartService.AddItemToCartAsync(dto);

            return Ok(cart);
        }

        [HttpPut("items/{cartItemId}")]
        public async Task<IActionResult> UpdateCartItem(int cartItemId, UpdateCartItemDto dto)
        {
            if (dto.Quantity <= 0)
            {
                return BadRequest("Quantity must be greater than zero.");
            }

            bool result = await _cartService.UpdateCartItemAsync(cartItemId, dto);

            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpDelete("items/{cartItemId}")]
        public async Task<IActionResult> RemoveCartItem(int cartItemId)
        {
            bool result = await _cartService.RemoveCartItemAsync(cartItemId);

            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
