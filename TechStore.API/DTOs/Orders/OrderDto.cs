namespace TechStore.API.DTOs.Orders
{
    public class OrderDto
    {
        public int Id { get; set; }

        public string OrderNumber { get; set; } = string.Empty;

        public int? UserId { get; set; }

        public string UserName { get; set; } = string.Empty;

        public string UserEmail { get; set; } = string.Empty;

        public int? UserAddressId { get; set; }

        public string AddressTitle { get; set; } = string.Empty;

        public string GuestFullName { get; set; } = string.Empty;

        public string GuestEmail { get; set; } = string.Empty;

        public string GuestPhone { get; set; } = string.Empty;

        public string GuestAddress { get; set; } = string.Empty;

        public decimal TotalPrice { get; set; }

        public DateTime OrderDate { get; set; }

        public string Status { get; set; } = string.Empty;

        public List<OrderItemDto> Items { get; set; } = new();

        public bool HasPayment { get; set; }

        public int? PaymentId { get; set; }

        public string PaymentMethod { get; set; } = string.Empty;

        public string PaymentStatus { get; set; } = string.Empty;
    }
}
