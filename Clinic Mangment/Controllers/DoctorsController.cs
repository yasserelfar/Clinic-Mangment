using ClinicManagement.Data;
using Microsoft.AspNetCore.Mvc;

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
}