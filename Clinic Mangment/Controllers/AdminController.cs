using ClinicManagement.Data;
using ClinicManagement.Enums;
using ClinicManagement.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Clinic_Mangment.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly AppDbContext _context;
    private readonly PasswordHasher<User> _passwordHasher;

    public AdminController(AppDbContext context)
    {
        _context = context;
        _passwordHasher = new PasswordHasher<User>();
    }


    // =========================================================
    // Dashboard
    // =========================================================

    [HttpGet]
    public IActionResult Dashboard()
    {
        var totalPatients = _context.Patients.Count();

        var totalDoctors = _context.Doctors.Count();

        var totalSpecialties = _context.Specialties.Count();

        var totalVisits = _context.Visits.Count();

        var pendingVisits = _context.Visits
            .Count(v => v.Status == VisitStatus.Pending);

        var completedVisits = _context.Visits
            .Count(v => v.Status == VisitStatus.Completed);

        ViewBag.TotalPatients = totalPatients;
        ViewBag.TotalDoctors = totalDoctors;
        ViewBag.TotalSpecialties = totalSpecialties;
        ViewBag.TotalVisits = totalVisits;
        ViewBag.PendingVisits = pendingVisits;
        ViewBag.CompletedVisits = completedVisits;

        return View();
    }


    // =========================================================
    // Create Staff
    // =========================================================

    [HttpGet]
    public IActionResult CreateStaff()
    {
        ViewBag.Specialties = _context.Specialties
            .OrderBy(s => s.Name)
            .ToList();

        return View();
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult CreateStaff(
        string name,
        string email,
        string password,
        string role,
        int? specialtyId)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            ModelState.AddModelError(
                "name",
                "Name is required."
            );
        }

        if (string.IsNullOrWhiteSpace(email))
        {
            ModelState.AddModelError(
                "email",
                "Email is required."
            );
        }

        if (string.IsNullOrWhiteSpace(password))
        {
            ModelState.AddModelError(
                "password",
                "Password is required."
            );
        }

        if (role != "Doctor" && role != "Reception")
        {
            ModelState.AddModelError(
                "role",
                "Invalid role."
            );
        }

        if (role == "Doctor" && specialtyId == null)
        {
            ModelState.AddModelError(
                "specialtyId",
                "Please select a specialty."
            );
        }

        var emailExists = _context.Users
            .Any(u => u.Email == email);

        if (emailExists)
        {
            ModelState.AddModelError(
                "email",
                "This email is already registered."
            );
        }

        if (!ModelState.IsValid)
        {
            ViewBag.Specialties = _context.Specialties
                .OrderBy(s => s.Name)
                .ToList();

            return View();
        }

        if (role == "Doctor")
        {
            var specialtyExists = _context.Specialties
                .Any(s => s.Id == specialtyId);

            if (!specialtyExists)
            {
                ModelState.AddModelError(
                    "specialtyId",
                    "Selected specialty does not exist."
                );

                ViewBag.Specialties = _context.Specialties
                    .OrderBy(s => s.Name)
                    .ToList();

                return View();
            }
        }

        var user = new User
        {
            Name = name.Trim(),
            Email = email.Trim(),
            Role = role,
            IsActive = true
        };

        user.PasswordHash =
            _passwordHasher.HashPassword(
                user,
                password
            );

        _context.Users.Add(user);

        _context.SaveChanges();

        if (role == "Doctor")
        {
            var doctor = new Doctor
            {
                UserId = user.Id,
                SpecialtyId = specialtyId!.Value
            };

            _context.Doctors.Add(doctor);

            _context.SaveChanges();
        }

        return RedirectToAction("Dashboard");
    }


    // =========================================================
    // Doctors
    // =========================================================

    [HttpGet]
    public IActionResult Doctors()
    {
        var doctors = _context.Doctors
            .Include(d => d.User)
            .Include(d => d.Specialty)
            .OrderBy(d => d.User.Name)
            .ToList();

        return View(doctors);
    }


    // =========================================================
    // Deactivate Doctor
    // =========================================================

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult DeactivateDoctor(int id)
    {
        var doctor = _context.Doctors
            .Include(d => d.User)
            .FirstOrDefault(d => d.Id == id);

        if (doctor == null)
        {
            return NotFound();
        }

        doctor.User.IsActive = false;

        _context.SaveChanges();

        return RedirectToAction("Doctors");
    }


    // =========================================================
    // Activate Doctor
    // =========================================================

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult ActivateDoctor(int id)
    {
        var doctor = _context.Doctors
            .Include(d => d.User)
            .FirstOrDefault(d => d.Id == id);

        if (doctor == null)
        {
            return NotFound();
        }

        doctor.User.IsActive = true;

        _context.SaveChanges();

        return RedirectToAction("Doctors");
    }


    // =========================================================
    // Delete Doctor
    // =========================================================

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteDoctor(int id)
    {
        var doctor = _context.Doctors
            .Include(d => d.User)
            .FirstOrDefault(d => d.Id == id);

        if (doctor == null)
        {
            return NotFound();
        }

        var hasVisits = _context.Visits
            .Any(v => v.DoctorId == doctor.Id);

        if (hasVisits)
        {
            TempData["Error"] =
                "This doctor cannot be deleted because they have visits.";

            return RedirectToAction("Doctors");
        }

        _context.Doctors.Remove(doctor);
        _context.Users.Remove(doctor.User);

        _context.SaveChanges();

        return RedirectToAction("Doctors");
    }


    // =========================================================
    // Specialties - List
    // =========================================================

    [HttpGet]
    public IActionResult Specialties()
    {
        var specialties = _context.Specialties
            .Include(s => s.Doctors)
            .OrderBy(s => s.Name)
            .ToList();

        return View(specialties);
    }


    // =========================================================
    // Specialties - Create
    // =========================================================

    [HttpGet]
    public IActionResult CreateSpecialty()
    {
        return View();
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult CreateSpecialty(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            ModelState.AddModelError(
                "name",
                "Specialty name is required."
            );

            return View();
        }

        name = name.Trim();

        var exists = _context.Specialties
            .Any(s => s.Name.ToLower() == name.ToLower());

        if (exists)
        {
            ModelState.AddModelError(
                "name",
                "This specialty already exists."
            );

            return View();
        }

        var specialty = new Specialty
        {
            Name = name
        };

        _context.Specialties.Add(specialty);

        _context.SaveChanges();

        return RedirectToAction("Specialties");
    }


    // =========================================================
    // Specialties - Edit
    // =========================================================

    [HttpGet]
    public IActionResult EditSpecialty(int id)
    {
        var specialty = _context.Specialties
            .FirstOrDefault(s => s.Id == id);

        if (specialty == null)
        {
            return NotFound();
        }

        return View(specialty);
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult EditSpecialty(
        int id,
        string name)
    {
        var specialty = _context.Specialties
            .FirstOrDefault(s => s.Id == id);

        if (specialty == null)
        {
            return NotFound();
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            ModelState.AddModelError(
                "name",
                "Specialty name is required."
            );

            return View(specialty);
        }

        name = name.Trim();

        var exists = _context.Specialties
            .Any(s =>
                s.Id != id &&
                s.Name.ToLower() == name.ToLower());

        if (exists)
        {
            ModelState.AddModelError(
                "name",
                "This specialty already exists."
            );

            return View(specialty);
        }

        specialty.Name = name;

        _context.SaveChanges();

        return RedirectToAction("Specialties");
    }


    // =========================================================
    // Specialties - Delete
    // =========================================================

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteSpecialty(int id)
    {
        var specialty = _context.Specialties
            .FirstOrDefault(s => s.Id == id);

        if (specialty == null)
        {
            return NotFound();
        }

        var hasDoctors = _context.Doctors
            .Any(d => d.SpecialtyId == id);

        var hasVisits = _context.Visits
            .Any(v => v.SpecialtyId == id);

        if (hasDoctors || hasVisits)
        {
            TempData["Error"] =
                "This specialty cannot be deleted because it is being used.";

            return RedirectToAction("Specialties");
        }

        _context.Specialties.Remove(specialty);

        _context.SaveChanges();

        return RedirectToAction("Specialties");
    }


    // =========================================================
    // Users
    // =========================================================

    [HttpGet]
    public IActionResult Users()
    {
        var users = _context.Users
            .OrderBy(u => u.Name)
            .ToList();

        return View(users);
    }
    
// =========================================================
// Deactivate User
// =========================================================

[HttpPost]
[ValidateAntiForgeryToken]
public IActionResult DeactivateUser(int id)
    {
        var user = _context.Users
            .FirstOrDefault(u => u.Id == id);

        if (user == null)
        {
            return NotFound();
        }

        // Prevent admin from deactivating himself
        var currentUserId = User.FindFirstValue(
            ClaimTypes.NameIdentifier
        );

        if (currentUserId == user.Id.ToString())
        {
            TempData["Error"] =
                "You cannot deactivate your own account.";

            return RedirectToAction("Users");
        }

        user.IsActive = false;

        _context.SaveChanges();

        return RedirectToAction("Users");
    }


    // =========================================================
    // Activate User
    // =========================================================

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult ActivateUser(int id)
    {
        var user = _context.Users
            .FirstOrDefault(u => u.Id == id);

        if (user == null)
        {
            return NotFound();
        }

        user.IsActive = true;

        _context.SaveChanges();

        return RedirectToAction("Users");
    }

}
