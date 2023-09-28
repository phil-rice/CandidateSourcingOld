using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using xingyi.relationships;

namespace xingyi.relationships
{
    public class RelationshipDbContext : DbContext
    {
        public RelationshipDbContext(DbContextOptions<RelationshipDbContext> options) : base(options) { }

        public DbSet<Relationship> Relationships { get; set; }
    }

}
