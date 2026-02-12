namespace YapYapAPI.Models
{
    public class Status
    {
        public int Id { get; set; }
        public string StatusType { get; set; } = string.Empty;

        public ICollection<User> Users { get; set; } = new List<User>();
    }

    public class StatusDto
    {
        public int Id { get; set; }
        public string StatusType { get; set; } = string.Empty;
    }

    public class CreateStatusDto
    {
        public string StatusType { get; set; } = string.Empty;
    }
}