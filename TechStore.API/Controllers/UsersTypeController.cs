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

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserTypeById(int id)
        {
            var userType = await _userTypeService.GetUserTypeByIdAsync(id);

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

            return CreatedAtAction(nameof(GetUserTypeById), new { id = userType.Id }, userType);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUserType(int id, UpdateUserTypeDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.TypeName))
            {
                return BadRequest("User type name cannot be empty.");
            }

            var result = await _userTypeService.UpdateUserTypeAsync(id, dto);

            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUserType(int id)
        {
            var result = await _userTypeService.DeleteUserTypeAsync(id);

            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}