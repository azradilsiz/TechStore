namespace TechStore.API.DTOs.Orders
{
    public class CreateGuestOrderDto
    {
        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Phone { get; set; } = string.Empty;

        public string City { get; set; } = string.Empty;

        public string District { get; set; } = string.Empty;

        public string AddressDetail { get; set; } = string.Empty;

        public string PaymentMethod { get; set; } = string.Empty;

        public List<CreateOrderItemDto> Items { get; set; } = new();
    }
}
