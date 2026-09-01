using ClinicManagement.Data;
using ClinicManagement.Enums;
using ClinicManagement.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Clinic_Mangment.Controllers;

[Authorize]
public class VisitsController : Controller
{
    private readonly AppDbContext _context;

    public VisitsController(AppDbContext context)
    {
        _context = context;
    }


    // =====================================================
    // INDEX
    // =====================================================

    [HttpGet]
    public IActionResult Index()
    {
        var visits = _context.Visits
            .Include(v => v.Patient)
            .Include(v => v.Doctor)
                .ThenInclude(d => d.User)
            .Include(v => v.Specialty)
            .OrderByDescending(v => v.CreatedAt)
            .ToList();

        return View(visits);
    }


    // =====================================================
    // CREATE VISIT - GET
    // Reception
    // =====================================================

    [Authorize(Roles = "Reception")]
    [HttpGet]
    public IActionResult Create(int patientId)
    {
        var patient = _context.Patients
            .FirstOrDefault(p => p.Id == patientId);

        if (patient == null)
        {
            return NotFound("Patient not found.");
        }

        ViewBag.Specialties = _context.Specialties
            .OrderBy(s => s.Name)
            .ToList();

        return View(patient);
    }


    // =====================================================
    // CREATE VISIT - POST
    // Reception
    // =====================================================

    [Authorize(Roles = "Reception")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(
        int patientId,
        int specialtyId,
        int doctorId)
    {
        // Find patient

        var patient = _context.Patients
            .FirstOrDefault(p => p.Id == patientId);

        if (patient == null)
        {
            return NotFound("Patient not found.");
        }


        // Find specialty

        var specialty = _context.Specialties
            .FirstOrDefault(s => s.Id == specialtyId);

        if (specialty == null)
        {
            return NotFound("Specialty not found.");
        }


        // Find doctor

        var doctor = _context.Doctors
            .FirstOrDefault(d => d.Id == doctorId);

        if (doctor == null)
        {
            return NotFound("Doctor not found.");
        }


        // Make sure doctor belongs to specialty

        if (doctor.SpecialtyId != specialtyId)
        {
            return BadRequest(
                "Doctor does not belong to this specialty."
            );
        }


        // Create visit

        var visit = new Visit
        {
            PatientId = patientId,

            SpecialtyId = specialtyId,

            DoctorId = doctorId,

            Status = VisitStatus.Pending,

            CreatedAt = DateTime.UtcNow
        };


        _context.Visits.Add(visit);

        _context.SaveChanges();


        // Return to reception

        return RedirectToAction(
            "Dashboard",
            "Reception"
        );
    }


    // =====================================================
    // DETAILS
    // =====================================================

    [HttpGet]
    public IActionResult Details(int id)
    {
        var visit = _context.Visits
            .Include(v => v.Patient)
            .Include(v => v.Doctor)
                .ThenInclude(d => d.User)
            .Include(v => v.Specialty)
            .Include(v => v.Diagnosis)
            .Include(v => v.Prescriptions)
            .FirstOrDefault(v => v.Id == id);

        if (visit == null)
        {
            return NotFound();
        }

        return View(visit);
    }


    // =====================================================
    // COMPLETE VISIT
    // Doctor
    // =====================================================

    [Authorize(Roles = "Doctor")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Complete(
        int visitId,
        string diagnosisText,
        string? notes)
    {
        // Get current logged-in user

        var userId = User.FindFirstValue(
            ClaimTypes.NameIdentifier
        );

        if (userId == null)
        {
            return Unauthorized();
        }


        // Find doctor profile

        var doctor = _context.Doctors
            .FirstOrDefault(d =>
                d.UserId == int.Parse(userId));

        if (doctor == null)
        {
            return NotFound(
                "Doctor profile not found."
            );
        }


        // Find visit

        var visit = _context.Visits
            .FirstOrDefault(v => v.Id == visitId);

        if (visit == null)
        {
            return NotFound();
        }


        // Make sure this visit belongs to this doctor

        if (visit.DoctorId != doctor.Id)
        {
            return Forbid();
        }


        // Make sure visit is in progress

        if (visit.Status != VisitStatus.InProgress)
        {
            return BadRequest(
                "Visit is not in progress."
            );
        }


        // Validate diagnosis

        if (string.IsNullOrWhiteSpace(diagnosisText))
        {
            ModelState.AddModelError(
                "diagnosisText",
                "Diagnosis is required."
            );

            return View(visit);
        }


        // Create diagnosis

        var diagnosis = new Diagnosis
        {
            VisitId = visitId,

            DiagnosisText = diagnosisText,

            Notes = notes,

            CreatedAt = DateTime.UtcNow
        };


        _context.Diagnoses.Add(diagnosis);


        // Complete visit

        visit.Status = VisitStatus.Completed;

        visit.CompletedAt = DateTime.UtcNow;


        _context.SaveChanges();


        // Return to doctor dashboard

        return RedirectToAction(
            "Dashboard",
            "Doctors"
        );
    }
}