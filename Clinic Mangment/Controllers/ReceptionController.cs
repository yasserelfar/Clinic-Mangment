using ClinicManagement.Data;
using ClinicManagement.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClinicManagement.Controllers;

[Authorize(Roles = "Reception")]
public class ReceptionController : Controller
{
    private readonly AppDbContext _context;

    public ReceptionController(AppDbContext context)
    {
        _context = context;
    }

    // =========================================================
    // Reception Dashboard
    // =========================================================

    [HttpGet]
    public IActionResult Dashboard()
    {
        var today = DateTime.UtcNow.Date;

        // Today's visits
        var totalToday = _context.Visits
            .Count(v => v.CreatedAt >= today);


        // Pending visits
        var pendingVisits = _context.Visits
            .Count(v => v.Status == VisitStatus.Pending);


        // Completed visits
        var completedVisits = _context.Visits
            .Count(v => v.Status == VisitStatus.Completed);


        // Recent visits
        var recentVisits = _context.Visits
            .Include(v => v.Patient)
            .Include(v => v.Doctor)
                .ThenInclude(d => d.User)
            .Include(v => v.Specialty)
            .Include(v => v.Diagnosis)
            .OrderByDescending(v => v.CreatedAt)
            .Take(10)
            .ToList();


        // Send statistics to View

        ViewBag.TotalToday = totalToday;

        ViewBag.PendingVisits = pendingVisits;

        ViewBag.CompletedVisits = completedVisits;


        // Send visits to View

        return View(recentVisits);
    }
}
