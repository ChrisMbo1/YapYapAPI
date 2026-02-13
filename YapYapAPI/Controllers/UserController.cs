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
    public class UserController : ControllerBase
    {
        private readonly YapYapDbContext _context;

        public UserController(YapYapDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetAllUsers()
        {
            var users = await _context.Users
                .Include(u => u.Status)
                .Select(u => new UserDto
                {
                    Id = u.Id,
                    Name = u.Name,
                    BIO = u.BIO,
                    status_id = u.status_id,
                    created_at = u.created_at
                })
                .ToListAsync();

            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> GetUser(int id)
        {
            var user = await _context.Users
                .Include(u => u.Status)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }

            var userDto = new UserDto
            {
                Id = user.Id,
                Name = user.Name,
                BIO = user.BIO,
                status_id = user.status_id,
                created_at = user.created_at
            };

            return Ok(userDto);
        }

        [HttpGet("me")]
        public async Task<ActionResult<UserDto>> GetCurrentUser()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            var user = await _context.Users
                .Include(u => u.Status)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }

            var userDto = new UserDto
            {
                Id = user.Id,
                Name = user.Name,
                BIO = user.BIO,
                status_id = user.status_id,
                created_at = user.created_at
            };

            return Ok(userDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] RegisterDto updatedUser)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            if (currentUserId != id)
            {
                return Forbid();
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }

            if (!string.IsNullOrEmpty(updatedUser.Name))
            {
                user.Name = updatedUser.Name;
            }

            if (!string.IsNullOrEmpty(updatedUser.BIO))
            {
                user.BIO = updatedUser.BIO;
            }

            await _context.SaveChangesAsync();

            var userDto = new UserDto
            {
                Id = user.Id,
                Name = user.Name,
                BIO = user.BIO,
                status_id = user.status_id,
                created_at = user.created_at
            };

            return Ok(userDto);
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            if (currentUserId != id)
            {
                return Forbid();
            }

            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }

 
    }
}