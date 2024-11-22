namespace EntertaimentLib_API.Models
{
    public class LogData
    {
        public string? EmailOrMobile { get; set; }
        public string? Password { get; set; }
    }

    public class WatchList
    {
        public string? WatchId { get; set; }
        public string? EntId { get; set; }
        public string? EntTitle { get; set; }
    }
    public class UserRecord 
    {
        public string? UserId { get; set; } = null;
        public string? UserName { get; set; } = null;
        public string? MNo { get; set; } = null;
        public string? Email { get; set; } = null;
        public string? Password { get; set; } = null;
        public List<WatchList>? WatchList { get; set; } = new List<WatchList>();
        public string? Created { get; set;} = null;
    }
}
