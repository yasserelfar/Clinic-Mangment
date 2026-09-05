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
    public async Task<IActionResult> Index()
    {
        var visits = await _context.Visits
            .Include(v => v.Patient)
            .Include(v => v.Doctor)
                .ThenInclude(d => d.User)
            .Include(v => v.Specialty)
            .OrderByDescending(v => v.CreatedAt)
            .ToListAsync();

        return View(visits);
    }


    // =====================================================
    // CREATE VISIT - GET
    // Reception
    // =====================================================

    [Authorize(Roles = "Reception")]
    [HttpGet]
    public async Task<IActionResult> Create(int patientId)
    {
        var patient = await _context.Patients
            .FirstOrDefaultAsync(p => p.Id == patientId);

        if (patient == null)
        {
            return NotFound("Patient not found.");
        }

        ViewBag.Specialties = await _context.Specialties
            .OrderBy(s => s.Name)
            .ToListAsync();

        return View(patient);
    }


    // =====================================================
    // CREATE VISIT - POST
    // Reception
    // =====================================================

    [Authorize(Roles = "Reception")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(int patientId,int specialtyId,int doctorId)
    {
        // Find patient

        var patient = await _context.Patients
            .FirstOrDefaultAsync(p => p.Id == patientId);

        if (patient == null)
        {
            return NotFound("Patient not found.");
        }


        // Find specialty

        var specialty = await _context.Specialties
            .FirstOrDefaultAsync(s => s.Id == specialtyId);

        if (specialty == null)
        {
            return NotFound("Specialty not found.");
        }


        // Find doctor

        var doctor = await _context.Doctors
            .FirstOrDefaultAsync(d => d.Id == doctorId);

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

        await _context.SaveChangesAsync();


        TempData["ToastMessage"] =
            "Visit created successfully.";

        TempData["ToastType"] =
            "success";


        return RedirectToAction(
            "Dashboard",
            "Reception"
        );
    }


    // =====================================================
    // DETAILS
    // =====================================================

    [Authorize(Roles = "Reception,Doctor")]
    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var visit = await _context.Visits

            .Include(v => v.Patient)

            .Include(v => v.Doctor)
                .ThenInclude(d => d.User)

            .Include(v => v.Specialty)

            .Include(v => v.Diagnosis)

            .Include(v => v.Attachments)

            .FirstOrDefaultAsync(v => v.Id == id);


        if (visit == null)
        {
            return NotFound();
        }


        return View(visit);
    }


    // =====================================================
    // UPLOAD ATTACHMENTS
    // Reception / Doctor
    // =====================================================

    [Authorize(Roles = "Reception,Doctor")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UploadAttachment(
        int visitId,
        IFormFileCollection files)
    {
        var visit = await _context.Visits
            .FirstOrDefaultAsync(v => v.Id == visitId);

        if (visit == null)
        {
            return NotFound(new
            {
                success = false,
                message = "Visit not found."
            });
        }

        if (files == null || files.Count == 0)
        {
            return BadRequest(new
            {
                success = false,
                message = "Please select at least one file."
            });
        }

        var userIdClaim = User.FindFirstValue(
            ClaimTypes.NameIdentifier
        );

        if (!int.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized(new
            {
                success = false,
                message = "Unauthorized."
            });
        }

        const long maxFileSize = 10 * 1024 * 1024;

        var allowedExtensions = new HashSet<string>(
            StringComparer.OrdinalIgnoreCase)
    {
        ".jpg",
        ".jpeg",
        ".png",
        ".pdf"
    };

        var uploadFolder = Path.Combine(
            Directory.GetCurrentDirectory(),
            "wwwroot",
            "uploads",
            "visits",
            visitId.ToString()
        );

        Directory.CreateDirectory(uploadFolder);

        var uploadedAttachments = new List<VisitAttachment>();

        foreach (var file in files)
        {
            if (file == null || file.Length == 0)
                continue;

            if (file.Length > maxFileSize)
            {
                return BadRequest(new
                {
                    success = false,
                    message =
                        $"File '{file.FileName}' exceeds the 10 MB limit."
                });
            }

            var extension = Path.GetExtension(file.FileName);

            if (string.IsNullOrWhiteSpace(extension) ||
                !allowedExtensions.Contains(extension))
            {
                return BadRequest(new
                {
                    success = false,
                    message =
                        $"File '{file.FileName}' is not allowed."
                });
            }

            var storedFileName =
                $"{Guid.NewGuid():N}{extension.ToLowerInvariant()}";

            var physicalFilePath = Path.Combine(
                uploadFolder,
                storedFileName
            );

            await using (var stream = new FileStream(
                physicalFilePath,
                FileMode.CreateNew,
                FileAccess.Write,
                FileShare.None))
            {
                await file.CopyToAsync(stream);
            }

            var attachment = new VisitAttachment
            {
                VisitId = visitId,

                FileName = Path.GetFileName(file.FileName),

                FilePath =
                    $"/uploads/visits/{visitId}/{storedFileName}",

                ContentType =
                    string.IsNullOrWhiteSpace(file.ContentType)
                        ? "application/octet-stream"
                        : file.ContentType,

                FileSize = file.Length,

                UploadedAt = DateTime.UtcNow,

                UploadedByUserId = userId
            };

            _context.VisitAttachments.Add(attachment);

            uploadedAttachments.Add(attachment);
        }

        if (uploadedAttachments.Count == 0)
        {
            return BadRequest(new
            {
                success = false,
                message = "No valid files were uploaded."
            });
        }

        await _context.SaveChangesAsync();

        return Json(new
        {
            success = true,

            files = uploadedAttachments.Select(a => new
            {
                id = a.Id,
                fileName = a.FileName,
                filePath = a.FilePath,
                contentType = a.ContentType,
                fileSize = a.FileSize,

                uploadedAt = a.UploadedAt.ToLocalTime().ToString("dd/MM/yyyy HH:mm")
            })
        });
    }



    [Authorize(Roles = "Reception,Doctor")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteAttachment(int id)
    {
        var attachment = await _context.VisitAttachments
            .FirstOrDefaultAsync(a => a.Id == id);

        if (attachment == null)
        {
            return NotFound(new
            {
                success = false,
                message = "Attachment not found."
            });
        }

        // Delete physical file
        var physicalPath = Path.Combine(
            Directory.GetCurrentDirectory(),
            "wwwroot",
            attachment.FilePath.TrimStart('/')
                .Replace('/', Path.DirectorySeparatorChar)
        );

        if (System.IO.File.Exists(physicalPath))
        {
            System.IO.File.Delete(physicalPath);
        }

        // Delete database record
        _context.VisitAttachments.Remove(attachment);

        await _context.SaveChangesAsync();

        return Json(new
        {
            success = true,
            id = attachment.Id
        });
    }
    // =====================================================
    // COMPLETE VISIT
    // Doctor
    // =====================================================

    [Authorize(Roles = "Doctor")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Complete(int visitId,string diagnosisText,string? notes)
    {
        // Get current logged-in user

        var userId =
            User.FindFirstValue(
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
            .FirstOrDefault(v =>
                v.Id == visitId);


        if (visit == null)
        {
            return NotFound();
        }


        // Make sure visit belongs to doctor

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

        if (string.IsNullOrWhiteSpace(
            diagnosisText))
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
            VisitId =
                visitId,

            DiagnosisText =
                diagnosisText,

            Notes =
                notes,

            CreatedAt =
                DateTime.UtcNow
        };


        _context.Diagnoses.Add(
            diagnosis
        );


        // Complete visit

        visit.Status =
            VisitStatus.Completed;

        visit.CompletedAt =
            DateTime.UtcNow;


        _context.SaveChanges();


        // Return to doctor dashboard

        return RedirectToAction(
            "Dashboard",
            "Doctors"
        );
    }
}