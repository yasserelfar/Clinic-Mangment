using System.ComponentModel.DataAnnotations;

namespace ClinicManagement.Models;

public class User
{
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;


    [Required]
    [MaxLength(150)]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;


    [Required]
    public string PasswordHash { get; set; } = string.Empty;


    [Required]
    public string Role { get; set; } = string.Empty;


    public Doctor? Doctor { get; set; }
}