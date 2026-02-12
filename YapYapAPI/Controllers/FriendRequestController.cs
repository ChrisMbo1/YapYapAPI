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
    public class FriendRequestController : ControllerBase
    {
        private readonly YapYapDbContext _context;

        public FriendRequestController(YapYapDbContext context)
        {
            _context = context;
        }

        [HttpGet("sent")]
        public async Task<ActionResult<IEnumerable<FriendRequestDto>>> GetSentRequests()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            var requests = await _context.FriendRequests
                .Where(fr => fr.SenderId == userId)
                .Include(fr => fr.Receiver)
                .Select(fr => new FriendRequestDto
                {
                    Id = fr.Id,
                    SenderId = fr.SenderId,
                    SenderName = fr.Sender.Name,
                    ReceiverId = fr.ReceiverId,
                    ReceiverName = fr.Receiver.Name,
                    Status = fr.Status,
                    CreatedAt = fr.CreatedAt
                })
                .ToListAsync();

            return Ok(requests);
        }

        [HttpGet("received")]
        public async Task<ActionResult<IEnumerable<FriendRequestDto>>> GetReceivedRequests()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            var requests = await _context.FriendRequests
                .Where(fr => fr.ReceiverId == userId && fr.Status == "pending")
                .Include(fr => fr.Sender)
                .Select(fr => new FriendRequestDto
                {
                    Id = fr.Id,
                    SenderId = fr.SenderId,
                    SenderName = fr.Sender.Name,
                    ReceiverId = fr.ReceiverId,
                    ReceiverName = fr.Receiver.Name,
                    Status = fr.Status,
                    CreatedAt = fr.CreatedAt
                })
                .ToListAsync();

            return Ok(requests);
        }

        [HttpPost]
        public async Task<ActionResult<FriendRequestDto>> SendFriendRequest([FromBody] CreateFriendRequestDto dto)
        {
            var senderId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            var existingRequest = await _context.FriendRequests
    .FirstOrDefaultAsync(fr =>
        (fr.SenderId == senderId && fr.ReceiverId == dto.ReceiverId) ||
        (fr.SenderId == dto.ReceiverId && fr.ReceiverId == senderId));

            if (existingRequest != null)
            {
                return BadRequest(new { message = "Friend request already exists" });
            }

            var areFriends = await _context.Friends
    .AnyAsync(f =>
        (f.UserOne == senderId && f.UserTwo == dto.ReceiverId) ||
        (f.UserOne == dto.ReceiverId && f.UserTwo == senderId));

            if (areFriends)
            {
                return BadRequest(new { message = "Already friends" });
            }

            var friendRequest = new FriendRequest
            {
                SenderId = senderId,
                ReceiverId = dto.ReceiverId,
                Status = "pending"
            };

            _context.FriendRequests.Add(friendRequest);
            await _context.SaveChangesAsync();

            var sender = await _context.Users.FindAsync(senderId);
            var receiver = await _context.Users.FindAsync(dto.ReceiverId);

            var requestDto = new FriendRequestDto
            {
                Id = friendRequest.Id,
                SenderId = friendRequest.SenderId,
                SenderName = sender?.Name ?? "",
                ReceiverId = friendRequest.ReceiverId,
                ReceiverName = receiver?.Name ?? "",
                Status = friendRequest.Status,
                CreatedAt = friendRequest.CreatedAt
            };

            return CreatedAtAction(nameof(GetReceivedRequests), requestDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateFriendRequest(int id, [FromBody] UpdateFriendRequestDto dto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            var friendRequest = await _context.FriendRequests.FindAsync(id);

            if (friendRequest == null)
            {
                return NotFound(new { message = "Friend request not found" });
            }

            if (friendRequest.ReceiverId != userId)
            {
                return Forbid();
            }

            if (dto.Status != "accepted" && dto.Status != "rejected")
            {
                return BadRequest(new { message = "Status must be 'accepted' or 'rejected'" });
            }

            friendRequest.Status = dto.Status;
            _context.FriendRequests.Update(friendRequest);

            if (dto.Status == "accepted")
            {
                var friendship = new Friend
                {
                    UserOne = friendRequest.SenderId,
                    UserTwo = friendRequest.ReceiverId
                };
                _context.Friends.Add(friendship);
            }

            await _context.SaveChangesAsync();

            return Ok(new { message = $"Friend request {dto.Status}" });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFriendRequest(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            var friendRequest = await _context.FriendRequests.FindAsync(id);

            if (friendRequest == null)
            {
                return NotFound(new { message = "Friend request not found" });
            }

            if (friendRequest.SenderId != userId && friendRequest.ReceiverId != userId)
            {
                return Forbid();
            }

            _context.FriendRequests.Remove(friendRequest);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}