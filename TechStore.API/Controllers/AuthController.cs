using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using TechStore.API.DTOs.Auth;
using TechStore.API.Helpers;
using TechStore.API.Services;

namespace TechStore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Email))
            {
                return BadRequest("Email cannot be empty.");
            }

            if (!InputValidationHelper.IsEmailValid(dto.Email))
            {
                return BadRequest("Email format is invalid.");
            }

            if (string.IsNullOrWhiteSpace(dto.Password))
            {
                return BadRequest("Password cannot be empty.");
            }

            AuthResponseDto? result = await _authService.LoginAsync(dto);

            if (result == null)
            {
                return Unauthorized("Email or password is incorrect.");
            }

            return Ok(result);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.UserName))
            {
                return BadRequest("Username cannot be empty.");
            }

            if (string.IsNullOrWhiteSpace(dto.Password))
            {
                return BadRequest("Password cannot be empty.");
            }

            if (dto.Password.Trim().Length < 6)
            {
                return BadRequest("Password must be at least 6 characters.");
            }

            if (string.IsNullOrWhiteSpace(dto.FirstName))
            {
                return BadRequest("First name cannot be empty.");
            }

            if (string.IsNullOrWhiteSpace(dto.LastName))
            {
                return BadRequest("Last name cannot be empty.");
            }

            if (string.IsNullOrWhiteSpace(dto.Email))
            {
                return BadRequest("Email cannot be empty.");
            }

            if (!InputValidationHelper.IsEmailValid(dto.Email))
            {
                return BadRequest("Email format is invalid.");
            }

            AuthResponseDto? result = await _authService.RegisterAsync(dto);

            if (result == null)
            {
                return Conflict("Email is already registered.");
            }

            return Ok(result);
        }

        [Authorize]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword(ChangePasswordDto dto)
        {
            int? userId = User.GetUserId();

            if (userId == null)
            {
                return Unauthorized();
            }

            if (string.IsNullOrWhiteSpace(dto.CurrentPassword))
            {
                return BadRequest("Current password cannot be empty.");
            }

            if (string.IsNullOrWhiteSpace(dto.NewPassword))
            {
                return BadRequest("New password cannot be empty.");
            }

            if (dto.NewPassword.Trim().Length < 6)
            {
                return BadRequest("New password must be at least 6 characters.");
            }

            bool result = await _authService.ChangePasswordAsync(userId.Value, dto);

            if (!result)
            {
                return BadRequest("Current password is incorrect.");
            }

            return NoContent();
        }
    }
}
