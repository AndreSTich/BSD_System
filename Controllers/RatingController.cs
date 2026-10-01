using BsdSystem.Api.DTOs;
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
    public class RatingController : ControllerBase
    {
        private readonly BsdDbContext _context;

        public RatingController(BsdDbContext context)
        {
            _context = context;
        }

        [HttpGet("top")]
        public async Task<IActionResult> GetTopRatings()
        {
            var employees = await _context.Employees
                .Include(e => e.Subdivision)
                .OrderByDescending(e => e.Rating ?? 0) 
                .ToListAsync();

            var ratingBoard = new List<RatingBoardDto>();
            int currentPlace = 1;

            foreach (var emp in employees)
            {
                ratingBoard.Add(new RatingBoardDto
                {
                    Place = currentPlace++,
                    FullName = $"{emp.LastName} {emp.FirstName}",

                    SubdivisionTitle = emp.Subdivision?.Title ?? "Не назначено",

                    Rating = emp.Rating ?? 0
                });
            }

            return Ok(ratingBoard);
        }
    }
}