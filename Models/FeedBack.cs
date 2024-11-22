using System.ComponentModel.DataAnnotations;

namespace EntertaimentLib_API.Models
{
    public class Comment
    {
        public string? CmtId { get; set; }
        public string? TxtCmt { get; set; }
        public DateTime DropAt { get; set; } = DateTime.UtcNow;
    }

    public class FeedBack
    {
        public string? Fid { get; set; } = null;

        [Required]
        public string? Email { get; set; } = null;

        [Required]
        public string? EntId { get; set; } = null;

        public bool Like { get; set; } = false;
        public List<Comment>? Comment { get; set; } = new List<Comment>();
        public int Rating { get; set; }
    }
}
