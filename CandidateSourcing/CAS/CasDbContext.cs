namespace CandidateSourcing.CAS
{
    using CandidateSourcing.common;
    using Microsoft.EntityFrameworkCore;

    public class CasDbContext : DbContext
    {
        // Constructor to set the context options
        public CasDbContext(DbContextOptions<CasDbContext> options) : base(options) { }

        // DbSet for ContentItem
        public DbSet<ContentItem> ContentItems { get; set; }
    }

}
