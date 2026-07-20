using TechStore.API.Entities;

namespace TechStore.API.Repositories.Interfaces
{
    public interface ICartRepository
    {
        Task<Cart?> GetByUserIdWithItemsAsync(int userId);

        Task<Cart?> GetByUserIdWithItemsAndProductsAsync(int userId);

        Task<CartItem?> GetCartItemByIdAsync(int cartItemId);

        Task<Product?> GetProductByIdAsync(int productId);

        Task AddCartAsync(Cart cart);

        Task AddCartItemAsync(CartItem cartItem);

        void RemoveCartItem(CartItem cartItem);

        Task SaveChangesAsync();
    }
}
