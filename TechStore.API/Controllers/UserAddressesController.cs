using Microsoft.AspNetCore.Mvc;
using TechStore.API.DTOs.UserAddresses;
using TechStore.API.Services;

namespace TechStore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserAddressesController : ControllerBase
    {
        private readonly UserAddressService _userAddressService;

        public UserAddressesController(UserAddressService userAddressService)
        {
            _userAddressService = userAddressService;
        }

        [HttpGet]
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

            return Ok(address);
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetUserAddressesByUserId(int userId)
        {
            List<UserAddressDto> addresses = await _userAddressService.GetUserAddressesByUserIdAsync(userId);

            return Ok(addresses);
        }

        [HttpPost]
        public async Task<IActionResult> CreateUserAddress(CreateUserAddressDto dto)
        {
            if (dto.UserId <= 0)
            {
                return BadRequest("UserId must be greater than zero.");
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

            UserAddressDto address = await _userAddressService.CreateUserAddressAsync(dto);

            return CreatedAtAction(nameof(GetUserAddressById), new { userAddressId = address.Id }, address);
        }

        [HttpPut("{userAddressId}")]
        public async Task<IActionResult> UpdateUserAddress(int userAddressId, UpdateUserAddressDto dto)
        {
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
            bool result = await _userAddressService.DeleteUserAddressAsync(userAddressId);

            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
