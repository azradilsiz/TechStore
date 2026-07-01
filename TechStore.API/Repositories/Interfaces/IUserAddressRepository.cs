using TechStore.API.Entities;

namespace TechStore.API.Repositories.Interfaces
{
    public interface IUserAddressRepository
    {
        Task<List<UserAddress>> GetAllAsync();

        Task<UserAddress?> GetByIdAsync(int id);

        Task<List<UserAddress>> GetByUserIdAsync(int userId);

        Task AddAsync(UserAddress address);

        void Delete(UserAddress address);

        Task SaveChangesAsync();
    }
}
