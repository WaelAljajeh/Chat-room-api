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
    public class RoomController : ControllerBase
    {
        // ✅ GET ALL ROOMS
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<List<RoomDTO>>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        public async Task<ActionResult<ApiResponse<List<RoomDTO>>>> GetAll()
        {
            var rooms = await Room.GetAllRooms();

            if (rooms == null || rooms.Count == 0)
                return NotFound(ApiResponse<List<RoomDTO>>.Fail("No rooms found."));

            return Ok(ApiResponse<List<RoomDTO>>.Ok(rooms, "Rooms retrieved successfully."));
        }

        // ✅ GET ROOM BY ID
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(ApiResponse<RoomDTO>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        public async Task<ActionResult<ApiResponse<RoomDTO>>> GetById(int id)
        {
            var room = await Room.Find(id);
            if (room == null)
                return NotFound(ApiResponse<RoomDTO>.Fail($"Room with ID {id} not found."));

            return Ok(ApiResponse<RoomDTO>.Ok(room.ToRoomDTO(), "Room retrieved successfully."));
        }

        // ✅ ADD ROOM
        [HttpPost("{userId:int}")]
        [ProducesResponseType(typeof(ApiResponse<RoomDTO>), 201)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<ActionResult<ApiResponse<RoomDTO>>> Add([FromBody] RoomDTO dto, int userId)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.Name))
                return BadRequest(ApiResponse<RoomDTO>.Fail("Invalid room data."));

            var newRoom = new Room(dto, Room.enMode.Add);
            bool success = await newRoom.AddRoom(dto, userId);

            if (!success)
                return StatusCode(500, ApiResponse<RoomDTO>.Fail("Error creating room."));

            return CreatedAtAction(nameof(GetById),
                new { id = newRoom.Id },
                ApiResponse<RoomDTO>.Ok(newRoom.ToRoomDTO(), "Room created successfully."));
        }

        // ✅ DELETE ROOM
        [HttpDelete("{id:int}")]
        [ProducesResponseType(typeof(ApiResponse<object>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<ActionResult<ApiResponse<object>>> Delete(int id)
        {
            var existing = await Room.Find(id);
            if (existing == null)
                return NotFound(ApiResponse<object>.Fail("Room not found."));

            bool success = await Room.DeleteRoom(id);

            if (!success)
                return StatusCode(500, ApiResponse<object>.Fail("Error deleting room."));

            return Ok(ApiResponse<object>.Ok(null, "Room deleted successfully."));
        }
    }
}
