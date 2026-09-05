using System.Security.Claims;

using ClinicManagement.Data;
using ClinicManagement.Enums;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClinicManagement.Controllers;

public class DoctorsController : Controller
{
    private readonly AppDbContext _context;

    public DoctorsController(AppDbContext context)
    {
        _context = context;
    }


    // =========================================================
    // Get Doctors By Specialty
    // Used by Reception when creating a Visit
    // =========================================================

    [Authorize(Roles = "Reception,Doctor")]
    [HttpGet]
    public IActionResult BySpecialty(int specialtyId)
    {
        var doctors = _context.Doctors
            .Where(d => d.SpecialtyId == specialtyId)
            .Select(d => new
            {
                id = d.Id,
                name = d.User.Name
            })
            .ToList();

        return Json(doctors);
    }


    // =========================================================
    // Doctor Dashboard
    // Shows Pending Visits for the logged-in doctor only
    // =========================================================

    [Authorize(Roles = "Doctor")]
    [HttpGet]
    public IActionResult Dashboard()
    {
        var userId = User.FindFirstValue(
            ClaimTypes.NameIdentifier
        );

        if (userId == null)
        {
            return Unauthorized();
        }

        var doctor = _context.Doctors
            .FirstOrDefault(d =>
                d.UserId == int.Parse(userId));

        if (doctor == null)
        {
            return NotFound(
                "Doctor profile not found."
            );
        }

        var visits = _context.Visits
            .Include(v => v.Patient)
            .Include(v => v.Specialty)
            .Where(v =>
                v.DoctorId == doctor.Id &&
                v.Status == VisitStatus.Pending|| v.Status == VisitStatus.InProgress)
            .OrderBy(v => v.CreatedAt)
            .ToList();

        return View(visits);
    }


    // =========================================================
    // Start Examination
    // Opens a Pending Visit for the logged-in doctor
    // Changes status: Pending → InProgress
    // =========================================================

    [Authorize(Roles = "Doctor")]
    [HttpGet]
    public IActionResult Complete(int id)
    {
        var userId = User.FindFirstValue(
            ClaimTypes.NameIdentifier
        );

        if (userId == null)
        {
            return Unauthorized();
        }

        var doctor = _context.Doctors
            .FirstOrDefault(d =>
                d.UserId == int.Parse(userId));

        if (doctor == null)
        {
            return NotFound(
                "Doctor profile not found."
            );
        }

        var visit = _context.Visits
            .Include(v => v.Patient)
            .Include(v => v.Specialty)
            .Include(v => v.Diagnosis)
            .Include(v=>v.Attachments)
            .FirstOrDefault(v =>
                v.Id == id &&
                v.DoctorId == doctor.Id);

        if (visit == null)
        {
            return NotFound(
                "Visit not found."
            );
        }

        if (visit.Status == VisitStatus.Completed)
        {
            return BadRequest(
                "This visit is already completed."
            );
        }

        visit.Status = VisitStatus.InProgress;

        visit.StartedAt = DateTime.UtcNow;

        _context.SaveChanges();

        return View(visit);
    }


    // =========================================================
    // Complete Examination
    // Saves Diagnosis and completes the Visit
    // Changes status: InProgress → Completed
    // =========================================================

    [Authorize(Roles = "Doctor")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Complete(
        int visitId,
        string diagnosisText,
        string? notes)
    {
        var userId = User.FindFirstValue(
            ClaimTypes.NameIdentifier
        );

        if (userId == null)
        {
            return Unauthorized();
        }

        var doctor = _context.Doctors
            .FirstOrDefault(d =>
                d.UserId == int.Parse(userId));

        if (doctor == null)
        {
            return NotFound(
                "Doctor profile not found."
            );
        }

        var visit = _context.Visits
            .Include(v => v.Patient)
            .Include(v => v.Specialty)
            .FirstOrDefault(v =>
                v.Id == visitId &&
                v.DoctorId == doctor.Id);

        if (visit == null)
        {
            return NotFound(
                "Visit not found."
            );
        }

        if (visit.Status == VisitStatus.Completed)
        {
            return BadRequest(
                "This visit is already completed."
            );
        }

        if (string.IsNullOrWhiteSpace(diagnosisText))
        {
            ModelState.AddModelError(
                "diagnosisText",
                "Diagnosis is required."
            );

            return View(visit);
        }

        var diagnosis = new Models.Diagnosis
        {
            VisitId = visitId,

            DiagnosisText = diagnosisText,

            Notes = notes,

            CreatedAt = DateTime.UtcNow
        };

        _context.Diagnoses.Add(diagnosis);

        visit.Status = VisitStatus.Completed;

        visit.CompletedAt = DateTime.UtcNow;

        _context.SaveChanges();

        return RedirectToAction(
            "Dashboard",
            "Doctors"
        );
    }
}
