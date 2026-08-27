using ClinicManagement.Data;
using ClinicManagement.Models;
using Microsoft.AspNetCore.Mvc;

namespace Clinic_Mangment.Controllers
{
    public class PatientsController : Controller
    {
        private readonly AppDbContext _context;

        public PatientsController(AppDbContext context)
        {
            _context = context;
        }


        [HttpGet]
        public IActionResult Index()
        {
            var patients = _context.Patients.ToList();

            return View(patients);
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Patient patient)
        {
            if (!ModelState.IsValid)
            {
                return View(patient);
            }

            _context.Patients.Add(patient);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

    }
}
