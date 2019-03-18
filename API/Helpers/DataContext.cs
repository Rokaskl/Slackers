using Microsoft.EntityFrameworkCore;
using WebApi.Entities;

namespace WebApi.Helpers
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        public DbSet<User> Users
        { get; set; }
<<<<<<< HEAD
        public DbSet<Room> Rooms { get; set; }
=======
>>>>>>> 050215db5888b4bd489eacf3e31ad7d19694412d
    }
}