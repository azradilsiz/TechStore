namespace TechStore.API.DTOs.Orders
{
    public class OrderDto
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public string UserName { get; set; } = string.Empty;

        public int UserAddressId { get; set; }

        public string AddressTitle { get; set; } = string.Empty;

        public decimal TotalPrice { get; set; }

        public DateTime OrderDate { get; set; }

        public string Status { get; set; } = string.Empty;

        public List<OrderItemDto> Items { get; set; } = new();

        public bool HasPayment { get; set; }
    }
}
