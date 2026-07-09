using Microsoft.AspNetCore.Identity;
using TechStore.API.DTOs.Auth;
using TechStore.API.Entities;
using TechStore.API.Repositories.Interfaces;

namespace TechStore.API.Services
{
    public class AuthService
    {
        private const int DefaultCustomerUserTypeId = 2;

        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher<User> _passwordHasher;

        public AuthService(
            IUserRepository userRepository,
            IPasswordHasher<User> passwordHasher)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
        }

        public async Task<AuthResponseDto?> LoginAsync(LoginDto dto)
        {
            User? user = await _userRepository.GetByEmailAsync(dto.Email);

            if (user == null)
            {
                return null;
            }

            PasswordVerificationResult passwordResult =
                _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password);

            if (passwordResult == PasswordVerificationResult.Failed)
            {
                return null;
            }

            return MapUserToAuthResponse(user);
        }

        public async Task<AuthResponseDto?> RegisterAsync(RegisterDto dto)
        {
            User? existingUser = await _userRepository.GetByEmailAsync(dto.Email);

            if (existingUser != null)
            {
                return null;
            }

            User user = new User
            {
                UserTypeId = DefaultCustomerUserTypeId,
                UserName = dto.UserName,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email
            };

            user.PasswordHash = _passwordHasher.HashPassword(user, dto.Password);

            await _userRepository.AddAsync(user);
            await _userRepository.SaveChangesAsync();

            return MapUserToAuthResponse(user);
        }

        private static AuthResponseDto MapUserToAuthResponse(User user)
        {
            return new AuthResponseDto
            {
                UserId = user.Id,
                UserTypeId = user.UserTypeId,
                UserTypeName = user.UserType?.TypeName ?? string.Empty,
                UserName = user.UserName,
                Email = user.Email,
                FullName = $"{user.FirstName} {user.LastName}".Trim()
            };
        }
    }
}
