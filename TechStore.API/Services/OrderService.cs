using TechStore.API.Constants;
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
            List<Order> orders = await _orderRepository.GetAllWithDetailsAsync();

            return orders.Select(MapOrderToDto).ToList();
        }

        public async Task<OrderDto?> GetOrderByIdAsync(int id)
        {
            Order? order = await _orderRepository.GetByIdWithDetailsAsync(id);

            if (order == null)
            {
                return null;
            }

            return MapOrderToDto(order);
        }

        public async Task<List<OrderDto>> GetOrdersByUserIdAsync(int userId)
        {
            List<Order> orders = await _orderRepository.GetByUserIdWithDetailsAsync(userId);

            return orders.Select(MapOrderToDto).ToList();
        }

        public async Task<OrderDto?> CreateOrderAsync(int userId, CreateOrderDto dto)
        {
            if (!await _orderRepository.UserAddressBelongsToUserAsync(dto.UserAddressId, userId))
            {
                return null;
            }

            Dictionary<int, int> requestedQuantities = dto.Items
                .GroupBy(item => item.ProductId)
                .ToDictionary(group => group.Key, group => group.Sum(item => item.Quantity));

            List<int> productIds = requestedQuantities.Keys.ToList();

            Dictionary<int, Product> products = await _orderRepository.GetProductsByIdsAsync(productIds);

            if (!HasSufficientStock(products, requestedQuantities))
            {
                return null;
            }

            Order order = new Order
            {
                UserId = userId,
                UserAddressId = dto.UserAddressId,
                OrderDate = DateTime.UtcNow,
                Status = "Pending",
                IsStockDeducted = true
            };

            foreach ((int productId, int quantity) in requestedQuantities)
            {
                Product product = products[productId];
                product.Stock -= quantity;

                order.OrderItems.Add(new OrderItem
                {
                    ProductId = productId,
                    Quantity = quantity,
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
            if (!await _orderRepository.UserAddressBelongsToUserAsync(dto.UserAddressId, userId))
            {
                return null;
            }

            Cart? cart = await _orderRepository.GetCartByUserIdWithItemsAndProductsAsync(userId);

            if (cart == null || cart.CartItems.Count == 0)
            {
                return null;
            }

            if (cart.CartItems.Any(item => item.Product == null))
            {
                return null;
            }

            if (cart.CartItems.Any(item =>
                item.Product!.IsDeleted ||
                item.Quantity > item.Product.Stock ||
                item.Quantity > ShoppingRules.MaxQuantityPerProduct))
            {
                return null;
            }

            Order order = new Order
            {
                UserId = userId,
                UserAddressId = dto.UserAddressId,
                OrderDate = DateTime.UtcNow,
                Status = "Pending",
                IsStockDeducted = true
            };

            foreach (CartItem cartItem in cart.CartItems)
            {
                cartItem.Product!.Stock -= cartItem.Quantity;

                order.OrderItems.Add(new OrderItem
                {
                    ProductId = cartItem.ProductId,
                    Quantity = cartItem.Quantity,
                    UnitPrice = cartItem.Product!.Price
                });
            }

            order.TotalPrice = order.OrderItems
                .Sum(item => item.UnitPrice * item.Quantity);

            order.Payment = new Payment
            {
                Amount = order.TotalPrice,
                PaymentMethod = dto.PaymentMethod,
                PaymentStatus = "Pending",
                PaymentDate = DateTime.UtcNow
            };

            await _orderRepository.AddAsync(order);
            _orderRepository.RemoveCartItems(cart.CartItems);

            await _orderRepository.SaveChangesAsync();

            return await GetOrderByIdAsync(order.Id);
        }

        public async Task<OrderDto?> CreateGuestOrderAsync(CreateGuestOrderDto dto)
        {
            Dictionary<int, int> requestedQuantities = dto.Items
                .GroupBy(item => item.ProductId)
                .ToDictionary(group => group.Key, group => group.Sum(item => item.Quantity));

            List<int> productIds = requestedQuantities.Keys.ToList();

            Dictionary<int, Product> products = await _orderRepository.GetProductsByIdsAsync(productIds);

            if (!HasSufficientStock(products, requestedQuantities))
            {
                return null;
            }

            Order order = new Order
            {
                OrderDate = DateTime.UtcNow,
                Status = "Pending",
                IsStockDeducted = true,
                GuestFullName = $"{dto.FirstName} {dto.LastName}".Trim(),
                GuestEmail = dto.Email,
                GuestPhone = dto.Phone,
                GuestCity = dto.City,
                GuestDistrict = dto.District,
                GuestAddressDetail = dto.AddressDetail
            };

            foreach ((int productId, int quantity) in requestedQuantities)
            {
                Product product = products[productId];
                product.Stock -= quantity;

                order.OrderItems.Add(new OrderItem
                {
                    ProductId = productId,
                    Quantity = quantity,
                    UnitPrice = product.Price
                });
            }

            order.TotalPrice = order.OrderItems
                .Sum(item => item.UnitPrice * item.Quantity);

            order.Payment = new Payment
            {
                Amount = order.TotalPrice,
                PaymentMethod = dto.PaymentMethod,
                PaymentStatus = "Pending",
                PaymentDate = DateTime.UtcNow
            };

            await _orderRepository.AddAsync(order);
            await _orderRepository.SaveChangesAsync();

            return await GetOrderByIdAsync(order.Id);
        }

        public async Task<bool> UpdateOrderAsync(int id, UpdateOrderDto dto)
        {
            Order? order = await _orderRepository.GetByIdWithDetailsAsync(id);

            if (order == null)
            {
                return false;
            }

            if (order.Status == "Cancelled" && dto.Status != "Cancelled")
            {
                return false;
            }

            if (dto.Status == "Cancelled" && order.Status != "Cancelled" && order.IsStockDeducted)
            {
                RestoreStock(order);
                order.IsStockDeducted = false;
            }

            order.Status = dto.Status;

            await _orderRepository.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteOrderAsync(int id)
        {
            Order? order = await _orderRepository.GetByIdWithDetailsAsync(id);

            if (order == null)
            {
                return false;
            }

            if (order.Status != "Cancelled")
            {
                if (order.IsStockDeducted)
                {
                    RestoreStock(order);
                    order.IsStockDeducted = false;
                }

                order.Status = "Cancelled";
            }
            await _orderRepository.SaveChangesAsync();

            return true;
        }

        private static bool HasSufficientStock(
            Dictionary<int, Product> products,
            Dictionary<int, int> requestedQuantities)
        {
            return products.Count == requestedQuantities.Count &&
                requestedQuantities.All(item =>
                    item.Value <= ShoppingRules.MaxQuantityPerProduct &&
                    products[item.Key].Stock >= item.Value);
        }

        private static void RestoreStock(Order order)
        {
            foreach (OrderItem item in order.OrderItems)
            {
                if (item.Product != null)
                {
                    item.Product.Stock += item.Quantity;
                }
            }
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
                UserEmail = order.User?.Email ?? string.Empty,
                UserAddressId = order.UserAddressId,
                AddressTitle = order.UserAddress != null
                    ? order.UserAddress.Title
                    : order.GuestFullName,
                GuestFullName = order.GuestFullName,
                GuestEmail = order.GuestEmail,
                GuestPhone = order.GuestPhone,
                GuestAddress = string.IsNullOrWhiteSpace(order.GuestAddressDetail)
                    ? string.Empty
                    : $"{order.GuestAddressDetail}, {order.GuestDistrict} / {order.GuestCity}",
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
