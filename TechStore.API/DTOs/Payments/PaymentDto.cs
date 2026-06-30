namespace TechStore.API.DTOs.Payments
{
    public class PaymentDto
    {
        public int Id { get; set; }

        public int OrderId { get; set; }

        public decimal Amount { get; set; }

        public string PaymentMethod { get; set; } = string.Empty;

        public string PaymentStatus { get; set; } = string.Empty;

        public DateTime PaymentDate { get; set; }
    }
}
