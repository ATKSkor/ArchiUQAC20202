using System.Linq;
using Microsoft.EntityFrameworkCore;
using StableAPI.Models;

namespace StableAPI.Data
{
    public class StableContext : DbContext
    {
        public StableContext(DbContextOptions<StableContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }

            modelBuilder.Entity<Membership>().HasKey(m => new
            {
                m.StableID, m.PersonID
            });

            modelBuilder.Entity<Staff>().HasKey(s => new
            {
                s.StableID, s.PersonID
            });

            modelBuilder.Entity<StockEntry>().HasKey(se => new
            {
                se.StableID, se.ItemID
            });
        }

        public DbSet<Bill> Bills { get; set; }
        public DbSet<Box> Boxes { get; set; }
        public DbSet<Calendar> Calendars { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<EventType> EventTypes { get; set; }
        public DbSet<Horse> Horses { get; set; }
        public DbSet<License> Licenses { get; set; }
        public DbSet<MedicEntry> MedicEntries { get; set; }
        public DbSet<Membership> Memberships { get; set; }
        public DbSet<Person> Persons { get; set; }
        public DbSet<Registration> Registrations { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Secret> Secrets { get; set; }
        public DbSet<Stable> Stables { get; set; }
        public DbSet<Staff> Staff { get; set; }
        public DbSet<StockEntry> StockEntries { get; set; }
        public DbSet<StockItem> StockItems { get; set; }
        public DbSet<User> Users { get; set; }
    }
}