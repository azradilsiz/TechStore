namespace TechStore.API.Entities
{
    public class Order
    {

        public int Id { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public int? UserId { get; set; }
        public int? UserAddressId { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime OrderDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public bool IsStockDeducted { get; set; }
        public string GuestFullName { get; set; } = string.Empty;
        public string GuestEmail { get; set; } = string.Empty;
        public string GuestPhone { get; set; } = string.Empty;
        public string GuestCity { get; set; } = string.Empty;
        public string GuestDistrict { get; set; } = string.Empty;
        public string GuestAddressDetail { get; set; } = string.Empty;
        public User? User { get; set; }

        public UserAddress? UserAddress { get; set; }

        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

        public Payment? Payment { get; set; }
    }
}
