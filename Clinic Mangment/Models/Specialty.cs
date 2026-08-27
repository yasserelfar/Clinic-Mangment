using System.ComponentModel.DataAnnotations;

namespace ClinicManagement.Models;

public class Specialty
{
    public int Id { get; set; }
    [Required]
    public string Name { get; set; } = string.Empty;

    public ICollection<Doctor> Doctors { get; set; } = new List<Doctor>();

    public ICollection<Visit> Visits { get; set; } = new List<Visit>();
}