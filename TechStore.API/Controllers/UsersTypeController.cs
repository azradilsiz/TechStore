using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using TechStore.API.DTOs.UserTypes;
using TechStore.API.Services;

namespace TechStore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
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
            List<UserTypeDto> userTypes = await _userTypeService.GetAllUserTypesAsync();

            return Ok(userTypes);
        }

        [HttpGet("{userTypeId}")]
        public async Task<IActionResult> GetUserTypeById(int userTypeId)
        {
            UserTypeDto? userType = await _userTypeService.GetUserTypeByIdAsync(userTypeId);

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

            UserTypeDto userType = await _userTypeService.CreateUserTypeAsync(dto);

            return CreatedAtAction(nameof(GetUserTypeById), new { userTypeId = userType.Id }, userType);
        }

        [HttpPut("{userTypeId}")]
        public async Task<IActionResult> UpdateUserType(int userTypeId, UpdateUserTypeDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.TypeName))
            {
                return BadRequest("User type name cannot be empty.");
            }

            bool result = await _userTypeService.UpdateUserTypeAsync(userTypeId, dto);

            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpDelete("{userTypeId}")]
        public async Task<IActionResult> DeleteUserType(int userTypeId)
        {
            bool result = await _userTypeService.DeleteUserTypeAsync(userTypeId);

            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
