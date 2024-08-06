using Microsoft.AspNetCore.Mvc;
using UserManagementService.Domain.Models;
using UserManagementService.DTOs;
using UserManagementService.Services;

namespace UserManagementService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("create-user")]
        public async Task<IActionResult> CreateUser([FromBody] User user, [FromHeader(Name = "x-Device")] string device)
        {
            var createdUser = await _userService.CreateUserAsync(user, device);
            return CreatedAtAction(nameof(GetUserById), new { id = createdUser.Id }, createdUser);
        }

        [HttpGet("get-user/{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);

            if (user == null)
                return NotFound();

            return Ok(user);
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchUsers([FromQuery] UserSearchDto userSearchDto)
        {
            if (userSearchDto.IsEmpty())
                return BadRequest("At least one search parameter is required");

            var users = await _userService.SearchUsersAsync(userSearchDto);
            return Ok(users);
        }
    }
}
