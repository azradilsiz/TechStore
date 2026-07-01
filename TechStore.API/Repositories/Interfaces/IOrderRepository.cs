using TechStore.API.Entities;

namespace TechStore.API.Repositories.Interfaces
{
    public interface IOrderRepository
    {
        Task<List<Order>> GetAllWithDetailsAsync();

        Task<Order?> GetByIdWithDetailsAsync(int id);

        Task<List<Order>> GetByUserIdWithDetailsAsync(int userId);

        Task<Dictionary<int, Product>> GetProductsByIdsAsync(List<int> productIds);

        Task<Cart?> GetCartByUserIdWithItemsAndProductsAsync(int userId);

        Task<Order?> GetByIdWithItemsAndPaymentAsync(int id);

        Task<Order?> GetByIdAsync(int id);

        Task AddAsync(Order order);

        void RemoveCartItems(IEnumerable<CartItem> cartItems);

        void RemoveOrderItems(IEnumerable<OrderItem> orderItems);

        void RemovePayment(Payment payment);

        void RemoveOrder(Order order);

        Task SaveChangesAsync();
    }
}
