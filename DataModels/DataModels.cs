using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineChat.DataModels
{
    public class User
    {
        public int Id { get; set; }
        public required string Username { get; set; }
    }

    public class ChatInfo
    {
        public int Id { get; set; }
        public required string ChatName { get; set; }
        public int OwnerId { get; set; }
    }

    public class ChatInfoWithChatHistory : ChatInfo
    { 
        public List<ChatHistory> ChatHistories { get; set; }
    }

    public class ChatHistory
    {
        public int Id { get; set; }

        public int  ChatId { get; set; }

        public required ChatInfo Chat { get; set; }

        public required string Content { get; set; }

        public DateTime MessageDate { get; set; }

        public required User User { get; set; }
    }
}
