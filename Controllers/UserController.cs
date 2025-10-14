using Microsoft.AspNetCore.Mvc;
using Chat_Room_api_project.Models;
using BussinesLogic;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Chat_Room_api_project.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class UserController : ControllerBase
    {
        // ✅ GET ALL USERS
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<List<UserDTO>>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        public async Task<ActionResult<ApiResponse<List<UserDTO>>>> GetAll()
        {
            var users = await BussinesLogic.User.GetAllUsers();

            if (users == null || users.Count == 0)
                return NotFound(ApiResponse<List<UserDTO>>.Fail("No users found."));

            return Ok(ApiResponse<List<UserDTO>>.Ok(users, "Users retrieved successfully."));
        }

        // ✅ GET USER BY ID
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(ApiResponse<UserDTO>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        public async Task<ActionResult<ApiResponse<UserDTO>>> GetById(int id)
        {
            var user = await BussinesLogic.User.Find(id);
            if (user == null)
                return NotFound(ApiResponse<UserDTO>.Fail($"User with ID {id} not found."));

            return Ok(ApiResponse<UserDTO>.Ok(user.ToUserDTO(), "User retrieved successfully."));
        }

        // ✅ GET USER BY USERNAME
        [HttpGet("by-username/{username}")]
        [ProducesResponseType(typeof(ApiResponse<UserDTO>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        public async Task<ActionResult<ApiResponse<UserDTO>>> GetByUsername(string username)
        {
            var user = await BussinesLogic.User.Find(username);
            if (user == null)
                return NotFound(ApiResponse<UserDTO>.Fail($"User '{username}' not found."));

            return Ok(ApiResponse<UserDTO>.Ok(user.ToUserDTO(), "User retrieved successfully."));
        }

        // ✅ ADD USER
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<UserDTO>), 201)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<ActionResult<ApiResponse<UserDTO>>> Add([FromBody] UserDTO dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.Username) || string.IsNullOrWhiteSpace(dto.PasswordHash))
                return BadRequest(ApiResponse<UserDTO>.Fail("Invalid user data."));

            var newUser = new User(dto, BussinesLogic.User.enMode.Add);
            var success = await newUser.Save();

            if (!success)
                return StatusCode(500, ApiResponse<UserDTO>.Fail("Error adding user."));

            return CreatedAtAction(nameof(GetById),
                new { id = newUser.Id },
                ApiResponse<UserDTO>.Ok(newUser.ToUserDTO(), "User created successfully."));
        }

        // ✅ UPDATE USER
        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(ApiResponse<UserDTO>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<ActionResult<ApiResponse<UserDTO>>> Update(int id, [FromBody] UserDTO dto)
        {
            if (dto == null || id != dto.Id)
                return BadRequest(ApiResponse<UserDTO>.Fail("User ID mismatch or invalid data."));

            var existing = await BussinesLogic.User.Find(id);
            if (existing == null)
                return NotFound(ApiResponse<UserDTO>.Fail("User not found."));

            var updatedUser = new User(dto, BussinesLogic.User.enMode.Update);
            var success = await updatedUser.Save();

            if (!success)
                return StatusCode(500, ApiResponse<UserDTO>.Fail("Failed to update user."));

            return Ok(ApiResponse<UserDTO>.Ok(updatedUser.ToUserDTO(), "User updated successfully."));
        }

        // ✅ DELETE USER
        [HttpDelete("{id:int}")]
        [ProducesResponseType(typeof(ApiResponse<object>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<ActionResult<ApiResponse<object>>> Delete(int id)
        {
            var existing = await BussinesLogic.User.Find(id);
            if (existing == null)
                return NotFound(ApiResponse<object>.Fail("User not found."));

            var success = await BussinesLogic.User.DeleteUser(id);

            if (!success)
                return StatusCode(500, ApiResponse<object>.Fail("Error deleting user."));

            return Ok(ApiResponse<object>.Ok(null, "User deleted successfully."));
        }
    }
}
