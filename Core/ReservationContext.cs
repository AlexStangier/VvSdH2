using Microsoft.EntityFrameworkCore;

namespace Core
{
    public class ReservationContext : DbContext
    {
       public DbSet<Attribute> Attributes { get; set; }
       public DbSet<Reservation> Reservations { get; set; }
       public DbSet<Right> Rights { get; set; }
       public DbSet<Room> Rooms { get; set; }
       public DbSet<User> Users { get; set; }
       public DbSet<Holiday> Holydays { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
       {
           optionsBuilder.UseSqlServer("Server=95.111.228.241;Database=vvsdhDB;User Id=sa;Password=vvsdhAdminPW123");
           optionsBuilder.EnableDetailedErrors();
       }
    }
    
}
