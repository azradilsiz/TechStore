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
            var addresses = await _userAddressService.GetAllUserAddressesAsync();

            return Ok(addresses);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserAddressById(int id)
        {
            var address = await _userAddressService.GetUserAddressByIdAsync(id);

            if (address == null)
            {
                return NotFound();
            }

            return Ok(address);
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetUserAddressesByUserId(int userId)
        {
            var addresses = await _userAddressService.GetUserAddressesByUserIdAsync(userId);

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

            var address = await _userAddressService.CreateUserAddressAsync(dto);

            return CreatedAtAction(nameof(GetUserAddressById), new { id = address.Id }, address);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUserAddress(int id, UpdateUserAddressDto dto)
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

            var result = await _userAddressService.UpdateUserAddressAsync(id, dto);

            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUserAddress(int id)
        {
            var result = await _userAddressService.DeleteUserAddressAsync(id);

            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}