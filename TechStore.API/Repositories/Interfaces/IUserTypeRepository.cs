using TechStore.API.Entities;

namespace TechStore.API.Repositories.Interfaces
{
    public interface IUserTypeRepository
    {
        Task<List<UserType>> GetAllAsync();

        Task<UserType?> GetByIdAsync(int id);

        Task AddAsync(UserType userType);

        Task<bool> HasUsersAsync(int userTypeId);

        void Delete(UserType userType);

        Task SaveChangesAsync();
    }
}
