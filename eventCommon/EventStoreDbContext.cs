using Microsoft.EntityFrameworkCore;

namespace xingyi.events
{
    public class EventStoreDbContext : DbContext
    {
        public EventStoreDbContext(DbContextOptions<EventStoreDbContext> options)
            : base(options)
        {
        }

        public DbSet<StoredEvent> StoredEvents { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Define the composite key
            modelBuilder.Entity<StoredEvent>()
                .HasKey(se => new { se.Namespace, se.Name });
        }
    }
}