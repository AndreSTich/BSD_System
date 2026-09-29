namespace BsdSystem.Api.DTOs
{
    public class EventCalendarDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public TimeSpan Time { get; set; }
        public string Location { get; set; } = string.Empty;
        public bool IsRegistered { get; set; }
    }
}
