namespace YapYapAPI.Models
{
    public class Group
    {
        public int Id { get; set; }
        public int AdminId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public User Admin { get; set; } = null!;
        public ICollection<UserGroup> UserGroups { get; set; } = new List<UserGroup>();
        public ICollection<GroupInvite> GroupInvites { get; set; } = new List<GroupInvite>();
        public ICollection<ChatMessage> ChatMessages { get; set; } = new List<ChatMessage>();
    }

    public class GroupDto
    {
        public int Id { get; set; }
        public int AdminId { get; set; }
        public string AdminName { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public int MemberCount { get; set; }
    }

    public class CreateGroupDto
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }

    public class UpdateGroupDto
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}