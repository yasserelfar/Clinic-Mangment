using System.ComponentModel.DataAnnotations;

namespace ClinicManagement.Models;

public class Diagnosis
{
    public int Id { get; set; }

    public int VisitId { get; set; }
    [Required]
    public string DiagnosisText { get; set; } = string.Empty;

    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; }

    public Visit Visit { get; set; } = null!;
}