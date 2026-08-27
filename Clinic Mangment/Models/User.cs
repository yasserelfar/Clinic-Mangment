using System.ComponentModel.DataAnnotations;
using System.Numerics;

namespace ClinicManagement.Models;

public class User
{
    public int Id { get; set; }
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;


    [Required]
    [MaxLength(150) ]
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;

    public string Role { get; set; } = string.Empty;

    //public Doctor? Doctor { get; set; }
}