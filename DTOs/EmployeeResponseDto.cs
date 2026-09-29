namespace BSDSystem.API.DTOs
{
    public class EmployeeResponseDto
    {
        public int Id { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string BadgeNumber { get; set; }
        public string RoleTitle { get; set; }
        public string SubdivisionTitle { get; set; }

        public int Rating { get; set; }
    }

    public class EmployeeCreateDto
    {
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string Logine { get; set; }
        public string Password { get; set; }
        public int RoleId { get; set; }
        public int SubdivisionId { get; set; }
    }
}
