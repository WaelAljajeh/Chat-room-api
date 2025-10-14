using Microsoft.AspNetCore.SignalR;
using Chat_Room_api_project.Models;
using BussinesLogic;
using System.Threading.Tasks;

namespace Chat_Room_api_project.Hubs
{
    public class SendMessage : Hub
    {
        // ✅ Method to send a message to all clients in a specific room
        public async Task SendMessageToRoom(MessageDTO dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.Text) || dto.UserId <= 0 || dto.RoomId <= 0)
            {
                await Clients.Caller.SendAsync("Error", "Invalid message data.");
                return;
            }

            // Save message to database
            var newMessage = new Message(dto, Message.enMode.Add);
            var success = await newMessage.Save();

            if (!success)
            {
                await Clients.Caller.SendAsync("Error", "Error saving message.");
                return;
            }

            var messageDto = newMessage.ToMessageDTO();

            // Send the message to all clients in the room
            await Clients.Group($"Room_{dto.RoomId}").SendAsync("ReceiveMessage", messageDto);
        }

        // ✅ Join a room
        public async Task JoinRoom(int roomId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"Room_{roomId}");
            await Clients.Group($"Room_{roomId}").SendAsync("UserJoined", $"{Context.ConnectionId} joined Room {roomId}");
        }

        // ✅ Leave a room
        public async Task LeaveRoom(int roomId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"Room_{roomId}");
            await Clients.Group($"Room_{roomId}").SendAsync("UserLeft", $"{Context.ConnectionId} left Room {roomId}");
        }

        // Optional: notify when a user connects
        public override async Task OnConnectedAsync()
        {
            await Clients.Caller.SendAsync("Connected", $"Connected with ID: {Context.ConnectionId}");
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(System.Exception exception)
        {
            // Optional: handle disconnection logic
            await base.OnDisconnectedAsync(exception);
        }
    }
}
