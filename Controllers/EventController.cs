using BsdSystem.Api.DTOs;
using BSDSystem.API.DTOs;
using BSDSystem.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BSDSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] 
    public class EventController : ControllerBase
    {
        private readonly BsdDbContext _context; 

        public EventController(BsdDbContext context)
        {
            _context = context;
        }

        [HttpGet("month")]
        public async Task<IActionResult> GetEventsForMonth([FromQuery] int year, [FromQuery] int month)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdString, out int currentUserId))
            {
                return Unauthorized();
            }

            var events = await _context.Eventes
                .Where(e => e.Dates.Year == year && e.Dates.Month == month)
                .Select(e => new EventCalendarDto
                {
                    Id = e.Id,
                    Title = e.Title,
                    Type = e.Ttype,
                    Date = e.Dates,
                    Time = e.Times,
                    Location = e.Locations,
                    IsRegistered = _context.EventParticipants.Any(ep => ep.EventId == e.Id && ep.EmployeeId == currentUserId)
                })
                .ToListAsync();

            return Ok(events);
        }

        [HttpPost("{id}/register")]
        public async Task<IActionResult> RegisterForEvent(int id)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdString, out int currentUserId))
            {
                return Unauthorized();
            }

            var eventEntity = await _context.Eventes.FindAsync(id);
            if (eventEntity == null)
            {
                return NotFound(new { message = "Мероприятие не найдено" });
            }

            var alreadyRegistered = await _context.EventParticipants
                .AnyAsync(ep => ep.EventId == id && ep.EmployeeId == currentUserId);

            if (alreadyRegistered)
            {
                return BadRequest(new { message = "Вы уже записаны на это мероприятие" });
            }

            var participant = new EventParticipant
            {
                EventId = id,
                EmployeeId = currentUserId,
                Statuse = false, 
                ApproverId = eventEntity.CreatorId
            };

            _context.EventParticipants.Add(participant);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Вы успешно записались на мероприятие!" });
        }
    }
}