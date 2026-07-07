using TechStore.API.DTOs.UserAddresses;
using TechStore.API.Entities;
using TechStore.API.Repositories.Interfaces;

namespace TechStore.API.Services
{
    public class UserAddressService
    {
        private readonly IUserAddressRepository _userAddressRepository;

        public UserAddressService(IUserAddressRepository userAddressRepository)
        {
            _userAddressRepository = userAddressRepository;
        }

        public async Task<List<UserAddressDto>> GetAllUserAddressesAsync()
        {
            List<UserAddress> addresses = await _userAddressRepository.GetAllAsync();

            return addresses.Select(address => MapAddressToDto(address)).ToList();
        }

        public async Task<UserAddressDto?> GetUserAddressByIdAsync(int id)
        {
            UserAddress? address = await _userAddressRepository.GetByIdAsync(id);

            if (address == null)
            {
                return null;
            }

            return MapAddressToDto(address);
        }

        public async Task<List<UserAddressDto>> GetUserAddressesByUserIdAsync(int userId)
        {
            List<UserAddress> addresses = await _userAddressRepository.GetByUserIdAsync(userId);

            return addresses.Select(address => MapAddressToDto(address)).ToList();
        }

        public async Task<UserAddressDto> CreateUserAddressAsync(CreateUserAddressDto dto)
        {
            UserAddress address = new UserAddress
            {
                UserId = dto.UserId,
                City = dto.City,
                District = dto.District,
                AddressDetail = dto.AddressDetail,
                Phone = dto.Phone,
                Title = dto.Title
            };

            await _userAddressRepository.AddAsync(address);
            await _userAddressRepository.SaveChangesAsync();

            return MapAddressToDto(address);
        }

        public async Task<bool> UpdateUserAddressAsync(int id, UpdateUserAddressDto dto)
        {
            UserAddress? address = await _userAddressRepository.GetByIdAsync(id);

            if (address == null)
            {
                return false;
            }

            address.City = dto.City;
            address.District = dto.District;
            address.AddressDetail = dto.AddressDetail;
            address.Phone = dto.Phone;
            address.Title = dto.Title;

            await _userAddressRepository.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteUserAddressAsync(int id)
        {
            UserAddress? address = await _userAddressRepository.GetByIdAsync(id);

            if (address == null)
            {
                return false;
            }

            address.IsDeleted = true;
            await _userAddressRepository.SaveChangesAsync();

            return true;
        }

        private static UserAddressDto MapAddressToDto(UserAddress address)
        {
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
    }
}
