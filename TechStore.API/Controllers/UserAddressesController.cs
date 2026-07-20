using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using TechStore.API.DTOs.UserAddresses;
using TechStore.API.Helpers;
using TechStore.API.Services;

namespace TechStore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserAddressesController : ControllerBase
    {
        private readonly UserAddressService _userAddressService;

        public UserAddressesController(UserAddressService userAddressService)
        {
            _userAddressService = userAddressService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllUserAddresses()
        {
            List<UserAddressDto> addresses = await _userAddressService.GetAllUserAddressesAsync();

            return Ok(addresses);
        }

        [HttpGet("{userAddressId}")]
        public async Task<IActionResult> GetUserAddressById(int userAddressId)
        {
            UserAddressDto? address = await _userAddressService.GetUserAddressByIdAsync(userAddressId);

            if (address == null)
            {
                return NotFound();
            }

            int? currentUserId = User.GetUserId();
            if (currentUserId == null)
            {
                return Unauthorized();
            }

            if (!User.IsInRole("Admin") && address.UserId != currentUserId.Value)
            {
                return Forbid();
            }

            return Ok(address);
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetUserAddressesByUserId(int userId)
        {
            int? currentUserId = User.GetUserId();
            if (currentUserId == null)
            {
                return Unauthorized();
            }

            if (!User.IsInRole("Admin") && userId != currentUserId.Value)
            {
                return Forbid();
            }

            List<UserAddressDto> addresses = await _userAddressService.GetUserAddressesByUserIdAsync(userId);

            return Ok(addresses);
        }

        [HttpPost]
        public async Task<IActionResult> CreateUserAddress(CreateUserAddressDto dto)
        {
            int? currentUserId = User.GetUserId();
            if (currentUserId == null)
            {
                return Unauthorized();
            }

            if (string.IsNullOrWhiteSpace(dto.City))
            {
                return BadRequest("City cannot be empty.");
            }

            if (string.IsNullOrWhiteSpace(dto.District))
            {
                return BadRequest("District cannot be empty.");
            }

            if (string.IsNullOrWhiteSpace(dto.AddressDetail))
            {
                return BadRequest("Address detail cannot be empty.");
            }

            if (!InputValidationHelper.IsPhoneValid(dto.Phone))
            {
                return BadRequest("Phone number must be 10 digits or 11 digits starting with 0.");
            }

            UserAddressDto address = await _userAddressService.CreateUserAddressAsync(currentUserId.Value, dto);

            return CreatedAtAction(nameof(GetUserAddressById), new { userAddressId = address.Id }, address);
        }

        [HttpPut("{userAddressId}")]
        public async Task<IActionResult> UpdateUserAddress(int userAddressId, UpdateUserAddressDto dto)
        {
            UserAddressDto? address = await _userAddressService.GetUserAddressByIdAsync(userAddressId);
            if (address == null)
            {
                return NotFound();
            }

            int? currentUserId = User.GetUserId();
            if (currentUserId == null)
            {
                return Unauthorized();
            }

            if (!User.IsInRole("Admin") && address.UserId != currentUserId.Value)
            {
                return Forbid();
            }

            if (string.IsNullOrWhiteSpace(dto.City))
            {
                return BadRequest("City cannot be empty.");
            }

            if (string.IsNullOrWhiteSpace(dto.District))
            {
                return BadRequest("District cannot be empty.");
            }

            if (string.IsNullOrWhiteSpace(dto.AddressDetail))
            {
                return BadRequest("Address detail cannot be empty.");
            }

            if (!InputValidationHelper.IsPhoneValid(dto.Phone))
            {
                return BadRequest("Phone number must be 10 digits or 11 digits starting with 0.");
            }

            bool result = await _userAddressService.UpdateUserAddressAsync(userAddressId, dto);

            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpDelete("{userAddressId}")]
        public async Task<IActionResult> DeleteUserAddress(int userAddressId)
        {
            UserAddressDto? address = await _userAddressService.GetUserAddressByIdAsync(userAddressId);
            if (address == null)
            {
                return NotFound();
            }

            int? currentUserId = User.GetUserId();
            if (currentUserId == null)
            {
                return Unauthorized();
            }

            if (!User.IsInRole("Admin") && address.UserId != currentUserId.Value)
            {
                return Forbid();
            }

            bool result = await _userAddressService.DeleteUserAddressAsync(userAddressId);

            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
