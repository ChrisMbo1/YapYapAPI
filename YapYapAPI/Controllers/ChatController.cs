using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using YapYapAPI.Data;
using YapYapAPI.Models;

namespace YapYapAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ChatController : ControllerBase
    {
        private readonly YapYapDbContext _context;

        public ChatController(YapYapDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ChatDto>>> GetMyChats()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            var chats = await _context.Chats
                .Where(c => c.UserOne == userId || c.UserTwo == userId)
                .Include(c => c.UserOneNavigation)
                .Include(c => c.UserTwoNavigation)
                .Include(c => c.ChatMessages.OrderByDescending(m => m.CreatedAt).Take(1))
                .ThenInclude(m => m.Sender)
                .ToListAsync();

            var chatDtos = chats.Select(c =>
            {
                var otherUser = c.UserOne == userId ? c.UserTwoNavigation : c.UserOneNavigation;
                var lastMessage = c.ChatMessages.FirstOrDefault();

                return new ChatDto
                {
                    Id = c.Id,
                    UserOne = c.UserOne,
                    UserTwo = c.UserTwo,
                    OtherUserId = otherUser.Id,
                    OtherUserName = otherUser.Name,
                    LastMessage = lastMessage != null ? new ChatMessageDto
                    {
                        Id = lastMessage.Id,
                        Message = lastMessage.Message,
                        SenderId = lastMessage.SenderId,
                        SenderName = lastMessage.Sender.Name,
                        ChatId = lastMessage.ChatId,
                        GroupId = lastMessage.GroupId,
                        CreatedAt = lastMessage.CreatedAt
                    } : null
                };
            }).ToList();

            return Ok(chatDtos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ChatDto>> GetChat(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            var chat = await _context.Chats
                .Include(c => c.UserOneNavigation)
                .Include(c => c.UserTwoNavigation)
                .Include(c => c.ChatMessages.OrderByDescending(m => m.CreatedAt).Take(1))
                .ThenInclude(m => m.Sender)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (chat == null)
            {
                return NotFound(new { message = "Chat not found" });
            }

            if (chat.UserOne != userId && chat.UserTwo != userId)
            {
                return Forbid();
            }

            var otherUser = chat.UserOne == userId ? chat.UserTwoNavigation : chat.UserOneNavigation;
            var lastMessage = chat.ChatMessages.FirstOrDefault();

            var chatDto = new ChatDto
            {
                Id = chat.Id,
                UserOne = chat.UserOne,
                UserTwo = chat.UserTwo,
                OtherUserId = otherUser.Id,
                OtherUserName = otherUser.Name,
                LastMessage = lastMessage != null ? new ChatMessageDto
                {
                    Id = lastMessage.Id,
                    Message = lastMessage.Message,
                    SenderId = lastMessage.SenderId,
                    SenderName = lastMessage.Sender.Name,
                    ChatId = lastMessage.ChatId,
                    GroupId = lastMessage.GroupId,
                    CreatedAt = lastMessage.CreatedAt
                } : null
            };

            return Ok(chatDto);
        }

        [HttpGet("{id}/messages")]
        public async Task<ActionResult<IEnumerable<ChatMessageDto>>> GetChatMessages(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            var chat = await _context.Chats.FindAsync(id);

            if (chat == null)
            {
                return NotFound(new { message = "Chat not found" });
            }

            if (chat.UserOne != userId && chat.UserTwo != userId)
            {
                return Forbid();
            }

            var messages = await _context.ChatMessages
                .Where(m => m.ChatId == id)
                .Include(m => m.Sender)
                .OrderBy(m => m.CreatedAt)
                .Select(m => new ChatMessageDto
                {
                    Id = m.Id,
                    Message = m.Message,
                    SenderId = m.SenderId,
                    SenderName = m.Sender.Name,
                    ChatId = m.ChatId,
                    GroupId = m.GroupId,
                    CreatedAt = m.CreatedAt
                })
                .ToListAsync();

            return Ok(messages);
        }

        [HttpPost]
        public async Task<ActionResult<ChatDto>> CreateChat([FromBody] CreateChatDto dto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            var existingChat = await _context.Chats
    .FirstOrDefaultAsync(c =>
        (c.UserOne == userId && c.UserTwo == dto.OtherUserId) ||
        (c.UserOne == dto.OtherUserId && c.UserTwo == userId));

            if (existingChat != null)
            {
                return BadRequest(new { message = "Chat already exists", chatId = existingChat.Id });
            }

            var areFriends = await _context.Friends
    .AnyAsync(f =>
        (f.UserOne == userId && f.UserTwo == dto.OtherUserId) ||
        (f.UserOne == dto.OtherUserId && f.UserTwo == userId));

            if (!areFriends)
            {
                return BadRequest(new { message = "Users must be friends to chat" });
            }

            var chat = new Chat
            {
                UserOne = userId,
                UserTwo = dto.OtherUserId
            };

            _context.Chats.Add(chat);
            await _context.SaveChangesAsync();

            var otherUser = await _context.Users.FindAsync(dto.OtherUserId);

            var chatDto = new ChatDto
            {
                Id = chat.Id,
                UserOne = chat.UserOne,
                UserTwo = chat.UserTwo,
                OtherUserId = otherUser?.Id ?? 0,
                OtherUserName = otherUser?.Name ?? "",
                LastMessage = null
            };

            return CreatedAtAction(nameof(GetChat), new { id = chat.Id }, chatDto);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteChat(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            var chat = await _context.Chats.FindAsync(id);

            if (chat == null)
            {
                return NotFound(new { message = "Chat not found" });
            }

            if (chat.UserOne != userId && chat.UserTwo != userId)
            {
                return Forbid();
            }

            _context.Chats.Remove(chat);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}