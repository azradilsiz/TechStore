using Microsoft.AspNetCore.Mvc;
using TechStore.API.DTOs.UserTypes;
using TechStore.API.Services;

namespace TechStore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserTypesController : ControllerBase
    {
        private readonly UserTypeService _userTypeService;

        public UserTypesController(UserTypeService userTypeService)
        {
            _userTypeService = userTypeService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUserTypes()
        {
            var userTypes = await _userTypeService.GetAllUserTypesAsync();

            return Ok(userTypes);
        }

        [HttpGet("{userTypeId}")]
        public async Task<IActionResult> GetUserTypeById(int userTypeId)
        {
            var userType = await _userTypeService.GetUserTypeByIdAsync(userTypeId);

            if (userType == null)
            {
                return NotFound();
            }

            return Ok(userType);
        }

        [HttpPost]
        public async Task<IActionResult> CreateUserType(CreateUserTypeDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.TypeName))
            {
                return BadRequest("User type name cannot be empty.");
            }

            var userType = await _userTypeService.CreateUserTypeAsync(dto);

            return CreatedAtAction(nameof(GetUserTypeById), new { userTypeId = userType.Id }, userType);
        }

        [HttpPut("{userTypeId}")]
        public async Task<IActionResult> UpdateUserType(int userTypeId, UpdateUserTypeDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.TypeName))
            {
                return BadRequest("User type name cannot be empty.");
            }

            var result = await _userTypeService.UpdateUserTypeAsync(userTypeId, dto);

            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpDelete("{userTypeId}")]
        public async Task<IActionResult> DeleteUserType(int userTypeId)
        {
            var result = await _userTypeService.DeleteUserTypeAsync(userTypeId);

            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
