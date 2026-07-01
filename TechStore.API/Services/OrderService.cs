using Microsoft.EntityFrameworkCore;
using TechStore.API.Data;
using TechStore.API.DTOs.Orders;
using TechStore.API.Entities;

namespace TechStore.API.Services
{
    public class OrderService
    {
        private readonly AppDbContext _context;

        public OrderService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<OrderDto>> GetAllOrdersAsync()
        {
            var orders = await _context.Orders
                .Include(o => o.User)
                .Include(o => o.UserAddress)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .Include(o => o.Payment)
                .ToListAsync();

            return orders.Select(MapOrderToDto).ToList();
        }

        public async Task<OrderDto?> GetOrderByIdAsync(int id)
        {
            var order = await _context.Orders
                .Include(o => o.User)
                .Include(o => o.UserAddress)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .Include(o => o.Payment)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return null;
            }

            return MapOrderToDto(order);
        }

        public async Task<List<OrderDto>> GetOrdersByUserIdAsync(int userId)
        {
            var orders = await _context.Orders
                .Include(o => o.User)
                .Include(o => o.UserAddress)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .Include(o => o.Payment)
                .Where(o => o.UserId == userId)
                .ToListAsync();

            return orders.Select(MapOrderToDto).ToList();
        }

        public async Task<OrderDto?> CreateOrderAsync(CreateOrderDto dto)
        {
            var productIds = dto.Items
                .Select(item => item.ProductId)
                .Distinct()
                .ToList();

            var products = await _context.Products
                .Where(product => productIds.Contains(product.Id))
                .ToDictionaryAsync(product => product.Id);

            if (products.Count != productIds.Count)
            {
                return null;
            }

            var order = new Order
            {
                UserId = dto.UserId,
                UserAddressId = dto.UserAddressId,
                OrderDate = DateTime.UtcNow,
                Status = "Pending"
            };

            foreach (var item in dto.Items)
            {
                var product = products[item.ProductId];

                order.OrderItems.Add(new OrderItem
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = product.Price
                });
            }

            order.TotalPrice = order.OrderItems
                .Sum(item => item.UnitPrice * item.Quantity);

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            return await GetOrderByIdAsync(order.Id);
        }

        public async Task<OrderDto?> CreateOrderFromCartAsync(int userId, CreateOrderFromCartDto dto)
        {
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null || cart.CartItems.Count == 0)
            {
                return null;
            }

            if (cart.CartItems.Any(item => item.Product == null))
            {
                return null;
            }

            var order = new Order
            {
                UserId = userId,
                UserAddressId = dto.UserAddressId,
                OrderDate = DateTime.UtcNow,
                Status = "Pending"
            };

            foreach (var cartItem in cart.CartItems)
            {
                order.OrderItems.Add(new OrderItem
                {
                    ProductId = cartItem.ProductId,
                    Quantity = cartItem.Quantity,
                    UnitPrice = cartItem.Product!.Price
                });
            }

            order.TotalPrice = order.OrderItems
                .Sum(item => item.UnitPrice * item.Quantity);

            _context.Orders.Add(order);
            _context.CartItems.RemoveRange(cart.CartItems);

            await _context.SaveChangesAsync();

            return await GetOrderByIdAsync(order.Id);
        }

        public async Task<bool> UpdateOrderAsync(int id, UpdateOrderDto dto)
        {
            var order = await _context.Orders.FindAsync(id);

            if (order == null)
            {
                return false;
            }

            order.Status = dto.Status;

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteOrderAsync(int id)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .Include(o => o.Payment)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return false;
            }

            _context.OrderItems.RemoveRange(order.OrderItems);

            if (order.Payment != null)
            {
                _context.Payments.Remove(order.Payment);
            }

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();

            return true;
        }

        private static OrderDto MapOrderToDto(Order order)
        {
            return new OrderDto
            {
                Id = order.Id,
                UserId = order.UserId,
                UserName = order.User != null
                    ? $"{order.User.FirstName} {order.User.LastName}".Trim()
                    : string.Empty,
                UserAddressId = order.UserAddressId,
                AddressTitle = order.UserAddress != null
                    ? order.UserAddress.Title
                    : string.Empty,
                TotalPrice = order.TotalPrice,
                OrderDate = order.OrderDate,
                Status = order.Status,
                Items = order.OrderItems.Select(orderItem => new OrderItemDto
                {
                    Id = orderItem.Id,
                    ProductId = orderItem.ProductId,
                    ProductName = orderItem.Product != null
                        ? orderItem.Product.Name
                        : string.Empty,
                    Quantity = orderItem.Quantity,
                    UnitPrice = orderItem.UnitPrice,
                    TotalPrice = orderItem.UnitPrice * orderItem.Quantity
                }).ToList(),
                HasPayment = order.Payment != null
            };
        }
    }
}
