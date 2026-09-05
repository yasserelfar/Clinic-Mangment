using System.ComponentModel.DataAnnotations;

namespace ClinicManagement.Models;

public class VisitAttachment
{
    public int Id { get; set; }

    public int VisitId { get; set; }

    [Required]
    [MaxLength(255)]
    public string FileName { get; set; } = string.Empty;

    [Required]
    public string FilePath { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string ContentType { get; set; } = string.Empty;

    public long FileSize { get; set; }

    public DateTime UploadedAt { get; set; }

    public int UploadedByUserId { get; set; }

    public Visit Visit { get; set; } = null!;

    public User UploadedByUser { get; set; } = null!;
}