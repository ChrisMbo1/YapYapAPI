using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using YapYapAPI.Data;
using YapYapAPI.Models;

namespace YapYapAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class StatusController : ControllerBase
    {
        private readonly YapYapDbContext _context;

        public StatusController(YapYapDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<StatusDto>>> GetAllStatuses()
        {
            var statuses = await _context.Statuses
                .Select(s => new StatusDto
                {
                    Id = s.Id,
                    StatusType = s.StatusType
                })
                .ToListAsync();

            return Ok(statuses);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<StatusDto>> GetStatus(int id)
        {
            var status = await _context.Statuses.FindAsync(id);

            if (status == null)
            {
                return NotFound(new { message = "Status not found" });
            }

            var statusDto = new StatusDto
            {
                Id = status.Id,
                StatusType = status.StatusType
            };

            return Ok(statusDto);
        }

        [HttpPost]
        public async Task<ActionResult<StatusDto>> CreateStatus([FromBody] CreateStatusDto createStatusDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var status = new Status
            {
                StatusType = createStatusDto.StatusType
            };

            _context.Statuses.Add(status);
            await _context.SaveChangesAsync();

            var statusDto = new StatusDto
            {
                Id = status.Id,
                StatusType = status.StatusType
            };

            return CreatedAtAction(nameof(GetStatus), new { id = status.Id }, statusDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] CreateStatusDto updateStatusDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var status = await _context.Statuses.FindAsync(id);

            if (status == null)
            {
                return NotFound(new { message = "Status not found" });
            }

            status.StatusType = updateStatusDto.StatusType;

            _context.Statuses.Update(status);
            await _context.SaveChangesAsync();

            var statusDto = new StatusDto
            {
                Id = status.Id,
                StatusType = status.StatusType
            };

            return Ok(statusDto);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStatus(int id)
        {
            var status = await _context.Statuses.FindAsync(id);

            if (status == null)
            {
                return NotFound(new { message = "Status not found" });
            }

            _context.Statuses.Remove(status);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}