namespace BSDSystem.API.DTOs
{
    public class AttendanceDto
    {
        public int EmployeeId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string? SubdivisionTitle { get; set; }
        public bool Statuse { get; set; }
    }
}