using ClinicManagement.Enums;
using ClinicManagement.Models;

namespace ClinicManagement.ViewModels;

public class PatientHistoryViewModel
{
    // Patient information
    public Patient Patient { get; set; } = null!;


    // Patient visits
    public List<Visit> Visits { get; set; } = new();


    // ==============================
    // Filters
    // ==============================

    public int? DoctorId { get; set; }

    public int? SpecialtyId { get; set; }

    public VisitStatus? Status { get; set; }

    public DateTime? FromDate { get; set; }

    public DateTime? ToDate { get; set; }


    // ==============================
    // Filter Options
    // ==============================

    public List<Doctor> Doctors { get; set; } = new();

    public List<Specialty> Specialties { get; set; } = new();
}
