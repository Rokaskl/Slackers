using Microsoft.EntityFrameworkCore;
using WebApi.Entities;

namespace WebApi.Helpers
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<TimeMark> TimeMarks { get; set; }
        public DbSet<Note> Notes { get; set; }
        public DbSet<AdditionalData> AdditionalDatas {get;set;}
        public DbSet<ChatLine> ChatLines { get; set; }
        public DbSet<Friendship> Friendships { get; set; }
        public DbSet<FriendshipRequest> FriendshipRequests { get; set; }
        public DbSet<Notifications> Notifications { get; set; }
        public DbSet<Log> Logs { get; set; }
    }
}