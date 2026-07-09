namespace TechStore.API.DTOs.Orders
{
    public class CreateOrderFromCartDto
    {
        public int UserAddressId { get; set; }

        public string PaymentMethod { get; set; } = string.Empty;
    }
}
