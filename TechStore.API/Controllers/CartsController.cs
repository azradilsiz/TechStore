using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using TechStore.API.DTOs.Carts;
using TechStore.API.Helpers;
using TechStore.API.Services;

namespace TechStore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
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
            int? currentUserId = User.GetUserId();
            if (currentUserId == null)
            {
                return Unauthorized();
            }

            if (userId != currentUserId.Value)
            {
                return Forbid();
            }

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
            int? currentUserId = User.GetUserId();
            if (currentUserId == null)
            {
                return Unauthorized();
            }

            if (dto.ProductId <= 0)
            {
                return BadRequest("ProductId must be greater than zero.");
            }

            if (dto.Quantity <= 0)
            {
                return BadRequest("Quantity must be greater than zero.");
            }

            CartDto cart = await _cartService.AddItemToCartAsync(currentUserId.Value, dto);

            return Ok(cart);
        }

        [HttpPut("items/{cartItemId}")]
        public async Task<IActionResult> UpdateCartItem(int cartItemId, UpdateCartItemDto dto)
        {
            int? currentUserId = User.GetUserId();
            if (currentUserId == null)
            {
                return Unauthorized();
            }

            if (dto.Quantity <= 0)
            {
                return BadRequest("Quantity must be greater than zero.");
            }

            bool result = await _cartService.UpdateCartItemAsync(currentUserId.Value, cartItemId, dto);

            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpDelete("items/{cartItemId}")]
        public async Task<IActionResult> RemoveCartItem(int cartItemId)
        {
            int? currentUserId = User.GetUserId();
            if (currentUserId == null)
            {
                return Unauthorized();
            }

            bool result = await _cartService.RemoveCartItemAsync(currentUserId.Value, cartItemId);

            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
