using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using YapYapAPI.Data;
using YapYapAPI.Models;
using YapYapAPI.Services;

namespace YapYapAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly YapYapDbContext _context;
        private readonly ITokenService _tokenService;

        public AuthController(YapYapDbContext context, ITokenService tokenService)
        {
            _context = context;
            _tokenService = tokenService;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<ActionResult<LoginResponseDto>> Register([FromBody] RegisterDto registerDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingUser = await _context.Users
    .FirstOrDefaultAsync(u => u.Name == registerDto.Name);

            if (existingUser != null)
            {
                return BadRequest(new { message = "Username already exists" });
            }

            var statusExists = await _context.Statuses.AnyAsync(s => s.Id == registerDto.status_id);
            if (!statusExists)
            {
                return BadRequest(new { message = "Invalid status_id" });
            }

            var user = new User
            {
                Name = registerDto.Name,
                Password = BCrypt.Net.BCrypt.HashPassword(registerDto.Password),
                BIO = registerDto.BIO,
                status_id = registerDto.status_id,
                created_at = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var token = _tokenService.GenerateToken(user);

            var response = new LoginResponseDto
            {
                Token = token,
                User = new UserDto
                {
                    Id = user.Id,
                    Name = user.Name,
                    BIO = user.BIO,
                    status_id = user.status_id,
                    created_at = user.created_at
                }
            };

            return Ok(response);
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginDto loginDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _context.Users
           .Include(u => u.Status)
           .FirstOrDefaultAsync(u => u.Name == loginDto.Name);

            if (user == null)
            {
                return Unauthorized(new { message = "Invalid username or password" });
            }

            if (!BCrypt.Net.BCrypt.Verify(loginDto.Password, user.Password))
            {
                return Unauthorized(new { message = "Invalid username or password" });
            }

            var token = _tokenService.GenerateToken(user);

            var response = new LoginResponseDto
            {
                Token = token,
                User = new UserDto
                {
                    Id = user.Id,
                    Name = user.Name,
                    BIO = user.BIO,
                    status_id = user.status_id,
                    created_at = user.created_at
                }
            };

            return Ok(response);
        }
    }
}