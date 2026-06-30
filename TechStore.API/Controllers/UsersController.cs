using Microsoft.AspNetCore.Mvc;
using TechStore.API.DTOs.Users;
using TechStore.API.Services;

namespace TechStore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UserService _userService;

        public UsersController(UserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetAllUsersAsync();

            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser(CreateUserDto dto)
        {
            if (dto.UserTypeId <= 0)
            {
                return BadRequest("UserTypeId must be greater than zero.");
            }

            if (string.IsNullOrWhiteSpace(dto.UserName))
            {
                return BadRequest("Username cannot be empty.");
            }

            if (string.IsNullOrWhiteSpace(dto.Password))
            {
                return BadRequest("Password cannot be empty.");
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

            var user = await _userService.CreateUserAsync(dto);

            return CreatedAtAction(nameof(GetUserById), new { id = user.Id }, user);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, UpdateUserDto dto)
        {
            if (dto.UserTypeId <= 0)
            {
                return BadRequest("UserTypeId must be greater than zero.");
            }

            if (string.IsNullOrWhiteSpace(dto.UserName))
            {
                return BadRequest("Username cannot be empty.");
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

            var result = await _userService.UpdateUserAsync(id, dto);

            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var result = await _userService.DeleteUserAsync(id);

            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
