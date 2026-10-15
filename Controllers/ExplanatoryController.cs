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
    public class ExplanatoryController : ControllerBase
    {
        private readonly BsdDbContext _context;

        public ExplanatoryController(BsdDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out int currentUserId))
            {
                return Unauthorized();
            }

            var currentUser = await _context.Employees
                .Include(e => e.Role)
                .FirstOrDefaultAsync(e => e.Id == currentUserId);

            if (currentUser == null) return Unauthorized();

            IQueryable<Explanatory> query = _context.Explanatories
                .Include(ex => ex.Employee)
                    .ThenInclude(e => e.Subdivision)
                .Include(ex => ex.Approver);

            if (currentUser.Role.Title == "штаб")
            {}
            else if (currentUser.Role.Title == "командир")
            {
                query = query.Where(ex => ex.Employee.SubdivisionId == currentUser.SubdivisionId);
            }
            else
            {
                query = query.Where(ex => ex.EmployeeId == currentUserId);
            }

            var list = await query
                .OrderByDescending(ex => ex.Dates)
                .ThenByDescending(ex => ex.Id)
                .Select(ex => new ExplanatoryResponseDto
                {
                    Id = ex.Id,
                    EmployeeId = ex.EmployeeId,
                    EmployeeFullName = $"{ex.Employee.LastName} {ex.Employee.FirstName}",
                    SubdivisionTitle = ex.Employee.Subdivision != null ? ex.Employee.Subdivision.Title : "—",
                    DisturberLastName = ex.DisturberLastName,
                    DisturberFirstName = ex.DisturberFirstName,
                    DisturberMiddleName = ex.DisturberMiddleName,
                    DisturberRoomNumber = ex.DisturberRoomNumber,
                    DisturberType = ex.DisturberType,
                    Dates = ex.Dates,
                    ApproverId = ex.ApproverId,
                    ApproverFullName = ex.ApproverId != ex.EmployeeId
                        ? $"{ex.Approver.LastName} {ex.Approver.FirstName}"
                        : "Ожидает проверки",
                    IsApproved = ex.ApproverId != ex.EmployeeId
                })
                .ToListAsync();

            return Ok(list);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ExplanatoryCreateDto dto)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out int currentUserId))
            {
                return Unauthorized();
            }

            if (string.IsNullOrWhiteSpace(dto.DisturberLastName) ||
                string.IsNullOrWhiteSpace(dto.DisturberFirstName) ||
                string.IsNullOrWhiteSpace(dto.DisturberType))
            {
                return BadRequest(new { message = "Заполните обязательные поля: Фамилия, Имя нарушителя и суть нарушения." });
            }

            var note = new Explanatory
            {
                EmployeeId = currentUserId,
                DisturberLastName = dto.DisturberLastName.Trim(),
                DisturberFirstName = dto.DisturberFirstName.Trim(),
                DisturberMiddleName = string.IsNullOrWhiteSpace(dto.DisturberMiddleName) ? null : dto.DisturberMiddleName.Trim(),
                DisturberRoomNumber = string.IsNullOrWhiteSpace(dto.DisturberRoomNumber) ? null : dto.DisturberRoomNumber.Trim(),
                DisturberType = dto.DisturberType.Trim(),
                Dates = dto.Dates,
                ApproverId = currentUserId
            };

            _context.Explanatories.Add(note);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Объяснительная успешно подана на рассмотрение!" });
        }

        [HttpPost("{id}/approve")]
        [Authorize(Roles = "командир,штаб")]
        public async Task<IActionResult> Approve(int id)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out int approverId))
            {
                return Unauthorized();
            }

            var note = await _context.Explanatories
                .Include(ex => ex.Employee)
                .FirstOrDefaultAsync(ex => ex.Id == id);

            if (note == null)
            {
                return NotFound(new { message = "Запись не найдена." });
            }

            if (note.ApproverId != note.EmployeeId)
            {
                return BadRequest(new { message = "Данная объяснительная уже была утверждена ранее." });
            }

            note.ApproverId = approverId;

            const int rewardPoints = 5;
            note.Employee.Rating = (note.Employee.Rating ?? 0) + rewardPoints;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Объяснительная утверждена! Сотруднику начислено +5 баллов рейтинга." });
        }
    }
}