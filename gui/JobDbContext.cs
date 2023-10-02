using Microsoft.EntityFrameworkCore;

namespace xingyi.job
{
    public class JobDbContext : DbContext
    {
        public JobDbContext(DbContextOptions<JobDbContext> options) : base(options)
        {
        }

        public DbSet<Job> Jobs { get; set; }
        public DbSet<JobSectionTemplate> JobSectionTemplates { get; set; }
        public DbSet<SectionTemplate> SectionTemplates { get; set; }
        public DbSet<SectionTemplateQuestion> SectionTemplateQuestions { get; set; }
        public DbSet<Application> Applications { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Section> Sections { get; set; }
        public DbSet<Answer> Answers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<JobSectionTemplate>()
                .HasKey(jst => new { jst.JobId, jst.SectionTemplateId });

            modelBuilder.Entity<JobSectionTemplate>()
                .HasOne(jst => jst.Job)
                .WithMany(j => j.JobSectionTemplates)
                .HasForeignKey(jst => jst.JobId);

            modelBuilder.Entity<JobSectionTemplate>()
                .HasOne(jst => jst.SectionTemplate)
                .WithMany(st => st.JobSectionTemplates)
                .HasForeignKey(jst => jst.SectionTemplateId);

            modelBuilder.Entity<SectionTemplateQuestion>()
                .HasKey(stq => new { stq.SectionTemplateId, stq.QuestionId });

            modelBuilder.Entity<SectionTemplateQuestion>()
                .HasOne(stq => stq.SectionTemplate)
                .WithMany(st => st.SectionTemplateQuestions)
                .HasForeignKey(stq => stq.SectionTemplateId);

            modelBuilder.Entity<SectionTemplateQuestion>()
                .HasOne(stq => stq.Question)
                .WithMany(q => q.SectionTemplateQuestions)
                .HasForeignKey(stq => stq.QuestionId);

            modelBuilder.Entity<Job>()
                .HasMany(j => j.Applications)
                .WithOne(a => a.Job)
                .HasForeignKey(a => a.JobId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Application>()
           .HasMany(a => a.Sections)
           .WithOne()  // We leave it empty since there's no navigation property back to Application from Section
           .HasForeignKey(s => s.SectionTemplateId)
           .OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<Section>()
          .HasMany(s => s.Answers)
          .WithOne()  // No direct navigation back to Section from Answer
          .HasForeignKey(a => a.SectionId)
          .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Answer>()
                .HasOne(a => a.Question)
                .WithMany()  // Since there's no collection navigation property in Question pointing to Answer
                .HasForeignKey(a => a.QuestionId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
