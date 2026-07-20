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
            Cart? cart = await _cartRepository.GetByUserIdWithItemsAndProductsAsync(userId);

            if (cart == null)
            {
                return null;
            }

            return MapCartToDto(cart);
        }

        public async Task<CartDto> AddItemToCartAsync(int userId, AddCartItemDto dto)
        {
            Cart? cart = await _cartRepository.GetByUserIdWithItemsAsync(userId);

            if (cart == null)
            {
                cart = new Cart
                {
                    UserId = userId
                };

                await _cartRepository.AddCartAsync(cart);
                await _cartRepository.SaveChangesAsync();
            }

            CartItem? existingItem = cart.CartItems
                .FirstOrDefault(cartItem => cartItem.ProductId == dto.ProductId);

            if (existingItem != null)
            {
                existingItem.Quantity += dto.Quantity;
            }
            else
            {
                CartItem cartItem = new CartItem
                {
                    CartId = cart.Id,
                    ProductId = dto.ProductId,
                    Quantity = dto.Quantity
                };

                await _cartRepository.AddCartItemAsync(cartItem);
            }

            await _cartRepository.SaveChangesAsync();

            return (await GetCartByUserIdAsync(userId))!;
        }

        public async Task<bool> UpdateCartItemAsync(int userId, int cartItemId, UpdateCartItemDto dto)
        {
            CartItem? cartItem = await _cartRepository.GetCartItemByIdAsync(cartItemId);

            if (cartItem == null || cartItem.Cart?.UserId != userId)
            {
                return false;
            }

            cartItem.Quantity = dto.Quantity;

            await _cartRepository.SaveChangesAsync();

            return true;
        }

        public async Task<bool> RemoveCartItemAsync(int userId, int cartItemId)
        {
            CartItem? cartItem = await _cartRepository.GetCartItemByIdAsync(cartItemId);

            if (cartItem == null || cartItem.Cart?.UserId != userId)
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
