namespace BSDSystem.API.DTOs
{
    public class ExplanatoryResponseDto
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public string EmployeeFullName { get; set; } = string.Empty;
        public string SubdivisionTitle { get; set; } = string.Empty;
        public string DisturberLastName { get; set; } = string.Empty;
        public string DisturberFirstName { get; set; } = string.Empty;
        public string? DisturberMiddleName { get; set; }
        public string? DisturberRoomNumber { get; set; }
        public string DisturberType { get; set; } = string.Empty;
        public DateOnly Dates { get; set; }
        public int ApproverId { get; set; }
        public string ApproverFullName { get; set; } = string.Empty;
        public bool IsApproved { get; set; }
    }

    public class ExplanatoryCreateDto
    {
        public string DisturberLastName { get; set; } = string.Empty;
        public string DisturberFirstName { get; set; } = string.Empty;
        public string? DisturberMiddleName { get; set; }
        public string? DisturberRoomNumber { get; set; }
        public string DisturberType { get; set; } = string.Empty;
        public DateOnly Dates { get; set; }
    }
}