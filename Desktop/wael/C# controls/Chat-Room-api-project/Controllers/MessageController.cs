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
    public class MessageController : ControllerBase
    {
        // ✅ GET ALL MESSAGES BY ROOM ID
        [HttpGet("by-room/{roomId:int}")]
        [ProducesResponseType(typeof(ApiResponse<List<MessageDTO>>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        public async Task<ActionResult<ApiResponse<List<MessageDTO>>>> GetMessagesByRoom(int roomId)
        {
            
            var messages = await Message.GetMessagesByRoom(roomId);

            if (messages == null || messages.Count == 0)
                return NotFound(ApiResponse<List<MessageDTO>>.Fail("No messages found for this room."));

            return Ok(ApiResponse<List<MessageDTO>>.Ok(messages, "Messages retrieved successfully."));
        }

        // ✅ GET MESSAGE BY ID
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(ApiResponse<MessageDTO>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        public async Task<ActionResult<ApiResponse<MessageDTO>>> GetById(int id)
        {
            
            var message = await Message.Find(id);

            if (message == null)
                return NotFound(ApiResponse<MessageDTO>.Fail($"Message with ID {id} not found."));

            return Ok(ApiResponse<MessageDTO>.Ok(message.ToMessageDTO(), "Message retrieved successfully."));
        }

        // ✅ ADD MESSAGE
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<MessageDTO>), 201)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<ActionResult<ApiResponse<MessageDTO>>> Add([FromBody] MessageDTO dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.Text) || dto.UserId <= 0 || dto.RoomId <= 0)
                return BadRequest(ApiResponse<MessageDTO>.Fail("Invalid message data."));

            var newMessage = new Message(dto, Message.enMode.Add);
            var success = await newMessage.Save();

            if (!success)
                return StatusCode(500, ApiResponse<MessageDTO>.Fail("Error adding message."));

            return CreatedAtAction(nameof(GetById),
                new { id = newMessage.Id },
                ApiResponse<MessageDTO>.Ok(newMessage.ToMessageDTO(), "Message created successfully."));
        }

        // ✅ UPDATE MESSAGE
        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(ApiResponse<MessageDTO>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<ActionResult<ApiResponse<MessageDTO>>> Update(int id, [FromBody] MessageDTO dto)
        {
            if (dto == null || id != dto.Id)
                return BadRequest(ApiResponse<MessageDTO>.Fail("Message ID mismatch or invalid data."));

            var existing = await Message.Find(id);
            if (existing == null)
                return NotFound(ApiResponse<MessageDTO>.Fail("Message not found."));

            var updatedMessage = new Message(dto, Message.enMode.Update);
            var success = await updatedMessage.Save();

            if (!success)
                return StatusCode(500, ApiResponse<MessageDTO>.Fail("Failed to update message."));

            return Ok(ApiResponse<MessageDTO>.Ok(updatedMessage.ToMessageDTO(), "Message updated successfully."));
        }

        // ✅ DELETE MESSAGE
        [HttpDelete("{id:int}")]
        [ProducesResponseType(typeof(ApiResponse<object>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<ActionResult<ApiResponse<object>>> Delete(int id)
        {
            var existing = await Message.Find(id);
            if (existing == null)
                return NotFound(ApiResponse<object>.Fail("Message not found."));

            var success = await existing.DeleteMessage(id);

            if (!success)
                return StatusCode(500, ApiResponse<object>.Fail("Error deleting message."));

            return Ok(ApiResponse<object>.Ok(null, "Message deleted successfully."));
        }
    }
}
