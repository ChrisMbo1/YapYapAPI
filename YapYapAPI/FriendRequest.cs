namespace YapYapAPI.Models
{
    public class FriendRequest
    {
        public int Id { get; set; }
        public int SenderId { get; set; }
        public int ReceiverId { get; set; }
        public string Status { get; set; } = "pending"; public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public User Sender { get; set; } = null!;
        public User Receiver { get; set; } = null!;
    }

    public class FriendRequestDto
    {
        public int Id { get; set; }
        public int SenderId { get; set; }
        public string SenderName { get; set; } = string.Empty;
        public int ReceiverId { get; set; }
        public string ReceiverName { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    public class CreateFriendRequestDto
    {
        public int ReceiverId { get; set; }
    }

    public class UpdateFriendRequestDto
    {
        public string Status { get; set; } = string.Empty;
    }
}