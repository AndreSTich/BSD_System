namespace BSDSystem.API.DTOs
{
    public class EventCreateDto
    {
        public string Title { get; set; } = string.Empty;
        public string? Type { get; set; }
        public DateOnly Date { get; set; }
        public TimeOnly Time { get; set; }
        public string Location { get; set; } = string.Empty;
    }
}