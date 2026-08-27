using ClinicManagement.Data;
using ClinicManagement.Enums;
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
    [HttpGet]
    public IActionResult Dashboard(int doctorId)
    {
        var visits = _context.Visits
            .Include(v => v.Patient)
            .Include(v => v.Specialty)
            .Where(v => v.DoctorId == doctorId &&
                        v.Status == VisitStatus.Pending)
            .OrderBy(v => v.CreatedAt)
            .ToList();

        return View(visits);
    }
    [HttpGet]
    public IActionResult Complete(int id)
    {
        var visit = _context.Visits
            .Include(v => v.Patient)
            .FirstOrDefault(v => v.Id == id);

        if (visit == null)
        {
            return NotFound();
        }

        if (visit.Status != VisitStatus.Pending)
        {
            return BadRequest("This visit is not pending.");
        }

        visit.Status = VisitStatus.InProgress;
        visit.StartedAt = DateTime.UtcNow;

        _context.SaveChanges();

        return View(visit);
    }
}