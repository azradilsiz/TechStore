using TechStore.API.DTOs.UserTypes;
using TechStore.API.Entities;
using TechStore.API.Repositories.Interfaces;

namespace TechStore.API.Services
{
    public class UserTypeService
    {
        private readonly IUserTypeRepository _userTypeRepository;

        public UserTypeService(IUserTypeRepository userTypeRepository)
        {
            _userTypeRepository = userTypeRepository;
        }

        public async Task<List<UserTypeDto>> GetAllUserTypesAsync()
        {
            var userTypes = await _userTypeRepository.GetAllAsync();

            return userTypes.Select(userType => new UserTypeDto
            {
                Id = userType.Id,
                TypeName = userType.TypeName
            }).ToList();
        }

        public async Task<UserTypeDto?> GetUserTypeByIdAsync(int id)
        {
            var userType = await _userTypeRepository.GetByIdAsync(id);

            if (userType == null)
            {
                return null;
            }

            return new UserTypeDto
            {
                Id = userType.Id,
                TypeName = userType.TypeName
            };
        }

        public async Task<UserTypeDto> CreateUserTypeAsync(CreateUserTypeDto dto)
        {
            var userType = new UserType
            {
                TypeName = dto.TypeName
            };

            await _userTypeRepository.AddAsync(userType);
            await _userTypeRepository.SaveChangesAsync();

            return new UserTypeDto
            {
                Id = userType.Id,
                TypeName = userType.TypeName
            };
        }

        public async Task<bool> UpdateUserTypeAsync(int id, UpdateUserTypeDto dto)
        {
            var userType = await _userTypeRepository.GetByIdAsync(id);

            if (userType == null)
            {
                return false;
            }

            userType.TypeName = dto.TypeName;

            await _userTypeRepository.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteUserTypeAsync(int id)
        {
            var userType = await _userTypeRepository.GetByIdAsync(id);

            if (userType == null)
            {
                return false;
            }

            _userTypeRepository.Delete(userType);
            await _userTypeRepository.SaveChangesAsync();

            return true;
        }
    }
}
