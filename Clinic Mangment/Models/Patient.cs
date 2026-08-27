using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClinicManagement.Models;

public class Patient
{
    public int Id { get; set; }
    [Required]
    [MaxLength(100)]

    public string Name { get; set; } = string.Empty;
    [Required]
    [MaxLength(20)]

    public string Phone { get; set; } = string.Empty;
    [Column(TypeName = "date")]
    public DateTime DateOfBirth { get; set; }

    public string Gender { get; set; } = string.Empty;

    public string? Address { get; set; }

    public ICollection<Visit> Visits { get; set; } = new List<Visit>();
}