using TechStore.API.DTOs.Orders;
using TechStore.API.Entities;
using TechStore.API.Repositories.Interfaces;

namespace TechStore.API.Services
{
    public class OrderService
    {
        private readonly IOrderRepository _orderRepository;

        public OrderService(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task<List<OrderDto>> GetAllOrdersAsync()
        {
            var orders = await _orderRepository.GetAllWithDetailsAsync();

            return orders.Select(MapOrderToDto).ToList();
        }

        public async Task<OrderDto?> GetOrderByIdAsync(int id)
        {
            var order = await _orderRepository.GetByIdWithDetailsAsync(id);

            if (order == null)
            {
                return null;
            }

            return MapOrderToDto(order);
        }

        public async Task<List<OrderDto>> GetOrdersByUserIdAsync(int userId)
        {
            var orders = await _orderRepository.GetByUserIdWithDetailsAsync(userId);

            return orders.Select(MapOrderToDto).ToList();
        }

        public async Task<OrderDto?> CreateOrderAsync(CreateOrderDto dto)
        {
            var productIds = dto.Items
                .Select(item => item.ProductId)
                .Distinct()
                .ToList();

            var products = await _orderRepository.GetProductsByIdsAsync(productIds);

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

            await _orderRepository.AddAsync(order);
            await _orderRepository.SaveChangesAsync();

            return await GetOrderByIdAsync(order.Id);
        }

        public async Task<OrderDto?> CreateOrderFromCartAsync(int userId, CreateOrderFromCartDto dto)
        {
            var cart = await _orderRepository.GetCartByUserIdWithItemsAndProductsAsync(userId);

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

            await _orderRepository.AddAsync(order);
            _orderRepository.RemoveCartItems(cart.CartItems);

            await _orderRepository.SaveChangesAsync();

            return await GetOrderByIdAsync(order.Id);
        }

        public async Task<bool> UpdateOrderAsync(int id, UpdateOrderDto dto)
        {
            var order = await _orderRepository.GetByIdAsync(id);

            if (order == null)
            {
                return false;
            }

            order.Status = dto.Status;

            await _orderRepository.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteOrderAsync(int id)
        {
            var order = await _orderRepository.GetByIdWithItemsAndPaymentAsync(id);

            if (order == null)
            {
                return false;
            }

            _orderRepository.RemoveOrderItems(order.OrderItems);

            if (order.Payment != null)
            {
                _orderRepository.RemovePayment(order.Payment);
            }

            _orderRepository.RemoveOrder(order);
            await _orderRepository.SaveChangesAsync();

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
