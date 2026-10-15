using BSDSystem.API.DTOs;
using BSDSystem.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BSDSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "штаб")]
    public class EmployeeController : ControllerBase
    {
        private readonly BsdDbContext _context;

        public EmployeeController(BsdDbContext context)
        {
            _context = context;
        }

        // Получить всех сотрудников для таблицы
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var employees = await _context.Employees
                .Include(e => e.Role)
                .Include(e => e.Subdivision)
                .OrderBy(e => e.LastName)
                .Select(e => new EmployeeResponseDto
                {
                    Id = e.Id,
                    LastName = e.LastName,
                    FirstName = e.FirstName,
                    MiddleName = e.MiddleName,
                    BadgeNumber = e.BadgeNumber,
                    Logine = e.Logine,
                    RoleId = e.RoleId,
                    RoleTitle = e.Role.Title,
                    SubdivisionId = e.SubdivisionId,
                    SubdivisionTitle = e.Subdivision.Title,
                    Rating = e.Rating ?? 0
                })
                .ToListAsync();

            return Ok(employees);
        }

        [HttpGet("meta")]
        public async Task<IActionResult> GetMetadata()
        {
            var roles = await _context.Roles
                .Select(r => new { r.Id, r.Title })
                .ToListAsync();

            var subdivisions = await _context.Subdivisions
                .Select(s => new { s.Id, s.Title })
                .ToListAsync();

            return Ok(new { roles, subdivisions });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] EmployeeCreateDto dto)
        {
            if (await _context.Employees.AnyAsync(e => e.Logine == dto.Logine))
            {
                return BadRequest(new { message = "Пользователь с таким логином уже существует." });
            }

            if (await _context.Employees.AnyAsync(e => e.BadgeNumber == dto.BadgeNumber))
            {
                return BadRequest(new { message = "Сотрудник с таким номером жетона уже зарегистрирован." });
            }

            var employee = new Employee
            {
                LastName = dto.LastName.Trim(),
                FirstName = dto.FirstName.Trim(),
                MiddleName = string.IsNullOrWhiteSpace(dto.MiddleName) ? null : dto.MiddleName.Trim(),
                BadgeNumber = dto.BadgeNumber.Trim(),
                Logine = dto.Logine.Trim(),
                Passworde = dto.Password,
                RoleId = dto.RoleId,
                SubdivisionId = dto.SubdivisionId,
                Rating = 0
            };

            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Сотрудник успешно создан.", id = employee.Id });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] EmployeeUpdateDto dto)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
            {
                return NotFound(new { message = "Сотрудник не найден." });
            }

            if (await _context.Employees.AnyAsync(e => e.BadgeNumber == dto.BadgeNumber && e.Id != id))
            {
                return BadRequest(new { message = "Этот номер жетона уже занят другим сотрудником." });
            }

            employee.LastName = dto.LastName.Trim();
            employee.FirstName = dto.FirstName.Trim();
            employee.MiddleName = string.IsNullOrWhiteSpace(dto.MiddleName) ? null : dto.MiddleName.Trim();
            employee.BadgeNumber = dto.BadgeNumber.Trim();
            employee.RoleId = dto.RoleId;
            employee.SubdivisionId = dto.SubdivisionId;

            if (!string.IsNullOrWhiteSpace(dto.Password))
            {
                employee.Passworde = dto.Password;
            }

            await _context.SaveChangesAsync();
            return Ok(new { message = "Данные сотрудника обновлены." });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
            {
                return NotFound(new { message = "Сотрудник не найден." });
            }

            var hasParticipants = await _context.EventParticipants.AnyAsync(ep => ep.EmployeeId == id);
            var hasEvents = await _context.Eventes.AnyAsync(ev => ev.CreatorId == id);
            var hasExplanatories = await _context.Explanatories.AnyAsync(ex => ex.EmployeeId == id || ex.ApproverId == id);

            if (hasParticipants || hasEvents || hasExplanatories)
            {
                return BadRequest(new { message = "Нельзя удалить сотрудника, за которым числятся посещения, события или объяснительные. Удаление отменено для целостности отчетов." });
            }

            _context.Employees.Remove(employee);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Сотрудник удален." });
        }
    }
}