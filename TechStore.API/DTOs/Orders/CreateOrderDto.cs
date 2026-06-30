namespace TechStore.API.DTOs.Orders
{
    public class CreateOrderDto
    {
        public int UserId { get; set; }

        public int UserAddressId { get; set; }


        public List<CreateOrderItemDto> Items { get; set; } = new();
    }
}
