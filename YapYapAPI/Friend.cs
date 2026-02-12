namespace YapYapAPI.Models
{
    public class Friend
    {
        public int Id { get; set; }
        public int UserOne { get; set; }
        public int UserTwo { get; set; }

        public User UserOneNavigation { get; set; } = null!;
        public User UserTwoNavigation { get; set; } = null!;
    }

    public class FriendDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string UserBio { get; set; } = string.Empty;
        public int UserStatusId { get; set; }
    }
}