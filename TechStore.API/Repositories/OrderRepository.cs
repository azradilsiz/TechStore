using Microsoft.EntityFrameworkCore;
using TechStore.API.Data;
using TechStore.API.Entities;
using TechStore.API.Repositories.Interfaces;

namespace TechStore.API.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly AppDbContext _context;

        public OrderRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Order>> GetAllWithDetailsAsync()
        {
            return await _context.Orders
                .Include(order => order.User)
                .Include(order => order.UserAddress)
                .Include(order => order.OrderItems)
                .ThenInclude(orderItem => orderItem.Product)
                .Include(order => order.Payment)
                .ToListAsync();
        }

        public async Task<Order?> GetByIdWithDetailsAsync(int id)
        {
            return await _context.Orders
                .Include(order => order.User)
                .Include(order => order.UserAddress)
                .Include(order => order.OrderItems)
                .ThenInclude(orderItem => orderItem.Product)
                .Include(order => order.Payment)
                .FirstOrDefaultAsync(order => order.Id == id);
        }

        public async Task<List<Order>> GetByUserIdWithDetailsAsync(int userId)
        {
            return await _context.Orders
                .Include(order => order.User)
                .Include(order => order.UserAddress)
                .Include(order => order.OrderItems)
                .ThenInclude(orderItem => orderItem.Product)
                .Include(order => order.Payment)
                .Where(order => order.UserId == userId)
                .ToListAsync();
        }

        public async Task<Dictionary<int, Product>> GetProductsByIdsAsync(List<int> productIds)
        {
            return await _context.Products
                .Where(product => productIds.Contains(product.Id))
                .ToDictionaryAsync(product => product.Id);
        }

        public async Task<Cart?> GetCartByUserIdWithItemsAndProductsAsync(int userId)
        {
            return await _context.Carts
                .Include(cart => cart.CartItems)
                .ThenInclude(cartItem => cartItem.Product)
                .FirstOrDefaultAsync(cart => cart.UserId == userId);
        }

        public async Task<Order?> GetByIdWithItemsAndPaymentAsync(int id)
        {
            return await _context.Orders
                .Include(order => order.OrderItems)
                .Include(order => order.Payment)
                .FirstOrDefaultAsync(order => order.Id == id);
        }

        public async Task<Order?> GetByIdAsync(int id)
        {
            return await _context.Orders.FindAsync(id);
        }

        public async Task AddAsync(Order order)
        {
            await _context.Orders.AddAsync(order);
        }

        public void RemoveCartItems(IEnumerable<CartItem> cartItems)
        {
            _context.CartItems.RemoveRange(cartItems);
        }

        public void RemoveOrderItems(IEnumerable<OrderItem> orderItems)
        {
            _context.OrderItems.RemoveRange(orderItems);
        }

        public void RemovePayment(Payment payment)
        {
            _context.Payments.Remove(payment);
        }

        public void RemoveOrder(Order order)
        {
            _context.Orders.Remove(order);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
