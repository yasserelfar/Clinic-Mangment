using ClinicManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace ClinicManagement.Data
{

    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; } 
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Specialty> Specialties { get; set; }
        public DbSet<Visit> Visits { get; set; }
        public DbSet<Diagnosis> Diagnoses { get; set; }
        public DbSet<Prescription> Prescriptions { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            // Patient
            modelBuilder.Entity<Patient>()
                .HasIndex(p => p.Phone)
                .IsUnique();

            modelBuilder.Entity<Doctor>()
                .HasOne(d => d.Specialty)
                .WithMany(s => s.Doctors)
                .HasForeignKey(d => d.SpecialtyId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Visit>()
                .HasOne(v => v.Patient)
                .WithMany(p => p.Visits)
                .HasForeignKey(v => v.PatientId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Visit>()
                .HasOne(v => v.Doctor)
                .WithMany(d => d.Visits)
                .HasForeignKey(v => v.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Visit>()
                .HasOne(v => v.Specialty)
                .WithMany(s => s.Visits)
                .HasForeignKey(v => v.SpecialtyId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Diagnosis>()
                .HasOne(d => d.Visit)
                .WithOne(v => v.Diagnosis)
                .HasForeignKey<Diagnosis>(d => d.VisitId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Prescription>()
                .HasOne(p => p.Visit)
                .WithMany(v => v.Prescriptions)
                .HasForeignKey(p => p.VisitId)
                .OnDelete(DeleteBehavior.Cascade);
        }

    }
}