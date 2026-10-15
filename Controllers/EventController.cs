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

        [HttpGet("{id}/participants")]
        [Authorize(Roles = "штаб,командир")]
        public async Task<IActionResult> GetEventParticipants(int id)
        {
            var eventExists = await _context.Eventes.AnyAsync(e => e.Id == id);
            if (!eventExists)
            {
                return NotFound(new { message = "Мероприятие не найдено." });
            }

            var participants = await _context.EventParticipants
                .Include(ep => ep.Employee)
                    .ThenInclude(e => e.Subdivision)
                .Where(ep => ep.EventId == id)
                .Select(ep => new AttendanceDto
                {
                    EmployeeId = ep.EmployeeId,
                    FullName = $"{ep.Employee.LastName} {ep.Employee.FirstName}" +
                               (string.IsNullOrEmpty(ep.Employee.MiddleName) ? "" : $" {ep.Employee.MiddleName}"),
                    SubdivisionTitle = ep.Employee.Subdivision != null ? ep.Employee.Subdivision.Title : "Не указано",
                    Statuse = ep.Statuse
                })
                .ToListAsync();

            return Ok(participants);
        }

        [HttpPost("{id}/attendance")]
        [Authorize(Roles = "штаб,командир")]
        public async Task<IActionResult> SaveAttendance(int id, [FromBody] List<AttendanceDto> updates)
        {
            const int pointsPerEvent = 10;

            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdString, out int approverId))
            {
                return Unauthorized(new { message = "Не удалось определить проверяющего." });
            }

            var currentParticipants = await _context.EventParticipants
                .Include(ep => ep.Employee)
                .Where(ep => ep.EventId == id)
                .ToListAsync();

            foreach (var update in updates)
            {
                var participant = currentParticipants.FirstOrDefault(p => p.EmployeeId == update.EmployeeId);
                if (participant != null && participant.Statuse != update.Statuse)
                {
                    participant.Statuse = update.Statuse;
                    participant.ApproverId = approverId;

                    int currentRating = participant.Employee.Rating ?? 0;

                    if (update.Statuse)
                    {
                        participant.Employee.Rating = currentRating + pointsPerEvent;
                    }
                    else
                    {
                        participant.Employee.Rating = Math.Max(0, currentRating - pointsPerEvent);
                    }
                }
            }

            await _context.SaveChangesAsync();
            return Ok(new { message = "Отметки сохранены, рейтинг пересчитан." });
        }
    }
}