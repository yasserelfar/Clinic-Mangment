//using ClinicManagement.Models;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.EntityFrameworkCore;

//namespace ClinicManagement.Data;

//public static class DbSeeder
//{
//    public static void Seed(AppDbContext context)
//    {
//        // Make sure database is created and migrations are applied
//        context.Database.Migrate();

//        // If users already exist, don't create them again
//        if (context.Users.Any())
//        {
//            return;
//        }

//        var passwordHasher = new PasswordHasher<User>();

//        // ==========================================
//        // Specialties
//        // ==========================================

//        var cardiology = new Specialty
//        {
//            Name = "Cardiology"
//        };

//        var dermatology = new Specialty
//        {
//            Name = "Dermatology"
//        };

//        context.Specialties.AddRange(
//            cardiology,
//            dermatology
//        );

//        context.SaveChanges();


//        // ==========================================
//        // Admin
//        // ==========================================

//        var admin = new User
//        {
//            Name = "System Admin",
//            Email = "admin@clinic.com",
//            Role = "Admin"
//        };

//        admin.PasswordHash =
//            passwordHasher.HashPassword(
//                admin,
//                "Admin123!"
//            );


//        // ==========================================
//        // Doctor User
//        // ==========================================

//        var doctorUser = new User
//        {
//            Name = "Dr. Ahmed",
//            Email = "doctor@clinic.com",
//            Role = "Doctor"
//        };

//        doctorUser.PasswordHash =
//            passwordHasher.HashPassword(
//                doctorUser,
//                "Doctor123!"
//            );


//        // ==========================================
//        // Reception User
//        // ==========================================

//        var reception = new User
//        {
//            Name = "Reception User",
//            Email = "reception@clinic.com",
//            Role = "Reception"
//        };

//        reception.PasswordHash =
//            passwordHasher.HashPassword(
//                reception,
//                "Reception123!"
//            );


//        context.Users.AddRange(
//            admin,
//            doctorUser,
//            reception
//        );

//        context.SaveChanges();


//        // ==========================================
//        // Doctor Profile
//        // ==========================================

//        var doctor = new Doctor
//        {
//            UserId = doctorUser.Id,
//            SpecialtyId = cardiology.Id
//        };

//        context.Doctors.Add(doctor);

//        context.SaveChanges();
//    }
//}