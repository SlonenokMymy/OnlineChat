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

    public class ChatHistory
    {
        public int Id { get; set; }

        public ChatInfo Chat { get; set; }

        public required string Content { get; set; }

        public DateTime MessageDate { get; set; }

        public User User { get; set; }
    }
}
