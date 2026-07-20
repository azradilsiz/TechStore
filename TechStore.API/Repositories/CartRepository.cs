using Microsoft.EntityFrameworkCore;
using TechStore.API.Data;
using TechStore.API.Entities;
using TechStore.API.Repositories.Interfaces;

namespace TechStore.API.Repositories
{
    public class CartRepository : ICartRepository
    {
        private readonly AppDbContext _context;

        public CartRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Cart?> GetByUserIdWithItemsAsync(int userId)
        {
            return await _context.Carts
                .Include(cart => cart.CartItems)
                .FirstOrDefaultAsync(cart => cart.UserId == userId);
        }

        public async Task<Cart?> GetByUserIdWithItemsAndProductsAsync(int userId)
        {
            return await _context.Carts
                .Include(cart => cart.CartItems)
                .ThenInclude(cartItem => cartItem.Product)
                .FirstOrDefaultAsync(cart => cart.UserId == userId);
        }

        public async Task<CartItem?> GetCartItemByIdAsync(int cartItemId)
        {
            return await _context.CartItems
                .Include(cartItem => cartItem.Cart)
                .FirstOrDefaultAsync(cartItem => cartItem.Id == cartItemId);
        }

        public async Task AddCartAsync(Cart cart)
        {
            await _context.Carts.AddAsync(cart);
        }

        public async Task AddCartItemAsync(CartItem cartItem)
        {
            await _context.CartItems.AddAsync(cartItem);
        }

        public void RemoveCartItem(CartItem cartItem)
        {
            _context.CartItems.Remove(cartItem);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
