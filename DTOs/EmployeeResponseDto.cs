namespace BSDSystem.API.DTOs
{
    public class EmployeeResponseDto
    {
        public int Id { get; set; }
        public string LastName { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string? MiddleName { get; set; }
        public string BadgeNumber { get; set; } = string.Empty;
        public string Logine { get; set; } = string.Empty; 
        public int RoleId { get; set; } 
        public string RoleTitle { get; set; } = string.Empty;
        public int SubdivisionId { get; set; } 
        public string SubdivisionTitle { get; set; } = string.Empty;
        public int Rating { get; set; }
    }

    public class EmployeeCreateDto
    {
        public string LastName { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string? MiddleName { get; set; }
        public string BadgeNumber { get; set; } = string.Empty; 
        public string Logine { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public int RoleId { get; set; }
        public int SubdivisionId { get; set; }
    }

    public class EmployeeUpdateDto
    {
        public string LastName { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string? MiddleName { get; set; }
        public string BadgeNumber { get; set; } = string.Empty;
        public string? Password { get; set; }
        public int RoleId { get; set; }
        public int SubdivisionId { get; set; }
    }
}