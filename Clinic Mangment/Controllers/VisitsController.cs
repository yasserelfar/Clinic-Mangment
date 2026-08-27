using ClinicManagement.Data;
using ClinicManagement.Enums;
using ClinicManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Clinic_Mangment.Controllers
{
    public class VisitsController : Controller
    {
        private readonly AppDbContext _context;

        public VisitsController(AppDbContext context)
        {
            _context = context;
        }
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

        [HttpGet]
        public IActionResult Create(int patientId)
        {
            var patient = _context.Patients.Find(patientId);

            if (patient == null)
            {
                return NotFound();
            }

            ViewBag.Specialties = _context.Specialties.ToList();

            return View(patient);
        }
        [HttpPost]
        public IActionResult Create(int patientId,int specialtyId,int doctorId)
        {
            var patient = _context.Patients.Find(patientId);

            if (patient == null)
            {
                return NotFound("Patient not found.");
            }

            var specialty = _context.Specialties.Find(specialtyId);

            if (specialty == null)
            {
                return NotFound("Specialty not found.");
            }

            var doctor = _context.Doctors.Find(doctorId);

            if (doctor == null)
            {
                return NotFound("Doctor not found.");
            }

            if (doctor.SpecialtyId != specialtyId)
            {
                return BadRequest("Doctor does not belong to this specialty.");
            }

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

            return RedirectToAction("Index");
        }
    }
}
