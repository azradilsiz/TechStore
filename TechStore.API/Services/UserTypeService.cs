using Microsoft.EntityFrameworkCore;
using TechStore.API.Data;
using TechStore.API.DTOs.UserTypes;
using TechStore.API.Entities;

namespace TechStore.API.Services
{
    public class UserTypeService
    {
        private readonly AppDbContext _context;

        public UserTypeService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<UserTypeDto>> GetAllUserTypesAsync()
        {
            return await _context.UserTypes
                .Select(userType => new UserTypeDto
                {
                    Id = userType.Id,
                    TypeName = userType.TypeName
                })
                .ToListAsync();
        }

        public async Task<UserTypeDto?> GetUserTypeByIdAsync(int id)
        {
            return await _context.UserTypes
                .Where(userType => userType.Id == id)
                .Select(userType => new UserTypeDto
                {
                    Id = userType.Id,
                    TypeName = userType.TypeName
                })
                .FirstOrDefaultAsync();
        }

        public async Task<UserTypeDto> CreateUserTypeAsync(CreateUserTypeDto dto)
        {
            var userType = new UserType
            {
                TypeName = dto.TypeName
            };

            _context.UserTypes.Add(userType);
            await _context.SaveChangesAsync();

            return new UserTypeDto
            {
                Id = userType.Id,
                TypeName = userType.TypeName
            };
        }

        public async Task<bool> UpdateUserTypeAsync(int id, UpdateUserTypeDto dto)
        {
            var userType = await _context.UserTypes.FindAsync(id);

            if (userType == null)
            {
                return false;
            }

            userType.TypeName = dto.TypeName;

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteUserTypeAsync(int id)
        {
            var userType = await _context.UserTypes.FindAsync(id);

            if (userType == null)
            {
                return false;
            }

            _context.UserTypes.Remove(userType);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}