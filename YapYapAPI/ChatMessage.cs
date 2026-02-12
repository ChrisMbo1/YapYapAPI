namespace YapYapAPI.Models
{
    public class ChatMessage
    {
        public int Id { get; set; }
        public string Message { get; set; } = string.Empty;
        public int SenderId { get; set; }
        public int? ChatId { get; set; }
        public int? GroupId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public User Sender { get; set; } = null!;
        public Chat? Chat { get; set; }
        public Group? Group { get; set; }
    }

    public class ChatMessageDto
    {
        public int Id { get; set; }
        public string Message { get; set; } = string.Empty;
        public int SenderId { get; set; }
        public string SenderName { get; set; } = string.Empty;
        public int? ChatId { get; set; }
        public int? GroupId { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CreateChatMessageDto
    {
        public string Message { get; set; } = string.Empty;
        public int? ChatId { get; set; }
        public int? GroupId { get; set; }
    }
}