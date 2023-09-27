namespace xingyi.cas.common
{
    using xingyi.common;
    using Microsoft.EntityFrameworkCore;

    public class CasDbContext : DbContext
    {
        // Constructor to set the context options
        public CasDbContext(DbContextOptions<CasDbContext> options) : base(options) { }

        // DbSet for ContentItem
        public DbSet<ContentItem> ContentItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ContentItem>()
                .HasKey(c => new {  c.Namespace, c.SHA });
        }

        public Task<int> saveAllChanges()
        {
            return SaveChangesAsync();
        }
    }

}
