using ClinicManagement.Data;
using ClinicManagement.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClinicManagement.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly AppDbContext _context;

    public AdminController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult Dashboard()
    {
        var totalPatients = _context.Patients.Count();

        var totalDoctors = _context.Doctors.Count();

        var totalVisits = _context.Visits.Count();

        var pendingVisits = _context.Visits
            .Count(v => v.Status == VisitStatus.Pending);

        ViewBag.TotalPatients = totalPatients;
        ViewBag.TotalDoctors = totalDoctors;
        ViewBag.TotalVisits = totalVisits;
        ViewBag.PendingVisits = pendingVisits;

        return View();
    }
}