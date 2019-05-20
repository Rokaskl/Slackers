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
    }
}