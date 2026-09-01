using ClinicManagement.Data;
using ClinicManagement.Enums;
using ClinicManagement.Models;
using ClinicManagement.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
        [HttpGet]
        public IActionResult Search(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
            {
                return View();
            }

            var patient = _context.Patients
                .FirstOrDefault(p => p.Phone == phone);

            return View(patient);
        }
       
    [HttpGet]
    public IActionResult History(
        int id,
        int? doctorId,
        int? specialtyId,
        VisitStatus? status,
        DateTime? fromDate,
        DateTime? toDate)
        {
            // ==========================================
            // Get Patient
            // ==========================================

            var patient = _context.Patients
                .FirstOrDefault(p => p.Id == id);

            if (patient == null)
            {
                return NotFound();
            }


            // ==========================================
            // Get Patient Visits
            // ==========================================

            var query = _context.Visits
                .Include(v => v.Doctor)
                    .ThenInclude(d => d.User)
                .Include(v => v.Specialty)
                .Include(v => v.Diagnosis)
                .Where(v => v.PatientId == id)
                .AsQueryable();


            // ==========================================
            // Doctor Filter
            // ==========================================

            if (doctorId.HasValue)
            {
                query = query.Where(v =>
                    v.DoctorId == doctorId.Value);
            }


            // ==========================================
            // Specialty Filter
            // ==========================================

            if (specialtyId.HasValue)
            {
                query = query.Where(v =>
                    v.SpecialtyId == specialtyId.Value);
            }


            // ==========================================
            // Status Filter
            // ==========================================

            if (status.HasValue)
            {
                query = query.Where(v =>
                    v.Status == status.Value);
            }


            // ==========================================
            // From Date Filter
            // ==========================================

            if (fromDate.HasValue)
            {
                query = query.Where(v =>
                    v.CreatedAt >= fromDate.Value);
            }


            // ==========================================
            // To Date Filter
            // ==========================================

            if (toDate.HasValue)
            {
                var endDate = toDate.Value.Date.AddDays(1);

                query = query.Where(v =>
                    v.CreatedAt < endDate);
            }


            // ==========================================
            // Execute Query
            // ==========================================

            var visits = query
                .OrderByDescending(v => v.CreatedAt)
                .ToList();


            // ==========================================
            // Create ViewModel
            // ==========================================

            var model = new PatientHistoryViewModel
            {
                Patient = patient,

                Visits = visits,

                DoctorId = doctorId,

                SpecialtyId = specialtyId,

                Status = status,

                FromDate = fromDate,

                ToDate = toDate,

                Doctors = _context.Doctors
                    .Include(d => d.User)
                    .OrderBy(d => d.User.Name)
                    .ToList(),

                Specialties = _context.Specialties
                    .OrderBy(s => s.Name)
                    .ToList()
            };


            return View(model);
        }


    }
}
