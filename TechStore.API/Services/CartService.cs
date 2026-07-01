using TechStore.API.DTOs.Carts;
using TechStore.API.Entities;
using TechStore.API.Repositories.Interfaces;

namespace TechStore.API.Services
{
    public class CartService
    {
        private readonly ICartRepository _cartRepository;

        public CartService(ICartRepository cartRepository)
        {
            _cartRepository = cartRepository;
        }

        public async Task<CartDto?> GetCartByUserIdAsync(int userId)
        {
            var cart = await _cartRepository.GetByUserIdWithItemsAndProductsAsync(userId);

            if (cart == null)
            {
                return null;
            }

            return MapCartToDto(cart);
        }

        public async Task<CartDto> AddItemToCartAsync(AddCartItemDto dto)
        {
            var cart = await _cartRepository.GetByUserIdWithItemsAsync(dto.UserId);

            if (cart == null)
            {
                cart = new Cart
                {
                    UserId = dto.UserId
                };

                await _cartRepository.AddCartAsync(cart);
                await _cartRepository.SaveChangesAsync();
            }

            var existingItem = cart.CartItems
                .FirstOrDefault(cartItem => cartItem.ProductId == dto.ProductId);

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

                await _cartRepository.AddCartItemAsync(cartItem);
            }

            await _cartRepository.SaveChangesAsync();

            return (await GetCartByUserIdAsync(dto.UserId))!;
        }

        public async Task<bool> UpdateCartItemAsync(int cartItemId, UpdateCartItemDto dto)
        {
            var cartItem = await _cartRepository.GetCartItemByIdAsync(cartItemId);

            if (cartItem == null)
            {
                return false;
            }

            cartItem.Quantity = dto.Quantity;

            await _cartRepository.SaveChangesAsync();

            return true;
        }

        public async Task<bool> RemoveCartItemAsync(int cartItemId)
        {
            var cartItem = await _cartRepository.GetCartItemByIdAsync(cartItemId);

            if (cartItem == null)
            {
                return false;
            }

            _cartRepository.RemoveCartItem(cartItem);
            await _cartRepository.SaveChangesAsync();

            return true;
        }

        private static CartDto MapCartToDto(Cart cart)
        {
            return new CartDto
            {
                Id = cart.Id,
                UserId = cart.UserId,
                Items = cart.CartItems.Select(cartItem => new CartItemDto
                {
                    Id = cartItem.Id,
                    ProductId = cartItem.ProductId,
                    ProductName = cartItem.Product != null ? cartItem.Product.Name : string.Empty,
                    Quantity = cartItem.Quantity,
                    UnitPrice = cartItem.Product != null ? cartItem.Product.Price : 0,
                    TotalPrice = cartItem.Product != null ? cartItem.Product.Price * cartItem.Quantity : 0
                }).ToList(),
                TotalPrice = cart.CartItems.Sum(cartItem =>
                    cartItem.Product != null ? cartItem.Product.Price * cartItem.Quantity : 0)
            };
        }
    }
}
