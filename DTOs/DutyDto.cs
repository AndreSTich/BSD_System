namespace BSDSystem.API.DTOs
{
    public class DutyDayDto
    {
        public int? DutyId { get; set; }
        public DateOnly Date { get; set; }
        public int ParticipantsCount { get; set; }
        public bool IsFull => ParticipantsCount >= 2;
        public bool IsCurrentUserRegistered { get; set; }
        public List<DutyParticipantDto> Participants { get; set; } = new();
    }

    public class DutyParticipantDto
    {
        public int EmployeeId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public bool Statuse { get; set; }
        public string? ApproverFullName { get; set; }
    }

    public class DutyRegisterRequestDto
    {
        public DateOnly Date { get; set; }
    }
}