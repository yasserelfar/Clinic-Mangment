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
        public DbSet<VisitAttachment> VisitAttachments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            // =====================================================
            // User
            // =====================================================

            // Email must be unique
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();


            // =====================================================
            // Patient
            // =====================================================

            // Phone number must be unique
            modelBuilder.Entity<Patient>()
                .HasIndex(p => p.Phone)
                .IsUnique();


            // =====================================================
            // User ↔ Doctor (One-to-One)
            // =====================================================

            modelBuilder.Entity<Doctor>()
                .HasOne(d => d.User)
                .WithOne(u => u.Doctor)
                .HasForeignKey<Doctor>(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // A User can have only one Doctor profile
            modelBuilder.Entity<Doctor>()
                .HasIndex(d => d.UserId)
                .IsUnique();


            // =====================================================
            // Doctor ↔ Specialty
            // =====================================================

            modelBuilder.Entity<Doctor>()
                .HasOne(d => d.Specialty)
                .WithMany(s => s.Doctors)
                .HasForeignKey(d => d.SpecialtyId)
                .OnDelete(DeleteBehavior.Restrict);


            // =====================================================
            // Visit ↔ Patient
            // =====================================================

            modelBuilder.Entity<Visit>()
                .HasOne(v => v.Patient)
                .WithMany(p => p.Visits)
                .HasForeignKey(v => v.PatientId)
                .OnDelete(DeleteBehavior.Restrict);


            // =====================================================
            // Visit ↔ Doctor
            // =====================================================

            modelBuilder.Entity<Visit>()
                .HasOne(v => v.Doctor)
                .WithMany(d => d.Visits)
                .HasForeignKey(v => v.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);


            // =====================================================
            // Visit ↔ Specialty
            // =====================================================

            modelBuilder.Entity<Visit>()
                .HasOne(v => v.Specialty)
                .WithMany(s => s.Visits)
                .HasForeignKey(v => v.SpecialtyId)
                .OnDelete(DeleteBehavior.Restrict);


            // =====================================================
            // Visit ↔ Diagnosis (One-to-One)
            // =====================================================

            modelBuilder.Entity<Diagnosis>()
                .HasOne(d => d.Visit)
                .WithOne(v => v.Diagnosis)
                .HasForeignKey<Diagnosis>(d => d.VisitId)
                .OnDelete(DeleteBehavior.Cascade);


            // =====================================================
            // Visit ↔ Prescription (One-to-Many)
            // =====================================================

            modelBuilder.Entity<Prescription>()
                .HasOne(p => p.Visit)
                .WithMany(v => v.Prescriptions)
                .HasForeignKey(p => p.VisitId)
                .OnDelete(DeleteBehavior.Cascade);
            // =====================================================
            // Visit ↔ VisitAttachment (One-to-Many)
            // =====================================================
            modelBuilder.Entity<VisitAttachment>()
            .HasOne(a => a.Visit)
            .WithMany(v => v.Attachments)
            .HasForeignKey(a => a.VisitId)
            .OnDelete(DeleteBehavior.Cascade);


            modelBuilder.Entity<VisitAttachment>()
            .HasOne(a => a.UploadedByUser)
            .WithMany()
            .HasForeignKey(a => a.UploadedByUserId)
            .OnDelete(DeleteBehavior.Restrict);
        }
    }
}