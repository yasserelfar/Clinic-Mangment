namespace ClinicManagement.Models;

public class Doctor
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int SpecialtyId { get; set; }

    public User User { get; set; } = null!;

    public Specialty Specialty { get; set; } = null!;

    public ICollection<Visit> Visits { get; set; } = new List<Visit>();
}