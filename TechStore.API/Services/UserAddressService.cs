using Microsoft.EntityFrameworkCore;
using TechStore.API.Data;
using TechStore.API.DTOs.UserAddresses;
using TechStore.API.Entities;

namespace TechStore.API.Services
{
    public class UserAddressService
    {
        private readonly AppDbContext _context;

        public UserAddressService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<UserAddressDto>> GetAllUserAddressesAsync()
        {
            return await _context.UserAddresses
                .Select(address => new UserAddressDto
                {
                    Id = address.Id,
                    UserId = address.UserId,
                    City = address.City,
                    District = address.District,
                    AddressDetail = address.AddressDetail,
                    Phone = address.Phone,
                    Title = address.Title
                })
                .ToListAsync();
        }

        public async Task<UserAddressDto?> GetUserAddressByIdAsync(int id)
        {
            return await _context.UserAddresses
                .Where(address => address.Id == id)
                .Select(address => new UserAddressDto
                {
                    Id = address.Id,
                    UserId = address.UserId,
                    City = address.City,
                    District = address.District,
                    AddressDetail = address.AddressDetail,
                    Phone = address.Phone,
                    Title = address.Title
                })
                .FirstOrDefaultAsync();
        }

        public async Task<List<UserAddressDto>> GetUserAddressesByUserIdAsync(int userId)
        {
            return await _context.UserAddresses
                .Where(address => address.UserId == userId)
                .Select(address => new UserAddressDto
                {
                    Id = address.Id,
                    UserId = address.UserId,
                    City = address.City,
                    District = address.District,
                    AddressDetail = address.AddressDetail,
                    Phone = address.Phone,
                    Title = address.Title
                })
                .ToListAsync();
        }

        public async Task<UserAddressDto> CreateUserAddressAsync(CreateUserAddressDto dto)
        {
            var address = new UserAddress
            {
                UserId = dto.UserId,
                City = dto.City,
                District = dto.District,
                AddressDetail = dto.AddressDetail,
                Phone = dto.Phone,
                Title = dto.Title
            };

            _context.UserAddresses.Add(address);
            await _context.SaveChangesAsync();

            return new UserAddressDto
            {
                Id = address.Id,
                UserId = address.UserId,
                City = address.City,
                District = address.District,
                AddressDetail = address.AddressDetail,
                Phone = address.Phone,
                Title = address.Title
            };
        }

        public async Task<bool> UpdateUserAddressAsync(int id, UpdateUserAddressDto dto)
        {
            var address = await _context.UserAddresses.FindAsync(id);

            if (address == null)
            {
                return false;
            }

            address.City = dto.City;
            address.District = dto.District;
            address.AddressDetail = dto.AddressDetail;
            address.Phone = dto.Phone;
            address.Title = dto.Title;

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteUserAddressAsync(int id)
        {
            var address = await _context.UserAddresses.FindAsync(id);

            if (address == null)
            {
                return false;
            }

            _context.UserAddresses.Remove(address);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}