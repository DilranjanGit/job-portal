using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using JobPortal.API.Domain;
using Microsoft.EntityFrameworkCore;

namespace JobPortal.API.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

public DbSet<StudentProfile> Students => Set<StudentProfile>();
        public DbSet<CompanyProfile> Companies => Set<CompanyProfile>();
        public DbSet<Job> Jobs => Set<Job>();
        public DbSet<JobApplication> JobApplications => Set<JobApplication>();
        public DbSet<Interview> Interviews => Set<Interview>();
        public DbSet<Message> Messages => Set<Message>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Unique constraints
            modelBuilder.Entity<StudentProfile>()
                .HasIndex(s => s.Email)
                .IsUnique();
            modelBuilder.Entity<StudentProfile>()

                .HasIndex(s => s.PhoneNumber)
                .IsUnique();
            modelBuilder.Entity<CompanyProfile>()

                .HasIndex(c => c.Email)
                .IsUnique();
                
                //Salary precision
            modelBuilder.Entity<Job>()
                .Property(j => j.Salary)
                .HasColumnType("decimal(18, 2)");

                // Useful indexes

            modelBuilder.Entity<Job>()
                .HasIndex(j => new {j.IsActive, j.Location });
                modelBuilder.Entity<Message>()
                .HasIndex(m => new {m.ReceiverEmail, m.IsRead});

                //Enum conversions
            modelBuilder.Entity<JobApplication>()
                .Property(ja => ja.Status)
                .HasConversion<int>();
modelBuilder.Entity<Message>()
                .Property(m => m.Sender)
                .HasConversion<int>();
                modelBuilder.Entity<Interview>()
                .Property(i => i.Mode)
                .HasConversion<int>();
                modelBuilder.Entity<Interview>()
                .Property(i => i.Status)
                .HasConversion<int>();

                // Relationships
                modelBuilder.Entity<Job>()
                .HasOne(j => j.Company)
                .WithMany(c => c.Jobs)
                .HasForeignKey(j => j.CompanyProfileId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<JobApplication>()
                .HasOne(ja => ja.Job)
                .WithMany(j => j.Applications)
                .HasForeignKey(ja => ja.JobId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<JobApplication>()
                .HasOne(ja => ja.Student)
                .WithMany()
                .HasForeignKey(ja => ja.StudentProfileId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Interview>()
                .HasOne(i => i.Application)
                .WithMany()
                .HasForeignKey(i => i.JobApplicationId)
                .OnDelete(DeleteBehavior.Cascade);

            
        }
    }
}