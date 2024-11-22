namespace EntertaimentLib_API.Models
{
    public class Like
    {
        public bool IsLike { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
