namespace TechStore.API.Entities
{
    public class User
    {
        public int Id { get; set; } 
        public int UserTypeId { get; set; } 
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public bool IsDeleted { get; set; }
        public UserType? UserType { get; set; }

        public ICollection<UserAddress> UserAddresses { get; set; } = new List<UserAddress>();

        public ICollection<Cart> Carts { get; set; } = new List<Cart>();

        public ICollection<Order> Orders { get; set; } = new List<Order>();

    }
}
