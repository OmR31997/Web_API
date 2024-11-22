namespace EntertaimentLib_API.Models
{
    public class Movie
    {
        public string? Id { get; set; } =null;
        public string? Title { get; set; } = null;
        public string? Category { get; set; } = null;
        public string? Language { get; set; } = null;
        public string? ReleaseDate { get; set; } = null;
        public string? Location { get; set; } = null;
        public string? Director { get; set; } = null;
        public string? Producer { get; set; } = null;
        public string? Cast { get; set; } = null;
        public string? Description { get; set; } = null;
        public string? Rating { get; set; } = null;
        public string? PosterUrl { get; set; } = null;
        public string? TrailerUrl { get; set; } = null;
        public string? Imdb { get; set; } = null;
    }
}
