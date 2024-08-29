using BusApplication.Models.Entity;
using Microsoft.EntityFrameworkCore;

namespace BusApplication.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        { 
        }

        public DbSet<UserDetailsEntity> User_Details { get; set; }

        public DbSet<BusDetailsEntity> Bus_Details {  get; set; } 

        public DbSet<BookingDetailsEntity> Booking_Details { get; set; }

        public DbSet<ContactUsDetailsEntity> ContactUs_Details { get; set; }
         
    }
}
