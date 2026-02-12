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
    public class GroupController : ControllerBase
    {
        private readonly YapYapDbContext _context;

        public GroupController(YapYapDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<GroupDto>>> GetMyGroups()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            var groups = await _context.UserGroups
                .Where(ug => ug.UserId == userId)
                .Include(ug => ug.Group)
                .ThenInclude(g => g.Admin)
                .Include(ug => ug.Group.UserGroups)
                .Select(ug => new GroupDto
                {
                    Id = ug.Group.Id,
                    AdminId = ug.Group.AdminId,
                    AdminName = ug.Group.Admin.Name,
                    Name = ug.Group.Name,
                    Description = ug.Group.Description,
                    CreatedAt = ug.Group.CreatedAt,
                    MemberCount = ug.Group.UserGroups.Count
                })
                .ToListAsync();

            return Ok(groups);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<GroupDto>> GetGroup(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            var isMember = await _context.UserGroups
    .AnyAsync(ug => ug.GroupId == id && ug.UserId == userId);

            if (!isMember)
            {
                return Forbid();
            }

            var group = await _context.Groups
                .Include(g => g.Admin)
                .Include(g => g.UserGroups)
                .FirstOrDefaultAsync(g => g.Id == id);

            if (group == null)
            {
                return NotFound(new { message = "Group not found" });
            }

            var groupDto = new GroupDto
            {
                Id = group.Id,
                AdminId = group.AdminId,
                AdminName = group.Admin.Name,
                Name = group.Name,
                Description = group.Description,
                CreatedAt = group.CreatedAt,
                MemberCount = group.UserGroups.Count
            };

            return Ok(groupDto);
        }

        [HttpGet("{id}/members")]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetGroupMembers(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            var isMember = await _context.UserGroups
    .AnyAsync(ug => ug.GroupId == id && ug.UserId == userId);

            if (!isMember)
            {
                return Forbid();
            }

            var members = await _context.UserGroups
                .Where(ug => ug.GroupId == id)
                .Include(ug => ug.User)
                .Select(ug => new UserDto
                {
                    Id = ug.User.Id,
                    Name = ug.User.Name,
                    BIO = ug.User.BIO,
                    status_id = ug.User.status_id,
                    created_at = ug.User.created_at
                })
                .ToListAsync();

            return Ok(members);
        }

        [HttpPost]
        public async Task<ActionResult<GroupDto>> CreateGroup([FromBody] CreateGroupDto dto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            var group = new Group
            {
                AdminId = userId,
                Name = dto.Name,
                Description = dto.Description
            };

            _context.Groups.Add(group);
            await _context.SaveChangesAsync();

            var userGroup = new UserGroup
            {
                GroupId = group.Id,
                UserId = userId
            };

            _context.UserGroups.Add(userGroup);
            await _context.SaveChangesAsync();

            var admin = await _context.Users.FindAsync(userId);

            var groupDto = new GroupDto
            {
                Id = group.Id,
                AdminId = group.AdminId,
                AdminName = admin?.Name ?? "",
                Name = group.Name,
                Description = group.Description,
                CreatedAt = group.CreatedAt,
                MemberCount = 1
            };

            return CreatedAtAction(nameof(GetGroup), new { id = group.Id }, groupDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateGroup(int id, [FromBody] UpdateGroupDto dto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            var group = await _context.Groups.FindAsync(id);

            if (group == null)
            {
                return NotFound(new { message = "Group not found" });
            }

            if (group.AdminId != userId)
            {
                return Forbid();
            }

            group.Name = dto.Name;
            group.Description = dto.Description;

            _context.Groups.Update(group);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Group updated successfully" });
        }

        [HttpPost("{id}/members")]
        public async Task<IActionResult> AddMember(int id, [FromBody] AddUserToGroupDto dto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            var group = await _context.Groups.FindAsync(id);

            if (group == null)
            {
                return NotFound(new { message = "Group not found" });
            }

            if (group.AdminId != userId)
            {
                return Forbid();
            }

            var isMember = await _context.UserGroups
    .AnyAsync(ug => ug.GroupId == id && ug.UserId == dto.UserId);

            if (isMember)
            {
                return BadRequest(new { message = "User already in group" });
            }

            var userGroup = new UserGroup
            {
                GroupId = id,
                UserId = dto.UserId
            };

            _context.UserGroups.Add(userGroup);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Member added successfully" });
        }

        [HttpDelete("{groupId}/members/{userId}")]
        public async Task<IActionResult> RemoveMember(int groupId, int userId)
        {
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            var group = await _context.Groups.FindAsync(groupId);

            if (group == null)
            {
                return NotFound(new { message = "Group not found" });
            }

            if (group.AdminId != currentUserId && userId != currentUserId)
            {
                return Forbid();
            }

            var userGroup = await _context.UserGroups
                .FirstOrDefaultAsync(ug => ug.GroupId == groupId && ug.UserId == userId);

            if (userGroup == null)
            {
                return NotFound(new { message = "User not in group" });
            }

            _context.UserGroups.Remove(userGroup);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGroup(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            var group = await _context.Groups.FindAsync(id);

            if (group == null)
            {
                return NotFound(new { message = "Group not found" });
            }

            if (group.AdminId != userId)
            {
                return Forbid();
            }

            _context.Groups.Remove(group);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}