namespace YapYapAPI.Models
{
    public class Chat
    {
        public int Id { get; set; }
        public int UserOne { get; set; }
        public int UserTwo { get; set; }

        public User UserOneNavigation { get; set; } = null!;
        public User UserTwoNavigation { get; set; } = null!;
        public ICollection<ChatMessage> ChatMessages { get; set; } = new List<ChatMessage>();
    }

    public class ChatDto
    {
        public int Id { get; set; }
        public int UserOne { get; set; }
        public int UserTwo { get; set; }
        public string OtherUserName { get; set; } = string.Empty;
        public int OtherUserId { get; set; }
        public ChatMessageDto? LastMessage { get; set; }
    }

    public class CreateChatDto
    {
        public int OtherUserId { get; set; }
    }
}