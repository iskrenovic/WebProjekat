using Microsoft.EntityFrameworkCore;

namespace Models
{
    public class HotelManagingContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Hotel> Hotels { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<RoomBooking> RoomBookings { get; set; }
        public DbSet<Employed> Employes{ get; set; }

        public HotelManagingContext(DbContextOptions options) : base(options)
        {

        }
    }
}