using ClinicManagement.Enums;

namespace ClinicManagement.Models;

public class Visit
{
    public int Id { get; set; }

    public int PatientId { get; set; }

    public int DoctorId { get; set; }

    public int SpecialtyId { get; set; }

    public VisitStatus Status { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? StartedAt { get; set; }

    public DateTime? CompletedAt { get; set; }

    public Patient Patient { get; set; } = null!;

    public Doctor Doctor { get; set; } = null!;

    public Specialty Specialty { get; set; } = null!;

    public Diagnosis? Diagnosis { get; set; }

    public ICollection<Prescription> Prescriptions { get; set; }
        = new List<Prescription>();
}