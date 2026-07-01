using Microsoft.EntityFrameworkCore;
using TechStore.API.Data;
using TechStore.API.Entities;
using TechStore.API.Repositories.Interfaces;

namespace TechStore.API.Repositories
{
    public class UserAddressRepository : IUserAddressRepository
    {
        private readonly AppDbContext _context;

        public UserAddressRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<UserAddress>> GetAllAsync()
        {
            return await _context.UserAddresses.ToListAsync();
        }

        public async Task<UserAddress?> GetByIdAsync(int id)
        {
            return await _context.UserAddresses.FindAsync(id);
        }

        public async Task<List<UserAddress>> GetByUserIdAsync(int userId)
        {
            return await _context.UserAddresses
                .Where(address => address.UserId == userId)
                .ToListAsync();
        }

        public async Task AddAsync(UserAddress address)
        {
            await _context.UserAddresses.AddAsync(address);
        }

        public void Delete(UserAddress address)
        {
            _context.UserAddresses.Remove(address);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
