namespace OnlineChat
{
    using Microsoft.EntityFrameworkCore;
    using OnlineChat.DataModels;

    public class ChatDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<ChatInfo> ChatInfos { get; set; }
        public DbSet<ChatHistory> ChatHistories { get; set; }

        public ChatDbContext(DbContextOptions<ChatDbContext> options)
           : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        // some basic sql queries, just in case
        // Select* from users
        // Select* from ChatInfos
        // Select* from ChatHistories
        // delete from ChatHistories where Id = '14'
    }
}
