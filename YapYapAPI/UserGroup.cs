namespace YapYapAPI.Models
{
    public class UserGroup
    {
        public int Id { get; set; }
        public int GroupId { get; set; }
        public int UserId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Group Group { get; set; } = null!;
        public User User { get; set; } = null!;
    }

    public class UserGroupDto
    {
        public int Id { get; set; }
        public int GroupId { get; set; }
        public string GroupName { get; set; } = string.Empty;
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    public class AddUserToGroupDto
    {
        public int UserId { get; set; }
    }
}