using Microsoft.EntityFrameworkCore;
using TechStore.API.Data;
using TechStore.API.DTOs.Carts;
using TechStore.API.Entities;

namespace TechStore.API.Services
{
    public class CartService
    {
        private readonly AppDbContext _context;

        public CartService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<CartDto?> GetCartByUserIdAsync(int userId)
        {
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
            {
                return null;
            }

            return new CartDto
            {
                Id = cart.Id,
                UserId = cart.UserId,
                Items = cart.CartItems.Select(ci => new CartItemDto
                {
                    Id = ci.Id,
                    ProductId = ci.ProductId,
                    ProductName = ci.Product != null ? ci.Product.Name : string.Empty,
                    Quantity = ci.Quantity,
                    UnitPrice = ci.Product != null ? ci.Product.Price : 0,
                    TotalPrice = ci.Product != null ? ci.Product.Price * ci.Quantity : 0
                }).ToList(),
                TotalPrice = cart.CartItems.Sum(ci => ci.Product != null ? ci.Product.Price * ci.Quantity : 0)
            };
        }

        public async Task<CartDto> AddItemToCartAsync(AddCartItemDto dto)
        {
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.UserId == dto.UserId);

            if (cart == null)
            {
                cart = new Cart
                {
                    UserId = dto.UserId
                };

                _context.Carts.Add(cart);
                await _context.SaveChangesAsync();
            }

            var existingItem = cart.CartItems
                .FirstOrDefault(ci => ci.ProductId == dto.ProductId);

            if (existingItem != null)
            {
                existingItem.Quantity += dto.Quantity;
            }
            else
            {
                var cartItem = new CartItem
                {
                    CartId = cart.Id,
                    ProductId = dto.ProductId,
                    Quantity = dto.Quantity
                };

                _context.CartItems.Add(cartItem);
            }

            await _context.SaveChangesAsync();

            return (await GetCartByUserIdAsync(dto.UserId))!;
        }

        public async Task<bool> UpdateCartItemAsync(int cartItemId, UpdateCartItemDto dto)
        {
            var cartItem = await _context.CartItems.FindAsync(cartItemId);

            if (cartItem == null)
            {
                return false;
            }

            cartItem.Quantity = dto.Quantity;

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> RemoveCartItemAsync(int cartItemId)
        {
            var cartItem = await _context.CartItems.FindAsync(cartItemId);

            if (cartItem == null)
            {
                return false;
            }

            _context.CartItems.Remove(cartItem);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}