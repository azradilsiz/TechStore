using Microsoft.EntityFrameworkCore;
using TechStore.API.Data;
using TechStore.API.Entities;
using TechStore.API.Repositories.Interfaces;

namespace TechStore.API.Repositories
{
    public class UserTypeRepository : IUserTypeRepository
    {
        private readonly AppDbContext _context;

        public UserTypeRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<UserType>> GetAllAsync()
        {
            return await _context.UserTypes.ToListAsync();
        }

        public async Task<UserType?> GetByIdAsync(int id)
        {
            return await _context.UserTypes.FindAsync(id);
        }

        public async Task AddAsync(UserType userType)
        {
            await _context.UserTypes.AddAsync(userType);
        }

        public async Task<bool> HasUsersAsync(int userTypeId)
        {
            return await _context.Users
                .AnyAsync(user => user.UserTypeId == userTypeId && !user.IsDeleted);
        }

        public void Delete(UserType userType)
        {
            _context.UserTypes.Remove(userType);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
