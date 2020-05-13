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

       protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
       {
           //TODO Clear me before commit
           optionsBuilder.UseSqlServer("Server=127.0.0.1;Database=vvsdhDB;User Id=sa;Password=reallyStrongPwd123");
           //https://database.guide/how-to-install-sql-server-on-a-mac/
           optionsBuilder.EnableDetailedErrors();
       }
    }
    
}
