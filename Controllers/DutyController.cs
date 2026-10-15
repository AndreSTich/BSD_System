using System.Security.Claims;
using BSDSystem.API.DTOs;
using BSDSystem.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BSDSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DutyController : ControllerBase
    {
        private readonly BsdDbContext _context;

        public DutyController(BsdDbContext context)
        {
            _context = context;
        }

        [HttpGet("month")]
        public async Task<IActionResult> GetDutiesForMonth([FromQuery] int year, [FromQuery] int month)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out int currentUserId))
            {
                return Unauthorized();
            }

            var user = await _context.Employees
                .Include(e => e.Subdivision)
                .FirstOrDefaultAsync(e => e.Id == currentUserId);

            if (user == null) return Unauthorized();

            var duties = await _context.Duties
                .Include(d => d.DutyRegistrations)
                    .ThenInclude(dr => dr.Employee)
                .Include(d => d.DutyRegistrations)
                    .ThenInclude(dr => dr.Approver)
                .Where(d => d.SubdivisionId == user.SubdivisionId &&
                            d.Dates.Year == year &&
                            d.Dates.Month == month)
                .ToListAsync();

            var result = duties.Select(d => new DutyDayDto
            {
                DutyId = d.Id,
                Date = d.Dates,
                ParticipantsCount = d.DutyRegistrations.Count,
                IsCurrentUserRegistered = d.DutyRegistrations.Any(dr => dr.EmployeeId == currentUserId),
                Participants = d.DutyRegistrations.Select(dr => new DutyParticipantDto
                {
                    EmployeeId = dr.EmployeeId,
                    FullName = $"{dr.Employee.LastName} {dr.Employee.FirstName}",
                    Statuse = dr.Statuse,
                    ApproverFullName = dr.Statuse ? $"{dr.Approver.LastName} {dr.Approver.FirstName}" : null
                }).ToList()
            }).ToList();

            return Ok(new
            {
                subdivisionTitle = user.Subdivision?.Title ?? "—",
                duties = result
            });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] DutyRegisterRequestDto dto)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out int currentUserId))
            {
                return Unauthorized();
            }

            var user = await _context.Employees.FindAsync(currentUserId);
            if (user == null) return Unauthorized();

            var duty = await _context.Duties
                .Include(d => d.DutyRegistrations)
                .FirstOrDefaultAsync(d => d.SubdivisionId == user.SubdivisionId && d.Dates == dto.Date);

            if (duty == null)
            {
                duty = new Duty
                {
                    SubdivisionId = user.SubdivisionId,
                    Dates = dto.Date
                };
                _context.Duties.Add(duty);
                await _context.SaveChangesAsync();
            }

           
            if (duty.DutyRegistrations.Count >= 2)
            {
                return BadRequest(new { message = "На этот день все места (2 из 2) уже заняты!" });
            }

            if (duty.DutyRegistrations.Any(dr => dr.EmployeeId == currentUserId))
            {
                return BadRequest(new { message = "Вы уже записаны на дежурство в этот день." });
            }

            var reg = new DutyRegistration
            {
                DutyId = duty.Id,
                EmployeeId = currentUserId,
                ApproverId = currentUserId,
                Statuse = false
            };

            _context.DutyRegistrations.Add(reg);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Вы успешно записались на дежурство!" });
        }

        [HttpPost("cancel")]
        public async Task<IActionResult> Cancel([FromBody] DutyRegisterRequestDto dto)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out int currentUserId))
            {
                return Unauthorized();
            }

            var user = await _context.Employees.FindAsync(currentUserId);
            if (user == null) return Unauthorized();

            var duty = await _context.Duties
                .Include(d => d.DutyRegistrations)
                .FirstOrDefaultAsync(d => d.SubdivisionId == user.SubdivisionId && d.Dates == dto.Date);

            if (duty == null)
            {
                return NotFound(new { message = "Дежурство не найдено." });
            }

            var reg = duty.DutyRegistrations.FirstOrDefault(dr => dr.EmployeeId == currentUserId);
            if (reg == null)
            {
                return BadRequest(new { message = "Вы не записаны на этот день." });
            }

            if (reg.Statuse)
            {
                user.Rating = Math.Max(0, (user.Rating ?? 0) - 10);
            }

            _context.DutyRegistrations.Remove(reg);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Запись на дежурство успешно отменена." });
        }

        [HttpPost("{dutyId}/attendance")]
        [Authorize(Roles = "командир,штаб")]
        public async Task<IActionResult> SaveAttendance(int dutyId, [FromBody] List<AttendanceDto> updates)
        {
            const int points = 10;
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out int approverId))
            {
                return Unauthorized();
            }

            var duty = await _context.Duties
                .Include(d => d.DutyRegistrations)
                    .ThenInclude(dr => dr.Employee)
                .FirstOrDefaultAsync(d => d.Id == dutyId);

            if (duty == null)
            {
                return NotFound(new { message = "Дежурство не найдено." });
            }

            foreach (var update in updates)
            {
                var reg = duty.DutyRegistrations.FirstOrDefault(r => r.EmployeeId == update.EmployeeId);
                if (reg != null && reg.Statuse != update.Statuse)
                {
                    reg.Statuse = update.Statuse;
                    reg.ApproverId = approverId;

                    int curRating = reg.Employee.Rating ?? 0;
                    if (update.Statuse)
                    {
                        reg.Employee.Rating = curRating + points;
                    }
                    else
                    {
                        reg.Employee.Rating = Math.Max(0, curRating - points);
                    }
                }
            }

            await _context.SaveChangesAsync();
            return Ok(new { message = "Отметки дежурства сохранены, рейтинг обновлен!" });
        }
    }
}