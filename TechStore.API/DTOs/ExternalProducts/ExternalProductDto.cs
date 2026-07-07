namespace TechStore.API.DTOs.ExternalProducts
{
    public class ExternalProductDto
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public string Category { get; set; } = string.Empty;

        public decimal Price { get; set; }

        public int Stock { get; set; }

        public string Thumbnail { get; set; } = string.Empty;
    }
}
