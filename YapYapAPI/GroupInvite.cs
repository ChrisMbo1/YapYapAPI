namespace YapYapAPI.Models
{
    public class GroupInvite
    {
        public int Id { get; set; }
        public int SenderId { get; set; }
        public int GroupId { get; set; }

        public User Sender { get; set; } = null!;
        public Group Group { get; set; } = null!;
    }

    public class GroupInviteDto
    {
        public int Id { get; set; }
        public int SenderId { get; set; }
        public string SenderName { get; set; } = string.Empty;
        public int GroupId { get; set; }
        public string GroupName { get; set; } = string.Empty;
    }

    public class CreateGroupInviteDto
    {
        public int GroupId { get; set; }
    }
}