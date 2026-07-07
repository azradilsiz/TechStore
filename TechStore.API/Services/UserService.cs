using TechStore.API.DTOs.Users;
using TechStore.API.Entities;
using TechStore.API.Repositories.Interfaces;

namespace TechStore.API.Services
{
    public class UserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<List<UserDto>> GetAllUsersAsync()
        {
            List<User> users = await _userRepository.GetAllAsync();

            return users.Select(user => new UserDto
            {
                Id = user.Id,
                UserTypeId = user.UserTypeId,
                UserName = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email
            }).ToList();
        }

        public async Task<UserDto?> GetUserByIdAsync(int id)
        {
            User? user = await _userRepository.GetByIdAsync(id);

            if (user == null)
            {
                return null;
            }

            return new UserDto
            {
                Id = user.Id,
                UserTypeId = user.UserTypeId,
                UserName = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email
            };
        }

        public async Task<UserDto> CreateUserAsync(CreateUserDto dto)
        {
            User user = new User
            {
                UserTypeId = dto.UserTypeId,
                UserName = dto.UserName,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,

                // Şimdilik düz metni kaydediyoruz.
                // JWT ve güvenlik kısmına geçtiğimizde hash'leyeceğiz.
                PasswordHash = dto.Password
            };

            await _userRepository.AddAsync(user);
            await _userRepository.SaveChangesAsync();

            return new UserDto
            {
                Id = user.Id,
                UserTypeId = user.UserTypeId,
                UserName = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email
            };
        }

        public async Task<bool> UpdateUserAsync(int id, UpdateUserDto dto)
        {
            User? user = await _userRepository.GetByIdAsync(id);

            if (user == null)
            {
                return false;
            }

            user.UserTypeId = dto.UserTypeId;
            user.UserName = dto.UserName;
            user.FirstName = dto.FirstName;
            user.LastName = dto.LastName;
            user.Email = dto.Email;

            await _userRepository.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            User? user = await _userRepository.GetByIdAsync(id);

            if (user == null)
            {
                return false;
            }

            user.IsDeleted = true;
            await _userRepository.SaveChangesAsync();

            return true;
        }
    }
}
