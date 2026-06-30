namespace TechStore.API.DTOs.Carts
{
    public class CartDto
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public List<CartItemDto> Items { get; set; } = new();
        
        public decimal TotalPrice { get; set; }
    }
}