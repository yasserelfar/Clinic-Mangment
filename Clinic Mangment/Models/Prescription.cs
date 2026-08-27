namespace ClinicManagement.Models;

public class Prescription
{
    public int Id { get; set; }

    public int VisitId { get; set; }

    public string Medication { get; set; } = string.Empty;

    public string? Dosage { get; set; }

    public string? Instructions { get; set; }

    public Visit Visit { get; set; } = null!;
}