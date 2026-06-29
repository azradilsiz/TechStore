namespace TechStore.API.Entities
{
    public class UserAddress
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string City { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string AddressDetail { get; set; } = string.Empty;
        public string District {  get; set; } = string.Empty;
        public string Phone {  get; set; } = string.Empty;
        public User? User { get; set; } 
    }
}
