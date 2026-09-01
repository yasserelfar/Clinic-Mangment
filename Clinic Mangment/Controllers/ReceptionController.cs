using ClinicManagement.Data;
using ClinicManagement.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClinicManagement.Controllers;

[Authorize(Roles = "Reception")]
public class ReceptionController : Controller
{
    private readonly AppDbContext _context;

    public ReceptionController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult Dashboard()
    {
        var today = DateTime.UtcNow.Date;

        var totalToday = _context.Visits
            .Count(v => v.CreatedAt >= today);

        var pendingVisits = _context.Visits
            .Count(v => v.Status == VisitStatus.Pending);

        var completedVisits = _context.Visits
            .Count(v => v.Status == VisitStatus.Completed);

        ViewBag.TotalToday = totalToday;
        ViewBag.PendingVisits = pendingVisits;
        ViewBag.CompletedVisits = completedVisits;

        return View();
    }
}