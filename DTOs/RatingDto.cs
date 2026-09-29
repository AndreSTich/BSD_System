namespace BsdSystem.Api.DTOs
{
    public class RatingBoardDto
    {
        public int Place { get; set; } 
        public string FullName { get; set; } = string.Empty;
        public string SubdivisionTitle { get; set; } = string.Empty;
        public int Rating { get; set; }
    }
}
